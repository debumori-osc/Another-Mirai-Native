# Websocket API
草稿版本，部分函数会出现：并未实现、有bug、返回名称与下文所写规范不符。若交互方式或传参方式让你恼火或觉得不可理喻，请帮忙指出，先在此一个滑跪。

## 消息
当连接鉴权成功之后，服务器会回应每条在指令枚举中的请求，不在指令枚举中的指令将不会回应
### 消息结构
#### 发送
```jsonc
{
    "type": 0, // 指令枚举
    "data": {
        // 参数表
    }
}
```
示例:
```jsonc
{
    "type": 8, // ReloadPlugin
    "data": {
        "authCode": 854123687
    }
}
```
#### 接收
```jsonc
{
    "Success": true, // 处理结果是否成功，如果失败Msg字段会包含错误信息
    "Data": {
        // 返回数据
    },
    "Msg": "", // 失败信息
    "Type": "" // 调用指令的名称
}
```
示例:
```jsonc
{
    "Success": false,
    "Msg": "插件不存在",
    "Type": "ReloadPlugin"
}
```

## 鉴权
### 身份
内置的WebSocket服务器提供了两种身份：`CQP`与`WebUI`，两种身份均用于调用所有接口的权限，而`WebUI`可获取与框架日志同步更新的推送，`CQP`用于在登录阶段向框架传递CQP.dll成功连接的信息，属于内置身份。所以需要额外控制框架的运行请使用`WebUI`身份

### 传递身份
在连接建立之后，发送一条如下所示的消息
```jsonc
{
    "type": 0, // Info
    "data": {
        "key": "xxxx", // 写在本地配置项中的WsServer_Key
        "role": 1 // 0表示CQP，1表示WebUI
    }
}
```
若根据返回结果判断是否鉴权成功即可

