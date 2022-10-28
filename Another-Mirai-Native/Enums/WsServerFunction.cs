namespace Another_Mirai_Native.Enums
{
    /// <summary>
    /// 描述WebSocket服务器函数类型
    /// </summary>
    public enum WsServerFunction
    {
        /// <summary>
        /// 描述连接的角色类型
        /// </summary>
        Info,
        /// <summary>
        /// 添加一条日志
        /// </summary>
        AddLog,
        /// <summary>
        /// 根据日志优先级选择配置中条数的日志
        /// </summary>
        GetLog,        
        /// <summary>
        /// 调用MiraiAPI
        /// </summary>
        CallMiraiAPI,
        /// <summary>
        /// 调用类CQAPI
        /// </summary>
        CallCQFunction,
        /// <summary>
        /// 退出程序
        /// </summary>
        Exit,
        /// <summary>
        /// 重启程序
        /// </summary>
        Restart,
        /// <summary>
        /// 根据路径添加插件
        /// </summary>
        AddPlugin,
        /// <summary>
        /// 重载插件
        /// </summary>
        ReloadPlugin,
        /// <summary>
        /// 获取插件列表
        /// </summary>
        GetPluginList,
        /// <summary>
        /// 切换插件启用或禁用状态
        /// </summary>
        SwitchPluginStatus,
        /// <summary>
        /// 获取Bot QQ与昵称
        /// </summary>
        BotInfo,
        /// <summary>
        /// 获取群列表
        /// </summary>
        GetGroupList,
        /// <summary>
        /// 获取好友列表
        /// </summary>
        GetFriendList,
        /// <summary>
        /// 获取群成员列表
        /// </summary>
        GetMemberList,
        /// <summary>
        /// 获取群成员信息
        /// </summary>
        GetMemberInfo,
        /// <summary>
        /// 获取好友信息
        /// </summary>
        GetFriendInfo,
        /// <summary>
        /// 获取目录以及文件列表
        /// </summary>
        GetDirectory,
        /// <summary>
        /// 未授权连接
        /// </summary>
        UnAuth,
        /// <summary>
        /// 获取Bot宿主机状态
        /// </summary>
        Status,        
        /// <summary>
        /// 获取Bot状态图表数据
        /// </summary>
        Table,        
        /// <summary>
        /// 获取Bot宿主机信息
        /// </summary>
        DeviceInfo,
        /// <summary>
        /// 心跳
        /// </summary>
        HeartBeat,
        /// <summary>
        /// 检查是否有插件在测试
        /// </summary>
        CheckTest,
        /// <summary>
        /// 启用插件测试
        /// </summary>
        EnableTest,
        /// <summary>
        /// 关闭插件测试
        /// </summary>
        DisableTest,
        /// <summary>
        /// 发送测试消息
        /// </summary>
        SendTestMsg,
        /// <summary>
        /// 激活消息转发器
        /// </summary>
        ActiveForwarder,
        /// <summary>
        /// 取消消息转发器
        /// </summary>
        InactiveForwarder,
        /// <summary>
        /// 上传图片至本机
        /// </summary>
        UploadImage,
        /// <summary>
        /// 删除图片
        /// </summary>
        DeleteImage,
        /// <summary>
        /// 创建本地Web服务器
        /// </summary>
        BuildWebServer,
        /// <summary>
        /// 销毁本地Web服务器
        /// </summary>
        DestroyWebServer,
        SendMsg,
        GetGroupChatHistory,
        GetFriendChatHistory,
        GetChatList
    }
    public enum WsClientType
    {
        CQP,
        WebUI,
        UnAuth
    }
}
