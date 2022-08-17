using Another_Mirai_Native.Adapter;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                        Enums.LogLevel log_priority = json["data"]["args"]["priority"].ToObject<Enums.LogLevel>();
                        string log_msg = json["data"]["args"]["msg"].ToObject<string>();
                        string log_type = "致命错误";
                        string log_origin = "AMN框架";
                        if (json["data"]["args"].ContainsKey("type"))
                        {
                            log_type = json["data"]["args"]["type"].ToObject<string>();
                        }                        
                        if (json["data"]["args"].ContainsKey("authCode"))
                        {
                            int authcode = json["data"]["args"]["authCode"].ToObject<int>();
                            var plugin = PluginManagment.Instance.Plugins.Find(x => x.appinfo.AuthCode == authcode);
                            if (plugin != null)
                            {
                                log_origin = plugin.appinfo.Name;
                            }
                        }
                        LogHelper.WriteLog(log_priority, log_origin, log_type, log_msg, "");
                        Send(new ApiResult { data = new { callResult = 1  }.ToJson() }.ToJson());
                        break;
                    case WsServerFunction.GetLog:
                        break;
                    case WsServerFunction.CallMiraiAPI:
                        logid = HandleMiraiFunction(json, logid);
                        break;
                    case WsServerFunction.CallCQFunction:
                        HandleCQFunction(json);
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
                if (logid == 0) return;
                string updatemsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
                LogHelper.UpdateLogStatus(logid, updatemsg);

            }
            public void HandleCQFunction(JObject json)
            {
                var apiType = json["data"]["type"].ToObject<string>();
                int authcode = json["data"]["args"]["authCode"].ToObject<int>();
                var plugin = PluginManagment.Instance.Plugins.Find(x => x.appinfo.AuthCode == authcode);
                if (plugin == null)
                {
                    Send(new ApiResult { success = false }.ToJson());
                    LogHelper.WriteLog("Authcode无效", "");
                    return;
                }
                switch (apiType)
                {
                    case "GetAppDirectory":
                        string path = @$"data\app\{plugin.appinfo.Id}";
                        if (Directory.Exists(path) is false)
                            Directory.CreateDirectory(path);
                        Send(new ApiResult { data = new { callResult = new DirectoryInfo(path).FullName + "\\" }.ToJson() }.ToJson());
                        break;
                    case "GetLoginQQ":
                        Send(new ApiResult { data = new { callResult = Helper.QQ }.ToJson() }.ToJson());
                        break;
                    case "GetLoginNick":
                        Send(new ApiResult { data = new { callResult = Helper.NickName }.ToJson() }.ToJson());
                        break;
                    default:
                        break;
                }
            }
            public int HandleMiraiFunction(JObject json, int logid)
            {
                var apiType = json["data"]["type"].ToObject<MiraiApiType>();
                int authcode = json["data"]["args"]["authCode"].ToObject<int>();
                object callResult = 0;
                var plugin = PluginManagment.Instance.Plugins.Find(x => x.appinfo.AuthCode == authcode);
                if (plugin == null)
                {
                    Send(new ApiResult { success = false }.ToJson());
                    LogHelper.WriteLog("Authcode无效", "");
                    return logid;
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
                        callResult = MiraiAPI.GetFriendList();
                        break;
                    case MiraiApiType.groupList:
                        callResult = MiraiAPI.GetGroupList();
                        break;
                    case MiraiApiType.memberList:
                        long memberList_groupid = json["data"]["args"]["groupId"].ToObject<long>();
                        callResult = MiraiAPI.GetGroupMemberList(memberList_groupid);
                        break;
                    case MiraiApiType.botProfile:
                    case MiraiApiType.friendProfile:
                        break;
                    case MiraiApiType.memberProfile:
                        long memberProfile_groupid = json["data"]["args"]["groupId"].ToObject<long>();
                        long memberProfile_QQId = json["data"]["args"]["QQId"].ToObject<long>();
                        callResult = MiraiAPI.GetGroupMemberInfo(memberProfile_groupid, memberProfile_QQId);
                        break;
                    case MiraiApiType.userProfile:
                        break;
                    case MiraiApiType.sendFriendMessage:
                        long friend_QQid = json["data"]["args"]["qqId"].ToObject<long>();
                        string friend_text = json["data"]["args"]["text"].ToObject<string>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "[↑]发送私聊消息", $"QQ:{friend_QQid} 消息:{friend_text}", "处理中...");
                        callResult = MiraiAPI.SendFriendMessage(friend_QQid, friend_text);
                        break;
                    case MiraiApiType.sendGroupMessage:
                        long group_groupid = json["data"]["args"]["groupid"].ToObject<long>();
                        string group_text = json["data"]["args"]["text"].ToObject<string>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "[↑]发送群聊消息", $"群:{group_groupid} 消息:{group_text}", "处理中...");
                        callResult = MiraiAPI.SendGroupMessage(group_groupid, group_text);
                        break;
                    case MiraiApiType.sendTempMessage:
                    case MiraiApiType.sendNudge:
                        break;
                    case MiraiApiType.recall:
                        int recall_msgId = json["data"]["args"]["msgId"].ToObject<int>();
                        string recallMsg = MiraiAPI.GetMessageByMsgId(recall_msgId);
                        if (string.IsNullOrEmpty(recallMsg)) recallMsg = "消息拉取失败";
                        logid = LogHelper.WriteLog(Enums.LogLevel.Info, plugin.appinfo.Name, "撤回消息", $"msgid={recall_msgId}, 内容={recallMsg}", "处理中...");                        
                        callResult = MiraiAPI.RecallMessage(recall_msgId, 0);
                        break;
                    case MiraiApiType.roamingMessages:
                    case MiraiApiType.file_list:
                    case MiraiApiType.file_info:
                    case MiraiApiType.file_mkdir:
                    case MiraiApiType.file_delete:
                    case MiraiApiType.file_move:
                    case MiraiApiType.file_rename:
                    case MiraiApiType.deleteFriend:
                        break;
                    case MiraiApiType.mute:
                        long mute_group = json["data"]["args"]["groupId"].ToObject<long>();
                        long mute_qq = json["data"]["args"]["qqId"].ToObject<long>();
                        long mute_time = json["data"]["args"]["time"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.Info, plugin.appinfo.Name, "禁言群成员", $"禁言群{mute_group} 成员{mute_qq} {mute_time}秒", "处理中...");
                        callResult = MiraiAPI.MuteGroupMemeber(mute_group, mute_qq, mute_time);
                        break;
                    case MiraiApiType.unmute:
                        long unmute_group = json["data"]["args"]["groupId"].ToObject<long>();
                        long unmute_qq = json["data"]["args"]["qqId"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "解除禁言群成员", $"解除禁言群{unmute_group} 成员{unmute_qq}", "处理中...");
                        callResult = MiraiAPI.UnmuteGroupMemeber(unmute_group, unmute_qq);
                        break;
                    case MiraiApiType.kick:
                        long kick_group = json["data"]["args"]["groupId"].ToObject<long>();
                        long kick_qq = json["data"]["args"]["qqId"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "踢出群成员", $"移除群{kick_group} 成员{kick_qq}", "处理中...");
                        callResult = MiraiAPI.KickGroupMember(kick_group, kick_qq);
                        break;
                    case MiraiApiType.quit:
                        long quit_group = json["data"]["args"]["groupId"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.Info, plugin.appinfo.Name, "退出群", $"退出群{quit_group}", "处理中...");
                        callResult = MiraiAPI.GroupMute(quit_group);
                        break;
                    case MiraiApiType.muteAll:
                        long muteAll_group = json["data"]["args"]["groupId"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.Info, plugin.appinfo.Name, "群禁言", $"禁言群{muteAll_group}", "处理中...");
                        callResult = MiraiAPI.GroupMute(muteAll_group);
                        break;
                    case MiraiApiType.unmuteAll:
                        long unmuteAll_group = json["data"]["args"]["groupId"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.Info, plugin.appinfo.Name, "解除群禁言", $"解除禁言群{unmuteAll_group}", "处理中...");
                        callResult = MiraiAPI.GroupUnmute(unmuteAll_group);
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
                        long memberInfo_group = json["data"]["args"]["groupId"].ToObject<long>();
                        long memberInfo_qq = json["data"]["args"]["qqId"].ToObject<long>();
                        if (json["data"]["args"].ContainsKey("title"))
                        {
                            string memberInfo_title = json["data"]["args"]["title"].ToObject<string>();
                            logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "设置群成员头衔", $"设置群{memberInfo_group} 成员{memberInfo_qq} 头衔 {memberInfo_title}", "处理中...");
                            callResult = MiraiAPI.SetSpecialTitle(memberInfo_group, memberInfo_qq, memberInfo_title);
                        }
                        else
                        {
                            string memberInfo_card = json["data"]["args"]["newCard"].ToObject<string>();
                            logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "设置群成员名片", $"设置群{memberInfo_group} 成员{memberInfo_qq} 名片 {memberInfo_card}", "处理中...");
                            callResult = MiraiAPI.SetGroupCard(memberInfo_group, memberInfo_qq, memberInfo_card);
                        }
                        break;
                    case MiraiApiType.memberAdmin:
                        long admin_group = json["data"]["args"]["groupId"].ToObject<long>();
                        long admin_qq = json["data"]["args"]["qqId"].ToObject<long>();
                        bool admin_set = json["data"]["args"]["isSet"].ToObject<bool>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, $"{(admin_set?"设置":"取消")}群成员管理", $"{(admin_set?"设置":"取消")}群{admin_group} 成员{admin_qq}", "处理中...");
                        callResult = MiraiAPI.SetAdmin(admin_group, admin_qq, admin_set);
                        break;
                    case MiraiApiType.anno_list:
                        break;
                    case MiraiApiType.anno_publish:
                        break;
                    case MiraiApiType.anno_delete:
                        break;
                    case MiraiApiType.resp_newFriendRequestEvent:
                        long respf_eventId = json["data"]["args"]["eventId"].ToObject<long>();
                        int respf_operate = json["data"]["args"]["requestType"].ToObject<int>();
                        string respf_message = json["data"]["args"]["message"].ToObject<string>();
                        string respf_msg = "";
                        switch (respf_operate)
                        {
                            case 0:
                                respf_msg = "同意";
                                break;
                            case 1:
                                respf_msg = "拒绝";
                                break;
                        }
                        long respf_fromId = 0;
                        string respf_nick = ""; 
                        if (Cache.FriendRequset.ContainsKey(respf_eventId))
                        {
                            respf_fromId = Cache.FriendRequset[respf_eventId].Item1;
                            respf_nick = Cache.FriendRequset[respf_eventId].Item2;
                        }
                        Cache.FriendRequset.Remove(respf_eventId);
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, $"好友添加申请", $"来源: {respf_fromId}({respf_nick}) 操作: {respf_msg}", "处理中...");
                        callResult = MiraiAPI.HandleFriendRequest(respf_eventId, respf_operate, respf_message);
                        break;
                    case MiraiApiType.resp_memberJoinRequestEvent:
                        long respm_eventId = json["data"]["args"]["eventId"].ToObject<long>();
                        int respm_operate = json["data"]["args"]["requestType"].ToObject<int>();
                        string respm_message = json["data"]["args"]["message"].ToObject<string>();
                        string respm_msg = "";
                        switch (respm_operate)
                        {
                            case 0:
                                respf_msg = "同意";
                                break;
                            case 1:
                                respf_msg = "拒绝";
                                break;
                        }
                        long respm_fromId = 0, respm_groupId = 0;
                        string respm_nick = "", respm_groupname ="";
                        if (Cache.GroupRequset.ContainsKey(respm_eventId))
                        {
                            respm_fromId = Cache.GroupRequset[respm_eventId].Item1;
                            respm_nick = Cache.GroupRequset[respm_eventId].Item2;
                            respm_groupId = Cache.GroupRequset[respm_eventId].Item3;
                            respm_groupname = Cache.GroupRequset[respm_eventId].Item4;
                        }
                        Cache.GroupRequset.Remove(respm_eventId);
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, $"群添加申请", $"来源: {respm_fromId}({respm_nick}) 目标群: {respm_groupId}({respm_groupname}) 操作: {respm_msg}", "处理中...");
                        callResult = MiraiAPI.HandleGroupRequest(respm_eventId, respm_operate, respm_message);
                        break;
                    case MiraiApiType.resp_botInvitedJoinGroupRequestEvent:
                        long respb_eventId = json["data"]["args"]["eventId"].ToObject<long>();
                        int respb_operate = json["data"]["args"]["requestType"].ToObject<int>();
                        string respb_message = json["data"]["args"]["message"].ToObject<string>();
                        string respb_msg = "";
                        switch (respb_operate)
                        {
                            case 0:
                                respf_msg = "同意";
                                break;
                            case 1:
                                respf_msg = "拒绝";
                                break;
                        }
                        long respb_fromId = 0, respb_groupId = 0;
                        string respb_nick = "", respb_groupname = "";
                        if (Cache.GroupRequset.ContainsKey(respb_eventId))
                        {
                            respb_fromId = Cache.GroupRequset[respb_eventId].Item1;
                            respb_nick = Cache.GroupRequset[respb_eventId].Item2;
                            respb_groupId = Cache.GroupRequset[respb_eventId].Item3;
                            respb_groupname = Cache.GroupRequset[respb_eventId].Item4;
                        }
                        Cache.GroupRequset.Remove(respb_eventId);
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, $"群邀请添加申请", $"来源群: {respb_groupId}({respb_groupname}) 来源人: {respb_fromId}({respb_nick}) 操作: {respb_msg}", "处理中...");
                        callResult = MiraiAPI.HandleInviteRequest(respb_eventId, respb_operate, respb_message);
                        break;
                    default:
                        break;
                }
                Send(new ApiResult { data = new { callResult }.ToJson() }.ToJson());
                return logid;
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
