using Newtonsoft.Json.Linq;
using System.Diagnostics;
using WebSocket4Net;

namespace Another_Mirai_Native
{
    public class MiraiAdapter
    {
        public string WsURL { get; set; }
        public string AuthKey { get; set; }
        public string QQ { get; set; }
        public string SessionKey { get; set; }
        public WebSocket websocket;
        public delegate void ConnectedStateChange(bool status, string msg);
        public event ConnectedStateChange ConnectedStateChanged;
        public MiraiAdapter(string url, string qq, string authkey)
        {
            if (url.EndsWith("/")) url = url[..^1];
            WsURL = url;
            AuthKey = authkey;
            QQ = qq;
            string connect_url = $"{WsURL}/all?verifyKey={AuthKey}&qq={QQ}";
            websocket = new(connect_url);
            websocket.Opened += Websocket_Opened;
            websocket.MessageReceived += Websocket_MessageReceived;
            websocket.Closed += Websocket_Closed;
        }

        private void Websocket_Closed(object? sender, EventArgs e)
        {
            Debug.WriteLine("Closed");
            // ConnectedStateChanged?.Invoke(false, "连接断开");
        }

        private void Websocket_MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            JObject json = JObject.Parse(e.Message);
            if(string.IsNullOrWhiteSpace(SessionKey))
            {
                if (json["data"]["code"].ToString() == "0")
                {
                    SessionKey = json["data"]["session"].ToString();
                    ConnectedStateChanged?.Invoke(true, "");
                }
                else
                {
                    if (json["data"].ContainsKey("msg"))
                    {
                        ConnectedStateChanged?.Invoke(false, json["data"]["msg"].ToString());
                    }
                    else
                    {
                        ConnectedStateChanged?.Invoke(false, "连接失败");
                    }
                    websocket.Close();
                }
            }
            
        }

        private void Websocket_Opened(object? sender, EventArgs e)
        {
            Debug.WriteLine("Connect");
        }

        public bool Connect()
        {
            websocket.Open();
            return false;
        }
    }
}
