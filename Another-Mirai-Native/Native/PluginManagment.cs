using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Native
{
    /// <summary>
    /// 管理CQ插件的类
    /// </summary>
    public class PluginManagment
    {
        public static PluginManagment Instance { get; set; }
        public List<CQPlugin> Plugins { get; set; } = new();
        public Dictionary<IntPtr, AppDomain> AppDomains { get; set; } = new();
        public bool Loading { get; set; } = false;
        private JObject PluginStatus;
        public PluginManagment()
        {
            Instance = this;
            if (File.Exists("conf/Status.json"))
                PluginStatus = JObject.Parse(File.ReadAllText("conf/Status.json"));
            else
                PluginStatus = new JObject();
        }
        /// <summary>
        /// 从 data\plugins 文件夹下载入所有拥有同名json的dll插件，不包含子文件夹
        /// </summary>
        public void Load()
        {
            Loading = true;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string path = Path.Combine(Environment.CurrentDirectory, "data", "plugins");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            DirectoryInfo directoryInfo = new(path);
            int count = 0;
            foreach (var item in directoryInfo.GetFiles().Where(x => x.Extension == ".dll"))
            {
                if (Load(item.FullName))
                    count++;
            }
            sw.Stop();
            LogHelper.WriteLog(LogLevel.Info, "插件载入", $"一共加载了{count}个插件", $"√ {sw.ElapsedMilliseconds} ms");
            NotifyIconHelper.AddManageMenu();
        }
        /// <summary>
        /// 以绝对路径路径载入拥有同名json的dll插件
        /// </summary>
        /// <param name="filepath">插件dll的绝对路径</param>
        /// <returns>载入是否成功</returns>
        public bool Load(string filepath)
        {
            FileInfo plugininfo = new FileInfo(filepath);
            if (!File.Exists(plugininfo.FullName.Replace(".dll", ".json")))
            {
                LogHelper.WriteLog(LogLevel.Error, "插件载入", $"插件 {plugininfo.Name} 加载失败,原因:缺少json文件");
                return false;
            }

            JObject json = JObject.Parse(File.ReadAllText(plugininfo.FullName.Replace(".dll", ".json")));
            int authcode = new Random().Next();
            Dll dll = new Dll();
            if (!Directory.Exists(@"data\plugins\tmp"))
                Directory.CreateDirectory(@"data\plugins\tmp");
            //复制需要载入的插件至临时文件夹,可直接覆盖原dll便于插件重载
            string destpath = @"data\plugins\tmp\" + plugininfo.Name;
            File.Copy(plugininfo.FullName, destpath, true);
            //复制它的json
            File.Copy(plugininfo.FullName.Replace(".dll", ".json"), destpath.Replace(".dll", ".json"), true);

            IntPtr iLib = dll.Load(destpath, json);//将dll插件LoadLibrary,并进行函数委托的实例化;

            AppDomainSetup ads = new()
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                DisallowBindingRedirects = false,
                DisallowCodeDownload = true,
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
            };
            AppDomain newappDomain = AppDomain.CreateDomain(iLib.ToString(), null, ads);
            AppDomains.Add(iLib, newappDomain);
            Proxy.Init(iLib, json);

            if (iLib == (IntPtr)0)
            {
                LogHelper.WriteLog(LogLevel.Error, "插件载入", $"插件 {plugininfo.Name} 加载失败,返回句柄为空,GetLastError={Dll.GetLastError()}");
            }
            //执行插件的init,分配一个authcode
            dll.DoInitialize(authcode);
            //获取插件的appinfo,返回示例 9,me.cqp.luohuaming.Sign,分别为ApiVer以及AppID
            KeyValuePair<int, string> appInfotext = dll.GetAppInfo();
            AppInfo appInfo = new(appInfotext.Value, 0, appInfotext.Key
                , json["name"].ToString(), json["version"].ToString(), Convert.ToInt32(json["version_id"].ToString())
                , json["author"].ToString(), json["description"].ToString(), authcode);
            bool enabled = GetPluginState(appInfo);//获取插件启用状态
            //保存至插件列表
            CQPlugin CQPlugin = (CQPlugin)newappDomain.CreateInstanceAndUnwrap(typeof(CQPlugin).Assembly.FullName
                , typeof(CQPlugin).FullName
                , false
                , BindingFlags.CreateInstance
                , null
                , new object[] { iLib, appInfo, json.ToString(), dll, enabled, filepath }
                , null, null);
            Plugins.Add(CQPlugin);

            cq_start(Marshal.StringToHGlobalAnsi(destpath), authcode);
            //将它的窗口写入托盘右键菜单
            NotifyIconHelper.LoadMenu(json);
            LogHelper.WriteLog(LogLevel.InfoSuccess, "插件载入", $"插件 {appInfo.Name} 加载成功");

            return true;
        }

        private void NewappDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            Console.WriteLine();
        }

        /// <summary>
        /// 翻转插件启用状态
        /// </summary>
        public void FlipPluginState(CQPlugin CQPlugin)
        {
            if(CQPlugin.Enable)
            {
                CQPlugin.dll.CallFunction(FunctionEnums.Exit);
                CQPlugin.dll.CallFunction(FunctionEnums.Disable);
            }
            else
            {
                CQPlugin.dll.CallFunction(FunctionEnums.Enable);
                CQPlugin.dll.CallFunction(FunctionEnums.StartUp);
            }
            var c = PluginStatus["Status"]
                            .Where(x => x["Name"].ToString() == CQPlugin.appinfo.Id)
                            .FirstOrDefault();
            c["Enabled"] = c["Enabled"].Value<int>() == 1 ? 0 : 1;
            File.WriteAllText(@"conf\Status.json", PluginStatus.ToString());
        }
        /// <summary>
        /// 从配置获取插件启用状态
        /// </summary>
        /// <param name="appInfo">需要获取的Appinfo</param>
        private bool GetPluginState(AppInfo appInfo)
        {
            //没有states键,新建一个
            if (PluginStatus["Status"] is not JArray statesArray)
            {
                var b = new JProperty("Status", new JArray());
                PluginStatus.Add(b);
                File.WriteAllText(@"conf\Status.json", PluginStatus.ToString());
                statesArray = PluginStatus["Status"] as JArray;
            }
            //没有此插件的配置,新建一个并返回false
            if (!statesArray.Any(x => x["Name"].ToString() == appInfo.Id))
            {
                JObject CQPlugin = new()
                {
                    new JProperty("Name",appInfo.Id),
                    new JProperty("Enabled",1)
                };
                statesArray.Add(CQPlugin);
                File.WriteAllText(@"conf\Status.json", PluginStatus.ToString());
                return false;
            }
            else
            {
                return statesArray.Where(x => x["Name"].ToString() == appInfo.Id).First()["Enabled"]
                    .Value<int>() == 1;
            }
        }
        /// <summary>
        /// 卸载插件，执行被卸载事件，从菜单移除此插件的菜单
        /// </summary>
        /// <param name="CQPlugin"></param>
        public void UnLoad(CQPlugin CQPlugin)
        {
            try
            {
                CQPlugin.dll.CallFunction(FunctionEnums.Disable);
                CQPlugin.dll.CallFunction(FunctionEnums.Exit);
                CQPlugin.dll.UnLoad();
                Plugins.Remove(CQPlugin);
                LogHelper.WriteLog(LogLevel.InfoSuccess, "插件卸载", $"插件 {CQPlugin.appinfo.Name} 卸载成功");
                CQPlugin = null; GC.Collect();
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(LogLevel.Error, "插件卸载", e.Message + e.StackTrace);
            }
        }

        public void ReLoad(CQPlugin CQPlugin)
        {
            Loading = true;
            var ilib = CQPlugin.handle;
            var name = CQPlugin.appinfo.Name;
            var path = CQPlugin.path;
            UnLoad(CQPlugin);
            var appdomain = AppDomains.First(x => x.Key == ilib);
            AppDomains.Remove(ilib);
            AppDomain.Unload(appdomain.Value);
            GC.Collect();
            Load(path);
            Loading = false;
            var pluginReload = Plugins.Find(x => x.appinfo.Name == name);
            pluginReload.dll.CallFunction(FunctionEnums.StartUp);
            pluginReload.dll.CallFunction(FunctionEnums.Enable);
        }

        /// <summary>
        /// 插件全部卸载
        /// </summary>
        public void UnLoad()
        {
            Loading = true;
            LogHelper.WriteLog("开始卸载插件...");
            CallFunction(FunctionEnums.Exit);
            CallFunction(FunctionEnums.Disable);
            NotifyIconHelper.ClearAppMenu();
            foreach (var item in AppDomains)
            {
                var c = Plugins.Find(x => x.handle == item.Key);
                UnLoad(c);
                AppDomain.Unload(item.Value);
            }
            Plugins.Clear();
            AppDomains.Clear();
            GC.Collect();
            Loading = false;
            LogHelper.WriteLog("插件卸载完毕");
        }
        /// <summary>
        /// 成员初始化，用于删除上次运行的临时目录、加载插件以及执行启动事件
        /// </summary>
        public void Init()
        {
            if (Directory.Exists(@"data\plugins\tmp"))
                Directory.Delete(@"data\plugins\tmp", true);
            if (PluginStatus.Count == 0)
            {
                PluginStatus.Add(new JProperty("Status", new JArray()));
                File.WriteAllText(@"conf\Status.json", PluginStatus.ToString());
            }
            Load();
            LogHelper.WriteLog("遍历启动事件……");
            CallFunction(FunctionEnums.StartUp);
            CallFunction(FunctionEnums.Enable);
            LogHelper.WriteLog("插件启动完成，开始处理消息逻辑……");
            Loading = false;
        }
        [DllImport("CQP.dll", EntryPoint = "cq_start")]
        private static extern bool cq_start(IntPtr path, int authcode);
        [DllImport("CQP.dll", EntryPoint = "CQ_addLog")]
        private static extern int CQ_addLog(int authCode, int priority, IntPtr type, IntPtr msg);
        /// <summary>
        /// 核心方法调用，将前端处理的数据传递给插件对应事件处理，尝试捕获非托管插件的异常
        /// </summary>
        /// <param name="ApiName">调用的事件名称，前端统一名称，或许应该写成枚举</param>
        /// <param name="args">参数表</param>
        [HandleProcessCorruptedStateExceptions]
        public CQPlugin CallFunction(FunctionEnums ApiName, params object[] args)
        {
            JObject json = new JObject
            {
                new JProperty("ApiName",JsonConvert.SerializeObject(ApiName)),
                new JProperty("Args",JsonConvert.SerializeObject(args))
            };
            if ((ApiName != FunctionEnums.Disable && ApiName != FunctionEnums.Exit &&
                ApiName != FunctionEnums.Enable && ApiName != FunctionEnums.StartUp)
                && Loading)
            {
                LogHelper.WriteLog(LogLevel.Warning, "AMN框架", "插件逻辑处理", "插件模块处理中...", "x 不处理");
                return null;
            }
            //遍历插件列表,遇到标记消息阻断则跳出
            foreach (var item in Plugins)
            {
                Dll dll = item.dll;
                //先看此插件是否已禁用
                if (item.Enable is false) continue;
                if (item.Testing)
                {
                    Debug.WriteLine($"{item.appinfo.Name} 插件测试中，忽略消息投递");
                    continue;
                }
                try
                {
                    int result = dll.CallFunction(ApiName, args);
                    //调用函数, 返回 1 表示消息阻塞, 跳出后续
                    if (result == 1)
                    {
                        return item;
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog(LogLevel.Error, "函数执行异常", $"插件 {item.appinfo.Name} {ApiName} 函数发生错误，错误信息:{e.Message} {e.StackTrace}");
                    Thread thread = new(() =>
                    {
                        var b = Error_TaskDialog.ShowErrorDialog($"错误模块：{item.appinfo.Name}\n{ApiName} 函数发生错误\n", $"错误信息:\n{e.Message} {e.StackTrace}");
                        switch (b)
                        {
                            case Error_TaskDialog.TaskDialogResult.ReloadApp:
                                ReLoad();
                                break;
                            case Error_TaskDialog.TaskDialogResult.Exit:
                                NotifyIconHelper.HideNotifyIcon();
                                Environment.Exit(0);
                                break;
                        }
                        //报错误但仍继续执行
                    });
                    thread.Start();
                }
            }
            return null;
        }
        /// <summary>
        /// 重载应用
        /// </summary>
        public void ReLoad()
        {
            UnLoad();
            Load();
            LogHelper.WriteLog("遍历启动事件……");
            CallFunction(FunctionEnums.StartUp);
            CallFunction(FunctionEnums.Enable);
            LogHelper.WriteLog("插件启动完成，开始处理消息逻辑……");
            this.Loading = false;
        }
        public static CQPlugin GetPluginByID(string id) => Instance.Plugins.FirstOrDefault(x => x.appinfo.Id == id);
    }
}
