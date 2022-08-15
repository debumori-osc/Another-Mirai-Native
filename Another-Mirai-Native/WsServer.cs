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
            protected override void OnMessage(MessageEventArgs e)
            {
                JObject json = JObject.Parse(e.Data);
                switch (json["type"].ToObject<WsServerFunction>())
                {
                    case WsServerFunction.AddLog:
                        break;
                    case WsServerFunction.GetLog:
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
