using Cryville.EEW.ComponentModel;
using Cryville.EEW.Features;
using JianProject.CysTerra;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cryville.EEW;

namespace JianProject.CysTerra.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianFeatureGeneratorBuilder : SimpleBuilder<JianFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
