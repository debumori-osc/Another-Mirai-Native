using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Another_Mirai_Native.Adapter;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Native;

namespace Another_Mirai_Native
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool ignoreProcessCheck = false, waitForExit = false, customArg = false;
            // 防止启动多个程序
            Process[] process = Process.GetProcessesByName("AnotherMiraiNative");
            if(args.Length > 0)
            {
                for(int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-i":
                            ignoreProcessCheck = true;
                            break;
                        case "-r":
                            waitForExit = true;
                            break;
                        case "-q":
                        case "-ws":
                        case "-wsk":
                            customArg = true;
                            i++;
                            if (i >= args.Length)
                            {
                                MessageBox.Show("命令行参数错误");
                                Environment.Exit(0);
                            }
                            if (args[i - 1] == "-q") Helper.QQ = args[i];
                            else if (args[i - 1] == "-ws") Helper.WsURL = args[i];
                            else if (args[i - 1] == "-wsk") Helper.WsAuthKey = args[i];
                            break;
                        default:
                            break;
                    }
                }
                if (customArg && (string.IsNullOrEmpty(Helper.QQ)
                    || string.IsNullOrEmpty(Helper.WsURL)
                    || string.IsNullOrEmpty(Helper.WsAuthKey)))
                {
                    MessageBox.Show("命令行参数错误");
                    Environment.Exit(0);
                }
            }            
            if (waitForExit)// 如果含有 -r 参数 则等待前者进程退出之后再启动
            {
                int initialNum = process.Length;
                if (initialNum != 1)
                {
                    process = Process.GetProcessesByName("AnotherMiraiNative");
                    while (process.Length != initialNum - 1)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }

            if (ignoreProcessCheck is false)
            {
                if (process.Length != 1)
                {
                    MessageBox.Show("已经启动了一个程序");
                    Environment.Exit(0);
                }
            }
            if(ConfigHelper.GetConfig<bool>("Enable_UsageMonitor", false))
            {
                UsageMonitor.CreateDB();
                UsageMonitor.StartRecord();
            }

            Application.ThreadException += Application_ThreadException;
            Application.ApplicationExit += (a, b) =>
            { 
                UsageMonitor.ExitFlag = true; 
                MiraiAdapter.ExitFlag = true; 
            };
            //未处理的异常捕获
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new Login());
        }
        /// <summary>
        /// 不可逆错误
        /// </summary>
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                var b = Error_TaskDialog.ShowErrorDialog(ex.Message, ex.StackTrace, false);
                if (b == Error_TaskDialog.TaskDialogResult.Exit)
                {
                    Environment.Exit(0);
                }
                else if (b == Error_TaskDialog.TaskDialogResult.Restart)
                {
                    Helper.RestartApplication();
                }
            }
        }
        /// <summary>
        /// 可忽略错误
        /// </summary>
        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                var b = Error_TaskDialog.ShowErrorDialog(e.Exception.Message, e.Exception.StackTrace);
                if (b == Error_TaskDialog.TaskDialogResult.ReloadApp)
                {
                    PluginManagment.Instance.ReLoad();
                }
                else if (b == Error_TaskDialog.TaskDialogResult.Exit)
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
