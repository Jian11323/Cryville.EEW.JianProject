using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.JianProject {
	public sealed class JianWsWorker : WebSocketWorker, ISourceWorker {
		static readonly string[] ListCommands = { "cenclist", "cwalist", "jmalist", "hkolist" };

		readonly HashSet<string>? _filter;
		readonly Dictionary<string, string> _seen = new();
		Timer? _listTimer;

		public JianWsWorker(Uri uri, string? filter) : base(uri) {
			_filter = ParseFilter(filter);
		}

		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		public event Handler<object?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override async Task OnConnected(CancellationToken cancellationToken) {
			await SendListsAsync(cancellationToken).ConfigureAwait(false);
			_listTimer?.Dispose();
			_listTimer = new Timer(_ => {
				_ = SendListsSafeAsync(CancellationToken.None);
			}, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
		}

		async Task SendListsSafeAsync(CancellationToken cancellationToken) {
			try {
				await SendListsAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex) when (ex is not OperationCanceledException) {
				ErrorEmitted?.Invoke(this, ex);
			}
		}

		async Task SendListsAsync(CancellationToken cancellationToken) {
			foreach (var cmd in ListCommands) {
				using var stream = new MemoryStream(Encoding.UTF8.GetBytes(cmd));
				await SendAsync(stream, WebSocketMessageType.Text, cancellationToken).ConfigureAwait(false);
			}
		}

		protected override async Task Handle(Stream stream, WebSocketMessageType messageType, CancellationToken cancellationToken) {
			if (messageType != WebSocketMessageType.Text) {
				return;
			}
			try {
				using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(true);
				foreach (var evt in JianParser.Parse(doc.RootElement, Accept)) {
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

		bool Accept(string agency) {
			if (_filter == null) {
				return true;
			}
			var key = agency.Trim().ToLowerInvariant();
			if (_filter.Contains(key)) {
				return true;
			}
			var head = key.Split(':')[0];
			return _filter.Contains(head);
		}

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);

		protected override void Dispose(bool disposing) {
			if (disposing) {
				_listTimer?.Dispose();
				_listTimer = null;
			}
			base.Dispose(disposing);
		}

		static HashSet<string>? ParseFilter(string? filter) {
			if (string.IsNullOrWhiteSpace(filter)) {
				return null;
			}
			var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (var part in filter.Split(new[] { ',', ';', ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)) {
				set.Add(part.Trim().ToLowerInvariant());
			}
			return set.Count == 0 ? null : set;
		}
	}
}
