using Cryville.EEW.ComponentModel;
using Cryville.EEW.Features;
using Cryville.EEW.JianProject;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.JianProject.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public sealed class JianFeatureGeneratorBuilder : SimpleBuilder<JianFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
