using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            // 防止启动多个程序
            Process[] process = Process.GetProcessesByName("AnotherMiraiNative");
            if (args.Length != 0 && args[0] == "-r")// 如果含有 -r 参数 则等待前者进程退出之后再启动
            {
                int initialNum = process.Length;
                if (initialNum != 1)
                {
                    while (Process.GetProcessesByName("AnotherMiraiNative").Length != initialNum - 1)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            else if(args.Length != 0 && args[0] == "-i")// 含有 -i 参数 忽略进程检测
            {
                // 忽略进程检测
            }
            else
            {
                if (process.Length != 1)
                {
                    MessageBox.Show("已经启动了一个程序");
                    return;
                }
            }
            Application.ThreadException += Application_ThreadException;
            //未处理的异常捕获
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
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
