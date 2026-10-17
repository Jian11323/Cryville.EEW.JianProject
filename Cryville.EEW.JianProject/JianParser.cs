using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Cryville.EEW.JianProject {
	public static class JianParser {
		public static IEnumerable<JianEvent> Parse(JsonElement root, Func<string, bool>? accept) {
			if (root.ValueKind != JsonValueKind.Object) {
				yield break;
			}

			var type = FirstText(root, "type");
			if (type == "heartbeat" || type == "pong" || type == "error" || type == "starting") {
				yield break;
			}

			if (type == "all") {
				foreach (var prop in root.EnumerateObject()) {
					if (!prop.Name.StartsWith("source", StringComparison.Ordinal)) {
						continue;
					}
					var agency = AgencyFromSourceKey(prop.Name);
					if (prop.Value.ValueKind != JsonValueKind.Object) {
						continue;
					}
					var data = prop.Value.TryGetProperty("Data", out var dataEl) && dataEl.ValueKind == JsonValueKind.Object
						? dataEl
						: prop.Value;
					var evt = Map(agency, data);
					if (evt != null && (accept == null || accept(evt.Agency))) {
						yield return evt;
					}
				}
				yield break;
			}

			if (type != null && type.EndsWith("list_response", StringComparison.Ordinal)) {
				var agency = AgencyFromListResponse(type);
				if (root.TryGetProperty("Data", out var arr) && arr.ValueKind == JsonValueKind.Array) {
					foreach (var item in arr.EnumerateArray()) {
						if (item.ValueKind != JsonValueKind.Object) {
							continue;
						}
						var evt = Map(agency, item);
						if (evt != null && (accept == null || accept(evt.Agency))) {
							yield return evt;
						}
					}
				}
				yield break;
			}

			if (string.IsNullOrEmpty(type) || IsStationFeed(type)) {
				yield break;
			}

			var payload = root.TryGetProperty("Data", out var dataNode) && dataNode.ValueKind == JsonValueKind.Object
				? dataNode
				: root;
			var single = Map(type, payload);
			if (single != null && (accept == null || accept(single.Agency))) {
				yield return single;
			}
		}

		static JianEvent? Map(string agency, JsonElement data) {
			agency = NormalizeAgency(agency, data);
			if (agency.Length == 0 || IsStationFeed(agency)) {
				return null;
			}
			if (Flag(data, "isTraining", "is_training", "training")) {
				return null;
			}

			var kind = Classify(agency);
			var place = FirstText(data, "placeName", "Hypocenter", "hypocenter", "place", "epicenter", "location", "Place");
			var title = FirstText(data, "title", "Title", "headline");
			var infoType = FirstText(data, "infoTypeName", "infoType", "alertflg");
			var magnitude = Num(data, "magnitude", "Magunitude", "Magnitude");
			var depth = DepthKm(data);
			var latitude = Num(data, "latitude", "Latitude");
			var longitude = Num(data, "longitude", "Longitude");
			var origin = Time(data, "originTime", "shockTime", "time", "Time", "reportTime");
			var intensity = FirstText(data, "maxIntensity", "MaxIntensity", "intensity", "epiIntensity");
			var detail = DetailText(data, kind);
			var serial = Int(data, "serial", "number", "Serial", "updates", "reportNum", "ReportNum");
			var cancel = Flag(data, "isCancel", "isCanceled", "isCancelled", "canceled", "cancelled", "is_cancel", "cancel")
				|| Contains(infoType, "取消")
				|| Contains(title, "取消");
			var final = Flag(data, "isFinal", "is_final", "final", "isLast", "isLastInfo");
			var assumption = Flag(data, "isAssumption", "is_assumption", "assumption", "isAssumed", "isPLUM", "isPlum", "is_plum")
				|| Contains(place, "仮定")
				|| Contains(place, "調査中")
				|| Contains(place, "调查中");

			if (kind == JianKind.Other && place == null && title == null && magnitude == null && detail == null) {
				return null;
			}

			var eventId = FirstText(data, "id", "EventID", "eventId", "event_id");
			var unit = eventId ?? BuildUnit(agency, origin, latitude, longitude, place, title);
			var fingerprint = string.Join("|",
				agency, unit, serial?.ToString(CultureInfo.InvariantCulture) ?? "",
				cancel ? "1" : "0", final ? "1" : "0",
				Fmt(magnitude), Fmt(depth), Fmt(latitude), Fmt(longitude),
				place ?? "", title ?? "", intensity ?? "", infoType ?? "", detail ?? "");

			return new JianEvent {
				Kind = kind,
				Agency = agency,
				UnitId = unit,
				Serial = serial,
				IsCancel = cancel,
				IsFinal = final,
				IsAssumption = assumption,
				Place = place,
				Title = title,
				InfoType = infoType,
				Magnitude = magnitude,
				DepthKm = depth,
				Latitude = latitude,
				Longitude = longitude,
				OriginUtc = origin,
				Intensity = intensity,
				Detail = detail,
				Fingerprint = fingerprint,
			};
		}

		static string NormalizeAgency(string agency, JsonElement data) {
			agency = agency.Trim();
			if (agency.Equals("cea-pr", StringComparison.OrdinalIgnoreCase)) {
				var province = FirstText(data, "province");
				if (!string.IsNullOrEmpty(province)) {
					return "cea-pr:" + province;
				}
			}
			return agency;
		}

		static JianKind Classify(string agency) {
			var a = agency.Trim().ToLowerInvariant().Replace('_', '-');
			var head = a.Split(':')[0];
			if (head is "jma-tsunami" or "cwa-tsunami" or "nmefc-tsunami" || head.EndsWith("-tsunami", StringComparison.Ordinal)) {
				return JianKind.Tsunami;
			}
			if (head == "weather") {
				return JianKind.Weather;
			}
			if (head is "jma-eew" or "cea" or "cea-pr" or "cwa-eew" or "sa" or "kma-eew" or "early-est" or "earlyest"
				|| head.EndsWith("-eew", StringComparison.Ordinal)
				|| head.StartsWith("cea-pr", StringComparison.Ordinal)) {
				return JianKind.EarlyWarning;
			}
			if (IsKnownBulletin(head)) {
				return JianKind.Bulletin;
			}
			return JianKind.Other;
		}

		static bool IsKnownBulletin(string head) => head is
			"cenc" or "cenc-ir" or "ningxia" or "yunnan" or "shanxi" or "beijing" or "cwa" or "jma" or "hko"
			or "kma" or "tmd" or "mmd" or "bmkg" or "geonet" or "usgs" or "emsc" or "gfz" or "bcsf" or "ingv"
			or "usp" or "nrcan" or "afad" or "ipma" or "noa" or "sed" or "scsn" or "ipgp" or "infp" or "isc"
			or "knmi" or "ncedc" or "lmu" or "koeri" or "gsras" or "csn" or "phivolcs" or "ssn" or "ga"
			or "igepn" or "cenc-fjdzj";

		static bool IsStationFeed(string type) {
			var a = type.Trim().ToLowerInvariant();
			return a is "kmoni" or "s-net" or "kma-station" or "trem-station" or "pa-station" or "trem" or "pa";
		}

		static string AgencyFromSourceKey(string key) {
			const string wide = "source\uFF1A";
			if (key.StartsWith(wide, StringComparison.Ordinal)) {
				return key.Substring(wide.Length);
			}
			if (key.StartsWith("source:", StringComparison.Ordinal)) {
				return key.Substring("source:".Length);
			}
			var idx = key.IndexOf('\uFF1A');
			if (idx >= 0 && idx + 1 < key.Length) {
				return key.Substring(idx + 1);
			}
			idx = key.IndexOf(':');
			return idx >= 0 && idx + 1 < key.Length ? key.Substring(idx + 1) : key;
		}

		static string AgencyFromListResponse(string type) {
			const string suffix = "list_response";
			var body = type.Substring(0, type.Length - suffix.Length);
			return body.EndsWith("_", StringComparison.Ordinal) ? body.Substring(0, body.Length - 1) : body;
		}

		static string BuildUnit(string agency, DateTime? origin, double? lat, double? lon, string? place, string? title) {
			var sb = new StringBuilder(agency);
			sb.Append('|');
			if (origin != null) {
				sb.Append(origin.Value.Ticks.ToString(CultureInfo.InvariantCulture));
			}
			sb.Append('|');
			sb.Append(Fmt(lat));
			sb.Append('|');
			sb.Append(Fmt(lon));
			sb.Append('|');
			sb.Append(place ?? title ?? "");
			return sb.ToString();
		}

		static string? DetailText(JsonElement data, JianKind kind) {
			if (kind == JianKind.Tsunami && data.TryGetProperty("area", out var area)) {
				if (area.ValueKind == JsonValueKind.String) {
					return area.GetString();
				}
				if (area.ValueKind == JsonValueKind.Array) {
					var parts = new List<string>();
					foreach (var item in area.EnumerateArray()) {
						if (item.ValueKind == JsonValueKind.String) {
							var s = item.GetString();
							if (!string.IsNullOrWhiteSpace(s)) {
								parts.Add(s.Trim());
							}
							continue;
						}
						if (item.ValueKind != JsonValueKind.Object) {
							continue;
						}
						var name = FirstText(item, "name");
						var kindName = FirstText(item, "kind");
						if (string.IsNullOrEmpty(name)) {
							continue;
						}
						parts.Add(string.IsNullOrEmpty(kindName) ? name : name + " " + kindName);
						if (parts.Count >= 8) {
							break;
						}
					}
					if (parts.Count > 0) {
						return string.Join("\u3001", parts);
					}
				}
			}
			return FirstText(data, "description", "text", "content", "message", "level");
		}

		static string? FirstText(JsonElement obj, params string[] keys) {
			if (obj.ValueKind != JsonValueKind.Object) {
				return null;
			}
			foreach (var key in keys) {
				if (!obj.TryGetProperty(key, out var v)) {
					continue;
				}
				var text = v.ValueKind switch {
					JsonValueKind.String => v.GetString(),
					JsonValueKind.Number => v.GetRawText(),
					JsonValueKind.True => "true",
					JsonValueKind.False => "false",
					_ => null,
				};
				text = text?.Trim();
				if (!string.IsNullOrEmpty(text) && text != "\u2014") {
					return text;
				}
			}
			return null;
		}

		static double? Num(JsonElement obj, params string[] keys) {
			if (obj.ValueKind != JsonValueKind.Object) {
				return null;
			}
			foreach (var key in keys) {
				if (!obj.TryGetProperty(key, out var v)) {
					continue;
				}
				if (v.ValueKind == JsonValueKind.Number && v.TryGetDouble(out var n)) {
					return n;
				}
				if (v.ValueKind == JsonValueKind.String &&
					double.TryParse(v.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var s)) {
					return s;
				}
			}
			return null;
		}

		static int? Int(JsonElement obj, params string[] keys) {
			var n = Num(obj, keys);
			if (n == null) {
				return null;
			}
			var v = (int)n.Value;
			return v > 0 ? v : null;
		}

		static double? DepthKm(JsonElement data) {
			if (data.ValueKind != JsonValueKind.Object) {
				return null;
			}
			foreach (var key in new[] { "depth", "Depth" }) {
				if (!data.TryGetProperty(key, out var v)) {
					continue;
				}
				if (v.ValueKind == JsonValueKind.String) {
					var s = v.GetString()?.Trim();
					if (s is "\u5F88\u6D45" or "\u3054\u304F\u6D45\u3044" or "\u6975\u6D45") {
						return 0;
					}
				}
			}
			return Num(data, "depth", "Depth");
		}

		static DateTime? Time(JsonElement obj, params string[] keys) {
			if (obj.ValueKind != JsonValueKind.Object) {
				return null;
			}
			foreach (var key in keys) {
				if (!obj.TryGetProperty(key, out var v)) {
					continue;
				}
				if (v.ValueKind == JsonValueKind.Number && v.TryGetDouble(out var n)) {
					var ms = NormalizeEpochMs(n);
					if (ms > 0) {
						return DateTime.UnixEpoch.AddMilliseconds(ms);
					}
				}
				if (v.ValueKind == JsonValueKind.String) {
					var s = v.GetString()?.Trim();
					if (string.IsNullOrEmpty(s)) {
						continue;
					}
					if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var sn)) {
						var ms = NormalizeEpochMs(sn);
						if (ms > 0) {
							return DateTime.UnixEpoch.AddMilliseconds(ms);
						}
					}
					if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dt)) {
						return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
					}
				}
			}
			return null;
		}

		static long NormalizeEpochMs(double n) {
			if (n <= 0) {
				return 0;
			}
			if (n > 1_000_000_000_000d) {
				return (long)n;
			}
			if (n > 1_000_000_000d) {
				return (long)(n * 1000d);
			}
			return 0;
		}

		static bool Flag(JsonElement obj, params string[] keys) {
			if (obj.ValueKind != JsonValueKind.Object) {
				return false;
			}
			foreach (var key in keys) {
				if (!obj.TryGetProperty(key, out var v)) {
					continue;
				}
				switch (v.ValueKind) {
					case JsonValueKind.True:
						return true;
					case JsonValueKind.Number:
						if (v.TryGetDouble(out var n) && n != 0) {
							return true;
						}
						break;
					case JsonValueKind.String:
						var s = v.GetString()?.Trim();
						if (s is "1" or "true" or "True" or "TRUE" or "yes" or "cancel" or "canceled" or "cancelled"
							or "\u6700\u7D42" or "\u6700\u7EC8" or "\u6700\u7D42\u5831" or "\u6700\u7EC8\u62A5" or "\u53D6\u6D88") {
							return true;
						}
						break;
				}
			}
			return false;
		}

		static bool Contains(string? text, string needle) =>
			text != null && text.Contains(needle, StringComparison.Ordinal);

		static string Fmt(double? n) =>
			n?.ToString("G", CultureInfo.InvariantCulture) ?? "";
	}
}
