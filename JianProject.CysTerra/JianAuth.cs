using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Cryville.EEW;

namespace JianProject.CysTerra {
	/// <summary>
	/// lk_ 只换一次长期 rt_ 并落盘（签发固定主站）；连接用 rt_ 换约 1 小时的 at_。
	/// 连备服 API 时 at_ 须在备服鉴权口签发，否则备服本地票仓尚未同步主站新 at_ 会判无效。
	/// </summary>
	static class JianAuth {
		const string PrimaryAccessUrl = "https://auth.sismotide.top/api/access";
		const string BackupAccessUrl = "https://auth.jianproject.cloud/api/access";
		const string RefreshUrl = "https://auth.sismotide.top/api/refresh";

		static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
		static readonly object Gate = new object();
		static string? _refresh;
		static string? _access;
		static string? _accessUrlUsed;
		static DateTime _accessUntilUtc;

		public static string AccessUrlForApi(string apiHost) {
			if (!string.IsNullOrEmpty(apiHost)
				&& apiHost.Equals(JianEndpoint.BackupHost, StringComparison.OrdinalIgnoreCase)) {
				return BackupAccessUrl;
			}
			return PrimaryAccessUrl;
		}

		public static string AccessFor(string credential, Action<string>? saveRefresh, string? apiHost = null) {
			var value = credential.Trim();
			var accessUrl = AccessUrlForApi(apiHost ?? JianEndpoint.PrimaryHost);
			if (value.StartsWith("at_", StringComparison.Ordinal)) {
				// 备服票仓不认主站新签发的 at_；有本地 rt_ 则改走备服换票口。
				if (string.Equals(accessUrl, BackupAccessUrl, StringComparison.OrdinalIgnoreCase)) {
					var saved = Read();
					if (saved.Refresh.StartsWith("rt_", StringComparison.Ordinal)) {
						value = saved.Refresh;
					} else {
						throw new SourceWorkerClientException(
							"备服务器不能直接使用 at_；请填写 lk_ 或 rt_，由扩展在备服鉴权口换票。");
					}
				} else {
					return value;
				}
			}
			lock (Gate) {
				var refresh = RefreshLocked(value, saveRefresh);
				if (_access != null
					&& string.Equals(_refresh, refresh, StringComparison.Ordinal)
					&& string.Equals(_accessUrlUsed, accessUrl, StringComparison.OrdinalIgnoreCase)
					&& DateTime.UtcNow < _accessUntilUtc) {
					return _access;
				}
				var issued = Exchange(accessUrl, refresh, "at_");
				_refresh = refresh;
				_access = issued.Token;
				_accessUrlUsed = accessUrl;
				var keep = issued.ExpiresSec > 90 ? issued.ExpiresSec - 60 : Math.Max(30, issued.ExpiresSec / 2);
				_accessUntilUtc = DateTime.UtcNow.AddSeconds(keep);
				return _access;
			}
		}

		static string RefreshLocked(string value, Action<string>? saveRefresh) {
			if (value.StartsWith("rt_", StringComparison.Ordinal)) {
				Remember(null, value, saveRefresh);
				return value;
			}
			if (!value.StartsWith("lk_", StringComparison.Ordinal)) {
				throw new SourceWorkerClientException("访问票须为 lk_、rt_ 或 at_。");
			}
			var saved = Read();
			if (saved.Login == value && saved.Refresh.StartsWith("rt_", StringComparison.Ordinal)) {
				_refresh = saved.Refresh;
				saveRefresh?.Invoke(saved.Refresh);
				return saved.Refresh;
			}
			var issued = Exchange(RefreshUrl, value, "rt_");
			Remember(value, issued.Token, saveRefresh);
			_access = null;
			_accessUrlUsed = null;
			return issued.Token;
		}

		static void Remember(string? login, string refresh, Action<string>? saveRefresh) {
			var previous = Read();
			var boundLogin = login ?? (previous.Refresh == refresh ? previous.Login : "");
			Write(boundLogin, refresh);
			_refresh = refresh;
			saveRefresh?.Invoke(refresh);
		}

		static string StorePath() {
			var dir = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"JianProject", "CysTerra");
			Directory.CreateDirectory(dir);
			var path = Path.Combine(dir, "refresh-token.json");
			if (!File.Exists(path)) {
				// 兼容旧版 Cryville.EEW.JianProject 落盘位置
				var legacy = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
					"Cryville", "EEW", "JianProject", "refresh-token.json");
				if (File.Exists(legacy)) {
					File.Copy(legacy, path);
				}
			}
			return path;
		}

		static (string Login, string Refresh) Read() {
			try {
				var path = StorePath();
				if (!File.Exists(path)) {
					return ("", "");
				}
				using var doc = JsonDocument.Parse(File.ReadAllText(path));
				var root = doc.RootElement;
				var login = root.TryGetProperty("login", out var loginEl) ? loginEl.GetString() ?? "" : "";
				var refresh = root.TryGetProperty("refresh", out var refreshEl) ? refreshEl.GetString() ?? "" : "";
				return (login.Trim(), refresh.Trim());
			}
			catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException) {
				return ("", "");
			}
		}

		static void Write(string login, string refresh) {
			var path = StorePath();
			var json = "{\"login\":" + JsonSerializer.Serialize(login) + ",\"refresh\":" + JsonSerializer.Serialize(refresh) + "}";
			var temp = path + ".tmp";
			File.WriteAllText(temp, json);
			if (File.Exists(path)) {
				File.Delete(path);
			}
			File.Move(temp, path);
		}

		static Issued Exchange(string url, string bearer, string expectPrefix) {
			using var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + bearer);
			request.Headers.TryAddWithoutValidation("Accept", "application/json");
			using var response = Http.SendAsync(request).GetAwaiter().GetResult();
			var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			JsonDocument doc;
			try {
				doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(body) ? "{}" : body);
			}
			catch (JsonException) {
				throw new SourceWorkerClientException("换票失败");
			}
			using (doc) {
				var root = doc.RootElement;
				if (!response.IsSuccessStatusCode || (root.TryGetProperty("ok", out var ok) && ok.ValueKind == JsonValueKind.False)) {
					var message = root.TryGetProperty("message", out var msg) ? msg.GetString() : null;
					throw new SourceWorkerClientException(string.IsNullOrEmpty(message) ? "换票失败" : message);
				}
				if (!root.TryGetProperty("token", out var tokenEl)) {
					throw new SourceWorkerClientException("换票响应没有 token");
				}
				var token = tokenEl.GetString()?.Trim() ?? "";
				if (!token.StartsWith(expectPrefix, StringComparison.Ordinal)) {
					throw new SourceWorkerClientException("换票结果不是 " + expectPrefix + "。");
				}
				var seconds = 3600;
				if (root.TryGetProperty("expires_after_sec", out var secEl) && secEl.TryGetInt32(out var sec) && sec > 0) {
					seconds = sec;
				}
				return new Issued(token, seconds);
			}
		}

		readonly struct Issued {
			public Issued(string token, int expiresSec) {
				Token = token;
				ExpiresSec = expiresSec;
			}
			public string Token { get; }
			public int ExpiresSec { get; }
		}
	}
}
