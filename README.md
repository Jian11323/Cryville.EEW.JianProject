# Jian Project API（CysTerra）

订阅 `wss://api.sismotide.top/all`。扩展只在 CysTerra 通用版可用，说明见 [开发文档](https://docs.eew.cryville.world/) 与 [打包](https://docs.eew.cryville.world/docs/packaging.html#versioning)。

版权所有 © 2026 纪安。本程序以 **GNU LGPL-3.0-or-later** 授权。LGPL 正文见 `LICENSE.txt`，其所引用的 GPL-3.0 正文见 `COPYING.txt`。对应源代码即本仓库；发布的扩展包内同时附带这两份许可证。

三个都要装，版本 `0.1.3.0`：

| 组件 | 导入包 |
| --- | --- |
| 事件源 | `dist/Cryville.EEW.JianProject-0.1.3.0.zip` |
| 事物 | `dist/Cryville.EEW.JianProject.Features-0.1.3.0.zip` |
| TTS | `dist/Cryville.EEW.JianProject.TTS-0.1.3.0.zip` |

每个 zip 的主 dll 在根目录，可直接导入。

## 配置

- **访问票**：`lk_`、`rt_` 或 `at_`。`lk_` 只向 `auth.sismotide.top` 换一次长期 `rt_`，写入访问票和本机 `%LocalAppData%\\Cryville\\EEW\\JianProject\\refresh-token.json`。断线重连用这份 `rt_` 再换一小时的 `at_`。
- **筛选项**：按来源开关。默认全开。
- 连接后不发送列表指令。卡片机构名用正式缩写（EMSC 带测定机构，如 `EMSC | GFZ`）。地名优先 `placename_zh`。
- 不接收气象预警、火山、Early-est，也不接收日本气象厅以外的海啸。烈度速报不在 `/all` 里，扩展不提供该开关。日本气象厅海啸会生成海啸事件。
- 上游没有的震级、深度、坐标不会补上。

```text
dotnet build Cryville.EEW.JianProject.sln -c Release
```

