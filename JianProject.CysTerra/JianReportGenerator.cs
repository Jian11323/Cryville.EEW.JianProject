using Cryville.Common.Compat;
using Cryville.EEW.Report;
using System;
using System.Globalization;
using Cryville.EEW;

namespace JianProject.CysTerra {
	public sealed class JianReportGenerator : IContextedGenerator<JianEvent, IReportGeneratorContext, ReportModel?> {
		readonly JianSource _source;

		public JianReportGenerator(JianSource source) {
			_source = source;
		}

		public ReportModel? Generate(JianEvent e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			if (JianSources.Resolve(e.Agency) != _source) {
				return null;
			}
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var titleKey = TitleKey(e);
			var result = new ReportModel {
				Source = JianSources.AuthorityLabel(culture, e.Agency, e.ForwardAgency),
				Title = res.GetStringRequired(titleKey),
				TimeZone = TimeZoneInfo.Utc,
				UtcIssueTime = DateTime.UtcNow,
			};
			if (e.Latitude is double nameLat && e.Longitude is double nameLon) {
				context.NameLocationTo(result, nameLat, nameLon, SharedCultures.Get("zh-CN"), culture);
			}
			if (result.Location == null) {
				var fallback = e.PlaceZh ?? e.Place ?? e.Title;
				if (!string.IsNullOrEmpty(fallback)) {
					result.Location = fallback;
					result.LocationSpecificity = e.PlaceZh != null ? 3 : 8;
				}
			}

			if (e.OriginUtc is DateTime origin) {
				result.Time = new DateTimeOffset(DateTime.SpecifyKind(origin, DateTimeKind.Utc));
			}

			if (e.Kind == JianKind.EarlyWarning || e.Kind == JianKind.Tsunami) {
				var issued = e.OriginUtc ?? DateTime.UtcNow;
				result.InvalidatedTime = new DateTimeOffset(DateTime.SpecifyKind(issued, DateTimeKind.Utc) + TimeSpan.FromMinutes(e.IsFinal ? 10 : 3));
			}
			if (e.IsCancel) {
				result.InvalidatedTime = new DateTimeOffset(DateTime.UtcNow);
			}

			result.GroupKeys.Add(new ReportUnitKey(e.Agency, e.UnitId));
			result.RevisionKey = new ReportRevisionKey(e.Serial ?? 0, e.IsCancel, e.IsFinal);

			if (e.Latitude is double lat && e.Longitude is double lon && e.Magnitude is double mag
				&& e.DepthKm is double depth && e.OriginUtc is DateTime hypoTime) {
				result.GroupKeys.Add(new HypocenterGroupKey(
					(float)lat, (float)lon, DateTime.SpecifyKind(hypoTime, DateTimeKind.Utc), (float)mag, (float)depth));
			}

			// Intensity first: list card left block uses the key property color from SeverityScheme.
			if (!string.IsNullOrEmpty(e.Intensity)) {
				var intensityKey = JianIntensity.KeyForAgency(e.Agency);
				result.Properties.Add(new(
					intensityKey,
					res.GetStringRequired("PropertyIntensity"),
					e.Intensity,
					context.SeverityScheme,
					JianIntensity.RawValue(intensityKey, e.Intensity)) { AccuracyOrder = 30 });
			}

			if (e.Magnitude is double magnitude) {
				result.Properties.Add(new(
					"Magnitude",
					res.GetStringRequired("PropertyMagnitude"),
					magnitude.ToString("0.0", culture),
					context.SeverityScheme,
					magnitude) { AccuracyOrder = 30 });
			}

			if (e.DepthKm is double depthKm) {
				result.Properties.Add(new(
					"HypocenterDepth",
					res.GetStringRequired("PropertyDepth"),
					string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depthKm.ToString("0.0", culture)),
					context.SeverityScheme,
					depthKm) { AccuracyOrder = 30 });
			}

			if (e.Serial is int serial) {
				result.Properties.Add(new(
					"Serial",
					res.GetStringRequired("PropertySerial"),
					serial.ToString(culture),
					context.SeverityScheme,
					serial) { AccuracyOrder = 40 });
			}

			if (!string.IsNullOrEmpty(e.Detail) && e.Detail != e.Place && e.Detail != e.Title && e.Detail != e.PlaceZh) {
				result.Predicate = e.Detail;
			}
			else if (e.IsAssumption) {
				result.Predicate = res.GetStringRequired("Assumption");
			}

			return result;
		}

		static bool IsJmaEew(string agency) {
			var head = agency.Trim().ToLowerInvariant().Replace('_', '-').Split(':')[0];
			return head == "jma-eew";
		}

		static string TitleKey(JianEvent e) {
			if (e.IsCancel) {
				return e.Kind == JianKind.Tsunami ? "TitleTsunamiCancel" : "TitleCancel";
			}
			if (e.Kind == JianKind.EarlyWarning && IsJmaEew(e.Agency)) {
				// 对齐 Wolfx / Fan Studio：紧急地震速报（预报/警报），不写「地震预警更新」
				return e.IsWarn ? "TitleJmaEewWarning" : "TitleJmaEewForecast";
			}
			return e.Kind switch {
				// CEA / CWA / KMA / ShakeAlert 等：地震预警（续报用「更新」）
				JianKind.EarlyWarning => e.Serial is > 1 ? "TitleEewUpdate" : "TitleEew",
				JianKind.Bulletin => "TitleBulletin",
				JianKind.Tsunami => "TitleTsunami",
				JianKind.Weather => "TitleWeather",
				_ => "TitleOther",
			};
		}

		sealed record ReportUnitKey(string Agency, string UnitId) : IReportUnitKey { }

		sealed record ReportRevisionKey(int Number, bool Cancel, bool Final) : IReportRevisionKey {
			public int? Serial => Number > 0 ? Number : null;
			public bool IsCancellation => Cancel;
			public bool IsFinalRevision => Final;

			public bool IsComparableWith(IReportRevisionKey obj) => obj is ReportRevisionKey;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not ReportRevisionKey other) {
					throw new ArgumentException("Mismatched revision key type.");
				}
				var cmp = Number.CompareTo(other.Number);
				if (cmp != 0) {
					return cmp;
				}
				if (Cancel != other.Cancel) {
					return Cancel ? 1 : -1;
				}
				return 0;
			}
		}
	}
}
