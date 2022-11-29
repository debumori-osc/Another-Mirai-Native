using Another_Mirai_Native.Adapter;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CQP
{
    public class DllMain
    {
        static Encoding GB18030 = Encoding.GetEncoding("GB18030");

        [DllExport(ExportName = "CQ_sendGroupMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendGroupMsg(int authCode, long groupid, IntPtr msg)
        {
            string text = msg.ToString(GB18030);
            return CQPAdapter.SendGroupMessage(authCode, groupid, text);
        }

        [DllExport(ExportName = "CQ_sendPrivateMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendPrivateMsg(int authCode, long qqId, IntPtr msg)
        {
            string text = msg.ToString(GB18030);
            return CQPAdapter.SendPrivateMessage(authCode, qqId, text);
        }

        [DllExport(ExportName = "CQ_deleteMsg", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_deleteMsg(int authCode, long msgId)
        {
            return CQPAdapter.DeleteMsg(authCode, msgId);
        }

        [DllExport(ExportName = "CQ_sendLikeV2", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_sendLikeV2(int authCode, long qqId, int count)
        {
            return CQPAdapter.SendLikeV2(authCode, qqId, count);
        }

        [DllExport(ExportName = "CQ_getCookiesV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getCookiesV2(int authCode, IntPtr domain)
        {
            return CQPAdapter.GetCookiesV2(authCode, domain.ToString(GB18030)).ToNative();
        }

        [DllExport(ExportName = "CQ_getRecordV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getRecordV2(int authCode, IntPtr file, IntPtr format)
        {
            return CQPAdapter.GetRecordV2(authCode, file.ToString(GB18030), format.ToString(GB18030)).ToNative();
        }

        [DllExport(ExportName = "CQ_getCsrfToken", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getCsrfToken(int authCode)
        {
            return CQPAdapter.GetCsrfToken(authCode).ToNative();
        }

        [DllExport(ExportName = "CQ_getAppDirectory", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getAppDirectory(int authCode)
        {
            return CQPAdapter.GetAppDirectory(authCode).ToNative();
        }

        [DllExport(ExportName = "CQ_getLoginQQ", CallingConvention = CallingConvention.StdCall)]
        public static long CQ_getLoginQQ(int authCode)
        {
            return CQPAdapter.GetLoginQQ(authCode);
        }

        [DllExport(ExportName = "CQ_getLoginNick", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getLoginNick(int authCode)
        {
            return CQPAdapter.GetLoginNick(authCode).ToNative();
        }

        [DllExport(ExportName = "CQ_setGroupKick", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupKick(int authCode, long groupId, long qqId, bool refuses)
        {
            return CQPAdapter.SetGroupKick(authCode, groupId, qqId, refuses);
        }

        [DllExport(ExportName = "CQ_setGroupBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupBan(int authCode, long groupId, long qqId, long time)
        {
            return CQPAdapter.SetGroupBan(authCode, groupId, qqId, time);
        }

        [DllExport(ExportName = "CQ_setGroupAdmin", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAdmin(int authCode, long groupId, long qqId, bool isSet)
        {
            return CQPAdapter.SetGroupAdmin(authCode, groupId, qqId, isSet);
        }

        [DllExport(ExportName = "CQ_setGroupSpecialTitle", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupSpecialTitle(int authCode, long groupId, long qqId, IntPtr title, long durationTime)
        {
            return CQPAdapter.SetGroupSpecialTitle(authCode, groupId, qqId, title.ToString(GB18030));
        }

        [DllExport(ExportName = "CQ_setGroupWholeBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupWholeBan(int authCode, long groupId, bool isOpen)
        {
            return CQPAdapter.SetGroupWholeBan(authCode, groupId, isOpen);
        }

        [DllExport(ExportName = "CQ_setGroupAnonymousBan", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAnonymousBan(int authCode, long groupId, IntPtr anonymous, long banTime)
        {
            return CQPAdapter.SetGroupAnonymousBan(authCode, groupId, anonymous.ToString(GB18030), banTime);
        }

        [DllExport(ExportName = "CQ_setGroupAnonymous", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAnonymous(int authCode, long groupId, bool isOpen)
        {
            return CQPAdapter.SetGroupAnonymous(authCode, groupId, isOpen);
        }

        [DllExport(ExportName = "CQ_setGroupCard", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupCard(int authCode, long groupId, long qqId, IntPtr newCard)
        {
            return CQPAdapter.SetGroupCard(authCode, groupId, qqId, newCard.ToString(GB18030));
        }

        [DllExport(ExportName = "CQ_setGroupLeave", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupLeave(int authCode, long groupId, bool isDisband)
        {
            return CQPAdapter.SetGroupLeave(authCode, groupId, isDisband);
        }

        [DllExport(ExportName = "CQ_setDiscussLeave", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setDiscussLeave(int authCode, long disscussId)
        {
            return CQPAdapter.SetDiscussLeave(authCode, disscussId);
        }

        [DllExport(ExportName = "CQ_setFriendAddRequest", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setFriendAddRequest(int authCode, IntPtr identifying, int requestType, IntPtr appendMsg)
        {
            return CQPAdapter.SetFriendAddRequest(authCode, Convert.ToInt64(identifying.ToString(GB18030)), requestType, appendMsg.ToString(GB18030));
        }

        [DllExport(ExportName = "CQ_setGroupAddRequestV2", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setGroupAddRequestV2(int authCode, IntPtr identifying, int requestType, int responseType, IntPtr appendMsg)
        {
            return CQPAdapter.SetGroupAddRequestV2(authCode, Convert.ToInt64(identifying.ToString(GB18030)), requestType, responseType, appendMsg.ToString(GB18030));
        }

        [DllExport(ExportName = "CQ_addLog", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_addLog(int authCode, int priority, IntPtr type, IntPtr msg)
        {
            CQPAdapter.AddLog(authCode, priority, type.ToString(GB18030), msg.ToString(GB18030));
            return 1;
        }

        [DllExport(ExportName = "CQ_setFatal", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_setFatal(int authCode, IntPtr errorMsg)
        {
            CQPAdapter.AddLog(authCode, 40, "致命错误", errorMsg.ToString(GB18030));
            return 1;
        }

        [DllExport(ExportName = "CQ_getGroupMemberInfoV2", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupMemberInfoV2(int authCode, long groupId, long qqId, bool isCache)
        {
            return CQPAdapter.GetGroupMemberInfoV2(authCode, groupId, qqId, isCache).ToNative();
        }

        [DllExport(ExportName = "CQ_getGroupMemberList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupMemberList(int authCode, long groupId)
        {
            return CQPAdapter.GetGroupMemberList(authCode, groupId).ToNative();
        }

        [DllExport(ExportName = "CQ_getGroupList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupList(int authCode)
        {
            return CQPAdapter.GetGroupList(authCode).ToNative();
        }

        [DllExport(ExportName = "CQ_getStrangerInfo", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getStrangerInfo(int authCode, long qqId, bool notCache)
        {
            return CQPAdapter.GetStrangerInfo(authCode, qqId, notCache).ToNative();
        }

        [DllExport(ExportName = "CQ_canSendImage", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_canSendImage(int authCode)
        {
            return CQPAdapter.CanSendImage(authCode);
        }

        [DllExport(ExportName = "CQ_canSendRecord", CallingConvention = CallingConvention.StdCall)]
        public static int CQ_canSendRecord(int authCode)
        {
            return CQPAdapter.CanSendRecord(authCode);
        }

        [DllExport(ExportName = "CQ_getImage", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getImage(int authCode, IntPtr file)
        {
            return CQPAdapter.GetImage(authCode, file.ToString(GB18030)).ToNative();
        }

        [DllExport(ExportName = "CQ_getGroupInfo", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getGroupInfo(int authCode, long groupId, bool notCache)
        {
            return CQPAdapter.GetGroupInfo(authCode, groupId, notCache).ToNative();
        }

        [DllExport(ExportName = "CQ_getFriendList", CallingConvention = CallingConvention.StdCall)]
        public static IntPtr CQ_getFriendList(int authCode, bool reserved)
        {
            return CQPAdapter.GetFriendList(authCode, reserved).ToNative();
        }
        [DllExport(ExportName = "cq_start", CallingConvention = CallingConvention.StdCall)]
        public static bool cq_start(IntPtr path, int authCode)
        {
            return true;
        }
    }
}
