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

		/// <summary>显示名：Jian Project（{数据源}），对齐官方 FAN Studio 按源拆分 TTS。</summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string? SourceName(string type, [NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var typed = res.GetStringSetRequired("SourceNameTypes").GetString(type) ?? type;
			return string.Format(culture, res.GetStringRequired("SourceNameTyped"), typed);
		}
	}
}
