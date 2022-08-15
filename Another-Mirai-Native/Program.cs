using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            Process[] process = Process.GetProcessesByName("AnotherMiraiNative");
            if (args.Length != 0 && args[0] == "-r")
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
            else if(args.Length != 0 && args[0] == "-i")
            {
                // Save.IgnoreProcessChecking = true;
                // Do nothing. Ignore Process Checking
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
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                var b = Error_TaskDialog.ShowErrorDialog(Login.Instance_Handle, ex.Message, ex.StackTrace, false);
                if (b == Error_TaskDialog.TaskDialogResult.ReloadApp)
                {
                    //Login.pluginManagment.ReLoad();
                }
                else if (b == Error_TaskDialog.TaskDialogResult.Exit)
                {
                    Environment.Exit(0);
                }
                else if (b == Error_TaskDialog.TaskDialogResult.Restart)
                {
                    string path = typeof(Login).Assembly.Location;
                    Process.Start(path, $"-r");
                    //NotifyIconHelper.HideNotifyIcon();
                    Environment.Exit(0);
                }
            }
        }

        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                var b = Error_TaskDialog.ShowErrorDialog(Login.Instance_Handle, e.Exception.Message, e.Exception.StackTrace);
                if (b == Error_TaskDialog.TaskDialogResult.ReloadApp)
                {
                    // Login.pluginManagment.ReLoad();
                }
                else if (b == Error_TaskDialog.TaskDialogResult.Exit)
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
