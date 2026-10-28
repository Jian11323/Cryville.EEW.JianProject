using Cryville.Common.Compat;
using Cryville.EEW.Features;
using JianProject.CysTerra;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;
using Cryville.EEW;

namespace JianProject.CysTerra.Features {
	public sealed class JianFeatureGenerator : IGenerator<JianEvent, Feature?> {
		static readonly Unit Kilometre = Units.Metre.WithPrefix(MetricPrefixes.Kilo);
		readonly JianSource _source;

		public JianFeatureGenerator(JianSource source) {
			_source = source;
		}

		public Feature? Generate(JianEvent e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			if (JianSources.Resolve(e.Agency) != _source) {
				return null;
			}

			var kind = e.Kind switch {
				JianKind.Tsunami => Tsunami,
				JianKind.EarlyWarning => ReportForecast,
				JianKind.Bulletin => ReportObservation,
				_ => Cryville.EEW.TagTypeKeys.Report,
			};

			var feature = new Feature {
				{ Is, e.Kind is JianKind.EarlyWarning or JianKind.Bulletin ? Earthquake : kind },
				{ Ongoing, !e.IsCancel && !e.IsFinal && e.Kind != JianKind.Bulletin },
				{ Source, e.Agency },
			};

			if (e.Kind is JianKind.EarlyWarning or JianKind.Bulletin) {
				feature[Subject] = kind;
			}

			var placeName = PlaceName(e);
			if (placeName != null) {
				feature[Name] = placeName;
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
				var intensityKey = JianIntensity.KeyForAgency(e.Agency);
				feature[intensityKey] = JianIntensity.RawValue(intensityKey, e.Intensity);
			}

			return feature;
		}

		static object? PlaceName(JianEvent e) {
			var zh = e.PlaceZh;
			var en = e.Place ?? e.Title;
			if (string.IsNullOrEmpty(zh) && string.IsNullOrEmpty(en)) {
				return null;
			}
			var names = new LocalizableCollection<string>();
			if (!string.IsNullOrEmpty(zh)) {
				names.Add(new Localized<string>(zh, SharedCultures.Get("zh-CN")));
			}
			if (!string.IsNullOrEmpty(en) && en != zh) {
				names.Add(new Localized<string>(en, SharedCultures.Get("en-US")));
			}
			return names;
		}
	}
}
