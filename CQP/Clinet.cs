using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public class ApiResult
        {
            public bool success { get; set; }
            public string data { get; set; }
            public JObject json { get; set; }
        }
        public WebSocket ServerConnection;
        public static Clinet Instance;
        public Clinet()
        {
            Instance = this;
            Directory.CreateDirectory("logs/cqp");
            File.AppendAllLines("logs/cqp/log.txt", new string[] { $"[{DateTime.Now:G}] CQP Client Started" });
            ServerConnection = new($"ws://127.0.0.1:{ConfigHelper.GetConfig<int>("Ws_ServerPort")}/amn");
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
            if (ApiQueue.Count != 0)
            {
                ApiQueue.Peek().result = e.Data;
            }
        }

        private void ServerConnection_OnOpen(object sender, EventArgs e)
        {
            File.AppendAllLines("logs/cqp/log.txt", new string[] { $"[{DateTime.Now:G}] 连接成功" });
            Send(WsServerFunction.Info, new { role = WsClientType.CQP, key = ConfigHelper.GetConfig<string>("WsServer_Key") }, false);
        }
        Queue<QueueObject> ApiQueue = new();
        class QueueObject
        {
            public string request { get; set; }
            public string result { get; set; } = "";
        }
        public ApiResult Send(WsServerFunction type, object data, bool queue = true)
        {
            if(ServerConnection.ReadyState == WebSocketState.Open)
            {
                if (!queue)
                {
                    ServerConnection.Send(new { type, data }.ToJson());
                    return null;
                }
                QueueObject queueObject = new() { request = new { type, data }.ToJson() };
                ApiQueue.Enqueue(queueObject);
                if (ApiQueue.Count == 1)
                    ServerConnection.Send(queueObject.request);
                // 超时脱出
                int timoutCountMax = 1000;
                int timoutCount = 0;
                while (queueObject.result == "")
                {
                    if (timoutCount > timoutCountMax)
                    {
                        queueObject.result = "{\"data\": \"\\\"callResult\\\": null\"}";
                    }
                    Thread.Sleep(10);
                    timoutCount++;
                }
                ApiQueue.Dequeue();
                if (ApiQueue.Count != 0)
                    ServerConnection.Send(ApiQueue.Peek().request);
                var r = JsonConvert.DeserializeObject<ApiResult>(queueObject.result);
                r.json = JObject.Parse(r.data);
                return r;
            }
            return null;
        }
    }
}
