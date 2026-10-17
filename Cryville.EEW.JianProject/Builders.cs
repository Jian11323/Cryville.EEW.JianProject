using Cryville.Common.Compat;
using Cryville.EEW.ComponentModel;
using Cryville.EEW.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.JianProject {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public sealed class JianWsWorkerBuilder : IBuilder<JianWsWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		[JsonSerializedProperty]
		[LocalizableDisplayName("PNWebSocketUri")]
		public string? WebSocketUri { get; set; } = "wss://api.sismotide.top/all";

		[JsonSerializedProperty]
		[LocalizableDisplayName("PNAccessKey")]
		[LocalizableDescription("PDAccessKey")]
		public string? AccessKey { get; set; }

		[JsonSerializedProperty]
		[LocalizableDisplayName("PNFilter")]
		[LocalizableDescription("PDFilter")]
		public string? Filter { get; set; }

		public JianWsWorker Build(ref CultureInfo? culture) {
			ThrowHelper.ThrowIfNull(WebSocketUri);
			if (!Uri.TryCreate(WebSocketUri, UriKind.Absolute, out var baseUri)
				|| (baseUri.Scheme != "ws" && baseUri.Scheme != "wss")) {
				throw new SourceWorkerClientException("Invalid WebSocket URI: " + WebSocketUri);
			}
			if (string.IsNullOrWhiteSpace(AccessKey)) {
				throw new SourceWorkerClientException("Access key (at_) is required.");
			}
			return new JianWsWorker(WithKey(baseUri, AccessKey.Trim()), Filter);
		}

		static Uri WithKey(Uri uri, string key) {
			var builder = new UriBuilder(uri);
			var query = builder.Query;
			if (query.StartsWith("?", StringComparison.Ordinal)) {
				query = query.Substring(1);
			}
			var parts = new List<string>();
			if (query.Length > 0) {
				foreach (var part in query.Split('&')) {
					if (part.Length == 0 || part.StartsWith("key=", StringComparison.Ordinal)) {
						continue;
					}
					parts.Add(part);
				}
			}
			parts.Add("key=" + Uri.EscapeDataString(key));
			builder.Query = string.Join("&", parts);
			return builder.Uri;
		}
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public sealed class JianReportGeneratorBuilder : SimpleBuilder<JianReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
