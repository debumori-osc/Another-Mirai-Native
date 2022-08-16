using Another_Mirai_Native.Adapter.CQCode.Expand;
using Another_Mirai_Native.Adapter.MiraiEventArgs;
using Another_Mirai_Native.Adapter.MiraiMessageEventArgs;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Native;
using Newtonsoft.Json;
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
    public class MiraiAdapter
    {
        public static MiraiAdapter Instance { get; set; }
        public string WsURL { get; set; }
        public string AuthKey { get; set; }
        public string QQ { get; set; }
        public string SessionKey_Message { get; set; }
        public string SessionKey_Event { get; set; }
        public int reConnect { get; set; }

        public WebSocket MessageSocket;
        public WebSocket EventSocket;
        public delegate void ConnectedStateChange(bool status, string msg);
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

        private void EventSocket_OnOpen(object sender, EventArgs e)
        {
            LogHelper.WriteLog($"连接到事件服务器");
            reConnect = 0;
        }

        private void MessageSocket_OnOpen(object sender, EventArgs e)
        {
            LogHelper.WriteLog($"连接到消息服务器");
            reConnect = 0;
        }

        private void EventSocket_OnClose(object sender, CloseEventArgs e)
        {
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

        private void MessageSocket_OnMessage(object sender, MessageEventArgs e)
        {
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
            if (json.ContainsKey("code"))
            {
                ApiQueue.Dequeue().result = json.ToString();
                Debug.WriteLine(e.Data);
                return;
            }
            if (json.ContainsKey("data") && json["data"].ContainsKey("code"))
            {
                ApiQueue.Dequeue().result = json["data"].ToString();
                Debug.WriteLine(e.Data);
                return;
            }
            ParseMessage(json);
        }

        private void EventSocket_OnMessage(object sender, MessageEventArgs e)
        {
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
            if (json["data"].ContainsKey("code"))
            {
                ApiQueue.Dequeue().result = json["data"].ToString();
                return;
            }
            ParseEvent(json);
        }

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
                case MiraiEvents.BotOfflineEventActive:
                case MiraiEvents.BotOfflineEventForce:
                case MiraiEvents.BotOfflineEventDropped:
                case MiraiEvents.BotReloginEvent:
                case MiraiEvents.FriendInputStatusChangedEvent:
                case MiraiEvents.FriendNickChangedEvent:
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
                case MiraiEvents.BotLeaveEventActive:
                case MiraiEvents.BotLeaveEventKick:
                case MiraiEvents.BotLeaveEventDisband:
                    break;
                case MiraiEvents.GroupRecallEvent:
                    var groupRecall = raw.ToObject<GroupRecallEvent>();
                    // TODO: 获取消息内容
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群撤回", $"群:{groupRecall.group.id}({groupRecall.group.name}) QQ:{groupRecall.authorId} 内容:...", "处理中...");
                    break;
                case MiraiEvents.FriendRecallEvent:
                    var friendRecall = raw.ToObject<FriendRecallEvent>();
                    // TODO: 获取消息内容
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "私聊撤回", $"QQ:{friendRecall.authorId} 内容:...", "处理中...");
                    break;
                case MiraiEvents.NudgeEvent:
                case MiraiEvents.GroupNameChangeEvent:
                case MiraiEvents.GroupEntranceAnnouncementChangeEvent:
                case MiraiEvents.GroupMuteAllEvent:
                    var groupMuteAllRecall = raw.ToObject<GroupMuteAllEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "全体禁言", $"群:{groupMuteAllRecall._operator.group.id}({groupMuteAllRecall._operator.group.name}) 操作人:{groupMuteAllRecall._operator.id}({groupMuteAllRecall._operator.memberName})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupBan, 2, Helper.TimeStamp, groupMuteAllRecall._operator.group.id, 0, 0);
                    break;
                case MiraiEvents.GroupAllowAnonymousChatEvent:
                case MiraiEvents.GroupAllowConfessTalkEvent:
                case MiraiEvents.GroupAllowMemberInviteEvent:
                    break;
                case MiraiEvents.MemberJoinEvent:
                    var memberJoin = raw.ToObject<MemberJoinEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群成员增加", $"QQ:{memberJoin.member.id}({memberJoin.member.memberName}) 群:{memberJoin.member.group.id}({memberJoin.member.group.name}) ", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupMemberIncrease, memberJoin.invitor == null ? 1 : 2, Helper.TimeStamp, memberJoin.member.group.id, 10001, memberJoin.member.id);
                    break;
                case MiraiEvents.MemberLeaveEventKick:
                    var memberLeaveEventKick = raw.ToObject<MemberLeaveEventKick>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群成员被踢出", $"QQ:{memberLeaveEventKick.member.id}({memberLeaveEventKick.member.memberName}) 群:{memberLeaveEventKick.member.group.id}({memberLeaveEventKick.member.group.name}) 操作者:{memberLeaveEventKick._operator.group.id}({memberLeaveEventKick._operator.group.name})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupMemberDecrease, 2, Helper.TimeStamp, memberLeaveEventKick.member.group.id, memberLeaveEventKick._operator.group.id, memberLeaveEventKick.member.id);
                    break;
                case MiraiEvents.MemberLeaveEventQuit:
                    var memberLeaveEventQuit = raw.ToObject<MemberLeaveEventQuit>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群成员离开", $"QQ:{memberLeaveEventQuit.member.id}({memberLeaveEventQuit.member.memberName}) 群:{memberLeaveEventQuit.member.group.id}({memberLeaveEventQuit.member.group.name})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupMemberDecrease, 1, Helper.TimeStamp, memberLeaveEventQuit.member.group.id, 0, memberLeaveEventQuit.member.id);
                    break;
                case MiraiEvents.MemberCardChangeEvent:
                case MiraiEvents.MemberSpecialTitleChangeEvent:
                case MiraiEvents.MemberPermissionChangeEvent:
                    var memberPermissionChangeEvent = raw.ToObject<MemberPermissionChangeEvent>();
                    int memberPermissionChangeStatus = 1;
                    if (memberPermissionChangeEvent.origin == "MEMBER")
                        botGroupPermissionChangeStatus = 2;
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群员权限变更", $"QQ:{memberPermissionChangeEvent.member.id}({memberPermissionChangeEvent.member.memberName}) 群:{memberPermissionChangeEvent.member.group.id}({memberPermissionChangeEvent.member.group.name}) 新权限为:{memberPermissionChangeEvent.current}", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.AdminChange, memberPermissionChangeStatus, Helper.TimeStamp, memberPermissionChangeEvent.member.group.id, memberPermissionChangeEvent.member.id);
                    break;
                case MiraiEvents.MemberMuteEvent:
                    var memberMuteEvent = raw.ToObject<MemberMuteEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群员被禁言", $"QQ:{memberMuteEvent.member.id}({memberMuteEvent.member.memberName}) 群:{memberMuteEvent._operator.group.id}({memberMuteEvent._operator.group.name}) 禁言时长:{memberMuteEvent.durationSeconds} 秒 操作人:{memberMuteEvent._operator.id}({memberMuteEvent._operator.memberName})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupBan, 2, Helper.TimeStamp, memberMuteEvent._operator.group.id, memberMuteEvent.member.id, memberMuteEvent.durationSeconds);
                    break;
                case MiraiEvents.MemberUnmuteEvent:
                    var memberUnmuteEvent = raw.ToObject<MemberUnmuteEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "群员被解除禁言", $"QQ:{memberUnmuteEvent.member.id}({memberUnmuteEvent.member.memberName}) 群:{memberUnmuteEvent._operator.group.id}({memberUnmuteEvent._operator.group.name}) 操作人:{memberUnmuteEvent._operator.id}({memberUnmuteEvent._operator.memberName})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupBan, 1, Helper.TimeStamp, memberUnmuteEvent._operator.group.id, memberUnmuteEvent.member.id, 0);
                    break;
                case MiraiEvents.MemberHonorChangeEvent:
                case MiraiEvents.NewFriendRequestEvent:
                    var newFriendRequestEvent = raw.ToObject<NewFriendRequestEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "添加好友请求", $"QQ:{newFriendRequestEvent.fromId}({newFriendRequestEvent.nick}) 备注:{newFriendRequestEvent.message} 来源群:{newFriendRequestEvent.groupId}", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.FriendRequest, 1, Helper.TimeStamp, newFriendRequestEvent.fromId, newFriendRequestEvent.nick, newFriendRequestEvent.eventId.ToString());
                    break;
                case MiraiEvents.MemberJoinRequestEvent:
                    var memberJoinRequestEvent = raw.ToObject<MemberJoinRequestEvent>();
                    logid = LogHelper.WriteLog(Enums.LogLevel.Info, "AMN框架", "添加群请求", $"QQ:{memberJoinRequestEvent.fromId}({memberJoinRequestEvent.nick}) 备注:{memberJoinRequestEvent.message} 群:{memberJoinRequestEvent.groupId}({memberJoinRequestEvent.groupName})", "处理中...");
                    handledPlugin = PluginManagment.Instance.CallFunction(FunctionEnums.GroupAddRequest, 1, Helper.TimeStamp, memberJoinRequestEvent.groupId, memberJoinRequestEvent.fromId, memberJoinRequestEvent.message, memberJoinRequestEvent.eventId.ToString());
                    break;
                case MiraiEvents.BotInvitedJoinGroupRequestEvent:
                    break;
                case MiraiEvents.OtherClientOnlineEvent:
                case MiraiEvents.OtherClientOfflineEvent:
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

        private void ParseMessage(JObject msg)
        {
            MiraiMessageEvents events = Helper.String2Enum<MiraiMessageEvents>(msg["data"]["type"].ToString());
            List<MiraiMessageBase> chainMsg = new();
            MiraiMessageTypeDetail.Source source = null;
            foreach (var item in msg["data"]["messageChain"] as JArray)
            {
                MiraiMessageType msgType = Helper.String2Enum<MiraiMessageType>(item["type"].ToString());
                switch (msgType)
                {
                    case MiraiMessageType.Source:
                        source = item.ToObject<MiraiMessageTypeDetail.Source>();
                        chainMsg.Add(source);
                        break;
                    case MiraiMessageType.Quote:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Quote>());
                        break;
                    case MiraiMessageType.At:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.At>());
                        break;
                    case MiraiMessageType.AtAll:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.AtAll>());
                        break;
                    case MiraiMessageType.Face:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Face>());
                        break;
                    case MiraiMessageType.Plain:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Plain>());
                        break;
                    case MiraiMessageType.Image:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Image>());
                        break;
                    case MiraiMessageType.FlashImage:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.FlashImage>());
                        break;
                    case MiraiMessageType.Voice:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Voice>());
                        break;
                    case MiraiMessageType.Xml:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Xml>());
                        break;
                    case MiraiMessageType.Json:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Json>());
                        break;
                    case MiraiMessageType.App:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.App>());
                        break;
                    case MiraiMessageType.Poke:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Poke>());
                        break;
                    case MiraiMessageType.Dice:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Dice>());
                        break;
                    case MiraiMessageType.MarketFace:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.MarketFace>());
                        break;
                    case MiraiMessageType.MusicShare:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.MusicShare>());
                        break;
                    case MiraiMessageType.Forward:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Forward>());
                        break;
                    case MiraiMessageType.File:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.File>());
                        break;
                    case MiraiMessageType.MiraiCode:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.MiraiCode>());
                        break;
                    default:
                        break;
                }
            }

            string parsedMsg = CQCodeBuilder.Parse(chainMsg);
            DispatchMessage(events, msg["data"], source, chainMsg, parsedMsg);
        }
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
                    LogHelper.WriteLog(Enums.LogLevel.InfoReceive, "OPQBot框架", "文件上传", $"来源群:{group.sender.group.id}({group.sender.group.name}) 来源QQ:{group.sender.id}({group.sender.memberName}) " +
                        $"文件名:{file.name} 大小:{file.size / 1000}KB FileID:{file.id}", $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");
                    return;
                }
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

        public bool Connect()
        {
            MessageSocket.Connect();
            EventSocket.Connect();
            return false;
        }
        class QueueObject
        {
            public string request { get; set; }
            public string result { get; set; } = "";
        }
        Queue<QueueObject> ApiQueue = new();
        public string CallMiraiAPI(MiraiApiType type, object data)
        {
            string apiType = Enum.GetName(typeof(MiraiApiType), type);
            string command=apiType, subCommand="";
            if (apiType.Contains("_"))
            {
                var c = apiType.Split('_');
                command = c[0];
                subCommand = c[1];
            }
            QueueObject queueObject = new() { request= new { syncId=-1, command, subCommand, content = data  }.ToJson() };
            ApiQueue.Enqueue(queueObject);
            if(ApiQueue.Count == 1)
                MessageSocket.Send(queueObject.request);
            while (queueObject.result == "")
            {
                Thread.Sleep(10);
            }
            if (ApiQueue.Count != 0)
                MessageSocket.Send(ApiQueue.Peek().request);
            return queueObject.result;
        }
        public int CallMiraiAPI(MiraiApiType type, params object[] args)
        {
            switch (type)
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
                    
                    break;
                case MiraiApiType.sendGroupMessage:
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
            return 0;
        }
    }
}
