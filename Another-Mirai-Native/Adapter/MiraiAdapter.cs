using Another_Mirai_Native.Adapter.CQCode.Expand;
using Another_Mirai_Native.Adapter.MiraiEventArgs;
using Another_Mirai_Native.Adapter.MiraiMessageEventArgs;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Another_Mirai_Native.Adapter
{
    /// <summary>
    /// 描述与Mirai-Http-Api(MHA) 交互的类
    /// </summary>
    public class MiraiAdapter
    {
        public static MiraiAdapter Instance { get; set; }
        /// <summary>
        /// 与MHA通信的连接, 通常以ws开头
        /// </summary>
        public string WsURL { get; set; }
        /// <summary>
        /// MHA配置中所定义的SecretKey
        /// </summary>
        public string AuthKey { get; set; }
        /// <summary>
        /// Mirai框架中登录中 且 希望控制逻辑的QQ号
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 保存消息服务器的SessionKey
        /// </summary>
        public string SessionKey_Message { get; set; }
        /// <summary>
        /// 保存事件服务器的SessionKey
        /// </summary>
        public string SessionKey_Event { get; set; }
        /// <summary>
        /// 重连次数
        /// </summary>
        public int reConnect { get; set; }

        public WebSocket MessageSocket;
        public WebSocket EventSocket;
        public delegate void ConnectedStateChange(bool status, string msg);
        /// <summary>
        /// 连接状态变更事件
        /// </summary>
        public event ConnectedStateChange ConnectedStateChanged;

        static Encoding GB18030 = Encoding.GetEncoding("GB18030");
        public MiraiAdapter(string url, string qq, string authkey)
        {
            Instance = this;
            if (url.EndsWith("/")) url = url[..^1];
            WsURL = url;
            AuthKey = authkey;
            QQ = qq;
            string message_connecturl = $"{WsURL}/message?verifyKey={AuthKey}&qq={QQ}";
            string event_connecturl = $"{WsURL}/event?verifyKey={AuthKey}&qq={QQ}";
            MessageSocket = new WebSocket(message_connecturl);
            MessageSocket.OnMessage += MessageSocket_OnMessage;
            MessageSocket.OnClose += MessageSocket_OnClose;
            MessageSocket.OnOpen += MessageSocket_OnOpen;

            EventSocket = new WebSocket(event_connecturl);
            EventSocket.OnMessage += EventSocket_OnMessage;
            EventSocket.OnClose += EventSocket_OnClose;
            EventSocket.OnOpen += EventSocket_OnOpen;
        }
        private long NoEventTimeout = 0;
        private long NoEventTimeoutMax = 600;

        private void EventSocket_OnOpen(object sender, EventArgs e)
        {
            LogHelper.WriteLog(Enums.LogLevel.Debug, "事件服务器", "连接到事件服务器");
            reConnect = 0;
            new Thread(() =>
            {
                while (NoEventTimeout < NoEventTimeoutMax)
                {
                    Thread.Sleep(1000);
                    NoEventTimeout++;
                }
                LogHelper.WriteLog(Enums.LogLevel.Debug, "消息服务器疑似无响应", $"status={EventSocket.ReadyState}");
                EventSocket.Close();
            }).Start();
        }
        private long NoMsgTimeout = 0;
        private long NoMsgTimeoutMax = 600;
        private void MessageSocket_OnOpen(object sender, EventArgs e)
        {
            LogHelper.WriteLog(Enums.LogLevel.Debug, "消息服务器", "连接到消息服务器");
            reConnect = 0;
            new Thread(() =>
            {
                while (NoMsgTimeout < NoMsgTimeoutMax)
                {
                    Thread.Sleep(1000);
                    NoMsgTimeout++;
                }
                LogHelper.WriteLog(Enums.LogLevel.Debug, "消息服务器疑似无响应", $"status={MessageSocket.ReadyState}");
                MessageSocket.Close();
            }).Start();
        }

        private void EventSocket_OnClose(object sender, CloseEventArgs e)
        {
            SessionKey_Event = "";
            reConnect++;
            LogHelper.WriteLog($"与事件服务器断开连接...第 {reConnect} 次重连");
            Thread.Sleep(3000);
            string event_connecturl = $"{WsURL}/event?verifyKey={AuthKey}&qq={QQ}";
            EventSocket = new WebSocket(event_connecturl);
            EventSocket.OnMessage += EventSocket_OnMessage;
            EventSocket.OnClose += EventSocket_OnClose;
            EventSocket.OnOpen += EventSocket_OnOpen;
            EventSocket.Connect();
        }

        private void MessageSocket_OnClose(object sender, CloseEventArgs e)
        {
            SessionKey_Message = "";
            reConnect++;
            LogHelper.WriteLog($"与消息服务器断开连接...第 {reConnect} 次重连");
            Thread.Sleep(3000);
            string message_connecturl = $"{WsURL}/message?verifyKey={AuthKey}&qq={QQ}";
            MessageSocket = new WebSocket(message_connecturl);
            MessageSocket.OnMessage += MessageSocket_OnMessage;
            MessageSocket.OnClose += MessageSocket_OnClose;
            MessageSocket.OnOpen += MessageSocket_OnOpen;
            MessageSocket.Connect();
        }
        /// <summary>
        /// 处理消息, 使用了消息队列
        /// </summary>
        private void MessageSocket_OnMessage(object sender, MessageEventArgs e)
        {
            NoMsgTimeout = 0;
            JObject json = JObject.Parse(e.Data);
            if (string.IsNullOrWhiteSpace(SessionKey_Message))
            {
                if (json["data"]["code"].ToString() == "0")
                {
                    SessionKey_Message = json["data"]["session"].ToString();
                    ConnectedStateChanged?.Invoke(true, "");
                }
                else
                {
                    if (json["data"].ContainsKey("msg"))
                    {
                        ConnectedStateChanged?.Invoke(false, json["data"]["msg"].ToString());
                    }
                    else
                    {
                        ConnectedStateChanged?.Invoke(false, "连接失败");
                    }
                    MessageSocket.Close();
                }
                return;
            }
            if (json.ContainsKey("code"))// 为API调用结果
            {
                ApiQueue.Peek().result = json.ToString();
                Debug.WriteLine(e.Data);
                return;
            }
            if ((json.ContainsKey("data") && json["data"].ContainsKey("code"))
                || (json["data"].ContainsKey("nickname") && json["data"].ContainsKey("sex"))
                || json["data"].ContainsKey("specialTitle"))// 同样为API调用结果, 但是结果封装与上面那个不一样
            {
                ApiQueue.Peek().result = json["data"].ToString();
                Debug.WriteLine(e.Data);
                return;
            }
            ParseMessage(json);
        }

        private void EventSocket_OnMessage(object sender, MessageEventArgs e)
        {
            NoEventTimeout = 0;
            JObject json = JObject.Parse(e.Data);
            if (string.IsNullOrWhiteSpace(SessionKey_Event))
            {
                if (json["data"]["code"].ToString() == "0")
                {
                    SessionKey_Event = json["data"]["session"].ToString();
                    ConnectedStateChanged?.Invoke(true, "");
                }
                else
                {
                    if (json["data"].ContainsKey("msg"))
                    {
                        ConnectedStateChanged?.Invoke(false, json["data"]["msg"].ToString());
                    }
                    else
                    {
                        ConnectedStateChanged?.Invoke(false, "连接失败");
                    }
                    EventSocket.Close();
                }
                return;
            }
            if (json["data"].ContainsKey("code"))// 将结果写入队列第一个元素 并排出队列
            {
                ApiQueue.Peek().result = json["data"].ToString();// TODO: 验证
                return;
            }
            ParseEvent(json);
        }
        /// <summary>
        /// 处理分发事件
        /// </summary>
        private void ParseEvent(JObject msg)
        {
            Stopwatch sw = new();
            sw.Start();
            MiraiEvents events = Helper.String2Enum<MiraiEvents>(msg["data"]["type"].ToString());
            JToken raw = msg["data"];
            int logid = 0;
            CQPlugin handledPlugin = null;
            switch (events)
            {
                case MiraiEvents.BotOnlineEvent:
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "Bot登录成功", $"登录成功", "处理中...");
                    break;
                case MiraiEvents.BotOfflineEventActive:
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "Bot主动离线", $"主动离线", "处理中...");
                    break;
                case MiraiEvents.BotOfflineEventForce:
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "Bot被挤下线", $"被动离线", "处理中...");
                    break;
                case MiraiEvents.BotOfflineEventDropped:
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "Bot掉线", $"被服务器断开或因网络问题而掉线", "处理中...");
                    break;
                case MiraiEvents.BotReloginEvent:
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "Bot主动重新登录", $"主动重新登录", "处理中...");
                    break;
                case MiraiEvents.FriendInputStatusChangedEvent:
                    var friendInputStatusChangedEvent = raw.ToObject<FriendInputStatusChangedEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "好友输入状态改变", $"QQ:{friendInputStatusChangedEvent.friend.id}({friendInputStatusChangedEvent.friend.nickname}) 变更为:{friendInputStatusChangedEvent.inputting}", "处理中...");
                    break;
                case MiraiEvents.FriendNickChangedEvent:
                    var friendNickChangedEvent = raw.ToObject<FriendNickChangedEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "好友昵称改变", $"QQ:{friendNickChangedEvent.friend.id}({friendNickChangedEvent.from}) 变更为:{friendNickChangedEvent.to}", "处理中...");
                    break;
                case MiraiEvents.BotGroupPermissionChangeEvent:
                    var botGroupPermissionChange = raw.ToObject<BotGroupPermissionChangeEvent>();
                    int botGroupPermissionChangeStatus = 1;
                    if (botGroupPermissionChange.origin == "MEMBER")
                        botGroupPermissionChangeStatus = 2;
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "Bot权限变更", $"群:{botGroupPermissionChange.group.id}({botGroupPermissionChange.group.name}) 新权限为:{botGroupPermissionChange.current}", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.AdminChange, botGroupPermissionChangeStatus, Helper.TimeStamp, botGroupPermissionChange.group.id, Helper.QQ);
                    break;
                case MiraiEvents.BotMuteEvent:
                    var botMute = raw.ToObject<BotMuteEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "Bot被禁言", $"群:{botMute._operator.group.id}({botMute._operator.group.name}) 禁言时长:{botMute.durationSeconds} 秒 操作人:{botMute._operator.id}({botMute._operator.memberName})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupBan, 2, Helper.TimeStamp, botMute._operator.group.id, Helper.QQ, botMute.durationSeconds);
                    break;
                case MiraiEvents.BotUnmuteEvent:
                    var botUnmute = raw.ToObject<BotUnmuteEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "Bot解除禁言", $"群:{botUnmute._operator.group.id}({botUnmute._operator.group.name}) 操作人:{botUnmute._operator.id}({botUnmute._operator.memberName})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupBan, 1, Helper.TimeStamp, botUnmute._operator.group.id, Helper.QQ, 0);
                    break;
                case MiraiEvents.BotJoinGroupEvent:
                    var botJoinGroup = raw.ToObject<BotJoinGroupEvent>();
                    string botJoinGroupInvitorMsg = "";
                    if (botJoinGroup.invitor != null)
                    {
                        botJoinGroupInvitorMsg = $" 邀请人:{botJoinGroup.invitor.id}({botJoinGroup.invitor.memberName})";
                    }
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "Bot加入群聊", $"群:{botJoinGroup.group.id}({botJoinGroup.group.name}){botJoinGroupInvitorMsg}", "处理中...");
                    break;
                case MiraiEvents.BotLeaveEventActive:
                    var botLeaveEventActive = raw.ToObject<BotLeaveEventActive>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "Bot主动退出群聊", $"群:{botLeaveEventActive.group.id}({botLeaveEventActive.group.name})", "处理中...");
                    break;
                case MiraiEvents.BotLeaveEventKick:
                    var botLeaveEventKick = raw.ToObject<BotLeaveEventKick>();
                    string botLeaveEventOperatorMsg = "";
                    if (botLeaveEventKick._operator != null)
                    {
                        botLeaveEventOperatorMsg = $" 操作人:{botLeaveEventKick._operator.id}({botLeaveEventKick._operator.memberName})";
                    }
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "Bot被踢出群聊", $"群:{botLeaveEventKick.group.id}({botLeaveEventKick.group.name}){botLeaveEventOperatorMsg}", "处理中...");
                    break;
                case MiraiEvents.BotLeaveEventDisband:
                    var botLeaveEventDisband = raw.ToObject<BotLeaveEventDisband>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群解散", $"群:{botLeaveEventDisband.group.id}({botLeaveEventDisband.group.name})", "处理中...");
                    break;
                case MiraiEvents.GroupRecallEvent:
                    var groupRecall = raw.ToObject<GroupRecallEvent>();
                    string groupRecallMsg = MiraiAPI.GetMessageByMsgId(groupRecall.messageId);
                    if (string.IsNullOrEmpty(groupRecallMsg)) groupRecallMsg = "消息拉取失败";
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群撤回", $"群:{groupRecall.group.id}({groupRecall.group.name}) QQ:{groupRecall.authorId} 内容:{groupRecallMsg}", "处理中...");
                    break;
                case MiraiEvents.FriendRecallEvent:
                    var friendRecall = raw.ToObject<FriendRecallEvent>();
                    string friendRecallMsg = MiraiAPI.GetMessageByMsgId(friendRecall.messageId);
                    if (string.IsNullOrEmpty(friendRecallMsg)) friendRecallMsg = "消息拉取失败";
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "私聊撤回", $"QQ:{friendRecall.authorId} 内容:{friendRecallMsg}", "处理中...");
                    break;
                case MiraiEvents.NudgeEvent:
                    var nudge = raw.ToObject<NudgeEvent>();
                    string nudgeMsg = "";
                    if (nudge.subject.kind == "Group")
                    {
                        nudgeMsg = $"群:{nudge.subject.id} QQ:{nudge.fromId}";
                    }
                    else
                    {
                        nudgeMsg = $"QQ:{nudge.fromId}";
                    }
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "戳一戳", $"{nudgeMsg} 内容:{nudge.action}bot{nudge.suffix}", "处理中...");
                    break;
                case MiraiEvents.GroupNameChangeEvent:
                    var groupNameChange = raw.ToObject<GroupNameChangeEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群名变更", $"群:{groupNameChange.group.id}({groupNameChange.origin}) 变更为:{groupNameChange.current} 操作人:{groupNameChange._operator.id}({groupNameChange._operator.memberName})", "处理中...");
                    break;
                case MiraiEvents.GroupEntranceAnnouncementChangeEvent:
                    var groupEntranceAnnouncementChange = raw.ToObject<GroupEntranceAnnouncementChangeEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群入群公告改变", $"群:{groupEntranceAnnouncementChange.group.id}({groupEntranceAnnouncementChange.group.name}) 内容:{groupEntranceAnnouncementChange.current}", "处理中...");
                    break;
                case MiraiEvents.GroupMuteAllEvent:
                    var groupMuteAllRecall = raw.ToObject<GroupMuteAllEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "全体禁言", $"群:{groupMuteAllRecall._operator.group.id}({groupMuteAllRecall._operator.group.name}) 操作人:{groupMuteAllRecall._operator.id}({groupMuteAllRecall._operator.memberName})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupBan, 2, Helper.TimeStamp, groupMuteAllRecall._operator.group.id, 0, 0);
                    break;
                case MiraiEvents.GroupAllowAnonymousChatEvent:
                    var groupAllowAnonymousChat = raw.ToObject<GroupAllowAnonymousChatEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群允许匿名聊天", $"群:{groupAllowAnonymousChat._operator.group.id}({groupAllowAnonymousChat._operator.group.name}) 操作人:{groupAllowAnonymousChat._operator.id}({groupAllowAnonymousChat._operator.memberName})", "处理中...");
                    break;
                case MiraiEvents.GroupAllowConfessTalkEvent:
                    var groupAllowConfessTalk = raw.ToObject<GroupAllowConfessTalkEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群允许坦白说", $"群:{groupAllowConfessTalk.group.id}({groupAllowConfessTalk.group.name})", "处理中...");
                    break;
                case MiraiEvents.GroupAllowMemberInviteEvent:
                    var groupAllowMemberInvite = raw.ToObject<GroupAllowMemberInviteEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群允许邀请入群", $"群:{groupAllowMemberInvite._operator.group.id}({groupAllowMemberInvite._operator.group.name}) 操作人:{groupAllowMemberInvite._operator.id}({groupAllowMemberInvite._operator.memberName})", "处理中...");
                    break;
                case MiraiEvents.MemberJoinEvent:
                    var memberJoin = raw.ToObject<MemberJoinEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群成员增加", $"群:{memberJoin.member.group.id}({memberJoin.member.group.name}) QQ:{memberJoin.member.id}({memberJoin.member.memberName})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupMemberIncrease, memberJoin.invitor == null ? 1 : 2, Helper.TimeStamp, memberJoin.member.group.id, 10001, memberJoin.member.id);
                    break;
                case MiraiEvents.MemberLeaveEventKick:
                    var memberLeaveEventKick = raw.ToObject<MemberLeaveEventKick>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群成员被踢出", $"群:{memberLeaveEventKick.member.group.id}({memberLeaveEventKick.member.group.name}) QQ:{memberLeaveEventKick.member.id}({memberLeaveEventKick.member.memberName}) 操作者:{memberLeaveEventKick._operator.group.id}({memberLeaveEventKick._operator.group.name})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupMemberDecrease, 2, Helper.TimeStamp, memberLeaveEventKick.member.group.id, memberLeaveEventKick._operator.group.id, memberLeaveEventKick.member.id);
                    break;
                case MiraiEvents.MemberLeaveEventQuit:
                    var memberLeaveEventQuit = raw.ToObject<MemberLeaveEventQuit>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群成员离开", $"群:{memberLeaveEventQuit.member.group.id}({memberLeaveEventQuit.member.group.name}) QQ:{memberLeaveEventQuit.member.id}({memberLeaveEventQuit.member.memberName})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupMemberDecrease, 1, Helper.TimeStamp, memberLeaveEventQuit.member.group.id, 0, memberLeaveEventQuit.member.id);
                    break;
                case MiraiEvents.MemberCardChangeEvent:
                    var memberCardChangeEvent = raw.ToObject<MemberCardChangeEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群成员名片改变", $"群:{memberCardChangeEvent.member.group.id}({memberCardChangeEvent.member.group.name}) QQ:{memberCardChangeEvent.member.id}({memberCardChangeEvent.member.memberName}) 名片:{memberCardChangeEvent.current}", "处理中...");
                    break;
                case MiraiEvents.MemberSpecialTitleChangeEvent:
                    var memberSpecialTitleChangeEvent = raw.ToObject<MemberSpecialTitleChangeEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群成员头衔改变", $"群:{memberSpecialTitleChangeEvent.member.group.id}({memberSpecialTitleChangeEvent.member.group.name}) QQ:{memberSpecialTitleChangeEvent.member.id}({memberSpecialTitleChangeEvent.member.memberName}) 称号:{memberSpecialTitleChangeEvent.current}", "处理中...");
                    break;
                case MiraiEvents.MemberPermissionChangeEvent:
                    var memberPermissionChangeEvent = raw.ToObject<MemberPermissionChangeEvent>();
                    int memberPermissionChangeStatus = 1;
                    if (memberPermissionChangeEvent.origin == "MEMBER")
                        memberPermissionChangeStatus = 2;
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群员权限变更", $"群:{memberPermissionChangeEvent.member.group.id}({memberPermissionChangeEvent.member.group.name}) QQ:{memberPermissionChangeEvent.member.id}({memberPermissionChangeEvent.member.memberName}) 新权限为:{memberPermissionChangeEvent.current}", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.AdminChange, memberPermissionChangeStatus, Helper.TimeStamp, memberPermissionChangeEvent.member.group.id, memberPermissionChangeEvent.member.id);
                    break;
                case MiraiEvents.MemberMuteEvent:
                    var memberMuteEvent = raw.ToObject<MemberMuteEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群员被禁言", $"群:{memberMuteEvent.member.group.id}({memberMuteEvent.member.group.name}) QQ:{memberMuteEvent.member.id}({memberMuteEvent.member.memberName}) 禁言时长:{memberMuteEvent.durationSeconds} 秒 操作人:{memberMuteEvent._operator.id}({memberMuteEvent._operator.memberName})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupBan, 2, Helper.TimeStamp, memberMuteEvent._operator.group.id, memberMuteEvent.member.id, memberMuteEvent.durationSeconds);
                    break;
                case MiraiEvents.MemberUnmuteEvent:
                    var memberUnmuteEvent = raw.ToObject<MemberUnmuteEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群员被解除禁言", $"群:{memberUnmuteEvent.member.group.id}({memberUnmuteEvent.member.group.name}) QQ:{memberUnmuteEvent.member.id}({memberUnmuteEvent.member.memberName}) 操作人:{memberUnmuteEvent._operator.id}({memberUnmuteEvent._operator.memberName})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupBan, 1, Helper.TimeStamp, memberUnmuteEvent._operator.group.id, memberUnmuteEvent.member.id, 0);
                    break;
                case MiraiEvents.MemberHonorChangeEvent:
                    var memberHonorChangeEvent = raw.ToObject<MemberHonorChangeEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群员称号改变", $"群:{memberHonorChangeEvent.member.group.id}({memberHonorChangeEvent.member.group.name}) QQ:{memberHonorChangeEvent.member.id}({memberHonorChangeEvent.member.memberName}) 变更为:{memberHonorChangeEvent.honor}", "处理中...");
                    break;
                case MiraiEvents.NewFriendRequestEvent:
                    var newFriendRequestEvent = raw.ToObject<NewFriendRequestEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "添加好友请求", $"QQ:{newFriendRequestEvent.fromId}({newFriendRequestEvent.nick}) 备注:{newFriendRequestEvent.message} 来源群:{newFriendRequestEvent.groupId}", "处理中...");
                    Cache.FriendRequset.Add(newFriendRequestEvent.eventId, (newFriendRequestEvent.fromId, newFriendRequestEvent.nick));
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.FriendRequest, 1, Helper.TimeStamp, newFriendRequestEvent.fromId, newFriendRequestEvent.nick, newFriendRequestEvent.eventId.ToString());
                    break;
                case MiraiEvents.MemberJoinRequestEvent:
                    var memberJoinRequestEvent = raw.ToObject<MemberJoinRequestEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "添加群请求", $"群:{memberJoinRequestEvent.groupId}({memberJoinRequestEvent.groupName}) QQ:{memberJoinRequestEvent.fromId}({memberJoinRequestEvent.nick}) 备注:{memberJoinRequestEvent.message}", "处理中...");
                    Cache.GroupRequset.Add(memberJoinRequestEvent.eventId, (memberJoinRequestEvent.fromId, memberJoinRequestEvent.nick, memberJoinRequestEvent.groupId, memberJoinRequestEvent.groupName));
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupAddRequest, 1, Helper.TimeStamp, memberJoinRequestEvent.groupId, memberJoinRequestEvent.fromId, memberJoinRequestEvent.message, memberJoinRequestEvent.eventId.ToString());
                    break;
                case MiraiEvents.BotInvitedJoinGroupRequestEvent:
                    var botInvitedJoinGroupRequestEvent = raw.ToObject<BotInvitedJoinGroupRequestEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "收到入群邀请", $"群:{botInvitedJoinGroupRequestEvent.groupId}({botInvitedJoinGroupRequestEvent.groupName}) QQ:{botInvitedJoinGroupRequestEvent.fromId}({botInvitedJoinGroupRequestEvent.nick}) 备注:{botInvitedJoinGroupRequestEvent.message}", "处理中...");
                    break;
                case MiraiEvents.OtherClientOnlineEvent:
                    var otherClientOnlineEvent = raw.ToObject<OtherClientOnlineEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "其他设备上线", $"{otherClientOnlineEvent.client.platform}", "处理中...");
                    break;
                case MiraiEvents.OtherClientOfflineEvent:
                    var otherClientOfflineEvent = raw.ToObject<OtherClientOfflineEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "其他设备离线", $"{otherClientOfflineEvent.client.platform}", "处理中...");
                    break;
                case MiraiEvents.CommandExecutedEvent:
                    break;
                default:
                    break;
            }
            sw.Stop();
            string updatemsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
            if (handledPlugin != null)
            {
                updatemsg += $"(由 {handledPlugin.appinfo.Name} 结束消息处理)";
            }
            LogHelper.UpdateLogStatus(logid, updatemsg);
        }
        /// <summary>
        /// 处理分发消息
        /// </summary>
        /// <param name="msg"></param>
        private void ParseMessage(JObject msg)
        {
            MiraiMessageEvents events = Helper.String2Enum<MiraiMessageEvents>(msg["data"]["type"].ToString());

            MiraiMessageTypeDetail.Source source = null;
            var chainMsg = CQCodeBuilder.ParseJArray2MiraiMessageBaseList(msg["data"]["messageChain"] as JArray);
            source = (MiraiMessageTypeDetail.Source)chainMsg.First(x => x.messageType == MiraiMessageType.Source);
            string parsedMsg = CQCodeBuilder.Parse(chainMsg);
            DispatchMessage(events, msg["data"], source, chainMsg, parsedMsg);
        }
        /// <summary>
        /// 将消息分发给插件处理
        /// </summary>
        /// <param name="msgEvent">消息来源</param>
        /// <param name="raw">原始json对象</param>
        /// <param name="source"></param>
        /// <param name="chainMsg">消息链</param>
        /// <param name="msg">CQ码转码后文本</param>
        private void DispatchMessage(MiraiMessageEvents msgEvent, JToken raw, MiraiMessageTypeDetail.Source source, List<MiraiMessageBase> chainMsg, string msg)
        {
            if (source == null)
            {
                LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "参数缺失", $"", "");
                return;
            }
            new Task(() =>
            {
                Stopwatch sw = new();
                sw.Start();
                // 文件上传优先处理
                if (chainMsg.Any(x => x.messageType == MiraiMessageType.File))
                {
                    var group = raw.ToObject<GroupMessage>();
                    var file = (MiraiMessageTypeDetail.File)chainMsg.First(x => x.messageType == MiraiMessageType.File);
                    MemoryStream stream = new();
                    BinaryWriter binaryWriter = new(stream);
                    BinaryWriterExpand.Write_Ex(binaryWriter, file.id);
                    BinaryWriterExpand.Write_Ex(binaryWriter, file.name);
                    BinaryWriterExpand.Write_Ex(binaryWriter, file.size);
                    BinaryWriterExpand.Write_Ex(binaryWriter, 0);
                    PluginManagment.Instance.CallFunction(FunctionEnums.Upload, 1, Helper.TimeStamp, group.sender.group.id, group.sender.id, Convert.ToBase64String(stream.ToArray()));
                    sw.Stop();
                    LogHelper.WriteLog(Enums.LogLevel.InfoReceive, "AMN框架", "文件上传", $"来源群:{group.sender.group.id}({group.sender.group.name}) 来源QQ:{group.sender.id}({group.sender.memberName}) " +
                        $"文件名:{file.name} 大小:{file.size / 1000}KB FileID:{file.id}", $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");
                    return;
                }
                // utf8转GB18030
                var b = Encoding.UTF8.GetBytes(msg);
                msg = GB18030.GetString(Encoding.Convert(Encoding.UTF8, GB18030, b));
                byte[] messageBytes = GB18030.GetBytes(msg + "\0");
                var messageIntptr = Marshal.AllocHGlobal(messageBytes.Length);
                Marshal.Copy(messageBytes, 0, messageIntptr, messageBytes.Length);

                int logid = 0;
                CQPlugin handledPlugin = null;
                switch (msgEvent)
                {
                    case MiraiMessageEvents.FriendMessage:
                        var friend = raw.ToObject<FriendMessage>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoReceive, "AMN框架", "[↓]收到好友消息", $"QQ:{friend.sender.id}({friend.sender.nickname}) {msg}", "处理中...");
                        handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.PrivateMsg, 11, source.id, friend.sender.id, messageIntptr, 0);
                        break;
                    case MiraiMessageEvents.GroupMessage:
                        var group = raw.ToObject<GroupMessage>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoReceive, "AMN框架", "[↓]收到消息", $"群:{group.sender.group.id}({group.sender.group.name}) QQ:{group.sender.id}({group.sender.memberName}) {msg}", "处理中...");
                        handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupMsg, 1, source.id, group.sender.group.id, group.sender.id, "", messageIntptr, 0);
                        break;
                    case MiraiMessageEvents.TempMessage:
                        var temp = raw.ToObject<TempMessage>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoReceive, "AMN框架", "[↓]收到群临时消息", $"群:{temp.sender.group.id}({temp.sender.group.name}) QQ:{temp.sender.id}({temp.sender.memberName}) {msg}", "处理中...");
                        handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.PrivateMsg, 2, source.id, temp.sender.id, messageIntptr, 0);
                        break;
                    case MiraiMessageEvents.StrangerMessage:
                        var stranger = raw.ToObject<StrangerMessage>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoReceive, "AMN框架", "[↓]收到陌生人消息", $"QQ:{stranger.sender.id}({stranger.sender.nickname}) {msg}", "处理中...");
                        handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.PrivateMsg, 1, source.id, stranger.sender.id, messageIntptr, 0);
                        break;
                    case MiraiMessageEvents.OtherClientMessage:
                        var other = raw.ToObject<OtherClientMessage>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoReceive, "AMN框架", "[↓]收到其他设备消息", $"QQ:{other.sender.id}({other.sender.platform}) {msg}", "x 不处理");
                        break;
                    default:
                        break;
                }
                sw.Stop();
                string updatemsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
                if (handledPlugin != null)
                {
                    updatemsg += $"(由 {handledPlugin.appinfo.Name} 结束消息处理)";
                }
                LogHelper.UpdateLogStatus(logid, updatemsg);
            }).Start();
        }
        /// <summary>
        /// 连接消息服务器与事件服务器
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            MessageSocket.Connect();
            EventSocket.Connect();
            return false;// ?
        }
        /// <summary>
        /// 描述消息队列的对象
        /// </summary>
        class QueueObject
        {
            /// <summary>
            /// 向MHA发送的请求
            /// </summary>
            public string request { get; set; }
            /// <summary>
            /// 处理后的结果
            /// </summary>
            public string result { get; set; } = "";
        }
        /// <summary>
        /// API等待队列
        /// </summary>
        Queue<QueueObject> ApiQueue = new();
        /// <summary>
        /// 调用MiraiAPI
        /// </summary>
        /// <param name="type">API类别</param>
        /// <param name="data">调用内容</param>
        public string CallMiraiAPI(MiraiApiType type, object data)
        {
            string apiType = Enum.GetName(typeof(MiraiApiType), type);
            string command = apiType, subCommand = "";
            if (apiType.Contains("_"))// 子命令
            {
                var c = apiType.Split('_');
                command = c[0];
                subCommand = c[1];
            }
            QueueObject queueObject = new() { request = new { syncId = -1, command, subCommand, content = data }.ToJson() };
            ApiQueue.Enqueue(queueObject);
            if (ApiQueue.Count == 1)// 无需队列等待 直接发送请求
                MessageSocket.Send(queueObject.request);
            // 超时脱出
            int timoutCountMax = 1000;
            int timoutCount = 0;
            // 等待消息处理后将结果放置在对象中
            while (queueObject.result == "")
            {
                if (timoutCount > timoutCountMax)// 超时
                {
                    // TODO: 请求是否需要设置默认值
                    break;
                }
                Thread.Sleep(10);
                timoutCount++;
            }
            // 从队列排出
            ApiQueue.Dequeue();
            if (ApiQueue.Count != 0)// 将队列下一元素的请求发送
                MessageSocket.Send(ApiQueue.Peek().request);
            return queueObject.result;// 返回请求结果
        }
    }
}
