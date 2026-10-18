# Jian Project API（CysTerra）

订阅 `wss://api.sismotide.top/all`。扩展只在 CysTerra 通用版可用，说明见 [开发文档](https://docs.eew.cryville.world/) 与 [打包](https://docs.eew.cryville.world/docs/packaging.html#versioning)。

版权所有 © 2026 纪安。本程序以 **GNU LGPL-3.0-or-later** 授权。LGPL 正文见 `LICENSE.txt`，其所引用的 GPL-3.0 正文见 `COPYING.txt`。对应源代码即本仓库；发布的扩展包内同时附带这两份许可证。

三个都要装，版本 `0.1.4.2`：

| 组件 | 导入包 |
| --- | --- |
| 事件源 | `dist/JianProject.CysTerra-0.1.4.2.zip` |
| 事物 | `dist/JianProject.CysTerra.Features-0.1.4.2.zip` |
| TTS | `dist/JianProject.CysTerra.TTS-0.1.4.2.zip` |

每个 zip 的主 dll 在根目录，可直接导入。

## 配置

- **访问票**：填写 `lk` 并保存。插件会自动更新密钥并保存。
- **筛选项**：按来源开关。默认全开。
- 连接后不发送列表指令。卡片机构名用正式缩写（EMSC 带测定机构，如 `EMSC | GFZ`）。地名优先 `placename_zh`。
- 连接后首帧业务数据整帧丢弃
- 不处理：Early-est、中国气象局气象预警、自然资源部海啸预警、JMA Atom 火山情报、USGS 火山警戒、NOAA/USGS 海啸预警。烈度速报不在 `/all` 里，扩展不提供该开关。日本气象厅海啸会生成海啸事件。
- 报告标题：日本气象厅为「紧急地震速报（预报/警报）」（按 `isWarn` / `infoTypeName`）；中国地震预警网、CWA、KMA、ShakeAlert 等为「地震预警」，续报为「地震预警更新」。
- 上游没有的震级、深度、坐标不会补上。
