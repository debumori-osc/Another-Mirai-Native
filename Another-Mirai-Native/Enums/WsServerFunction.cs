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
        /// 获取目录以及文件列表
        /// </summary>
        GetDirectroy,
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
        HeartBeat
    }
    public enum WsClientType
    {
        CQP,
        WebUI,
        UnAuth
    }
}