## 可用指令与参数表
### 目录
- [Info: 0](#info-0)
- [AddLog: 1](#addlog-1)
- [GetLog: 2](#getlog-2)
- [CallMiraiAPI: 3](#callmiraiapi-3)
- [CallCQFunction: 4](#callcqfunction-4)
- [Exit: 5](#exit-5)
- [Restart: 6](#restart-6)
- [AddPlugin: 7](#addplugin-7)
- [ReloadPlugin: 8](#reloadplugin-8)
- [GetPluginList: 9](#getpluginlist-9)
- [SwitchPluginStatus: 10](#switchpluginstatus-10)
- [BotInfo: 11](#botinfo-11)
- [GetGroupList: 12](#getgrouplist-12)
- [GetFriendList: 13](#getfriendlist-13)
- [GetMemberList: 14](#getmemberlist-14)
- [GetMemberInfo: 15](#getmemberinfo-15)
- [GetFriendInfo: 16](#getfriendinfo-16)
- [GetDirectory: 17](#getdirectory-17)
- [Status: 19](#status-19)
- [Table: 20](#table-20)
- [DeviceInfo: 21](#deviceinfo-21)
- [CheckTest: 23](#checktest-23)
- [EnableTest: 24](#enabletest-24)
- [DisableTest: 25](#disabletest-25)
- [SendTestMsg: 26](#sendtestmsg-26)
- [ActiveForwarder: 27](#activeforwarder-27)
- [InactiveForwarder: 28](#inactiveforwarder-28)
- [UploadImage: 29](#uploadimage-29)
- [DeleteImage: 30](#deleteimage-30)
- [BuildWebServer: 31](#buildwebserver-31)
- [DestroyWebServer: 32](#destroywebserver-32)
- [SendMsg: 33](#sendmsg-33)

### Info: 0
鉴权, 不进行鉴权无法调用任何指令
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|key|string|写在本地配置项中的`WsServer_Key`||
|role|int|0表示`CQP`，1表示`WebUI`||

### AddLog: 1
添加日志
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|priority|int|日志级别[枚举](#日志枚举)|
|msg|string|日志消息|
|type|string|日志类型|true
|authCode|int|日志来源插件的AuthCode|true

### GetLog: 2
获取日志列表
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|priority|int|最小日志级别枚举|
|itemsPerPage|int|每页获取最大条目数量|
|page|int|分页页数|
|date|array|日志时间筛选([DateTime.Parse](https://learn.microsoft.com/zh-cn/dotnet/api/system.datetime.parse)可解析的两个时间, 时间从小到大)|
|search|string|筛选词|
|sortBy|string|升序排序关键字|
|sortDesc|bool|是否降序排序|

### CallMiraiAPI: 3
调用MiraiAPI原生API，并非所有API都做了实现，只实现了所需的功能
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|type|int|MiraiAPI[枚举](#miraiapi枚举)|
|authCode|int|来源插件的AuthCode|
|args|object|各个函数所需的不同参数表，详情请查阅[代码](https://github.com/Hellobaka/Another-Mirai-Native/blob/master/Another-Mirai-Native/WsServer.cs#L759)填写具体参数列表|

### CallCQFunction: 4
实现部分CQP.dll函数
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|type|string|指令类型|
|authCode|int|来源插件的AuthCode|
|args|object|各个函数所需的不同参数表，详情请查阅[代码](https://github.com/Hellobaka/Another-Mirai-Native/blob/master/Another-Mirai-Native/WsServer.cs#L713)填写具体参数列表|

#### 可用CQP函数
|名称|含义|
|----|----|
|GetAppDirectory|获取插件的数据路径|
|GetLoginQQ|获取框架登录的QQ号|
|GetLoginNick|获取框架登录的QQ昵称|
|GetImage|通过CQ码下载图片|
|GetGroupInfo|获取群信息|

### Exit: 5
强制退出框架，不调用插件结束流程

### Restart: 6
重启框架，关闭当前进程并重新启动

### AddPlugin: 7
根据插件的dll绝对路径添加插件
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|path|string|插件dll的绝对路径|

### ReloadPlugin: 8
重载插件
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|authCode|int|需要重载单个插件时传递此参数，重载所有插件则不添加此参数|true

### GetPluginList: 9
获取插件列表

### SwitchPluginStatus: 10
切换插件的禁用或启用状态
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|authCode|int|需要切换插件的AuthCode|
|status|bool|启用或禁用|

### BotInfo: 11
获取Bot QQ号与昵称

### GetGroupList: 12
获取群列表

### GetFriendList: 13
获取好友列表
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|reserved|bool|是否倒序|

### GetMemberList: 14
获取群成员列表
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|groupId|long|需要获取列表的群号|

### GetMemberInfo: 15
获取群成员信息
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|groupId|long|需要获取列表的群号|
|qqId|long|需要获取的QQ号|

### GetFriendInfo: 16
获取好友信息
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|qqId|long|需要获取的QQ号|

### GetDirectory: 17
根据传入的路径, 获取子目录以及文件列表
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|dir|string|绝对路径，若传递为空则传递驱动器列表|
|filter|string|文件类型过滤|

### Status: 19
获取宿主机CPU占用率 频率、剩余内存、句柄线程数量、消息处理速度、系统启动时间、框架启动时间、插件数量的信息

### Table: 20
获取CPU、内存、消息速度的一分钟内表格数据点

### DeviceInfo: 21
获取宿主机系统信息、CPU数量、总内存大小的信息

### CheckTest: 23
获取当前正在测试插件的信息，若没有则返回空数据

### EnableTest: 24
启用插件测试，此时插件将不会处理框架发送的消息，只会处理测试的消息，若超过5分钟无操作则会自动退出测试。同时只能有一个插件处于测试状态
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|authCode|int|测试插件的AuthCode|

### DisableTest: 25
退出测试，不需要单独指定

### SendTestMsg: 26
向测试插件投递消息
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|msg|string|消息内容，允许CQ码|
|isGroup|bool|消息来源是否为群聊|
|groupId|long|群ID|当`isGroup`为`false`时可空
|qqId|long|QQ|

### ActiveForwarder: 27
激活消息转发器，当消息到达框架时，会将消息转发给此连接，Type为`Msg`

### InactiveForwarder: 28
销毁消息转发器

### UploadImage: 29
向框架上传图片，之后会返回可用的文件名
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|base64|string|图片base64编码|

### DeleteImage: 30
删除上传的图片
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|fileName|string|通过上传获取的文件名|

### BuildWebServer: 31
激活内置Web服务器

### DestroyWebServer: 32
销毁内置Web服务器

### SendMsg: 33
调用发送消息API
删除上传的图片
|参数名称|类型|描述|是否可空|
|----|----|----|----|
|isGroup|bool|是否是群消息|
|groupId|long|群ID|若`isGroup`为`false`时可空
|qqId|long|QQ|
|message|string|欲发送的消息|

## 日志枚举
|日志级别|名称|
|----|----|
|0|Debug|
|10|Info|
|11|InfoSuccess|
|12|InfoReceive|
|13|InfoSend|
|20|Warning|
|30|Error|
|40|Fatal|

## MiraiAPI枚举
|枚举|名称|
|----|----|
|0|about|
|1|botList|
|2|messageFromId|
|3|friendList|
|4|groupList|
|5|memberList|
|6|botProfile|
|7|friendProfile|
|8|memberProfile|
|9|userProfile|
|10|sendFriendMessage|
|11|sendGroupMessage|
|12|sendTempMessage|
|13|sendNudge|
|14|recall|
|15|roamingMessages|
|16|file_list|
|17|file_info|
|18|file_mkdir|
|19|file_delete|
|20|file_move|
|21|file_rename|
|22|deleteFriend|
|23|mute|
|24|unmute|
|25|kick|
|26|quit|
|27|muteAll|
|28|unmuteAll|
|29|setEssence|
|30|groupConfig_get|
|31|groupConfig_update|
|32|memberInfo_get|
|33|memberInfo_update|
|34|memberAdmin|
|35|anno_list|
|36|anno_publish|
|37|anno_delete|
|38|resp_newFriendRequestEvent|
|39|resp_memberJoinRequestEvent|
|40|resp_botInvitedJoinGroupRequestEvent|