using Cryville.EEW.ComponentModel;
using JianProject.CysTerra;
using Cryville.EEW.TTS;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cryville.EEW;

namespace JianProject.CysTerra.TTS {
	/// <summary>按数据源导出独立 TTS 构建器，名称形如「Jian Project（…）」。</summary>
	public abstract class JianTtsSourceBuilder : IBuilder<IGenerator<TTSEntry>> {
		protected abstract JianSource Source { get; }

		public string? GetName([NotNull] ref CultureInfo? culture) =>
			SharedResources.SourceName(Source.ToString(), ref culture);

		public IGenerator<TTSEntry> Build(ref CultureInfo? culture) => (IGenerator<TTSEntry>)(IGenerator<TTSEntry?>)new JianTtsGenerator(Source);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianCeaTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Cea;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianCeaPrTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.CeaPr;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianCwaEewTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.CwaEew;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianJmaEewTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.JmaEew;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianJmaTsunamiTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.JmaTsunami;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianSaTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Sa;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianKmaEewTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.KmaEew;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianCencTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Cenc;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianCencFjdzjTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.CencFjdzj;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianNingxiaTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Ningxia;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianYunnanTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Yunnan;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianShanxiTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Shanxi;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianBeijingTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Beijing;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianCwaTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Cwa;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianJmaTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Jma;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianHkoTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Hko;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianKmaTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Kma;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianTmdTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Tmd;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianMmdTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Mmd;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianBmkgTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Bmkg;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianGeonetTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Geonet;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianUsgsTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Usgs;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianEmscTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Emsc;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianGfzTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Gfz;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianBcsfTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Bcsf;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianIngvTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Ingv;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianUspTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Usp;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianNrcanTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Nrcan;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianAfadTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Afad;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianIpmaTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Ipma;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianNoaTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Noa;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianSedTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Sed;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianScsnTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Scsn;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianIpgpTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Ipgp;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianInfpTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Infp;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianIscTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Isc;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianKnmiTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Knmi;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianNcedcTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Ncedc;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianLmuTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Lmu;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianKoeriTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Koeri;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianGsrasTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Gsras;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianCsnTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Csn;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianPhivolcsTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Phivolcs;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianSsnTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Ssn;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianGaTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Ga;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianIgepnTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Igepn;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianPeruTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Peru;
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianOtherTtsBuilder : JianTtsSourceBuilder {
		protected override JianSource Source => JianSource.Other;
	}

}