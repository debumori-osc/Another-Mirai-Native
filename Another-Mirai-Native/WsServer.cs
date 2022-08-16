using Another_Mirai_Native.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Another_Mirai_Native
{
    public class WsServer
    {
        private class Handler : WebSocketBehavior
        {
            private WsClientType ClientType = WsClientType.UnAuth;
            protected override void OnMessage(MessageEventArgs e)
            {
                JObject json = JObject.Parse(e.Data);
                var data = json["data"];
                switch (json["type"].ToObject<WsServerFunction>())
                {
                    case WsServerFunction.Info:
                        switch (data["role"].ToObject<WsClientType>())
                        {
                            case WsClientType.CQP:
                            case WsClientType.WebUI:
                                string key = data["key"].ToString();
                                if (key == ConfigHelper.GetConfig<string>("WsServer_Key"))
                                {
                                    ClientType = data["role"].ToObject<WsClientType>();
                                }
                                break;
                            case WsClientType.UnAuth:
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                if(ClientType == WsClientType.UnAuth)
                {
                    Send(WsServerFunction.UnAuth, "");
                    return;
                }
                switch (json["type"].ToObject<WsServerFunction>())
                {
                    case WsServerFunction.AddLog:
                        break;
                    case WsServerFunction.GetLog:
                        break;
                    case WsServerFunction.CallMiraiAPI:
                        break;
                    case WsServerFunction.CallCQFunction:
                        break;
                    case WsServerFunction.Exit:
                        break;
                    case WsServerFunction.Restart:
                        break;
                    case WsServerFunction.AddPlugin:
                        break;
                    case WsServerFunction.ReloadPlugin:
                        break;
                    case WsServerFunction.GetPluginList:
                        break;
                    case WsServerFunction.SwitchPluginStatus:
                        break;
                    case WsServerFunction.GetBotInfo:
                        break;
                    case WsServerFunction.GetGroupList:
                        break;
                    case WsServerFunction.GetFriendList:
                        break;
                    case WsServerFunction.GetStatus:
                        break;
                    default:
                        break;
                }
            }
            public void Send(WsServerFunction type, object data)
            {
                 Send(new { type, data }.ToJson());
            }
        }
        public static WsServer Instance { get; set; }
        public WebSocketServer Server { get; set; }
        public WsServer(int port)
        {
            Server = new(port);
            Server.AddWebSocketService<Handler>("/amn");
            Instance = this;
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
