using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Cryville.EEW;

namespace JianProject.CysTerra {
	public static class SharedResources {
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string? SourceName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			return lres.RootMessageStringSet.GetStringRequired("SourceName");
		}
	}
}
