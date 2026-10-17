using Cryville.Common.Compat;
using Cryville.EEW.JianProject;
using Cryville.EEW.TTS;
using System.Globalization;
using System.Text;

namespace Cryville.EEW.JianProject.TTS {
	public sealed class JianTtsGenerator : IContextedGenerator<JianEvent, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(JianEvent e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;

			var spoken = new StringBuilder();
			spoken.Append(string.Format(culture, res.GetStringRequired("TtsLead"), JianSources.AuthorityLabel(culture, e.Agency, e.ForwardAgency)));
			spoken.Append(res.GetStringRequired(TitleKey(e)));
			var place = e.PlaceZh ?? e.Place ?? e.Title;
			if (!string.IsNullOrEmpty(place)) {
				spoken.Append(string.Format(culture, res.GetStringRequired("TtsPlace"), place));
			}
			if (e.Magnitude is double mag) {
				spoken.Append(string.Format(culture, res.GetStringRequired("TtsMagnitude"), mag.ToString("0.0", culture)));
			}
			if (e.DepthKm is double depth) {
				spoken.Append(string.Format(culture, res.GetStringRequired("TtsDepth"), depth.ToString("0", culture)));
			}
			if (!string.IsNullOrEmpty(e.Intensity)) {
				spoken.Append(string.Format(culture, res.GetStringRequired("TtsIntensity"), e.Intensity));
			}
			if (!string.IsNullOrEmpty(e.Detail) && e.Kind == JianKind.Tsunami) {
				spoken.Append(string.Format(culture, res.GetStringRequired("TtsDetail"), e.Detail));
			}

			var priority = e.Kind switch {
				JianKind.EarlyWarning => e.IsCancel ? 5 : 10,
				JianKind.Tsunami => 15,
				JianKind.Weather => 30,
				JianKind.Bulletin => 40,
				_ => 50,
			};

			return new TTSEntry(culture, res.GetStringRequired("TtsRepeatTitle"), spoken.ToString(), priority);
		}

		static string TitleKey(JianEvent e) {
			if (e.IsCancel) {
				return e.Kind == JianKind.Tsunami ? "TitleTsunamiCancel" : "TitleCancel";
			}
			return e.Kind switch {
				JianKind.EarlyWarning => "TitleEew",
				JianKind.Bulletin => "TitleBulletin",
				JianKind.Tsunami => "TitleTsunami",
				JianKind.Weather => "TitleWeather",
				_ => "TitleOther",
			};
		}
	}
}
