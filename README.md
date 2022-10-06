# AnotherMiraiNative（AMN）
[![ProjectBuild](https://github.com/Hellobaka/Another-Mirai-Native/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/Hellobaka/Another-Mirai-Native/actions/workflows/dotnet-desktop.yml)

来了，又是一个酷Q兼容项目

## 特点
相较于`Mirai-Native`具有以下特点：
- 由插件异常导致程序崩溃时，不会导致`Mirai`框架崩溃
- 图形化界面
- 可对单个插件重载
- 可对单个插件进行消息逻辑测试，省去在群内发消息
- 运行环境隔离，.net插件不会发生依赖冲突
- 服务器端可使用64位的JRE了

## 依赖
- .netframework48
- mirai-api-http v2

## 安装与配置mirai-api-http
https://github.com/project-mirai/mirai-api-http#安装mirai-api-http
- 开放`ws`，配置`verifyKey`
- 如需公网访问，请将`ws-host`设置为`0.0.0.0`，服务器开放ws的监听端口

## 使用流程
1. 下载最老版本的[Release](https://github.com/Hellobaka/Another-Mirai-Native/releases/download/1.5.0/Release.zip)并解压
2. 下载最新版本的[Release](https://github.com/Hellobaka/Another-Mirai-Native/releases/latest)，按照发行说明替换对应文件
3. 启动`AnotherMiraiNative.exe`，填入对应的配置，关于`ws`与`Authkey`在上一节

## 音频支持
https://github.com/Hellobaka/Another-Mirai-Native/releases/tag/1.5.5 下载其中的`tools.7z`并按说明解压即可

## 命令行参数
- `-i`：忽略进程检查
- `-q`：提供自动登录的QQ号
- `-ws`：提供自动登录的ws连接
- `-wsk`：提供自动登录的Authkey

- 程序默认阻止启动多个同名进程
- 当使用自动登录参数时，若缺少参数将无法启动程序

## 配置文件
|配置键|含义|类型|默认值|
|----|----|----|----|
|AutoLogin|启动程序时是否自动连接|bool|false|
|QQ|默认填入的QQ号|long||
|Ws_Url|默认填入的Ws_Url|string||
|Ws_AuthKey|默认填入的Ws_AuthKey|string||
|Ws_ServerPort|本地Ws服务器需要的端口|ushort|30303|
|MaxLogCount|日志窗口最大日志数量|int|500|
|FloatWindow_Location|悬浮窗位置|string(示例: 1841,903)||
|FloatWindow_Visible|悬浮窗是否可见|bool|false|
|FloatWindow_TopMost|悬浮窗是否置顶|bool|false|
|Tester_GroupID|最后一次插件测试的群号|long|随机生成|
|Tester_QQID|最后一次插件测试的QQ号|long|随机生成|
|API_Timeout|接口调用超时最大时长(100=1s)|int|6000(60s)|
|Enable_UsageMonitor|是否启用性能检测模块|bool|false|

## 内置WebSocket接口
[使用文档](https://github.com/Hellobaka/Another-Mirai-Native/blob/master/Wsapi.md)

### 交流群
671467200

## 待实现功能
- [ ] WebUI
- [ ] 糊的所有功能真的可用吗？

## 已知的问题
- ~~插件初始加载，点击启用时会有文件占用提示~~
- ~~禁言事件无法触发~~
- ~~事件日志不完备~~
- ~~没有发送音频支持~~
- 函数调用失败时，没有对应提示，比如bot被禁言、发送消息至未加入的群等
- ~~日志窗口无法调整尺寸~~
- ~~插件切换禁用启用时，只是执行了插件的禁用启用事件，而不是真正的重新加载，并且插件不能保证在卸载插件时能够清理所有冗余，不能保证即使多次执行启动事件也不会报错~~
- ~~某些情况下，日志窗口中的日志容器尺寸会溢出~~
- ~~奇怪的半夜与消息服务器断开，虽然显示重新连接但没有消息再刷新~~
- 易语言或C++编写的插件窗口有时无法正常拉起
