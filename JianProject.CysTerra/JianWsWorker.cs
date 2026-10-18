using Cryville.EEW.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Cryville.EEW;

namespace JianProject.CysTerra {
	public sealed class JianWsWorker : WebSocketWorker, ISourceWorker, IPropertiesHolder {
		readonly string _credential;
		readonly Action<string>? _saveRefresh;
		readonly Dictionary<string, string> _seen = new();
		ISet<JianSource> _filter = JianSources.DefaultAll();
		// Fan Studio /all 首包为 initial 快照；本站 /all 首包为 type=all。连接后第一帧业务数据整帧丢弃（只记指纹，不进卡片/TTS）。
		bool _dropFirstBusiness = true;

		public JianWsWorker(Uri uri, string credential, Action<string>? saveRefresh) : base(uri) {
			_credential = credential;
			_saveRefresh = saveRefresh;
		}

		[Obsolete("Filter is always whitelist.")]
		[Browsable(false)]
		public bool IsFilterWhitelist { get; set; }

		[LocalizableDisplayName("PNFilter")]
		[LocalizableDescription("PDFilter")]
		[SuppressMessage("Usage", "CA2227", Justification = "Component property")]
		public ISet<JianSource> Filter {
			get => _filter;
			set => _filter = value ?? new HashSet<JianSource>();
		}

		public IEnumerable<PropertyDescriptor> GetProperties() =>
			TypeDescriptor.GetProperties(this).OfType<PropertyDescriptor>().Where(p => p.IsBrowsable && !p.IsReadOnly);

		protected override Uri GetUri() => WithKey(InitialUri, JianAuth.AccessFor(_credential, _saveRefresh));

		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		public event Handler<object?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override Task OnConnected(CancellationToken cancellationToken) {
			_dropFirstBusiness = true;
			return Task.CompletedTask;
		}

		protected override async Task Handle(Stream stream, WebSocketMessageType messageType, CancellationToken cancellationToken) {
			if (messageType != WebSocketMessageType.Text) {
				return;
			}
			try {
				using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(true);
				var root = doc.RootElement;
				var type = root.ValueKind == JsonValueKind.Object && root.TryGetProperty("type", out var typeEl) && typeEl.ValueKind == JsonValueKind.String
					? typeEl.GetString()
					: null;
				var events = JianParser.Parse(root, Accept).ToList();
				if (events.Count == 0) {
					return;
				}
				// Fan Studio: initial 整帧丢弃；本站: type=all 快照 + 连接后首帧业务（含单源 ProvideLatest）
				var silent = _dropFirstBusiness
					|| string.Equals(type, "all", StringComparison.Ordinal)
					|| string.Equals(type, "initial", StringComparison.Ordinal);
				if (silent) {
					_dropFirstBusiness = false;
					foreach (var evt in events) {
						_seen[evt.UnitId] = evt.Fingerprint;
					}
					return;
				}
				foreach (var evt in events) {
					if (_seen.TryGetValue(evt.UnitId, out var prev) && prev == evt.Fingerprint) {
						continue;
					}
					_seen[evt.UnitId] = evt.Fingerprint;
					Received?.Invoke(this, evt);
				}
			}
			catch (JsonException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
		}

		bool Accept(string agency) => JianSources.Allows(_filter, agency);

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

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, Cryville.EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);

	}
}
