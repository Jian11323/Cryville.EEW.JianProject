using Cryville.EEW.ComponentModel;
using System;
using System.ComponentModel;
using System.Threading;

namespace JianProject.CysTerra {
	public enum JianEndpointMode {
		[LocalizableDisplayName(nameof(Auto), Path = ["EndpointModes"])] Auto = 0,
		[LocalizableDisplayName(nameof(Primary), Path = ["EndpointModes"])] Primary = 1,
		[LocalizableDisplayName(nameof(Backup), Path = ["EndpointModes"])] Backup = 2,
	}

	/// <summary>
	/// 主服务器 api.sismotide.top / 备服务器 api.jianproject.cloud。
	/// WebSocketWorker 无内置切服；自动切换由 OnError 识别 SourceWorkerNetworkException 后改写下一次 GetUri。
	/// </summary>
	static class JianEndpoint {
		public const string PrimaryHost = "api.sismotide.top";
		public const string BackupHost = "api.jianproject.cloud";

		/// <summary>短时间窗口内累计多少次网络异常后切换。</summary>
		const int FailThreshold = 3;
		/// <summary>网络异常计数窗口（毫秒）。</summary>
		const int FailWindowMs = 60_000;

		static int _active; // 0 primary, 1 backup
		static int _failStreak;
		static long _windowStartMs;

		static long NowMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		public static string ActiveHost(JianEndpointMode mode) {
			switch (mode) {
				case JianEndpointMode.Primary: return PrimaryHost;
				case JianEndpointMode.Backup: return BackupHost;
				default:
					return Volatile.Read(ref _active) == 1 ? BackupHost : PrimaryHost;
			}
		}

		public static void Reset(JianEndpointMode mode) {
			Interlocked.Exchange(ref _failStreak, 0);
			Volatile.Write(ref _windowStartMs, 0);
			if (mode == JianEndpointMode.Primary) {
				Volatile.Write(ref _active, 0);
			} else if (mode == JianEndpointMode.Backup) {
				Volatile.Write(ref _active, 1);
			}
			// Auto：保留当前 _active，避免热重建冲掉已切换状态
		}

		/// <summary>连接成功：清空网络失败计数。</summary>
		public static void NoteSuccess(JianEndpointMode mode) {
			if (mode != JianEndpointMode.Auto) {
				return;
			}
			Interlocked.Exchange(ref _failStreak, 0);
			Volatile.Write(ref _windowStartMs, 0);
		}

		/// <summary>
		/// 仅统计 SourceWorkerNetworkException（短时间多次 → 切换主/备，供下次 GetUri 使用）。
		/// </summary>
		public static void NoteNetworkFailure(JianEndpointMode mode) {
			if (mode != JianEndpointMode.Auto) {
				return;
			}
			var now = NowMs();
			var windowStart = Volatile.Read(ref _windowStartMs);
			if (windowStart == 0 || now - windowStart > FailWindowMs) {
				Volatile.Write(ref _windowStartMs, now);
				Interlocked.Exchange(ref _failStreak, 1);
				return;
			}
			if (Interlocked.Increment(ref _failStreak) < FailThreshold) {
				return;
			}
			Interlocked.Exchange(ref _failStreak, 0);
			Volatile.Write(ref _windowStartMs, 0);
			var cur = Volatile.Read(ref _active);
			Volatile.Write(ref _active, cur == 0 ? 1 : 0);
		}

		public static Uri Rewrite(Uri uri, JianEndpointMode mode) {
			if (uri == null) throw new ArgumentNullException(nameof(uri));
			var host = uri.Host;
			if (!host.Equals(PrimaryHost, StringComparison.OrdinalIgnoreCase)
				&& !host.Equals(BackupHost, StringComparison.OrdinalIgnoreCase)) {
				return uri;
			}
			var target = ActiveHost(mode);
			if (host.Equals(target, StringComparison.OrdinalIgnoreCase)) return uri;
			var b = new UriBuilder(uri) { Host = target };
			return b.Uri;
		}
	}
}
