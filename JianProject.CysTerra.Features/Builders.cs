using Cryville.EEW.ComponentModel;
using Cryville.EEW.Features;
using JianProject.CysTerra;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cryville.EEW;

namespace JianProject.CysTerra.Features {
	/// <summary>按数据源导出独立事件详情构建器。</summary>
	public abstract class JianFeatureSourceBuilder : IBuilder<IGenerator<Feature>> {
		protected abstract JianSource Source { get; }

		public string? GetName([NotNull] ref CultureInfo? culture) =>
			SharedResources.SourceName(Source.ToString(), ref culture);

		public IGenerator<Feature> Build(ref CultureInfo? culture) =>
			(IGenerator<Feature>)(IGenerator<Feature?>)new JianFeatureGenerator(Source);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianCeaFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Cea;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianCeaPrFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.CeaPr;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianCwaEewFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.CwaEew;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianJmaEewFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.JmaEew;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianJmaTsunamiFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.JmaTsunami;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianSaFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Sa;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianKmaEewFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.KmaEew;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianCencFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Cenc;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianCencFjdzjFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.CencFjdzj;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianNingxiaFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Ningxia;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianYunnanFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Yunnan;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianShanxiFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Shanxi;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianBeijingFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Beijing;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianCwaFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Cwa;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianJmaFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Jma;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianHkoFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Hko;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianKmaFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Kma;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianTmdFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Tmd;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianMmdFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Mmd;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianBmkgFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Bmkg;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianGeonetFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Geonet;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianUsgsFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Usgs;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianEmscFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Emsc;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianGfzFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Gfz;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianBcsfFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Bcsf;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianIngvFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Ingv;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianUspFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Usp;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianNrcanFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Nrcan;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianAfadFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Afad;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianIpmaFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Ipma;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianNoaFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Noa;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianSedFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Sed;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianScsnFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Scsn;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianIpgpFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Ipgp;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianInfpFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Infp;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianIscFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Isc;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianKnmiFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Knmi;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianNcedcFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Ncedc;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianLmuFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Lmu;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianKoeriFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Koeri;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianGsrasFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Gsras;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianCsnFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Csn;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianPhivolcsFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Phivolcs;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianSsnFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Ssn;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianGaFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Ga;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianIgepnFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Igepn;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianPeruFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Peru;
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianOtherFeatureBuilder : JianFeatureSourceBuilder {
		protected override JianSource Source => JianSource.Other;
	}
}
