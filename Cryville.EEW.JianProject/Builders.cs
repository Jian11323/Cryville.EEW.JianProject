using Cryville.Common.Compat;
using Cryville.EEW.ComponentModel;
using Cryville.EEW.Report;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.JianProject {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public sealed class JianWsWorkerBuilder : IBuilder<JianWsWorker>, INotifyPropertyChanged {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		[JsonSerializedProperty]
		[LocalizableDisplayName("PNWebSocketUri")]
		public string? WebSocketUri { get; set; } = "wss://api.sismotide.top/all";

		[JsonSerializedProperty]
		[LocalizableDisplayName("PNAccessKey")]
		[LocalizableDescription("PDAccessKey")]
		public string? AccessKey { get; set; }

		public event PropertyChangedEventHandler? PropertyChanged;

		public JianWsWorker Build(ref CultureInfo? culture) {
			ThrowHelper.ThrowIfNull(WebSocketUri);
			if (!Uri.TryCreate(WebSocketUri, UriKind.Absolute, out var baseUri)
				|| (baseUri.Scheme != "ws" && baseUri.Scheme != "wss")) {
				throw new SourceWorkerClientException("Invalid WebSocket URI: " + WebSocketUri);
			}
			if (string.IsNullOrWhiteSpace(AccessKey)) {
				throw new SourceWorkerClientException("访问票不能为空。");
			}
			return new JianWsWorker(baseUri, AccessKey.Trim(), refresh => {
				if (string.Equals(AccessKey, refresh, StringComparison.Ordinal)) {
					return;
				}
				AccessKey = refresh;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AccessKey)));
			});
		}
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public sealed class JianReportGeneratorBuilder : SimpleBuilder<JianReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
