using Another_Mirai_Native.Adapter;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Another_Mirai_Native
{
    public class WsServer
    {
        public class ApiResult
        {
            public bool success { get; set; } = true;
            public string data { get; set; }
        }
        public class Handler : WebSocketBehavior
        {
            private WsClientType ClientType = WsClientType.UnAuth;
            protected override void OnMessage(MessageEventArgs e)
            {
                Stopwatch sw = new();
                sw.Start();
                JObject json = JObject.Parse(e.Data);
                var data = json["data"];
                switch (json["type"].ToObject<WsServerFunction>())
                {
                    case WsServerFunction.Info:
                        switch (data["role"].ToObject<WsClientType>())
                        {
                            case WsClientType.CQP:
                            case WsClientType.WebUI:
                                string key = data["key"].ToString();
                                if (key == ConfigHelper.GetConfig<string>("WsServer_Key"))
                                {
                                    ClientType = data["role"].ToObject<WsClientType>();
                                    Send(new ApiResult { data = "ok" }.ToJson());
                                    if (ClientType == WsClientType.CQP)
                                    {
                                        Instance.CQPConnected?.Invoke();
                                    }
                                }
                                break;
                            case WsClientType.UnAuth:
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                if(ClientType == WsClientType.UnAuth)
                {
                    Send(WsServerFunction.UnAuth, "");
                    return;
                }
                int logid = 0;
                switch (json["type"].ToObject<WsServerFunction>())
                {
                    case WsServerFunction.AddLog:
                        break;
                    case WsServerFunction.GetLog:
                        break;
                    case WsServerFunction.CallMiraiAPI:
                        var apiType = json["data"]["type"].ToObject<MiraiApiType>();
                        int authcode = json["data"]["args"]["authcode"].ToObject<int>();
                        var plugin = PluginManagment.Instance.Plugins.Find(x => x.appinfo.AuthCode == authcode);
                        if (plugin == null)
                        {
                            Send(new ApiResult { success = false }.ToJson());
                            LogHelper.WriteLog("Authcode无效", "");
                            return;
                        }
                        switch (apiType)
                        {
                            case MiraiApiType.about:
                                break;
                            case MiraiApiType.botList:
                                break;
                            case MiraiApiType.messageFromId:
                                break;
                            case MiraiApiType.friendList:
                                break;
                            case MiraiApiType.groupList:
                                break;
                            case MiraiApiType.memberList:
                                break;
                            case MiraiApiType.botProfile:
                                break;
                            case MiraiApiType.friendProfile:
                                break;
                            case MiraiApiType.memberProfile:
                                break;
                            case MiraiApiType.userProfile:
                                break;
                            case MiraiApiType.sendFriendMessage:
                                long friend_groupid = json["data"]["args"]["qqId"].ToObject<long>();
                                string friend_text = json["data"]["args"]["text"].ToObject<string>();
                                logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "[↑]发送私聊消息", friend_text, "处理中...");
                                int friend_msgSeq = MiraiAPI.SendFriendMessage(friend_groupid, friend_text);
                                Send(new ApiResult { data = new { callResult = friend_msgSeq }.ToJson() }.ToJson());
                                break;
                            case MiraiApiType.sendGroupMessage:
                                long group_groupid = json["data"]["args"]["groupid"].ToObject<long>();
                                string group_text = json["data"]["args"]["text"].ToObject<string>();
                                logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "[↑]发送群聊消息", group_text, "处理中...");
                                int group_msgSeq =  MiraiAPI.SendGroupMessage(group_groupid, group_text);
                                Send(new ApiResult { data = new { callResult = group_msgSeq }.ToJson() }.ToJson());
                                break;
                            case MiraiApiType.sendTempMessage:
                                break;
                            case MiraiApiType.sendNudge:
                                break;
                            case MiraiApiType.recall:
                                break;
                            case MiraiApiType.roamingMessages:
                                break;
                            case MiraiApiType.file_list:
                                break;
                            case MiraiApiType.file_info:
                                break;
                            case MiraiApiType.file_mkdir:
                                break;
                            case MiraiApiType.file_delete:
                                break;
                            case MiraiApiType.file_move:
                                break;
                            case MiraiApiType.file_rename:
                                break;
                            case MiraiApiType.deleteFriend:
                                break;
                            case MiraiApiType.mute:
                                break;
                            case MiraiApiType.unmute:
                                break;
                            case MiraiApiType.kick:
                                break;
                            case MiraiApiType.quit:
                                break;
                            case MiraiApiType.muteAll:
                                break;
                            case MiraiApiType.unmuteAll:
                                break;
                            case MiraiApiType.setEssence:
                                break;
                            case MiraiApiType.groupConfig_get:
                                break;
                            case MiraiApiType.groupConfig_update:
                                break;
                            case MiraiApiType.memberInfo_get:
                                break;
                            case MiraiApiType.memberInfo_update:
                                break;
                            case MiraiApiType.memberAdmin:
                                break;
                            case MiraiApiType.anno_list:
                                break;
                            case MiraiApiType.anno_publish:
                                break;
                            case MiraiApiType.anno_delete:
                                break;
                            case MiraiApiType.resp_newFriendRequestEvent:
                                break;
                            case MiraiApiType.resp_memberJoinRequestEvent:
                                break;
                            case MiraiApiType.resp_botInvitedJoinGroupRequestEvent:
                                break;
                            default:
                                break;
                        }
                        break;
                    case WsServerFunction.CallCQFunction:
                        break;
                    case WsServerFunction.Exit:
                        break;
                    case WsServerFunction.Restart:
                        break;
                    case WsServerFunction.AddPlugin:
                        break;
                    case WsServerFunction.ReloadPlugin:
                        break;
                    case WsServerFunction.GetPluginList:
                        break;
                    case WsServerFunction.SwitchPluginStatus:
                        break;
                    case WsServerFunction.GetBotInfo:
                        break;
                    case WsServerFunction.GetGroupList:
                        break;
                    case WsServerFunction.GetFriendList:
                        break;
                    case WsServerFunction.GetStatus:
                        break;
                    default:
                        break;
                }
                sw.Stop();
                string updatemsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
                LogHelper.UpdateLogStatus(logid, updatemsg);

            }
            public void Send(WsServerFunction type, object data)
            {
                 Send(new { type, data }.ToJson());
            }
        }
        public static WsServer Instance { get; set; }
        public WebSocketServer Server { get; set; }
        public delegate void CQPConnected_Handler();
        public event CQPConnected_Handler CQPConnected;
        public WsServer(int port)
        {
            Server = new(port);
            Server.AddWebSocketService<Handler>("/amn");
            Instance = this;
        }
        public static void Init(int port)
        {
            var server = new WsServer(port);
            server.Start();
            Debug.WriteLine("WebSocket服务器创建成功");
        }
        public void Start()
        {
            Server.Start();
        }
        public void Stop()
        {
            Server.Stop();
        }
    }
}
