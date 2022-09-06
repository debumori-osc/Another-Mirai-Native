namespace Another_Mirai_Native.Enums
{
    public enum WsServerFunction
    {
        Info,
        AddLog,
        GetLog,        
        CallMiraiAPI,
        CallCQFunction,
        Exit,
        Restart,
        AddPlugin,
        ReloadPlugin,
        GetPluginList,
        SwitchPluginStatus,
        GetBotInfo,
        GetGroupList,
        GetFriendList,
        GetStatus,
        GetDirectroy,
        UnAuth
    }
    public enum WsClientType
    {
        CQP,
        WebUI,
        UnAuth
    }
}
