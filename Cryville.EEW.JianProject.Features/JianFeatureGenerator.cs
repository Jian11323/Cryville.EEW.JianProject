using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.JianProject;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.JianProject.Features {
	public sealed class JianFeatureGenerator : IGenerator<JianEvent, Feature?> {
		static readonly Unit Kilometre = Units.Metre.WithPrefix(MetricPrefixes.Kilo);

		public Feature? Generate(JianEvent e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var kind = e.Kind switch {
				JianKind.Tsunami => Tsunami,
				JianKind.EarlyWarning => ReportForecast,
				JianKind.Bulletin => ReportObservation,
				_ => Cryville.EEW.TagTypeKeys.Report,
			};

			var feature = new Feature {
				{ Is, e.Kind is JianKind.EarlyWarning or JianKind.Bulletin ? Earthquake : kind },
				{ Ongoing, !e.IsCancel && e.Kind != JianKind.Bulletin },
				{ Source, e.Agency },
			};

			if (e.Kind is JianKind.EarlyWarning or JianKind.Bulletin) {
				feature[Subject] = kind;
			}

			if (!string.IsNullOrEmpty(e.Place)) {
				feature[Name] = e.Place;
			}
			else if (!string.IsNullOrEmpty(e.Title)) {
				feature[Name] = e.Title;
			}

			if (!string.IsNullOrEmpty(e.Detail)) {
				feature[Description] = e.Detail;
			}
			if (!string.IsNullOrEmpty(e.Title) && e.Title != e.Place) {
				feature[Comment] = e.Title;
			}

			if (e.OriginUtc is DateTime origin) {
				feature[Time] = new DateTimeOffset(DateTime.SpecifyKind(origin, DateTimeKind.Utc));
			}

			if (e.Magnitude is double mag) {
				var magKey = e.Agency.StartsWith("jma", StringComparison.OrdinalIgnoreCase) ? MagnitudeJMA : Magnitude;
				feature[magKey] = new Quantity(mag, Units.Dimensionless);
			}

			if (e.Latitude is double lat && e.Longitude is double lon) {
				var at = new Feature(new Point(lon, lat)) {
					{ Is, e.IsAssumption ? HypocenterAssumed : Hypocenter },
				};
				if (e.DepthKm is double depth) {
					at[HypocenterDepth] = new Quantity(depth, Kilometre);
				}
				feature[At] = at;
			}

			if (!string.IsNullOrEmpty(e.Intensity)) {
				feature[IntensityKey(e.Agency)] = e.Intensity;
			}

			return feature;
		}

		static TagTypeKey IntensityKey(string agency) {
			var a = agency.ToLowerInvariant();
			if (a.StartsWith("jma")) {
				return IntensityJMA;
			}
			if (a.StartsWith("cwa")) {
				return IntensityCWASIS;
			}
			if (a.Contains("cenc") || a.StartsWith("cea") || a.StartsWith("beijing")) {
				return IntensityCSIS;
			}
			if (a is "usgs" or "sa" || a.StartsWith("sa")) {
				return IntensityMMI;
			}
			return Intensity;
		}
	}
}
