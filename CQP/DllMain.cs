using Another_Mirai_Native.Enums;
using System;
using System.Collections.Generic;
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
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type=MiraiApiType.sendGroupMessage, args=new { authCode, groupid, text } });
            return (int)r.json["callResult"];
        }

        [DllExport(ExportName = "CQ_sendPrivateMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendPrivateMsg(int authCode, long qqId, IntPtr msg)
        {
            string text = msg.ToString(GB18030);
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.sendFriendMessage, args = new { authCode, qqId, text } });
            return (int)r.json["callResult"];
        }

        [DllExport(ExportName = "CQ_deleteMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_deleteMsg(int authCode, long msgId)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.recall, args = new { authCode, msgId } });
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
            var r = Clinet.Instance.Send(WsServerFunction.CallCQFunction, new { type = "GetAppDirectory", args = new { authCode } });
            return Marshal.StringToHGlobalAnsi(r.json["callResult"].ToString());
        }

        [DllExport(ExportName = "CQ_getLoginQQ", CallingConvention = CallingConvention.StdCall)]
        public static long CQ_getLoginQQ(int authCode)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallCQFunction, new { type = "GetLoginQQ", args = new { authCode } });
            return (long)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_getLoginNick", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getLoginNick(int authCode)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallCQFunction, new { type = "GetLoginNick", args = new { authCode } });
            return Marshal.StringToHGlobalAnsi(r.json["callResult"].ToString());
        }

        [DllExport(ExportName = "CQ_setGroupKick", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupKick(int authCode, long groupId, long qqId, bool refuses)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.kick, args = new { authCode, groupId, qqId } });
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupBan(int authCode, long groupId, long qqId, long time)
        {
            MiraiApiType type = MiraiApiType.mute;
            if(time == 0)
            {
                type = MiraiApiType.unmute;
            }
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type, args = new { authCode, groupId, qqId, time } });
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupAdmin", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAdmin(int authCode, long groupId, long qqId, bool isSet)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.memberAdmin, args = new { authCode, groupId, qqId, isSet } });
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupSpecialTitle", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupSpecialTitle(int authCode, long groupId, long qqId, IntPtr title, long durationTime)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.memberInfo_update, args = new { authCode, groupId, qqId, title=title.ToString(GB18030) } });
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
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type, args = new { authCode, groupId } });
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupAnonymousBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAnonymousBan(int authCode, long groupId, IntPtr anonymous, long banTime)
        {
            //未找到实现接口
            return -1;
        }

        [DllExport(ExportName = "CQ_setGroupAnonymous", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAnonymous(int authCode, long groupId, bool isOpen)
        {
            //未找到实现接口
            return -1;
        }

        [DllExport(ExportName = "CQ_setGroupCard", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupCard(int authCode, long groupId, long qqId, IntPtr newCard)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.memberInfo_update, args = new { authCode, groupId, qqId, newCard = newCard.ToString(GB18030) } });
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupLeave", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupLeave(int authCode, long groupId, bool isDisband)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.quit, args = new { authCode, groupId } });
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setDiscussLeave", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setDiscussLeave(int authCode, long disscussId)
        {
            //未找到实现接口
            return -1;
        }
        [DllExport(ExportName = "CQ_setFriendAddRequest", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setFriendAddRequest(int authCode, IntPtr identifying, int requestType, IntPtr appendMsg)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.resp_newFriendRequestEvent, args = new { authCode, eventId=identifying.ToString(GB18030), requestType, message=appendMsg.ToString(GB18030) } });
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_setGroupAddRequestV2", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAddRequestV2(int authCode, IntPtr identifying, int requestType, int responseType, IntPtr appendMsg)
        {
            MiraiApiType apiType = MiraiApiType.resp_memberJoinRequestEvent;
            if(requestType == 2)
            {
                apiType = MiraiApiType.resp_botInvitedJoinGroupRequestEvent;
            }
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = apiType, args = new { authCode, eventId = identifying.ToString(GB18030), responseType, message = appendMsg.ToString(GB18030) } });
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_addLog", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_addLog(int authCode, int priority, IntPtr type, IntPtr msg)
        {
            var r = Clinet.Instance.Send(WsServerFunction.AddLog, new { args = new { authCode, type = type.ToString(GB18030), priority, msg = msg.ToString(GB18030) } }, false);
            return 1;
        }

        [DllExport(ExportName = "CQ_setFatal", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setFatal(int authCode, IntPtr errorMsg)
        {
            var r = Clinet.Instance.Send(WsServerFunction.AddLog, new { args = new { authCode, priority = 40, msg = errorMsg.ToString(GB18030) } });
            return (int)(r.json["callResult"]);
        }

        [DllExport(ExportName = "CQ_getGroupMemberInfoV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupMemberInfoV2(int authCode, long groupId, long qqId, bool isCache)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.groupConfig_get, args = new { authCode, groupId, qqId  } });
            return Marshal.StringToHGlobalAnsi(r.json["callResult"].ToString());
        }

        [DllExport(ExportName = "CQ_getGroupMemberList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupMemberList(int authCode, long groupId)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.memberList, args = new { authCode, groupId } });
            return Marshal.StringToHGlobalAnsi(r.json["callResult"].ToString());
        }

        [DllExport(ExportName = "CQ_getGroupList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupList(int authCode)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.groupList, args = new { authCode } });
            return Marshal.StringToHGlobalAnsi(r.json["callResult"].ToString());
        }

        [DllExport(ExportName = "CQ_getStrangerInfo", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getStrangerInfo(int authCode, long qqId, bool notCache)
        {
            return IntPtr.Zero;
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
            var r = Clinet.Instance.Send(WsServerFunction.CallCQFunction, new { type = "GetImage", args = new { authCode, path=file.ToString(GB18030) } });
            return Marshal.StringToHGlobalAnsi(r.json["callResult"].ToString());
        }

        [DllExport(ExportName = "CQ_getGroupInfo", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupInfo(int authCode, long groupId, bool notCache)
        {
            return IntPtr.Zero;
        }

        [DllExport(ExportName = "CQ_getFriendList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getFriendList(int authCode, bool reserved)
        {
            var r = Clinet.Instance.Send(WsServerFunction.CallMiraiAPI, new { type = MiraiApiType.friendList, args = new { authCode } });
            return Marshal.StringToHGlobalAnsi(r.json["callResult"].ToString());
        }
        [DllExport(ExportName = "cq_start", CallingConvention = CallingConvention.StdCall)]
        public static bool cq_start(IntPtr path, int authCode)
        {            
            return true;
        }
    }
}
