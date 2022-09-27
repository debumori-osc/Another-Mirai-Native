using Another_Mirai_Native.Adapter.CQCode.Expand;
using Another_Mirai_Native.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Another_Mirai_Native.Adapter
{
    public static class MiraiAPI
    {
        static Encoding GB18030 = Encoding.GetEncoding("GB18030");

        public static string GetBotNickName()
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
            };
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.botProfile, request);
            if(json == null)
            {
                return "";
            }
            return json["nickname"].ToString();
        }
        public static string GetMessageByMsgId(int messageId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                id = messageId
            };
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.messageFromId, request);
            if (json == null)
            {
                return "";
            }
            if (((int)json["code"]) == 0)
            {
                return CQCodeBuilder.Parse(CQCodeBuilder.ParseJArray2MiraiMessageBaseList(json["data"]["messageChain"] as JArray));
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 发送未转码的CQ码消息
        /// </summary>
        /// <param name="group"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int SendGroupMessage(long group, string message)
        {
            MiraiMessageBase[] msgChains = CQCodeBuilder.BuildMessageChains(message).ToArray();
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = group,
                messageChain = msgChains
            };
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.sendGroupMessage, request);
            if (json == null)
            {
                return 0;
            }
            if (((int)json["code"]) == 0)
            {
                return (int)json["messageId"];
            }
            else
            {
                Debug.WriteLine(json["msg"].ToString());            
                return 0;
            }
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
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.sendFriendMessage, request);
            if (json == null)
            {
                return 0;
            }
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
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.recall, request);
            if (json == null)
            {
                return 0;
            }
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
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.kick, request);
            if (json == null)
            {
                return 0;
            }
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
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.mute, request);
            if (json == null)
            {
                return 0;
            }
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
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.unmute, request);
            if (json == null)
            {
                return 0;
            }
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
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.memberAdmin, request);
            if (json == null)
            {
                return 0;
            }
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
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.memberInfo_update, request);
            if (json == null)
            {
                return 0;
            }
            return (int)json["code"];
        }
        public static int GroupMute(long groupId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
            };
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.muteAll, request);
            if (json == null)
            {
                return 0;
            }
            return (int)json["code"];
        }
        public static int GroupUnmute(long groupId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
            };
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.unmuteAll, request);
            if (json == null)
            {
                return 0;
            }
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
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.memberInfo_update, request);
            if (json == null)
            {
                return 0;
            }
            return (int)json["code"];
        }
        public static int QuitGroup(long groupId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId
            };
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.quit, request);
            if (json == null)
            {
                return 0;
            }
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
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.resp_newFriendRequestEvent, request);
            if (json == null)
            {
                return 0;
            }
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
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.resp_memberJoinRequestEvent, request);
            if (json == null)
            {
                return 0;
            }
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
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.resp_botInvitedJoinGroupRequestEvent, request);
            if (json == null)
            {
                return 0;
            }
            return (int)json["code"];
        }
        public static string ParseGroupList2CQData(JArray groupList)
        {
            if (groupList == null) return string.Empty;
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
        public static JArray GetGroupList()
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
            };
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.groupList, request);
            if (json == null)
            {
                return null;
            }
            if (((int)json["code"]) == 0)
            {
                return json["data"] as JArray;
            }
            else
            {
                return null;
            }
        }
        public static string ParseFriendList2CQData(JArray friendList)
        {
            if (friendList == null) return string.Empty;

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
        public static JArray GetFriendList()
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
            };
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.friendList, request);
            if (json == null)
            {
                return null;
            }
            if (((int)json["code"]) == 0)
            {
               return json["data"] as JArray;
            }
            else
            {
                return null;
            }
        }
        public static string ParseMemberList2CQData(JArray memberList, long target)
        {
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
                        admin_num = 1;
                        break;
                    case "ADMINISTRATOR":
                        admin_num = 2;
                        break;
                    case "OWNER":
                        admin_num = 3;
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
                BinaryWriterExpand.Write_Ex(binaryWriter, Helper.DateTime2TimeStamp(DateTime.Now.AddYears(10)));
                BinaryWriterExpand.Write_Ex(binaryWriter, 1);

                BinaryWriterExpand.Write_Ex(binaryWriterMain, (short)stream.Length);
                binaryWriterMain.Write(stream.ToArray());
            }
            return Convert.ToBase64String(streamMain.ToArray());
        }
        public static JArray GetGroupMemberList(long target) 
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target
            };
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.memberList, request);
            if (json == null)
            {
                return null;
            }
            if (((int)json["code"]) == 0)
            {
                return json["data"] as JArray;
            }
            else
            {
                return null;
            }
        }
        public static string ParseGroupMemberInfo2CQData(JArray groupMemArr, JObject json, long groupId, long QQId)
        {
            if (json == null) return null;
            JObject appendInfo = (JObject)groupMemArr.FirstOrDefault(x => ((long)x["id"]) == QQId);
            if (appendInfo == null) return null;
            var targetuser = json;
            int userPermission = 0, sex = 0;
            switch (targetuser["permission"].ToString())
            {
                case "MEMBER":
                    userPermission = 1;
                    break;
                case "ADMINISTRATOR":
                    userPermission = 2;
                    break;
                case "OWNER":
                    userPermission = 3;
                    break;
                default:
                    break;
            }
            switch (appendInfo["sex"].ToString())
            {
                case "UNKNOWN":
                    sex = 255;
                    break;
                case "MALE":
                    sex = 0;
                    break;
                case "FEMALE":
                    sex = 1;
                    break;
                default:
                    break;
            }
            MemoryStream stream = new();
            BinaryWriter binaryWriter = new(stream);

            BinaryWriterExpand.Write_Ex(binaryWriter, groupId);
            BinaryWriterExpand.Write_Ex(binaryWriter, QQId);
            BinaryWriterExpand.Write_Ex(binaryWriter, targetuser["memberName"].ToString());
            BinaryWriterExpand.Write_Ex(binaryWriter, targetuser["memberName"].ToString());
            BinaryWriterExpand.Write_Ex(binaryWriter, sex);
            BinaryWriterExpand.Write_Ex(binaryWriter, ((int)appendInfo["age"]));
            BinaryWriterExpand.Write_Ex(binaryWriter, "中国");
            BinaryWriterExpand.Write_Ex(binaryWriter, (int)targetuser["joinTimestamp"]);
            BinaryWriterExpand.Write_Ex(binaryWriter, (int)targetuser["lastSpeakTimestamp"]);
            BinaryWriterExpand.Write_Ex(binaryWriter, targetuser["level"].ToString());
            BinaryWriterExpand.Write_Ex(binaryWriter, userPermission);
            BinaryWriterExpand.Write_Ex(binaryWriter, 0);
            BinaryWriterExpand.Write_Ex(binaryWriter, targetuser["specialTitle"].ToString());
            BinaryWriterExpand.Write_Ex(binaryWriter, Helper.DateTime2TimeStamp(DateTime.Now.AddYears(10)));
            BinaryWriterExpand.Write_Ex(binaryWriter, 1);
            return Convert.ToBase64String(stream.ToArray());
        }
        public static JObject GetGroupMemberInfo(long groupId, long QQId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = groupId,
                memberId = QQId
            };
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.memberInfo_get, request);
            if (json != null)
            {
                return json;
            }
            else
            {
                return null;
            }
        }
        public static JObject GetFriendInfo(long QQId)
        {
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = QQId
            };
            JObject json = MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.friendProfile, request);
            if (json != null)
            {
                return json;
            }
            else
            {
                return null;
            }
        }
        public static string ParseGroupInfo2CQData(long groupId, string name, int memberCount)
        {
            MemoryStream stream = new();
            BinaryWriter binaryWriter = new(stream);

            BinaryWriterExpand.Write_Ex(binaryWriter, groupId);
            BinaryWriterExpand.Write_Ex(binaryWriter, name);
            BinaryWriterExpand.Write_Ex(binaryWriter, memberCount);
            BinaryWriterExpand.Write_Ex(binaryWriter, memberCount + 100); // 群容量 无法获取
            return Convert.ToBase64String(stream.ToArray());
        }
    }
}
