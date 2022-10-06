using Another_Mirai_Native.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CQP
{
    internal class DllMain
    {
        static Encoding GB18030 = Encoding.GetEncoding("GB18030");

        [DllExport(ExportName = "ConnectServer", CallingConvention = CallingConvention.StdCall)]
        public static void ConnectServer()
        {
            new Clinet();
        }
        [DllExport(ExportName = "CQ_sendGroupMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendGroupMsg(int authCode, long groupid, IntPtr msg)
        {
            string text = msg.ToString(GB18030);
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.sendGroupMessage, authCode, args = new { groupid, text } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)r.json["callResult"];
        }

        [DllExport(ExportName = "CQ_sendPrivateMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendPrivateMsg(int authCode, long qqId, IntPtr msg)
        {
            string text = msg.ToString(GB18030);
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.sendFriendMessage, authCode, args = new { qqId, text } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)r.json["callResult"];
        }

        [DllExport(ExportName = "CQ_deleteMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_deleteMsg(int authCode, long msgId)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.recall, authCode, args = new { msgId } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)r.json["callResult"];
        }

        [DllExport(ExportName = "CQ_sendLikeV2", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendLikeV2(int authCode, long qqId, int count)
        {
            return 0;
        }

        [DllExport(ExportName = "CQ_getCookiesV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getCookiesV2(int authCode, IntPtr domain)
        {
            return IntPtr.Zero;
        }

        [DllExport(ExportName = "CQ_getRecordV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getRecordV2(int authCode, IntPtr file, IntPtr format)
        {
            return IntPtr.Zero;
        }

        [DllExport(ExportName = "CQ_getCsrfToken", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_getCsrfToken(int authCode)
        {
            return -1;
        }

        [DllExport(ExportName = "CQ_getAppDirectory", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getAppDirectory(int authCode)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallCQFunction, new { type = "GetAppDirectory", authCode });
            if (r.Fail)
            {
                return IntPtr.Zero;
            }
            return r.json["callResult"].ToNative();
        }

        [DllExport(ExportName = "CQ_getLoginQQ", CallingConvention = CallingConvention.StdCall)]
        public static long CQ_getLoginQQ(int authCode)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallCQFunction, new { type = "GetLoginQQ", authCode });
            if (r.Fail)
            {
                return -1;
            }
            return (long)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_getLoginNick", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getLoginNick(int authCode)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallCQFunction, new { type = "GetLoginNick", authCode });
            if (r.Fail)
            {
                return IntPtr.Zero;
            }
            return r.json["callResult"].ToNative();
        }

        [DllExport(ExportName = "CQ_setGroupKick", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupKick(int authCode, long groupId, long qqId, bool refuses)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.kick, authCode, args = new { groupId, qqId } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupBan(int authCode, long groupId, long qqId, long time)
        {
            MiraiApiType type = MiraiApiType.mute;
            if (time == 0)
            {
                type = MiraiApiType.unmute;
            }
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type, authCode, args = new { groupId, qqId, time } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupAdmin", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAdmin(int authCode, long groupId, long qqId, bool isSet)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.memberAdmin, authCode, args = new { groupId, qqId, isSet } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupSpecialTitle", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupSpecialTitle(int authCode, long groupId, long qqId, IntPtr title, long durationTime)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.memberInfo_update, authCode, args = new { groupId, qqId, title = title.ToString(GB18030) } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupWholeBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupWholeBan(int authCode, long groupId, bool isOpen)
        {
            MiraiApiType type = MiraiApiType.unmuteAll;
            if (isOpen)
            {
                type = MiraiApiType.muteAll;
            }
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type, authCode, args = new { groupId } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupAnonymousBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAnonymousBan(int authCode, long groupId, IntPtr anonymous, long banTime)
        {
            return -1;
        }

        [DllExport(ExportName = "CQ_setGroupAnonymous", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAnonymous(int authCode, long groupId, bool isOpen)
        {
            return -1;
        }

        [DllExport(ExportName = "CQ_setGroupCard", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupCard(int authCode, long groupId, long qqId, IntPtr newCard)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.memberInfo_update, authCode, args = new { groupId, qqId, newCard = newCard.ToString(GB18030) } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupLeave", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupLeave(int authCode, long groupId, bool isDisband)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.quit, authCode, args = new { groupId } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setDiscussLeave", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setDiscussLeave(int authCode, long disscussId)
        {
            return -1;
        }
        [DllExport(ExportName = "CQ_setFriendAddRequest", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setFriendAddRequest(int authCode, IntPtr identifying, int requestType, IntPtr appendMsg)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.resp_newFriendRequestEvent, authCode, args = new { eventId = identifying.ToString(GB18030), requestType, message = appendMsg.ToString(GB18030) } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupAddRequestV2", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAddRequestV2(int authCode, IntPtr identifying, int requestType, int responseType, IntPtr appendMsg)
        {
            MiraiApiType apiType = MiraiApiType.resp_memberJoinRequestEvent;
            if (requestType == 2)
            {
                apiType = MiraiApiType.resp_botInvitedJoinGroupRequestEvent;
            }
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = apiType, authCode, args = new { eventId = identifying.ToString(GB18030), responseType, message = appendMsg.ToString(GB18030) } });
            if (r.Fail)
            {
                return -1;
            }
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_addLog", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_addLog(int authCode, int priority, IntPtr type, IntPtr msg)
        {
            var r = Clinet.Instance.Send(WsServerFunction.AddLog, new { authCode, type = type.ToString(GB18030), priority, msg = msg.ToString(GB18030) } , false);
            return 1;
        }

        [DllExport(ExportName = "CQ_setFatal", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setFatal(int authCode, IntPtr errorMsg)
        {
            var r = Clinet.Instance.Send(WsServerFunction.AddLog, new { authCode, args = new { priority = 40, msg = errorMsg.ToString(GB18030) } }, false);
            return 1;
        }

        [DllExport(ExportName = "CQ_getGroupMemberInfoV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupMemberInfoV2(int authCode, long groupId, long qqId, bool isCache)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.memberProfile, authCode, args = new { groupId, qqId } });
            if (r.Fail)
            {
                return IntPtr.Zero;
            }
            return r.json["callResult"].ToNative();
        }

        [DllExport(ExportName = "CQ_getGroupMemberList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupMemberList(int authCode, long groupId)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.memberList, authCode, args = new { groupId } });
            if (r.Fail)
            {
                return IntPtr.Zero;
            }
            return r.json["callResult"].ToNative();
        }

        [DllExport(ExportName = "CQ_getGroupList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupList(int authCode)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.groupList, authCode });
            if (r.Fail)
            {
                return IntPtr.Zero;
            }
            return r.json["callResult"].ToNative();
        }

        [DllExport(ExportName = "CQ_getStrangerInfo", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getStrangerInfo(int authCode, long qqId, bool notCache)
        {
            MemoryStream streamMain = new();
            BinaryWriter binaryWriterMain = new(streamMain);
            BinaryWriterExpand.Write_Ex(binaryWriterMain, qqId);
            BinaryWriterExpand.Write_Ex(binaryWriterMain, "");
            BinaryWriterExpand.Write_Ex(binaryWriterMain, 0);

            return Marshal.StringToHGlobalAnsi(Convert.ToBase64String(streamMain.ToArray()));
        }

        [DllExport(ExportName = "CQ_canSendImage", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_canSendImage(int authCode)
        {
            return 1;
        }

        [DllExport(ExportName = "CQ_canSendRecord", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_canSendRecord(int authCode)
        {
            return 1;
        }

        [DllExport(ExportName = "CQ_getImage", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getImage(int authCode, IntPtr file)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallCQFunction, new { type = "GetImage", authCode, args = new { path = file.ToString(GB18030) } });
            if (r.Fail)
            {
                return IntPtr.Zero;
            }
            return r.json["callResult"].ToNative();
        }

        [DllExport(ExportName = "CQ_getGroupInfo", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupInfo(int authCode, long groupId, bool notCache)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallCQFunction, new { type = "GetGroupInfo", authCode, args = new { groupId } });
            if (r.Fail)
            {
                return IntPtr.Zero;
            }
            return r.json["callResult"].ToNative();
        }

        [DllExport(ExportName = "CQ_getFriendList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getFriendList(int authCode, bool reserved)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.friendList, authCode, args = new { reserved } });
            if (r.Fail)
            {
                return IntPtr.Zero;
            }
            return r.json["callResult"].ToNative();
        }
        [DllExport(ExportName = "cq_start", CallingConvention = CallingConvention.StdCall)]
        public static bool cq_start(IntPtr path, int authCode)
        {
            return true;
        }
    }
}
