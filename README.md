# Jian Project API（CysTerra）

订阅 `wss://api.sismotide.top/all`。扩展只在 CysTerra 通用版可用。对齐共通端 **Alpha 0.0.15**、接口层 **Cryville.EEW 0.8.14** / Features·TTS **0.8.1**。扩展间引用走 `IsExtension`；接口包写 `ExcludeAssets=runtime`（SDK 10 下与官方自动裁剪等价，见开发文档）。，说明见 [开发文档](https://docs.eew.cryville.world/) 与 [打包](https://docs.eew.cryville.world/docs/packaging.html#versioning)。

版权所有 © 2026 纪安。本程序以 **GNU LGPL-3.0-or-later** 授权。LGPL 正文见 `LICENSE.txt`，其所引用的 GPL-3.0 正文见 `COPYING.txt`。对应源代码即本仓库；发布的扩展包内同时附带这两份许可证。

三个都要装，版本 `0.1.4.3`：

| 组件 | 导入包 |
| --- | --- |
| 事件源 | `dist/JianProject.CysTerra-0.1.4.3.zip` |
| 事物 | `dist/JianProject.CysTerra.Features-0.1.4.3.zip` |
| TTS | `dist/JianProject.CysTerra.TTS-0.1.4.3.zip` |

每个 zip 的主 dll 在根目录，可直接导入。

## 配置

- **认证密钥**：填写邮件内 Token；拓展会自动置换并保存。
- **筛选项**：按来源开关。默认全开。
- 连接后不发送列表指令。卡片机构名用正式缩写（EMSC 带测定机构，如 `EMSC | GFZ`）。地名优先 `placename_zh`。
- 事件摘要、事件详情、TTS 均按数据源单独导出（名称形如「Jian Project（…）」），可分别开关
- 设置中不展示 WebSocket 地址（固定 `wss://api.sismotide.top/all`）
- 丢弃 `type=all` / `initial` 快照帧，且连接后首帧业务数据整帧丢弃，避免 TTS 轰炸
- 不处理：Early-est、中国气象局气象预警、自然资源部海啸预警、JMA Atom 火山情报、USGS 火山警戒、NOAA/USGS 海啸预警。烈度速报不在 `/all` 里，扩展不提供该开关。日本气象厅海啸会生成海啸事件。
- 报告标题：日本气象厅为「紧急地震速报（预报/警报）」（按 `isWarn` / `infoTypeName`）；中国地震预警网、CWA、KMA、ShakeAlert 等为「地震预警」，续报为「地震预警更新」。
- 上游没有的震级、深度、坐标不会补上。
