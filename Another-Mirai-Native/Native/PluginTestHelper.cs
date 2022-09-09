using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Another_Mirai_Native.Native
{
    public class PluginTestHelper
    {
        public static PluginTestHelper Instance { get; set; } = new PluginTestHelper();
        public CQPlugin TestingPlugin { get; private set; }
        public int ShutdownTime { get; set; } = 5 * 60;

        public delegate void PluginSendMsg(string msg);
        public event PluginSendMsg OnPluginSendMsg;
        public delegate void PluginChanged(CQPlugin plugin);
        public event PluginChanged OnPluginChanged;

        private int noMsgTime = 0;
        private Thread checkEnableThread = null;
        public void EnableTest(CQPlugin plugin)
        {
            if(CheckPlugin())
            {
                OnPluginChanged?.Invoke(plugin);
            }
            TestingPlugin = plugin;
            if(checkEnableThread != null)
            {
                noMsgTime = 0;
                return;
            }
            checkEnableThread = new Thread(() =>
            {
                while (noMsgTime < ShutdownTime)
                {
                    Thread.Sleep(1000);
                    noMsgTime++;
                }
                TestingPlugin.Testing = false;
                TestingPlugin = null;
                checkEnableThread = null;
            });
            checkEnableThread.Start();
        }
        public void DisableTest()
        {
            if(CheckPlugin())
            {
                TestingPlugin = null;
            }
        }
        public bool CheckPlugin()
        {
            return TestingPlugin != null;
        }
        private IntPtr RecodeMsg(string text)
        {
            var b = Encoding.UTF8.GetBytes(text);
            Encoding GB18030 = Encoding.GetEncoding("GB18030");
            text = GB18030.GetString(Encoding.Convert(Encoding.UTF8, GB18030, b));
            byte[] messageBytes = GB18030.GetBytes(text + "\0");
            IntPtr messageIntptr = Marshal.AllocHGlobal(messageBytes.Length);
            Marshal.Copy(messageBytes, 0, messageIntptr, messageBytes.Length);
            return messageIntptr;
        }
        public bool SendGroupMsg(string msg, long groupId, long QQId)
        {
            if (!CheckPlugin()) return false;
            noMsgTime = 0;

            ConfigHelper.SetConfig("Tester_GroupID", groupId);
            ConfigHelper.SetConfig("Tester_QQID", QQId);
            return TestingPlugin.dll.CallFunction(Enums.FunctionEnums.GroupMsg, 1, 0, groupId, QQId, "", RecodeMsg(msg), 0) == 1;
        }
        public void ReceiveMsg(string msg)
        {
            noMsgTime = 0;
            OnPluginSendMsg?.Invoke(msg);
        }
        public bool SendPrivateMsg(string msg, long QQId)
        {
            if (!CheckPlugin()) return false;
            noMsgTime = 0;

            ConfigHelper.SetConfig("Tester_QQID", QQId);
            return TestingPlugin.dll.CallFunction(Enums.FunctionEnums.PrivateMsg, 11, 0, QQId, RecodeMsg(msg), 0) == 1;
        }
    }
}
