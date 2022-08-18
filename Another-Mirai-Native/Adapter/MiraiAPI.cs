using Another_Mirai_Native.Adapter.CQCode.Expand;
using Another_Mirai_Native.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Another_Mirai_Native.Adapter
{
    public static class MiraiAPI
    {
        public static string GetBotNickName()
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.botProfile, request));
            return json["nickname"].ToString();
        }
        public static string GetMessageByMsgId(int messageId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                id = messageId
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.messageFromId, request));
            if (((int)json["code"]) == 0)
            {
                return CQCodeBuilder.Parse(CQCodeBuilder.ParseJArray2MiraiMessageBaseList(json["data"]["messageChain"] as JArray));
            }
            else
            {
                return string.Empty;
            }
        }
        public static int SendGroupMessage(long group, string message)
        {
            MiraiMessageBase[] msgChains = CQCodeBuilder.BuildMessageChains(message).ToArray();
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = group,
                messageChain = msgChains
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.sendGroupMessage, request));
            if (((int)json["code"]) == 0)
            {
                return (int)json["messageId"];
            }
            else
            {
                Debug.WriteLine(json["msg"].ToString());
            }
            return 0;
        }
        public static int SendFriendMessage(long QQ, string message)
        {
            MiraiMessageBase[] msgChains = CQCodeBuilder.BuildMessageChains(message).ToArray();
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = QQ,
                messageChain = msgChains
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.sendFriendMessage, request));
            if (((int)json["code"]) == 0)
            {
                return (int)json["messageId"];
            }
            return 0;
        }
        public static int RecallMessage(int msgId, long target)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target,
                messageId = msgId
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.recall, request));
            return (int)json["code"];
        }
        public static int KickGroupMember(long groupId, long QQId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
                memberId = QQId
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.kick, request));
            return (int)json["code"];
        }
        public static int MuteGroupMemeber(long groupId, long QQId, long time)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
                memberId = QQId,
                time
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.mute, request));
            return (int)json["code"];
        }
        public static int UnmuteGroupMemeber(long groupId, long QQId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
                memberId = QQId
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.unmute, request));
            return (int)json["code"];
        }
        public static int SetAdmin(long groupId, long QQId, bool set)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
                memberId = QQId,
                assign = set
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.memberAdmin, request));
            return (int)json["code"];
        }
        public static int SetSpecialTitle(long groupId, long QQId, string title)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
                memberId = QQId,
                info = new
                {
                    specialTitle = title
                }
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.memberInfo_update, request));
            return (int)json["code"];
        }
        public static int GroupMute(long groupId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.muteAll, request));
            return (int)json["code"];
        }
        public static int GroupUnmute(long groupId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.unmuteAll, request));
            return (int)json["code"];
        }
        public static int SetGroupCard(long groupId, long QQId, string newCard)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
                memberId = QQId,
                info = new
                {
                    name = newCard
                }
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.memberInfo_update, request));
            return (int)json["code"];
        }
        public static int QuitGroup(long groupId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.quit, request));
            return (int)json["code"];
        }
        public static int HandleFriendRequest(long eventId, int operate, string message)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                eventId,
                operate,
                message
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.resp_newFriendRequestEvent, request));
            return (int)json["code"];
        }
        public static int HandleGroupRequest(long eventId, int operate, string message)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                eventId,
                operate,
                message
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.resp_memberJoinRequestEvent, request));
            return (int)json["code"];
        }
        public static int HandleInviteRequest(long eventId, int operate, string message)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                eventId,
                operate,
                message
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.resp_botInvitedJoinGroupRequestEvent, request));
            return (int)json["code"];
        }
        public static string GetGroupList()
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.groupList, request));
            if (((int)json["code"]) == 0)
            {
                JArray groupList = json["data"] as JArray;
                MemoryStream streamMain = new();
                BinaryWriter binaryWriterMain = new(streamMain);//最终要进行编码的字节流
                BinaryWriterExpand.Write_Ex(binaryWriterMain, groupList.Count);//群数量
                foreach (var item in groupList)
                {
                    MemoryStream stream = new();
                    BinaryWriter binaryWriter = new(stream);//临时字节流,用于统计每个群信息的字节数量

                    BinaryWriterExpand.Write_Ex(binaryWriter, (long)item["id"]);
                    BinaryWriterExpand.Write_Ex(binaryWriter, item["name"].ToString());

                    BinaryWriterExpand.Write_Ex(binaryWriterMain, (short)stream.Length);//将临时字节流的字节长度写入主字节流
                    binaryWriterMain.Write(stream.ToArray());//写入数据
                }
                return Convert.ToBase64String(streamMain.ToArray());
            }
            else
            {
                return string.Empty;
            }
        }
        public static string GetFriendList()
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.friendList, request));
            if (((int)json["code"]) == 0)
            {
                JArray friendList = json["data"] as JArray;
                MemoryStream streamMain = new();
                BinaryWriter binaryWriterMain = new(streamMain);
                BinaryWriterExpand.Write_Ex(binaryWriterMain, friendList.Count);
                foreach (var item in friendList)
                {
                    MemoryStream stream = new();
                    BinaryWriter binaryWriter = new(stream);

                    BinaryWriterExpand.Write_Ex(binaryWriter, (long)item["id"]);
                    BinaryWriterExpand.Write_Ex(binaryWriter, item["nickname"].ToString());
                    BinaryWriterExpand.Write_Ex(binaryWriter, item["remark"].ToString());

                    BinaryWriterExpand.Write_Ex(binaryWriterMain, (short)stream.Length);
                    binaryWriterMain.Write(stream.ToArray());
                }
                return Convert.ToBase64String(streamMain.ToArray());
            }
            else
            {
                return string.Empty;
            }
        }
        public static string GetGroupMemberList(long target)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.memberList, request));
            if (((int)json["code"]) == 0)
            {
                JArray memberList = json["data"] as JArray;
                MemoryStream streamMain = new();
                BinaryWriter binaryWriterMain = new(streamMain);
                BinaryWriterExpand.Write_Ex(binaryWriterMain, memberList.Count);
                foreach (var item in memberList)
                {
                    MemoryStream stream = new();
                    BinaryWriter binaryWriter = new(stream);
                    string admin = item["permission"].ToString();
                    int admin_num = 0;
                    switch (admin)
                    {
                        case "MEMBER":
                            admin_num = 0;
                            break;
                        case "ADMINISTRATOR":
                            admin_num = 1;
                            break;
                        case "OWNER":
                            admin_num = 2;
                            break;
                        default:
                            break;
                    }
                    BinaryWriterExpand.Write_Ex(binaryWriter, target);
                    BinaryWriterExpand.Write_Ex(binaryWriter, Convert.ToInt64(item["id"].ToString()));
                    BinaryWriterExpand.Write_Ex(binaryWriter, item["memberName"].ToString());
                    BinaryWriterExpand.Write_Ex(binaryWriter, item["memberName"].ToString());
                    BinaryWriterExpand.Write_Ex(binaryWriter, 0);
                    BinaryWriterExpand.Write_Ex(binaryWriter, 0);
                    BinaryWriterExpand.Write_Ex(binaryWriter, "unkown");
                    BinaryWriterExpand.Write_Ex(binaryWriter, Convert.ToInt32(item["joinTimestamp"].ToString()));
                    BinaryWriterExpand.Write_Ex(binaryWriter, Convert.ToInt32(item["lastSpeakTimestamp"].ToString()));
                    BinaryWriterExpand.Write_Ex(binaryWriter, $"头衔0级");
                    BinaryWriterExpand.Write_Ex(binaryWriter, admin_num);
                    BinaryWriterExpand.Write_Ex(binaryWriter, 0);
                    BinaryWriterExpand.Write_Ex(binaryWriter, item["specialTitle"].ToString());
                    BinaryWriterExpand.Write_Ex(binaryWriter, 2051193600);
                    BinaryWriterExpand.Write_Ex(binaryWriter, 1);

                    BinaryWriterExpand.Write_Ex(binaryWriterMain, (short)stream.Length);
                    binaryWriterMain.Write(stream.ToArray());
                }
                return Convert.ToBase64String(streamMain.ToArray());
            }
            else
            {
                return string.Empty;
            }
        }
        public static string GetGroupMemberInfo(long groupId, long QQId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
                memberId = QQId
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.memberInfo_get, request));
            if (((int)json["code"]) == 0)
            {
                var targetuser = json["data"];
                MemoryStream stream = new();
                BinaryWriter binaryWriter = new(stream);

                BinaryWriterExpand.Write_Ex(binaryWriter, groupId);
                BinaryWriterExpand.Write_Ex(binaryWriter, QQId);
                BinaryWriterExpand.Write_Ex(binaryWriter, targetuser["nickname"].ToString());
                BinaryWriterExpand.Write_Ex(binaryWriter, "");
                BinaryWriterExpand.Write_Ex(binaryWriter, Convert.ToInt32(targetuser["sex"].ToString()));
                BinaryWriterExpand.Write_Ex(binaryWriter, Convert.ToInt32(targetuser["age"].ToString()));
                BinaryWriterExpand.Write_Ex(binaryWriter, "unkown");
                BinaryWriterExpand.Write_Ex(binaryWriter, 0);
                BinaryWriterExpand.Write_Ex(binaryWriter, 0);
                BinaryWriterExpand.Write_Ex(binaryWriter, $"头衔{0}级");
                BinaryWriterExpand.Write_Ex(binaryWriter, 0);
                BinaryWriterExpand.Write_Ex(binaryWriter, 0);
                BinaryWriterExpand.Write_Ex(binaryWriter, "");
                BinaryWriterExpand.Write_Ex(binaryWriter, 2051193600);
                BinaryWriterExpand.Write_Ex(binaryWriter, 1);
                return Convert.ToBase64String(stream.ToArray());
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
