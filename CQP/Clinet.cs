using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace CQP
{
    public class Clinet
    {
        public WebSocket ServerConnection;
        public static Clinet Instance;
        public Clinet()
        {
            Instance = this;
            Directory.CreateDirectory("logs/cqp");
            File.AppendAllLines("logs/cqp/log.txt", new string[] { $"[{DateTime.Now:G}] CQP Client Started" });
            ServerConnection = new($"ws://localhost:{ConfigHelper.GetConfig<int>("Ws_ServerPort")}/amn");
            ServerConnection.OnOpen += ServerConnection_OnOpen;
            ServerConnection.OnMessage += ServerConnection_OnMessage;
            ServerConnection.OnClose += ServerConnection_OnClose;
            ServerConnection.Connect();
        }

        private void ServerConnection_OnClose(object sender, CloseEventArgs e)
        {
            Thread.Sleep(3000);
            File.AppendAllLines("logs/cqp/log.txt", new string[] { $"[{DateTime.Now:G}] 与服务器连接断开..." });
            ServerConnection = new($"ws://localhost:{ConfigHelper.GetConfig<int>("Ws_ServerPort")}/amn");
            ServerConnection.OnOpen += ServerConnection_OnOpen;
            ServerConnection.OnMessage += ServerConnection_OnMessage;
            ServerConnection.OnClose += ServerConnection_OnClose;
            ServerConnection.Connect();
        }

        private void ServerConnection_OnMessage(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ServerConnection_OnOpen(object sender, EventArgs e)
        {
            File.AppendAllLines("logs/cqp/log.txt", new string[] { $"[{DateTime.Now:G}] 连接成功" });
            Send(WsServerFunction.Info, new { role = WsClientType.CQP, key = ConfigHelper.GetConfig<string>("WsServer_Key") });
        }
        public void Send(WsServerFunction type, object data)
        {
            if(ServerConnection.ReadyState == WebSocketState.Open)
            {
                ServerConnection.Send(new { type, data }.ToJson());
            }
        }
    }
}
