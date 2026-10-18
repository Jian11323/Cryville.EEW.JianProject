using Cryville.EEW.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Cryville.EEW;

namespace JianProject.CysTerra {
	public enum JianSource {
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		None,
		[LocalizableDisplayName(nameof(Cea), Path = ["SourceNameTypes"])] Cea,
		[LocalizableDisplayName(nameof(CeaPr), Path = ["SourceNameTypes"])] CeaPr,
		[LocalizableDisplayName(nameof(CwaEew), Path = ["SourceNameTypes"])] CwaEew,
		[LocalizableDisplayName(nameof(JmaEew), Path = ["SourceNameTypes"])] JmaEew,
		[LocalizableDisplayName(nameof(JmaTsunami), Path = ["SourceNameTypes"])] JmaTsunami,
		[LocalizableDisplayName(nameof(Sa), Path = ["SourceNameTypes"])] Sa,
		[LocalizableDisplayName(nameof(KmaEew), Path = ["SourceNameTypes"])] KmaEew,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[LocalizableDisplayName(nameof(EarlyEst), Path = ["SourceNameTypes"])] EarlyEst,
		[LocalizableDisplayName(nameof(Cenc), Path = ["SourceNameTypes"])] Cenc,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[LocalizableDisplayName(nameof(CencIr), Path = ["SourceNameTypes"])] CencIr,
		[LocalizableDisplayName(nameof(CencFjdzj), Path = ["SourceNameTypes"])] CencFjdzj,
		[LocalizableDisplayName(nameof(Ningxia), Path = ["SourceNameTypes"])] Ningxia,
		[LocalizableDisplayName(nameof(Yunnan), Path = ["SourceNameTypes"])] Yunnan,
		[LocalizableDisplayName(nameof(Shanxi), Path = ["SourceNameTypes"])] Shanxi,
		[LocalizableDisplayName(nameof(Beijing), Path = ["SourceNameTypes"])] Beijing,
		[LocalizableDisplayName(nameof(Cwa), Path = ["SourceNameTypes"])] Cwa,
		[LocalizableDisplayName(nameof(Jma), Path = ["SourceNameTypes"])] Jma,
		[LocalizableDisplayName(nameof(Hko), Path = ["SourceNameTypes"])] Hko,
		[LocalizableDisplayName(nameof(Kma), Path = ["SourceNameTypes"])] Kma,
		[LocalizableDisplayName(nameof(Tmd), Path = ["SourceNameTypes"])] Tmd,
		[LocalizableDisplayName(nameof(Mmd), Path = ["SourceNameTypes"])] Mmd,
		[LocalizableDisplayName(nameof(Bmkg), Path = ["SourceNameTypes"])] Bmkg,
		[LocalizableDisplayName(nameof(Geonet), Path = ["SourceNameTypes"])] Geonet,
		[LocalizableDisplayName(nameof(Usgs), Path = ["SourceNameTypes"])] Usgs,
		[LocalizableDisplayName(nameof(Emsc), Path = ["SourceNameTypes"])] Emsc,
		[LocalizableDisplayName(nameof(Gfz), Path = ["SourceNameTypes"])] Gfz,
		[LocalizableDisplayName(nameof(Bcsf), Path = ["SourceNameTypes"])] Bcsf,
		[LocalizableDisplayName(nameof(Ingv), Path = ["SourceNameTypes"])] Ingv,
		[LocalizableDisplayName(nameof(Usp), Path = ["SourceNameTypes"])] Usp,
		[LocalizableDisplayName(nameof(Nrcan), Path = ["SourceNameTypes"])] Nrcan,
		[LocalizableDisplayName(nameof(Afad), Path = ["SourceNameTypes"])] Afad,
		[LocalizableDisplayName(nameof(Ipma), Path = ["SourceNameTypes"])] Ipma,
		[LocalizableDisplayName(nameof(Noa), Path = ["SourceNameTypes"])] Noa,
		[LocalizableDisplayName(nameof(Sed), Path = ["SourceNameTypes"])] Sed,
		[LocalizableDisplayName(nameof(Scsn), Path = ["SourceNameTypes"])] Scsn,
		[LocalizableDisplayName(nameof(Ipgp), Path = ["SourceNameTypes"])] Ipgp,
		[LocalizableDisplayName(nameof(Infp), Path = ["SourceNameTypes"])] Infp,
		[LocalizableDisplayName(nameof(Isc), Path = ["SourceNameTypes"])] Isc,
		[LocalizableDisplayName(nameof(Knmi), Path = ["SourceNameTypes"])] Knmi,
		[LocalizableDisplayName(nameof(Ncedc), Path = ["SourceNameTypes"])] Ncedc,
		[LocalizableDisplayName(nameof(Lmu), Path = ["SourceNameTypes"])] Lmu,
		[LocalizableDisplayName(nameof(Koeri), Path = ["SourceNameTypes"])] Koeri,
		[LocalizableDisplayName(nameof(Gsras), Path = ["SourceNameTypes"])] Gsras,
		[LocalizableDisplayName(nameof(Csn), Path = ["SourceNameTypes"])] Csn,
		[LocalizableDisplayName(nameof(Phivolcs), Path = ["SourceNameTypes"])] Phivolcs,
		[LocalizableDisplayName(nameof(Ssn), Path = ["SourceNameTypes"])] Ssn,
		[LocalizableDisplayName(nameof(Ga), Path = ["SourceNameTypes"])] Ga,
		[LocalizableDisplayName(nameof(Igepn), Path = ["SourceNameTypes"])] Igepn,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[LocalizableDisplayName(nameof(JmaVolcano), Path = ["SourceNameTypes"])] JmaVolcano,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[LocalizableDisplayName(nameof(UsgsVolcano), Path = ["SourceNameTypes"])] UsgsVolcano,
		[LocalizableDisplayName(nameof(Peru), Path = ["SourceNameTypes"])] Peru,
		[LocalizableDisplayName(nameof(Other), Path = ["SourceNameTypes"])] Other,
	}

