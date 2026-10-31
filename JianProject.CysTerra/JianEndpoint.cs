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

	/// <summary>主服 api.sismotide.top / 备服 api.jianproject.cloud；鉴权仍走 auth.sismotide.top。</summary>
	static class JianEndpoint {
		public const string PrimaryHost = "api.sismotide.top";
		public const string BackupHost = "api.jianproject.cloud";

		static int _active; // 0 primary, 1 backup
		static int _failStreak;

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
			if (mode == JianEndpointMode.Primary) {
				Volatile.Write(ref _active, 0);
			} else if (mode == JianEndpointMode.Backup) {
				Volatile.Write(ref _active, 1);
			}
		}

		public static void NoteResult(bool ok, JianEndpointMode mode) {
			if (mode != JianEndpointMode.Auto) {
				if (ok) Interlocked.Exchange(ref _failStreak, 0);
				return;
			}
			if (ok) {
				Interlocked.Exchange(ref _failStreak, 0);
				return;
			}
			if (Interlocked.Increment(ref _failStreak) < 3) return;
			Interlocked.Exchange(ref _failStreak, 0);
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
