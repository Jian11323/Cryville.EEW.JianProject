using Cryville.EEW.ComponentModel;
using JianProject.CysTerra;
using Cryville.EEW.TTS;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cryville.EEW;

namespace JianProject.CysTerra.TTS {
	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public sealed class JianTtsGeneratorBuilder : SimpleBuilder<JianTtsGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