	public static class JianSources {
		public static HashSet<JianSource> DefaultAll() {
			var set = new HashSet<JianSource>();
			foreach (JianSource value in System.Enum.GetValues(typeof(JianSource))) {
				if (value is JianSource.None or JianSource.EarlyEst or JianSource.CencIr
					or JianSource.JmaVolcano or JianSource.UsgsVolcano) {
					continue;
				}
				set.Add(value);
			}
			return set;
		}

		public static bool Allows(ISet<JianSource> filter, string agency) {
			if (filter == null || filter.Count == 0) {
				return false;
			}
			return filter.Contains(Flag(agency));
		}

		public static string AuthorityLabel(CultureInfo culture, string agency, string? forward) {
			return AuthorityName(culture, agency, forward, "AuthorityNames");
		}

		/// <summary>TTS 用机构全称（卡片仍用 AuthorityNames 缩写）。</summary>
		public static string AuthoritySpeechLabel(CultureInfo culture, string agency, string? forward) {
			return AuthorityName(culture, agency, forward, "AuthoritySpeechNames");
		}

		static string AuthorityName(CultureInfo culture, string agency, string? forward, string setKey) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var name = res.GetStringSetRequired(setKey).GetString(Flag(agency).ToString())
				?? res.GetStringSetRequired("AuthorityNames").GetString(Flag(agency).ToString())
				?? agency;
			var head = Head(agency);
			if (head == "emsc" && !string.IsNullOrWhiteSpace(forward)
				&& !forward.Equals("EMSC", StringComparison.OrdinalIgnoreCase)) {
				return string.Format(culture, res.GetStringRequired("AuthorityNameForwarded"), name, forward.Trim());
			}
			return name;
		}

		static string Head(string agency) {
			var head = agency.Trim().ToLowerInvariant().Replace('_', '-');
			var colon = head.IndexOf(':');
			return colon >= 0 ? head.Substring(0, colon) : head;
		}

		static JianSource Flag(string agency) {
			return Head(agency) switch {
				"cea" => JianSource.Cea,
				"cea-pr" => JianSource.CeaPr,
				"cwa-eew" => JianSource.CwaEew,
				"jma-eew" => JianSource.JmaEew,
				"jma-tsunami" => JianSource.JmaTsunami,
				"sa" => JianSource.Sa,
				"kma-eew" => JianSource.KmaEew,
				"early-est" or "earlyest" => JianSource.EarlyEst,
				"cenc" => JianSource.Cenc,
				"cenc-ir" => JianSource.CencIr,
				"cenc-fjdzj" => JianSource.CencFjdzj,
				"ningxia" => JianSource.Ningxia,
				"yunnan" => JianSource.Yunnan,
				"shanxi" => JianSource.Shanxi,
				"beijing" => JianSource.Beijing,
				"cwa" => JianSource.Cwa,
				"jma" => JianSource.Jma,
				"hko" => JianSource.Hko,
				"kma" => JianSource.Kma,
				"tmd" => JianSource.Tmd,
				"mmd" => JianSource.Mmd,
				"bmkg" => JianSource.Bmkg,
				"geonet" => JianSource.Geonet,
				"usgs" => JianSource.Usgs,
				"emsc" => JianSource.Emsc,
				"gfz" => JianSource.Gfz,
				"bcsf" => JianSource.Bcsf,
				"ingv" => JianSource.Ingv,
				"usp" => JianSource.Usp,
				"nrcan" => JianSource.Nrcan,
				"afad" => JianSource.Afad,
				"ipma" => JianSource.Ipma,
				"noa" => JianSource.Noa,
				"sed" => JianSource.Sed,
				"scsn" => JianSource.Scsn,
				"ipgp" => JianSource.Ipgp,
				"infp" => JianSource.Infp,
				"isc" => JianSource.Isc,
				"knmi" => JianSource.Knmi,
				"ncedc" => JianSource.Ncedc,
				"lmu" => JianSource.Lmu,
				"koeri" => JianSource.Koeri,
				"gsras" => JianSource.Gsras,
				"csn" => JianSource.Csn,
				"phivolcs" => JianSource.Phivolcs,
				"ssn" => JianSource.Ssn,
				"ga" => JianSource.Ga,
				"igepn" => JianSource.Igepn,
				"jma-volcano" => JianSource.JmaVolcano,
				"usgs-volcano" => JianSource.UsgsVolcano,
				"peru" => JianSource.Peru,
				_ => JianSource.Other,
			};
		}
	}
}
