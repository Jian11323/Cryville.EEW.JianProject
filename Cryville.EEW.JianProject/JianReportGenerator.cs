using Cryville.Common.Compat;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.JianProject {
	public sealed class JianReportGenerator : IContextedGenerator<JianEvent, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(JianEvent e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var titleKey = TitleKey(e);
			var result = new ReportModel {
				Source = string.Format(culture, res.GetStringRequired("SourceNameTyped"), e.Agency),
				Title = res.GetStringRequired(titleKey),
				Location = e.Place ?? e.Title,
				LocationSpecificity = e.Latitude != null && e.Longitude != null ? 12 : (e.Place != null ? 8 : 0),
				TimeZone = TimeZoneInfo.Utc,
				UtcIssueTime = DateTime.UtcNow,
			};

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

			if (e.Magnitude is double magnitude) {
				result.Properties.Add(new(
					"Magnitude",
					res.GetStringRequired("PropertyMagnitude"),
					magnitude.ToString("0.0", culture),
					context.SeverityScheme,
					magnitude) { AccuracyOrder = 90 });
			}

			if (e.DepthKm is double depthKm) {
				result.Properties.Add(new(
					"HypocenterDepth",
					res.GetStringRequired("PropertyDepth"),
					string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depthKm.ToString("0.0", culture)),
					context.SeverityScheme,
					depthKm) { AccuracyOrder = 80 });
			}

			if (!string.IsNullOrEmpty(e.Intensity)) {
				result.Properties.Add(new(
					"Intensity",
					res.GetStringRequired("PropertyIntensity"),
					e.Intensity,
					context.SeverityScheme,
					e.Intensity) { AccuracyOrder = 85 });
			}

			if (e.Serial is int serial) {
				result.Properties.Add(new(
					"Serial",
					res.GetStringRequired("PropertySerial"),
					serial.ToString(culture),
					context.SeverityScheme,
					serial) { AccuracyOrder = 40 });
			}

			if (!string.IsNullOrEmpty(e.InfoType)) {
				result.Properties.Add(new(
					"InfoType",
					res.GetStringRequired("PropertyInfoType"),
					e.InfoType,
					context.SeverityScheme,
					e.InfoType) { AccuracyOrder = 30 });
			}

			if (!string.IsNullOrEmpty(e.Detail) && e.Detail != e.Place && e.Detail != e.Title) {
				result.Predicate = e.Detail;
			}
			else if (e.IsAssumption) {
				result.Predicate = res.GetStringRequired("Assumption");
			}

			return result;
		}

		static string TitleKey(JianEvent e) {
			if (e.IsCancel) {
				return e.Kind == JianKind.Tsunami ? "TitleTsunamiCancel" : "TitleCancel";
			}
			return e.Kind switch {
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
