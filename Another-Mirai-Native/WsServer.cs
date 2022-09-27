using Another_Mirai_Native.Adapter;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Forms;
using Another_Mirai_Native.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using Another_Mirai_Native.Adapter.MiraiEventArgs;
using System.Collections.Generic;
using SqlSugar;
using Another_Mirai_Native.Adapter.CQCode;
using WebServerSupport;

namespace Another_Mirai_Native
{
    /// <summary>
    /// WebSocket逻辑
    /// </summary>
    public class WsServer
    {
        /// <summary>
        /// 描述WebSocket逻辑返回结果的类
        /// </summary>
        public class ApiResult
        {
            /// <summary>
            /// 调用是否成功
            /// </summary>
            public bool Success { get; set; } = true;
            /// <summary>
            /// 调用返回的结果
            /// </summary>
            public object Data { get; set; }
            /// <summary>
            /// 错误消息
            /// </summary>
            public string Msg { get; set; } = "ok";
            /// <summary>
            /// 调用逻辑的类别
            /// </summary>
            public string Type { get; set; }
        }
        public class Handler : WebSocketBehavior
        {
            /// <summary>
            /// 连接类别
            /// </summary>
            private WsClientType ClientType = WsClientType.UnAuth;
            protected override void OnMessage(MessageEventArgs e)
            {
                Stopwatch sw = new();
                sw.Start();
                JObject json = JObject.Parse(e.Data);
                var data = json["data"];
                // 当type为Info时, 对连接进行鉴权, 若未授权的连接将无法调用逻辑
                // 认为所有连接都必须进行授权, 将鉴权密码写入配置文件中
                // CQP.dll在鉴权时也需要进行鉴权, 防止连接伪造为CQP连接而跳过鉴权流程
                switch (json["type"].ToObject<WsServerFunction>())
                {
                    case WsServerFunction.Info:
                        switch (data["role"].ToObject<WsClientType>())
                        {
                            case WsClientType.CQP:
                                InitRole(data);
                                Instance.CQPConnected?.Invoke();
                                break;
                            case WsClientType.WebUI:
                                InitRole(data);
                                LogHelper.LogAdded -= WebUI_LogAdded;
                                LogHelper.LogAdded += WebUI_LogAdded;
                                LogHelper.LogStatusUpdated -= WebUI_LogStatusUpdated;
                                LogHelper.LogStatusUpdated += WebUI_LogStatusUpdated;
                                break;
                            case WsClientType.UnAuth:
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                // 连接未授权, 拒绝处理逻辑
                if (ClientType == WsClientType.UnAuth)
                {
                    Send(new ApiResult { Type = "Info", Msg = "UnAuth", Success = false });
                    return;
                }
                int logid = 0;
                switch (json["type"].ToObject<WsServerFunction>())
                {
                    case WsServerFunction.AddLog:
                        Ws_AddLog(json);
                        break;
                    case WsServerFunction.GetLog:
                        Ws_GetLog(json);
                        break;
                    case WsServerFunction.CallMiraiAPI:
                        logid = HandleMiraiFunction(json, logid);
                        break;
                    case WsServerFunction.CallCQFunction:
                        HandleCQFunction(json);
                        break;
                    case WsServerFunction.Exit:
                        Ws_ExitApplication();
                        break;
                    case WsServerFunction.Restart:
                        Ws_RestartApplication();
                        break;
                    case WsServerFunction.AddPlugin:
                        Ws_AddPlugin(json);
                        break;
                    case WsServerFunction.ReloadPlugin:
                        Ws_ReloadPlugin(json);
                        break;
                    case WsServerFunction.GetPluginList:
                        Ws_GetPluginList();
                        break;
                    case WsServerFunction.SwitchPluginStatus:
                        Ws_SwitchPluginStatus(json);
                        break;
                    case WsServerFunction.BotInfo:
                        Ws_GetBotInfo();
                        break;
                    case WsServerFunction.GetGroupList:
                        Ws_GetGroupList();
                        break;
                    case WsServerFunction.GetMemberList:
                        Ws_GetMemberList(json);
                        break;
                    case WsServerFunction.GetMemberInfo:
                        Ws_GetMemberInfo(json);
                        break;
                    case WsServerFunction.GetFriendInfo:
                        Ws_GetFriendInfo(json);
                        break;
                    case WsServerFunction.GetFriendList:
                        Ws_GetFriendList(json);
                        break;
                    case WsServerFunction.GetDirectroy:
                        Ws_GetDirectory(json);
                        break;
                    case WsServerFunction.HeartBeat:
                        Send("ok");
                        break;
                    case WsServerFunction.Status:
                        Ws_GetStatus();
                        break;
                    case WsServerFunction.DeviceInfo:
                        Ws_GetDeviceInfo();
                        break;
                    case WsServerFunction.Table:
                        Ws_GetTable();
                        break;
                    case WsServerFunction.CheckTest:
                        Ws_CheckTest();
                        break;
                    case WsServerFunction.EnableTest:
                        Ws_EnableTest(json);
                        break;
                    case WsServerFunction.DisableTest:
                        Ws_DisableTest();
                        break;
                    case WsServerFunction.SendTestMsg:
                        Ws_SendTestMsg(json);
                        break;
                    case WsServerFunction.ActiveForwarder:
                        MiraiAdapter.Instance.OnMessageArrived += MiraiAdapter_OnMessageArrived;
                        break;
                    case WsServerFunction.InactiveForwarder:
                        MiraiAdapter.Instance.OnMessageArrived -= MiraiAdapter_OnMessageArrived;
                        break;
                    case WsServerFunction.UploadImage:
                        Ws_SendImage(json);
                        break;
                    case WsServerFunction.DeleteImage:
                        Ws_RemoveImage(json);
                        break;
                    case WsServerFunction.BuildWebServer:
                        Ws_BuildWebServer();
                        break;
                    case WsServerFunction.SendMsg:
                        Ws_SendMsg(json);
                        break;
                    default:
                        break;
                }
                sw.Stop();
                if (logid == 0) return;
                string updatemsg = $"√ {sw.ElapsedMilliseconds / (double)1000:f2} s";
                LogHelper.UpdateLogStatus(logid, updatemsg);
            }

            private void Ws_SendMsg(JObject json)
            {
                bool isGroup = ((bool)json["data"]["isGroup"]);
                string message = json["data"]["message"].ToString();
                string parsedMsg = ParseRichText2CQCode(message);

                bool success = false;
                if (isGroup)
                {
                    long groupId = ((long)json["data"]["groupId"]);
                    success =  MiraiAPI.SendGroupMessage(groupId, parsedMsg) != 0;
                }
                else
                {
                    long qqId = ((long)json["data"]["qqId"]);
                    success = MiraiAPI.SendFriendMessage(qqId, parsedMsg) != 0;
                }
                Send(new ApiResult { Success = success });
            }

            private void Ws_GetFriendInfo(JObject json)
            {
                long qqId = ((long)json["data"]["qqId"]);
                var friendInfo = MiraiAPI.GetFriendInfo(qqId);
                Send(new ApiResult { Data = friendInfo });
            }

            private void Ws_GetMemberInfo(JObject json)
            {
                long groupId = ((long)json["data"]["groupId"]);
                long qqId = ((long)json["data"]["qqId"]);
                var memberInfo = MiraiAPI.GetGroupMemberInfo(groupId, qqId);
                Send(new ApiResult { Data = memberInfo });
            }

            private void Ws_GetMemberList(JObject json)
            {
                long groupId = ((long)json["data"]["groupId"]);
                var memberList = MiraiAPI.GetGroupMemberList(groupId);
                Send(new ApiResult { Data = memberList });
            }

            private void Ws_GetGroupList()
            {
                var groupList = MiraiAPI.GetGroupList();
                Send(new ApiResult { Data = groupList });
            }

            private void Ws_GetFriendList(JObject json)
            {
                var friendList = MiraiAPI.GetFriendList();
                if (((bool)json["reserved"]))
                {
                    JArray arr = new();
                    for(int i = friendList.Count - 1; i >= 0; i--)
                    {
                        arr.Add(friendList[i]);
                    }
                    friendList = arr;
                }
                Send(new ApiResult { Data = friendList });
            }

            private void Ws_BuildWebServer()
            {
                int port = ConfigHelper.GetConfig<int>("LocalWebServer_Port");
                if (port == 0)
                {
                    port = 3080;
                    ConfigHelper.SetConfig("LocalWebServer_Port", port);
                }
                try
                {
                    WebServerBuilder.Instance.BuildServer(port);
                }
                catch (Exception e)
                {
                    Send(new ApiResult { Success = false, Msg = e.Message });
                }
                WebServerBuilder.Instance.ListeningPortChanged -= Instance_ListeningPortChanged;
                WebServerBuilder.Instance.ListeningPortChanged += Instance_ListeningPortChanged;
            }

            private void Instance_ListeningPortChanged(int port)
            {
                ConfigHelper.SetConfig("LocalWebServer_Port", port);
            }

            private void Ws_RemoveImage(JObject json)
            {
                string fileName = json["data"]["fileName"].ToString();
                string path = Path.Combine(@"data\image\serverTmp", fileName + ".jpg");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                Send(new ApiResult { });
            }

            private void Ws_SendImage(JObject json)
            {
                string base64 = json["data"]["base64"].ToString();
                string path = @"data\image\serverTmp";
                string fileName = Guid.NewGuid().ToString() + ".jpg";
                Directory.CreateDirectory(path);
                File.WriteAllBytes($"{path}\\{fileName}", Convert.FromBase64String(base64));
                Send(new ApiResult { Data = fileName });
            }
            private string ParseRichText2CQCode(string text)
            {
                if (text.Contains("<!IMG!>") is false) return text;
                string cqcode = "";
                int port = ConfigHelper.GetConfig<int>("LocalWebServer_Port");
                var p = text.Split("<!IMG!>");
                bool flag = false;
                for (int i = 0; i < p.Length; i++)
                {
                    if (p[i] == "<!IMG!>")
                    {
                        if (flag) continue;
                        else flag = true;
                        string imgPlaceHolder = p[i + 1];
                        string filePath = Path.Combine(@"data\image\serverTmp", imgPlaceHolder);
                        if (File.Exists(filePath))
                        {
                            File.WriteAllText($@"data\image\{imgPlaceHolder.Replace(".jpg", "")}.cqimg", $"[image]\nmd5=0\nsize=0\nurl=http://localhost:{port}/{imgPlaceHolder}");
                            cqcode += $"[CQ:image,file={imgPlaceHolder.Replace(".jpg", "")}]";
                        }
                        i++;
                    }
                    else
                    {
                        cqcode += p[i];
                    }
                }
                return cqcode;
            }
            private void MiraiAdapter_OnMessageArrived(string json)
            {
                Send(new ApiResult { Type = "Msg", Data = json });
            }

            private void Ws_CheckTest()
            {
                if (PluginTestHelper.Instance.CheckPlugin())
                {
                    CQPlugin plugin = PluginTestHelper.Instance.TestingPlugin;
                    Send(new ApiResult { Type = "CheckTest", Data = new { Enabled = plugin.Enable, Testing = true, AppInfo = plugin.appinfo } });
                }
                else
                {
                    Send(new ApiResult { Type = "CheckTest" });
                }
            }

            private void Ws_DisableTest()
            {
                PluginTestHelper.Instance.DisableTest();
                Send(new ApiResult { Type = "DisableTest" });
            }

            private void Ws_SendTestMsg(JObject json)
            {
                if (PluginTestHelper.Instance.CheckPlugin() is false)
                {
                    if (Ws_EnableTest(json) is false) return;
                }
                string msg = json["data"]["msg"].ToString();
                bool isGroup = ((bool)json["data"]["isGroup"]);
                long qqId = ((long)json["data"]["qqId"]);
                bool handleFlag = false;
                msg = ParseRichText2CQCode(msg);
                Stopwatch sw = new();
                sw.Start();
                if (isGroup)
                {
                    long groupId = ((long)json["data"]["groupId"]);
                    handleFlag = PluginTestHelper.Instance.SendGroupMsg(msg, groupId, qqId);
                }
                else
                {
                    handleFlag = PluginTestHelper.Instance.SendPrivateMsg(msg, qqId);
                }
                sw.Stop();
                Send(new ApiResult { Type = "SendTestMsg", Data = new { flag = handleFlag, time = sw.ElapsedMilliseconds } });
            }

            private bool Ws_EnableTest(JObject json)
            {
                int authCode = ((int)json["data"]["authCode"]);
                CQPlugin plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
                if (plugin == null)
                {
                    Send(new ApiResult { Type = "EnableTest", Msg = "插件不存在", Success = false });
                    return false;
                }
                plugin.Testing = true;
                PluginTestHelper.Instance.EnableTest(plugin);
                PluginTestHelper.Instance.OnPluginSendMsg -= PluginTestHelper_OnPluginSendMsg;
                PluginTestHelper.Instance.OnPluginSendMsg += PluginTestHelper_OnPluginSendMsg;
                Send(new ApiResult { Type = "EnableTest", Data = new { Enabled = plugin.Enable, Testing = true, AppInfo = plugin.appinfo } });
                return true;
            }

            private void PluginTestHelper_OnPluginSendMsg(string msg)
            {
                Send(new ApiResult { Type = "TestMsgReceive", Data = CQCodeBuilder.BuildMessageChains(msg) });
            }

            private void Ws_GetTable()
            {
                Send(new ApiResult { Type = "Table", Data = UsageMonitor.GetMinuteMonitor() });
            }

            private void Ws_GetStatus()
            {
                DeviceInformation instance = DeviceInformation.Instance;
                int PluginNum = 0;
                if (PluginManagment.Instance != null)
                {
                    PluginNum = PluginManagment.Instance.Plugins.Count;
                }

                Send(new ApiResult
                {
                    Type = "Status",
                    Data = new
                    {
                        CPUUsage = instance.CpuCounter.NextValue(),
                        MemoryLeftAvailable = instance.MemoryCounter.NextValue(),
                        CPUFrequency = instance.CpuBaseFrequencyCounter.NextValue() * (instance.CpuAccCounter.NextValue() / 100),
                        HandleCount = instance.HandleCounter.NextValue(),
                        ThreadCount = instance.ThreadCounter.NextValue(),
                        MessageSpeed = Helper.MsgSpeed.Count,
                        SystemUpTime = Environment.TickCount / 1000,
                        AMNUpTime = (DateTime.Now - Helper.StartUpTime).TotalSeconds,
                        PluginNum
                    }
                });
            }

            private void Ws_GetDeviceInfo()
            {
                DeviceInformation instance = DeviceInformation.Instance;
                Send(new ApiResult
                {
                    Type = "DeviceInfo",
                    Data = new
                    {
                        TotalMemory = instance.TotalMemory / 1024,
                        TotalVirtualMemory = instance.TotalVirtualMemory / 1024,
                        instance.CPUName,
                        instance.CPUCoreCount,
                        instance.OSName,
                        instance.OSVersion,
                        instance.OSArch
                    }
                });
            }

            /// <summary>
            /// 根据传入的路径, 获取子目录以及文件列表
            /// </summary>
            private void Ws_GetDirectory(JObject json)
            {
                string dir = json["data"]["dir"].ToString(); // 全路径
                if (dir.IsNullOrEmpty())// 为空时传递驱动器列表
                {
                    var drivels = DriveInfo.GetDrives();
                    Send(new ApiResult { Type = "GetDirectory", Data = new { driveList = drivels.Select(x => x.Name).ToList() } });
                }
                else
                {
                    string filter = json["data"]["filter"]?.ToString();
                    if (string.IsNullOrWhiteSpace(filter)) filter = ".dll";// 默认筛选dll文件
                    var dirInfo = new DirectoryInfo(dir);
                    Send(new ApiResult
                    {
                        Type = "GetDirectory",
                        Data = new { dirList = dirInfo.GetDirectories().Select(x => x.Name).ToList(), fileList = dirInfo.GetFiles().Where(x => x.Name.EndsWith(filter)).Select(x => x.Name).ToList() }
                    });
                }
            }
            /// <summary>
            /// 获取Bot QQ号与昵称
            /// </summary>
            private void Ws_GetBotInfo()
            {
                Send(new ApiResult { Type = "BotInfo", Data = new { Helper.QQ, nickname = Helper.NickName, version = Application.ProductVersion } }); ;
            }
            /// <summary>
            /// 根据完全路径加载插件
            /// </summary>
            private void Ws_AddPlugin(JObject json)
            {
                string path = json["data"]["path"].ToString();
                if (File.Exists(path) is false)
                {
                    Send(new ApiResult { Type = "AddPlugin", Msg = "文件不存在", Success = false });
                    return;
                }
                if (path.EndsWith(".dll") is false)
                {
                    Send(new ApiResult { Type = "AddPlugin", Msg = "所选文件非插件文件，请选择dll文件", Success = false });
                    return;
                }
                if (File.Exists(path[..^4] + ".json") is false)
                {
                    Send(new ApiResult { Type = "AddPlugin", Msg = "所选插件缺失json文件", Success = false });
                    return;
                }
                PluginManagment.Instance.Load(path);
                Send(new ApiResult());
            }
            /// <summary>
            /// 添加日志
            /// </summary>
            private void Ws_AddLog(JObject json)
            {
                Enums.LogLevel log_priority = json["data"]["args"]["priority"].ToObject<Enums.LogLevel>();
                string log_msg = json["data"]["args"]["msg"].ToObject<string>();
                string log_type = "致命错误";
                string log_origin = "AMN框架";
                if (json["data"]["args"].ContainsKey("type"))
                {
                    log_type = json["data"]["args"]["type"].ToObject<string>();
                }
                if (json["data"]["args"].ContainsKey("authCode"))
                {
                    int authcode = json["data"]["args"]["authCode"].ToObject<int>();
                    var plugin = PluginManagment.Instance.Plugins.Find(x => x.appinfo.AuthCode == authcode);
                    if (plugin != null)
                    {
                        log_origin = plugin.appinfo.Name;
                    }
                }
                LogHelper.WriteLog(log_priority, log_origin, log_type, log_msg, "");
                Send(new ApiResult { Type = "AddLog", Data = new { callResult = 1 } });
            }
            /// <summary>
            /// 根据优先级获取日志, 获取到的条数由配置决定
            /// </summary>
            private void Ws_GetLog(JObject json)
            {
                int priority = json["data"]["priority"].ToObject<int>();
                int pageSize = ((int)json["data"]["itemsPerPage"]);
                int pageIndex = ((int)json["data"]["page"]);
                string search = json["data"]["search"].ToString();

                JArray sortBy = (json["data"]["sortBy"] as JArray);
                string orderBy = string.Empty;
                bool orderByDesc = false;
                if (sortBy != null && sortBy.Count != 0)
                {
                    orderBy = sortBy[0].ToString();
                    orderByDesc = (bool)(json["data"]["sortDesc"] as JArray)![0];// 是否降序, 第一项为bool表示是否降序
                }
                List<DateTime> date = new();
                if (json["date"] is JArray array)
                {
                    foreach (var item in array)// 日志时间筛选
                    {
                        date.Add(DateTime.Parse(item.ToString()));
                    }
                }
                long dt1 = 0, dt2 = 0;
                if (date.Count != 0)
                {
                    dt1 = Helper.DateTime2TimeStamp(date[0]);
                    dt2 = Helper.DateTime2TimeStamp(date[1]);
                }
                var logList = LogHelper.DetailQueryLogs(priority, pageSize, pageIndex, search, orderBy, orderByDesc, dt1, dt2);
                Send(new ApiResult { Type = "GetLog", Data = new { items = logList.Item1, totalCount = logList.Item2 } });
            }
            /// <summary>
            /// 退出应用
            /// </summary>
            private void Ws_ExitApplication()
            {
                Send(new ApiResult { Type = "ExitApplication", });
                Environment.Exit(0);
            }
            /// <summary>
            /// 重启应用
            /// </summary>
            private void Ws_RestartApplication()
            {
                Send(new ApiResult { Type = "RestartApplication", });
                Helper.RestartApplication();
            }
            /// <summary>
            /// 重载单个或所有应用
            /// </summary>
            /// <param name="json"></param>
            private void Ws_ReloadPlugin(JObject json)
            {
                if (json.ContainsKey("authCode"))// 需要重载单个插件时传递插件的AuthCode
                {
                    int authCode = (int)json["data"]["authCode"];
                    var plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == authCode);
                    if (plugin == null)
                    {
                        Send(new ApiResult { Type = "ReloadPlugin", Msg = "插件不存在", Success = false });
                        return;
                    }
                    PluginManagment.Instance.ReLoad(plugin);
                }
                else
                {
                    PluginManagment.Instance.ReLoad();
                }
                PluginManagment.Instance.RefreshPluginList();
                Send(new ApiResult { Type = "ReloadPlugin", });
            }
            /// <summary>
            /// 获取插件列表
            /// </summary>
            private void Ws_GetPluginList()
            {
                var pluginList = PluginManagment.Instance.DistinctPluginList().Select(x => { return new { x.Enable, x.Testing, AppInfo = x.appinfo, json = x.json }; }).ToList();
                Send(new ApiResult { Type = "GetPluginList", Data = pluginList });
            }
            /// <summary>
            /// 切换插件的禁用或启用状态
            /// </summary>
            private void Ws_SwitchPluginStatus(JObject json)
            {
                var pluginAuthCode = (int)json["data"]["authCode"];
                bool status = ((bool)json["data"]["status"]);
                CQPlugin plugin = PluginManagment.Instance.SavedPlugins.FirstOrDefault(x => x.appinfo.AuthCode == pluginAuthCode);

                if (status)
                {
                    if (plugin == null)
                    {
                        Send(new ApiResult { Type = "SwitchPluginStatus", Msg = "插件不存在", Success = false });
                        return;
                    }
                    PluginManagment.Instance.FlipPluginState(plugin);
                    Send(new ApiResult { Type = "SwitchPluginStatus" });
                    return;
                }
                plugin = PluginManagment.Instance.Plugins.FirstOrDefault(x => x.appinfo.AuthCode == pluginAuthCode);
                if (plugin != null)
                {
                    PluginManagment.Instance.FlipPluginState(plugin);
                    Send(new ApiResult { Type = "SwitchPluginStatus" });
                }
                else
                {
                    Send(new ApiResult { Type = "SwitchPluginStatus", Msg = "插件不存在", Success = false });
                }
            }
            /// <summary>
            /// 日志状态更新事件
            /// </summary>
            private void WebUI_LogStatusUpdated(int logid, string status)
            {
                Send(new ApiResult { Type = "LogStatusUpdated", Data = new { logid, status } });
            }
            /// <summary>
            /// 添加日志事件
            /// </summary>
            private void WebUI_LogAdded(int logid, LogModel log)
            {
                Send(new ApiResult { Type = "LogAdded", Data = new { logid, log } });
            }
            /// <summary>
            /// 连接角色鉴权
            /// </summary>
            private void InitRole(JToken data)
            {
                string key = data["key"].ToString();
                if (key == ConfigHelper.GetConfig<string>("WsServer_Key"))
                {
                    ClientType = data["role"].ToObject<WsClientType>();
                    Send(new ApiResult { Type = "Info" });
                }
                else
                {
                    Send(new ApiResult { Type = "Info", Msg = "密码错误", Success = false });
                }
            }

            public void HandleCQFunction(JObject json)
            {
                var apiType = json["data"]["type"].ToObject<string>();
                int authcode = json["data"]["args"]["authCode"].ToObject<int>();
                var plugin = PluginManagment.Instance.Plugins.Find(x => x.appinfo.AuthCode == authcode);
                if (plugin == null)
                {
                    Send(new ApiResult { Success = false });
                    LogHelper.WriteLog("Authcode无效", "");
                    return;
                }
                switch (apiType)
                {
                    case "GetAppDirectory":
                        string path = @$"data\app\{plugin.appinfo.Id}";
                        if (Directory.Exists(path) is false)
                            Directory.CreateDirectory(path);
                        Send(new ApiResult { Type = "HandleCQFunction", Data = new { callResult = new DirectoryInfo(path).FullName + "\\" } });
                        break;
                    case "GetLoginQQ":
                        Send(new ApiResult { Type = "HandleCQFunction", Data = new { callResult = Helper.QQ } });
                        break;
                    case "GetLoginNick":
                        Send(new ApiResult { Type = "HandleCQFunction", Data = new { callResult = Helper.NickName } });
                        break;
                    case "GetImage":
                        string cqimg = json["data"]["args"]["path"].ToString();
                        string url = Helper.GetPicUrlFromCQImg(cqimg);
                        string imgFileName = cqimg + ".jpg";
                        string imgDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image");
                        var downloadTask = Helper.DownloadFile(url, imgFileName, imgDir);
                        downloadTask.Wait();
                        Send(new ApiResult { Type = "HandleCQFunction", Data = new { callResult = Path.Combine(imgDir, imgFileName) } });
                        break;
                    case "GetGroupInfo":
                        long groupId = ((long)json["data"]["args"]["groupId"]);
                        var group = MiraiAPI.GetGroupList().FirstOrDefault(x => ((long)x["id"]) == groupId);
                        var groupMemCount = MiraiAPI.GetGroupMemberList(groupId).Count;
                        Send(new ApiResult { Type = "HandleCQFunction", Data = new { callResult = MiraiAPI.ParseGroupInfo2CQData(groupId, group["name"].ToString(), groupMemCount) } });
                        break;
                    default:
                        break;
                }
                GC.Collect();
            }
            public int HandleMiraiFunction(JObject json, int logid)
            {
                var apiType = json["data"]["type"].ToObject<MiraiApiType>();
                int authcode = json["data"]["args"]["authCode"].ToObject<int>();
                object callResult = 0;
                var plugin = PluginManagment.Instance.Plugins.Find(x => x.appinfo.AuthCode == authcode);
                if (plugin == null)
                {
                    Send(new ApiResult { Type = "HandleMiraiFunction", Success = false });
                    LogHelper.WriteLog("Authcode无效", "");
                    return logid;
                }
                switch (apiType)
                {
                    case MiraiApiType.about:
                        break;
                    case MiraiApiType.botList:
                        break;
                    case MiraiApiType.messageFromId:
                        break;
                    case MiraiApiType.friendList:
                        callResult = MiraiAPI.ParseFriendList2CQData(MiraiAPI.GetFriendList());
                        break;
                    case MiraiApiType.groupList:
                        callResult = MiraiAPI.ParseGroupList2CQData(MiraiAPI.GetGroupList());
                        break;
                    case MiraiApiType.memberList:
                        long memberList_groupid = json["data"]["args"]["groupId"].ToObject<long>();
                        callResult = MiraiAPI.ParseMemberList2CQData(MiraiAPI.GetGroupMemberList(memberList_groupid), memberList_groupid);
                        break;
                    case MiraiApiType.botProfile:
                    case MiraiApiType.friendProfile:
                        break;
                    case MiraiApiType.memberProfile:
                        long memberProfile_groupid = json["data"]["args"]["groupId"].ToObject<long>();
                        long memberProfile_QQId = json["data"]["args"]["qqId"].ToObject<long>();
                        var memeberArr = MiraiAPI.GetGroupMemberList(memberProfile_groupid);
                        callResult = MiraiAPI.ParseGroupMemberInfo2CQData(memeberArr, MiraiAPI.GetGroupMemberInfo(memberProfile_groupid, memberProfile_QQId), memberProfile_groupid, memberProfile_QQId);
                        break;
                    case MiraiApiType.userProfile:
                        break;
                    case MiraiApiType.sendFriendMessage:
                        long friend_QQid = json["data"]["args"]["qqId"].ToObject<long>();
                        string friend_text = json["data"]["args"]["text"].ToObject<string>();
                        if (plugin.Testing)
                        {
                            PluginTestHelper.Instance.ReceiveMsg(friend_text);
                            callResult = 1;
                        }
                        else
                        {
                            logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "[↑]发送私聊消息", $"QQ:{friend_QQid} 消息:{friend_text}", "处理中...");
                            callResult = MiraiAPI.SendFriendMessage(friend_QQid, friend_text);
                        }
                        break;
                    case MiraiApiType.sendGroupMessage:
                        long group_groupid = json["data"]["args"]["groupid"].ToObject<long>();
                        string group_text = json["data"]["args"]["text"].ToObject<string>();
                        if (plugin.Testing)
                        {
                            PluginTestHelper.Instance.ReceiveMsg(group_text);
                            callResult = 1;
                        }
                        else
                        {
                            logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "[↑]发送群聊消息", $"群:{group_groupid} 消息:{group_text}", "处理中...");
                            callResult = MiraiAPI.SendGroupMessage(group_groupid, group_text);
                        }
                        break;
                    case MiraiApiType.sendTempMessage:
                    case MiraiApiType.sendNudge:
                        break;
                    case MiraiApiType.recall:
                        int recall_msgId = json["data"]["args"]["msgId"].ToObject<int>();
                        string recallMsg = MiraiAPI.GetMessageByMsgId(recall_msgId);
                        if (string.IsNullOrEmpty(recallMsg)) recallMsg = "消息拉取失败";
                        logid = LogHelper.WriteLog(Enums.LogLevel.Info, plugin.appinfo.Name, "撤回消息", $"msgid={recall_msgId}, 内容={recallMsg}", "处理中...");
                        callResult = MiraiAPI.RecallMessage(recall_msgId, 0);
                        break;
                    case MiraiApiType.roamingMessages:
                    case MiraiApiType.file_list:
                    case MiraiApiType.file_info:
                    case MiraiApiType.file_mkdir:
                    case MiraiApiType.file_delete:
                    case MiraiApiType.file_move:
                    case MiraiApiType.file_rename:
                    case MiraiApiType.deleteFriend:
                        break;
                    case MiraiApiType.mute:
                        long mute_group = json["data"]["args"]["groupId"].ToObject<long>();
                        long mute_qq = json["data"]["args"]["qqId"].ToObject<long>();
                        long mute_time = json["data"]["args"]["time"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.Info, plugin.appinfo.Name, "禁言群成员", $"禁言群{mute_group} 成员{mute_qq} {mute_time}秒", "处理中...");
                        callResult = MiraiAPI.MuteGroupMemeber(mute_group, mute_qq, mute_time);
                        break;
                    case MiraiApiType.unmute:
                        long unmute_group = json["data"]["args"]["groupId"].ToObject<long>();
                        long unmute_qq = json["data"]["args"]["qqId"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "解除禁言群成员", $"解除禁言群{unmute_group} 成员{unmute_qq}", "处理中...");
                        callResult = MiraiAPI.UnmuteGroupMemeber(unmute_group, unmute_qq);
                        break;
                    case MiraiApiType.kick:
                        long kick_group = json["data"]["args"]["groupId"].ToObject<long>();
                        long kick_qq = json["data"]["args"]["qqId"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "踢出群成员", $"移除群{kick_group} 成员{kick_qq}", "处理中...");
                        callResult = MiraiAPI.KickGroupMember(kick_group, kick_qq);
                        break;
                    case MiraiApiType.quit:
                        long quit_group = json["data"]["args"]["groupId"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.Info, plugin.appinfo.Name, "退出群", $"退出群{quit_group}", "处理中...");
                        callResult = MiraiAPI.GroupMute(quit_group);
                        break;
                    case MiraiApiType.muteAll:
                        long muteAll_group = json["data"]["args"]["groupId"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.Info, plugin.appinfo.Name, "群禁言", $"禁言群{muteAll_group}", "处理中...");
                        callResult = MiraiAPI.GroupMute(muteAll_group);
                        break;
                    case MiraiApiType.unmuteAll:
                        long unmuteAll_group = json["data"]["args"]["groupId"].ToObject<long>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.Info, plugin.appinfo.Name, "解除群禁言", $"解除禁言群{unmuteAll_group}", "处理中...");
                        callResult = MiraiAPI.GroupUnmute(unmuteAll_group);
                        break;
                    case MiraiApiType.setEssence:
                        break;
                    case MiraiApiType.groupConfig_get:
                        break;
                    case MiraiApiType.groupConfig_update:
                        break;
                    case MiraiApiType.memberInfo_get:
                        break;
                    case MiraiApiType.memberInfo_update:
                        long memberInfo_group = json["data"]["args"]["groupId"].ToObject<long>();
                        long memberInfo_qq = json["data"]["args"]["qqId"].ToObject<long>();
                        if (json["data"]["args"].ContainsKey("title"))
                        {
                            string memberInfo_title = json["data"]["args"]["title"].ToObject<string>();
                            logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "设置群成员头衔", $"设置群{memberInfo_group} 成员{memberInfo_qq} 头衔 {memberInfo_title}", "处理中...");
                            callResult = MiraiAPI.SetSpecialTitle(memberInfo_group, memberInfo_qq, memberInfo_title);
                        }
                        else
                        {
                            string memberInfo_card = json["data"]["args"]["newCard"].ToObject<string>();
                            logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, "设置群成员名片", $"设置群{memberInfo_group} 成员{memberInfo_qq} 名片 {memberInfo_card}", "处理中...");
                            callResult = MiraiAPI.SetGroupCard(memberInfo_group, memberInfo_qq, memberInfo_card);
                        }
                        break;
                    case MiraiApiType.memberAdmin:
                        long admin_group = json["data"]["args"]["groupId"].ToObject<long>();
                        long admin_qq = json["data"]["args"]["qqId"].ToObject<long>();
                        bool admin_set = json["data"]["args"]["isSet"].ToObject<bool>();
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, $"{(admin_set ? "设置" : "取消")}群成员管理", $"{(admin_set ? "设置" : "取消")}群{admin_group} 成员{admin_qq}", "处理中...");
                        callResult = MiraiAPI.SetAdmin(admin_group, admin_qq, admin_set);
                        break;
                    case MiraiApiType.anno_list:
                        break;
                    case MiraiApiType.anno_publish:
                        break;
                    case MiraiApiType.anno_delete:
                        break;
                    case MiraiApiType.resp_newFriendRequestEvent:
                        long respf_eventId = json["data"]["args"]["eventId"].ToObject<long>();
                        int respf_operate = json["data"]["args"]["requestType"].ToObject<int>();
                        string respf_message = json["data"]["args"]["message"].ToObject<string>();
                        string respf_msg = "";
                        switch (respf_operate)
                        {
                            case 0:
                                respf_msg = "同意";
                                break;
                            case 1:
                                respf_msg = "拒绝";
                                break;
                        }
                        long respf_fromId = 0;
                        string respf_nick = "";
                        if (Cache.FriendRequset.ContainsKey(respf_eventId))
                        {
                            respf_fromId = Cache.FriendRequset[respf_eventId].Item1;
                            respf_nick = Cache.FriendRequset[respf_eventId].Item2;
                        }
                        Cache.FriendRequset.Remove(respf_eventId);
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, $"好友添加申请", $"来源: {respf_fromId}({respf_nick}) 操作: {respf_msg}", "处理中...");
                        callResult = MiraiAPI.HandleFriendRequest(respf_eventId, respf_operate, respf_message);
                        break;
                    case MiraiApiType.resp_memberJoinRequestEvent:
                        long respm_eventId = json["data"]["args"]["eventId"].ToObject<long>();
                        int respm_operate = json["data"]["args"]["requestType"].ToObject<int>();
                        string respm_message = json["data"]["args"]["message"].ToObject<string>();
                        string respm_msg = "";
                        switch (respm_operate)
                        {
                            case 0:
                                respf_msg = "同意";
                                break;
                            case 1:
                                respf_msg = "拒绝";
                                break;
                        }
                        long respm_fromId = 0, respm_groupId = 0;
                        string respm_nick = "", respm_groupname = "";
                        if (Cache.GroupRequset.ContainsKey(respm_eventId))
                        {
                            respm_fromId = Cache.GroupRequset[respm_eventId].Item1;
                            respm_nick = Cache.GroupRequset[respm_eventId].Item2;
                            respm_groupId = Cache.GroupRequset[respm_eventId].Item3;
                            respm_groupname = Cache.GroupRequset[respm_eventId].Item4;
                        }
                        Cache.GroupRequset.Remove(respm_eventId);
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, $"群添加申请", $"来源: {respm_fromId}({respm_nick}) 目标群: {respm_groupId}({respm_groupname}) 操作: {respm_msg}", "处理中...");
                        callResult = MiraiAPI.HandleGroupRequest(respm_eventId, respm_operate, respm_message);
                        break;
                    case MiraiApiType.resp_botInvitedJoinGroupRequestEvent:
                        long respb_eventId = json["data"]["args"]["eventId"].ToObject<long>();
                        int respb_operate = json["data"]["args"]["requestType"].ToObject<int>();
                        string respb_message = json["data"]["args"]["message"].ToObject<string>();
                        string respb_msg = "";
                        switch (respb_operate)
                        {
                            case 0:
                                respf_msg = "同意";
                                break;
                            case 1:
                                respf_msg = "拒绝";
                                break;
                        }
                        long respb_fromId = 0, respb_groupId = 0;
                        string respb_nick = "", respb_groupname = "";
                        if (Cache.GroupRequset.ContainsKey(respb_eventId))
                        {
                            respb_fromId = Cache.GroupRequset[respb_eventId].Item1;
                            respb_nick = Cache.GroupRequset[respb_eventId].Item2;
                            respb_groupId = Cache.GroupRequset[respb_eventId].Item3;
                            respb_groupname = Cache.GroupRequset[respb_eventId].Item4;
                        }
                        Cache.GroupRequset.Remove(respb_eventId);
                        logid = LogHelper.WriteLog(Enums.LogLevel.InfoSend, plugin.appinfo.Name, $"群邀请添加申请", $"来源群: {respb_groupId}({respb_groupname}) 来源人: {respb_fromId}({respb_nick}) 操作: {respb_msg}", "处理中...");
                        callResult = MiraiAPI.HandleInviteRequest(respb_eventId, respb_operate, respb_message);
                        break;
                    default:
                        break;
                }
                Send(new ApiResult { Type = "HandleMiraiFunction", Data = new { callResult } });
                return logid;
            }
            public void Send(object objData)
            {
                Send(data: objData.ToJson());
            }
        }
        public static WsServer Instance { get; set; }
        public WebSocketServer Server { get; set; }
        public class DeviceInformation
        {
            public static DeviceInformation Instance { get; set; }
            public PerformanceCounter CpuCounter { get; set; }
            public PerformanceCounter CpuBaseFrequencyCounter { get; set; }
            public PerformanceCounter CpuAccCounter { get; set; }
            public PerformanceCounter HandleCounter { get; set; }
            public PerformanceCounter ThreadCounter { get; set; }
            public PerformanceCounter MemoryCounter { get; set; }
            public ManagementObjectCollection DeviceInfo { get; set; }

            public string CPUName { get; set; }
            public int CPUCoreCount { get; set; }
            public int CPUThreadCount { get; set; }

            public int TotalMemory { get; set; }
            public int TotalVirtualMemory { get; set; }

            public string OSVersion { get; set; }
            public string OSName { get; set; }
            public string OSArch { get; set; }
        }

        public delegate void CQPConnected_Handler();
        public event CQPConnected_Handler CQPConnected;
        public WsServer(int port)
        {
            Server = new(port);
            Server.AddWebSocketService<Handler>("/amn");
            Instance = this;
            new Thread(() =>
            {
                DeviceInformation.Instance = new()
                {
                    CpuCounter = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total"),
                    MemoryCounter = new PerformanceCounter("Memory", "Available MBytes"),
                    CpuBaseFrequencyCounter = new PerformanceCounter("Processor Information", "Processor Frequency", "_Total"),
                    CpuAccCounter = new PerformanceCounter("Processor Information", "% Processor Performance", "_Total"),
                    HandleCounter = new PerformanceCounter("Process", "Handle Count", "_Total"),
                    ThreadCounter = new PerformanceCounter("Process", "Thread Count", "_Total"),
                    DeviceInfo = new ManagementObjectSearcher("SELECT * FROM Win32_Processor").Get()
                };
                foreach (var item in DeviceInformation.Instance.DeviceInfo)
                {
                    DeviceInformation.Instance.CPUName = item["Name"].ToString();
                    DeviceInformation.Instance.CPUCoreCount = Convert.ToInt32(item["NumberOfCores"].ToString());
                    DeviceInformation.Instance.CPUThreadCount = Convert.ToInt32(item["ThreadCount"].ToString());
                }
                DeviceInformation.Instance.DeviceInfo = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get();
                foreach (var item in DeviceInformation.Instance.DeviceInfo)
                {
                    DeviceInformation.Instance.OSVersion = item["Version"].ToString();
                    DeviceInformation.Instance.OSName = item["Caption"].ToString();
                    DeviceInformation.Instance.TotalMemory = Convert.ToInt32(item["TotalVisibleMemorySize"].ToString());
                    DeviceInformation.Instance.TotalVirtualMemory = Convert.ToInt32(item["TotalVirtualMemorySize"].ToString());
                }
                DeviceInformation.Instance.OSArch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            }).Start();
        }
        public static void Init(int port)
        {
            var server = new WsServer(port);
            server.Start();
            Debug.WriteLine("WebSocket服务器创建成功");
        }
        public void Start()
        {
            Server.Start();
        }
        public void Stop()
        {
            Server.Stop();
        }
    }
}
