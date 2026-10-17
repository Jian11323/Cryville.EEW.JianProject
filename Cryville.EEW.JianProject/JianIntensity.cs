using System;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.JianProject {
	/// <summary>
	/// Maps agency id to the CysTerra intensity subtype used for list colors and feature tags.
	/// </summary>
	public static class JianIntensity {
		public static TagTypeKey KeyForAgency(string agency) {
			var a = agency.ToLowerInvariant();
			if (a.StartsWith("jma", StringComparison.Ordinal)) {
				return IntensityJMA;
			}
			if (a.StartsWith("cwa", StringComparison.Ordinal)) {
				return IntensityCWASIS;
			}
			if (a.Contains("cenc") || a.StartsWith("cea", StringComparison.Ordinal) || a.StartsWith("beijing", StringComparison.Ordinal)) {
				return IntensityCSIS;
			}
			if (a is "usgs" or "sa" || a.StartsWith("sa", StringComparison.Ordinal)) {
				return IntensityMMI;
			}
			return Intensity;
		}

		/// <summary>
		/// Raw value for <see cref="Cryville.EEW.Report.ISeverityScheme"/>: CSIS/MMI prefer numbers.
		/// </summary>
		public static object RawValue(TagTypeKey key, string intensity) {
			if (Equals(key, IntensityCSIS) || Equals(key, IntensityMMI)) {
				if (double.TryParse(intensity, System.Globalization.NumberStyles.Float,
					System.Globalization.CultureInfo.InvariantCulture, out var n)) {
					return n;
				}
			}
			return intensity;
		}
	}
}
