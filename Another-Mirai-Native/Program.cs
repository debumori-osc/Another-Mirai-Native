using Another_Mirai_Native.Native;
using System.Diagnostics;

namespace Another_Mirai_Native
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Process[] process = Process.GetProcessesByName("Another-Mirai-Native");
            if (args.Length != 0 && args[0] == "-r")
            {
                int initialNum = process.Length;
                if (initialNum != 1)
                {
                    while (Process.GetProcessesByName("Another-Mirai-Native").Length != initialNum - 1)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            else if (args.Length != 0 && args[0] == "-i")
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
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            ApplicationConfiguration.Initialize();
            Application.Run(new Login());
        }

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                var b = Error_TaskDialog.ShowErrorDialog(Login.Instance_Handle, $"{ex.Message}\n{ex.StackTrace}", false);
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
                var b = Error_TaskDialog.ShowErrorDialog(Login.Instance_Handle, $"{e.Exception.Message}\n{e.Exception.StackTrace}");
                if (b == Error_TaskDialog.TaskDialogResult.ReloadApp)
                {
                    //Login.pluginManagment.ReLoad();
                }
                else if (b == Error_TaskDialog.TaskDialogResult.Exit)
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}