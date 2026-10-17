# Jian Project API for CysTerra

CysTerra 通用版扩展，订阅 [Jian Project API](https://api.sismotide.top) 的 `wss://api.sismotide.top/all`，接收该聚合口下发的全部事件源（预警、速报、海啸、气象等）。测站实时流不在 `/all` 上，本扩展不连接测站口。

协议：[GNU LGPL v3](LICENSE.txt)。

开发说明见 [CysTerra 扩展文档](https://docs.eew.cryville.world/)，组件划分对齐 [官方扩展仓库](https://github.com/cryville/CysTerra.ExtensionRepo)。

| 组件 | 程序集 | 作用 |
| --- | --- | --- |
| Jian Project事件源组件 | `Cryville.EEW.JianProject` | 连接数据源并生成事件摘要 |
| Jian Project事物组件 | `Cryville.EEW.JianProject.Features` | 生成事件详情与地图要素 |
| Jian Project TTS组件 | `Cryville.EEW.JianProject.TTS` | 生成语音文本 |

三个扩展都需要安装。事件源组件里填写 `at_` 访问票；筛选项留空即接收全部机构。连接后会发送 `cenclist`、`cwalist`、`jmalist`、`hkolist`，并每 60 秒再拉一次。上游没有的震级、深度、坐标不会补写。

```text
dotnet build Cryville.EEW.JianProject.sln -c Release
```

产物在各项目的 `bin/ext/Release`（或 `bin/Release`）。把三个扩展目录装进 CysTerra 通用版即可。
