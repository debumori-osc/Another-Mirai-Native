using Another_Mirai_Native.Adapter.CQCode.Expand;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Native;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Another_Mirai_Native.Adapter
{
    public static class CQPAdapter
    {
        public static int SendGroupMessage(int authCode, long groupId, string msg, int msgId = 0)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            if (plugin.Testing)
            {
                PluginTestHelper.Instance.ReceiveMsg(msg);
                return 1;
            }
            else
            {
                int logid = LogHelper.WriteLog(LogLevel.InfoSend, plugin.appinfo.Name, "[↑]发送群聊消息", $"群:{groupId} 消息:{msg}", "处理中...");
                int callResult = MiraiAPI.SendGroupMessage(groupId, msg, msgId);
                sw.Stop();
                LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");
                return callResult;
            }
        }
        public static int SendPrivateMessage(int authCode, long qqId, string msg)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            if (plugin.Testing)
            {
                PluginTestHelper.Instance.ReceiveMsg(msg);
                return 1;
            }
            else
            {
                int logid = LogHelper.WriteLog(LogLevel.InfoSend, plugin.appinfo.Name, "[↑]发送私聊消息", $"QQ:{qqId} 消息:{msg}", "处理中...");
                int callResult = MiraiAPI.SendFriendMessage(qqId, msg);
                sw.Stop();
                LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");
                return callResult;
            }
        }
        public static int DeleteMsg(int authCode, long msgId)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            string recallMsg = MiraiAPI.GetMessageByMsgId(msgId, 0);
            if (string.IsNullOrEmpty(recallMsg)) recallMsg = "消息拉取失败";
            int logid = LogHelper.WriteLog(LogLevel.Info, plugin.appinfo.Name, "撤回消息", $"msgid={msgId}, 内容={recallMsg}", "处理中...");
            int callResult = MiraiAPI.RecallMessage(msgId, 0);

            sw.Stop();
            LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");
            return callResult;
        }
        public static int SendLikeV2(int authCode, long qqId, int count)
        {
            return 0;
        }
        public static string GetCookiesV2(int authCode, string domain)
        {
            return "";
        }
        public static string GetCsrfToken(int authCode)
        {
            return "";
        }
        public static string GetRecordV2(int authCode, string file, string format)
        {
            return "";
        }
        public static string GetAppDirectory(int authCode)
        {
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            string path;
            if (plugin == null)
            {
                path = @"data\app\undentified";
            }
            else
            {
                path = @$"data\app\{plugin.appinfo.Id}";
                if (Directory.Exists(path) is false)
                    Directory.CreateDirectory(path);
            }
            return new DirectoryInfo(path).FullName + "\\";
        }
        public static long GetLoginQQ(int authCode)
        {
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            return Convert.ToInt64(Helper.QQ);
        }
        public static string GetLoginNick(int authCode)
        {
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return "";
            }
            return Helper.NickName;
        }
        public static int SetGroupKick(int authCode, long groupId, long qqId, bool refuses)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            int logid = LogHelper.WriteLog(LogLevel.InfoSend, plugin.appinfo.Name, "踢出群成员", $"移除群{groupId} 成员{qqId}", "处理中...");
            int callResult = MiraiAPI.KickGroupMember(groupId, qqId);
            sw.Stop();
            LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");

            return callResult;
        }
        public static int SetGroupBan(int authCode, long groupId, long qqId, long time)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            int logid = 0, callResult = 0;
            if (time > 0)
            {
                logid = LogHelper.WriteLog(LogLevel.Info, plugin.appinfo.Name, "禁言群成员", $"禁言群{groupId} 成员{qqId} {time}秒", "处理中...");
                callResult = MiraiAPI.MuteGroupMemeber(groupId, qqId, time);
            }
            else
            {
                logid = LogHelper.WriteLog(LogLevel.InfoSend, plugin.appinfo.Name, "解除禁言群成员", $"解除禁言群{groupId} 成员{qqId}", "处理中...");
                callResult = MiraiAPI.UnmuteGroupMemeber(groupId, qqId);
            }
            sw.Stop();
            LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");

            return callResult;
        }
        public static int SetGroupAdmin(int authCode, long groupId, long qqId, bool isSet)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            int logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, $"{(isSet ? "设置" : "取消")}群成员管理", $"{(isSet ? "设置" : "取消")}群{groupId} 成员{qqId}", "处理中...");
            int callResult = MiraiAPI.SetAdmin(groupId, qqId, isSet);
            sw.Stop();
            LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");

            return callResult;
        }
        public static int SetGroupSpecialTitle(int authCode, long groupId, long qqId, string title)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            int logid = LogHelper.WriteLog(LogLevel.InfoSend, plugin.appinfo.Name, "设置群成员头衔", $"设置群{groupId} 成员{qqId} 头衔 {title}", "处理中...");
            int callResult = MiraiAPI.SetSpecialTitle(groupId, qqId, title);
            sw.Stop();
            LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");

            return callResult;
        }
        public static int SetGroupWholeBan(int authCode, long groupId, bool isOpen)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            int logid = 0, callResult = 0;
            if (isOpen)
            {
                logid = LogHelper.WriteLog(LogLevel.Info, plugin.appinfo.Name, "群全体禁言", $"禁言群{groupId}", "处理中...");
                callResult = MiraiAPI.GroupMute(groupId);
            }
            else
            {
                logid = LogHelper.WriteLog(LogLevel.Info, plugin.appinfo.Name, "解除群全体禁言", $"解除禁言群{groupId}", "处理中...");
                callResult = MiraiAPI.GroupUnmute(groupId);
            }
            sw.Stop();
            LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");

            return callResult;
        }
        public static int SetGroupAnonymousBan(int authCode, long groupId, string anonymous, long banTime)
        {
            return -1;
        }
        public static int SetGroupAnonymous(int authCode, long groupId, bool isOpen)
        {
            return -1;
        }
        public static int SetGroupCard(int authCode, long groupId, long qqId, string newCard)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            int logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "设置群成员名片", $"设置群{groupId} 成员{qqId} 名片 {newCard}", "处理中...");
            int callResult = MiraiAPI.SetGroupCard(groupId, qqId, newCard);
            sw.Stop();
            LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");

            return callResult;
        }
        public static int SetGroupLeave(int authCode, long groupId, bool isDisband)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            int logid = LogHelper.WriteLog(LogLevel.Info, plugin.appinfo.Name, "退出群", $"退出群{groupId}", "处理中...");
            int callResult = MiraiAPI.GroupMute(groupId);
            sw.Stop();
            LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");

            return callResult;
        }
        public static int SetDiscussLeave(int authCode, long disscussId)
        {
            return -1;
        }
        public static int SetFriendAddRequest(int authCode, long identifying, int requestType, string appendMsg)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            long respf_fromId = 0;
            string respf_nick = "";
            if (Cache.FriendRequset.ContainsKey(identifying))
            {
                respf_fromId = Cache.FriendRequset[identifying].Item1;
                respf_nick = Cache.FriendRequset[identifying].Item2;
            }
            Cache.FriendRequset.Remove(identifying);
            int logid = LogHelper.WriteLog(LogLevel.InfoSend, plugin.appinfo.Name, $"好友添加申请", $"来源: {respf_fromId}({respf_nick}) 操作: {(requestType == 0 ? "同意" : "拒绝")}", "处理中...");
            int callResult = MiraiAPI.HandleFriendRequest(identifying, requestType, appendMsg);
            sw.Stop();
            LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");

            return callResult;
        }
        public static int SetGroupAddRequestV2(int authCode, long identifying, int requestType, int responseType, string appendMsg)
        {
            Stopwatch sw = new();
            sw.Start();
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return 0;
            }
            long fromId = 0, groupId = 0;
            int logid = 0, callResult = 0;
            string nick = "", groupname = "";
            if (Cache.GroupRequset.ContainsKey(identifying))
            {
                fromId = Cache.GroupRequset[identifying].Item1;
                nick = Cache.GroupRequset[identifying].Item2;
                groupId = Cache.GroupRequset[identifying].Item3;
                groupname = Cache.GroupRequset[identifying].Item4;
            }
            Cache.GroupRequset.Remove(identifying);
            if (requestType == 2)
            {
                logid = LogHelper.WriteLog(LogLevel.InfoSend, plugin.appinfo.Name, $"群邀请添加申请", $"来源群: {groupId}({groupname}) 来源人: {fromId}({nick}) 操作: {appendMsg}", "处理中...");
                callResult = MiraiAPI.HandleInviteRequest(identifying, requestType, appendMsg);
            }
            else
            {
                logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, $"群添加申请", $"来源: {fromId}({nick}) 目标群: {groupId}({groupname}) 操作: {appendMsg}", "处理中...");
                callResult = MiraiAPI.HandleGroupRequest(identifying, requestType, appendMsg);
            }
            sw.Stop();
            LogHelper.UpdateLogStatus(logid, $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s");

            return callResult;
        }
        public static void AddLog(int authCode, int priority, string type, string msg)
        {
            LogLevel log_priority = (LogLevel)priority;
            string log_origin = "AMN框架";
            var plugin = PluginManagment.Instance.Plugins.Find(x => x.appinfo.AuthCode == authCode);
            if (plugin != null)
            {
                log_origin = plugin.appinfo.Name;
            }
            LogHelper.WriteLog(log_priority, log_origin, type, msg, "");
        }
        public static string GetGroupMemberInfoV2(int authCode, long groupId, long qqId, bool isCache)
        {
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return "";
            }
            var memeberArr = MiraiAPI.GetGroupMemberList(groupId);
            return MiraiAPI.ParseGroupMemberInfo2CQData(memeberArr, MiraiAPI.GetGroupMemberProfile(groupId, qqId), groupId, qqId) ?? "";
        }

        public static string GetGroupMemberList(int authCode, long groupId)
        {
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return "";
            }
            return MiraiAPI.ParseMemberList2CQData(MiraiAPI.GetGroupMemberList(groupId), groupId) ?? "";
        }

        public static string GetGroupList(int authCode)
        {
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return "";
            }
            return MiraiAPI.ParseGroupList2CQData(MiraiAPI.GetGroupList()) ?? "";
        }

        public static string GetStrangerInfo(int authCode, long qqId, bool notCache)
        {
            MemoryStream streamMain = new();
            BinaryWriter binaryWriterMain = new(streamMain);
            BinaryWriterExpand.Write_Ex(binaryWriterMain, qqId);
            BinaryWriterExpand.Write_Ex(binaryWriterMain, "");
            BinaryWriterExpand.Write_Ex(binaryWriterMain, 0);

            return Convert.ToBase64String(streamMain.ToArray());
        }

        public static int CanSendImage(int authCode)
        {
            return 1;
        }

        public static int CanSendRecord(int authCode)
        {
            return 1;
        }
        public static string GetImage(int authCode, string file)
        {
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return "";
            }
            string url = Helper.GetPicUrlFromCQImg(file);
            string imgFileName = file + ".jpg";
            string imgDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image");
            var downloadTask = Helper.DownloadFile(url, imgFileName, imgDir);
            downloadTask.Wait();

            return Path.Combine(imgDir, imgFileName);
        }
        public static string GetFriendList(int authCode, bool reserved)
        {
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return "";
            }
            return MiraiAPI.ParseFriendList2CQData(MiraiAPI.GetFriendList(reserved)) ?? "";
        }
        public static string GetGroupInfo(int authCode, long groupId, bool notCache)
        {
            var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
            if (plugin == null)
            {
                return "";
            }
            var group = MiraiAPI.GetGroupList().FirstOrDefault(x => ((long)x["id"]) == groupId);
            var groupMemCount = MiraiAPI.GetGroupMemberList(groupId).Count;
            return MiraiAPI.ParseGroupInfo2CQData(groupId, group["name"].ToString(), groupMemCount) ?? "";
        }
    }
}
