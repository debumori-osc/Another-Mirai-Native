using Another_Mirai_Native.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WebSocketSharp;

namespace Another_Mirai_Native.Adapter
{
    public class MiraiAdapter
    {
        public string WsURL { get; set; }
        public string AuthKey { get; set; }
        public string QQ { get; set; }
        public string SessionKey_Message { get; set; }
        public string SessionKey_Event { get; set; }
        
        public WebSocket MessageSocket;
        public WebSocket EventSocket;
        public delegate void ConnectedStateChange(bool status, string msg);
        public event ConnectedStateChange ConnectedStateChanged;
        public MiraiAdapter(string url, string qq, string authkey)
        {
            if (url.EndsWith("/")) url = url[..^1];
            WsURL = url;
            AuthKey = authkey;
            QQ = qq;
            string message_connecturl = $"{WsURL}/message?verifyKey={AuthKey}&qq={QQ}";
            string event_connecturl = $"{WsURL}/event?verifyKey={AuthKey}&qq={QQ}";
            MessageSocket = new WebSocket(message_connecturl);
            MessageSocket.OnMessage += MessageSocket_OnMessage;

            EventSocket = new WebSocket(message_connecturl);
            EventSocket.OnMessage += EventSocket_OnMessage;
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
            }
            
            ParseEvent(json);
        }

        private void ParseEvent(JObject msg)
        {
            MiraiEvents events = Helper.String2Enum<MiraiEvents>(msg["data"]["type"].ToString());
            switch (events)
            {
                case MiraiEvents.BotOnlineEvent:
                    break;
                case MiraiEvents.BotOfflineEventActive:
                    break;
                case MiraiEvents.BotOfflineEventForce:
                    break;
                case MiraiEvents.BotOfflineEventDropped:
                    break;
                case MiraiEvents.BotReloginEvent:
                    break;
                case MiraiEvents.FriendInputStatusChangedEvent:
                    break;
                case MiraiEvents.FriendNickChangedEvent:
                    break;
                case MiraiEvents.BotGroupPermissionChangeEvent:
                    break;
                case MiraiEvents.BotMuteEvent:
                    break;
                case MiraiEvents.BotUnmuteEvent:
                    break;
                case MiraiEvents.BotJoinGroupEvent:
                    break;
                case MiraiEvents.BotLeaveEventActive:
                    break;
                case MiraiEvents.BotLeaveEventKick:
                    break;
                case MiraiEvents.BotLeaveEventDisband:
                    break;
                case MiraiEvents.GroupRecallEvent:
                    break;
                case MiraiEvents.FriendRecallEvent:
                    break;
                case MiraiEvents.NudgeEvent:
                    break;
                case MiraiEvents.GroupNameChangeEvent:
                    break;
                case MiraiEvents.GroupEntranceAnnouncementChangeEvent:
                    break;
                case MiraiEvents.GroupMuteAllEvent:
                    break;
                case MiraiEvents.GroupAllowAnonymousChatEvent:
                    break;
                case MiraiEvents.GroupAllowConfessTalkEvent:
                    break;
                case MiraiEvents.GroupAllowMemberInviteEvent:
                    break;
                case MiraiEvents.MemberJoinEvent:
                    break;
                case MiraiEvents.MemberLeaveEventKick:
                    break;
                case MiraiEvents.MemberLeaveEventQuit:
                    break;
                case MiraiEvents.MemberCardChangeEvent:
                    break;
                case MiraiEvents.MemberSpecialTitleChangeEvent:
                    break;
                case MiraiEvents.MemberPermissionChangeEvent:
                    break;
                case MiraiEvents.MemberMuteEvent:
                    break;
                case MiraiEvents.MemberUnmuteEvent:
                    break;
                case MiraiEvents.MemberHonorChangeEvent:
                    break;
                case MiraiEvents.NewFriendRequestEvent:
                    break;
                case MiraiEvents.MemberJoinRequestEvent:
                    break;
                case MiraiEvents.BotInvitedJoinGroupRequestEvent:
                    break;
                case MiraiEvents.OtherClientOnlineEvent:
                    break;
                case MiraiEvents.OtherClientOfflineEvent:
                    break;
                case MiraiEvents.CommandExecutedEvent:
                    break;
                default:
                    break;
            }
        }

        private void ParseMessage(JObject msg)
        {
            MiraiMessageEvents events = Helper.String2Enum<MiraiMessageEvents>(msg["data"]["type"].ToString());
            List<MiraiMessageBase> chainMsg = new();
            foreach (var item in msg["data"]["messageChain"] as JArray)
            {
                MiraiMessageType msgType = Helper.String2Enum<MiraiMessageType>(item["type"].ToString());
                switch (msgType)
                {
                    case MiraiMessageType.Source:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Source>());
                        break;
                    case MiraiMessageType.Quote:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Quote>());
                        break;
                    case MiraiMessageType.At:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.At> ());
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
            DispatchMessage(events, parsedMsg);
        }
        private void DispatchMessage(MiraiMessageEvents msgEvent, string msg)
        {
            switch (msgEvent)
            {
                case MiraiMessageEvents.FriendMessage:
                    break;
                case MiraiMessageEvents.GroupMessage:
                    break;
                case MiraiMessageEvents.TempMessage:
                    break;
                case MiraiMessageEvents.StrangerMessage:
                    break;
                case MiraiMessageEvents.OtherClientMessage:
                    break;
                default:
                    break;
            }
        }
        private void Websocket_OnOpen(object sender, EventArgs e)
        {
            Debug.WriteLine("Connect");
        }

        public bool Connect()
        {
            MessageSocket.Connect();
            EventSocket.Connect();
            return false;
        }
    }
}
