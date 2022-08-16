using Another_Mirai_Native.Adapter.CQCode.Model;
using Another_Mirai_Native.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter
{
    public static class MiraiAPI
    {
        public static int SendGroupMessage(long group, string message)
        {
            MiraiMessageBase[] msgChains = CQCodeBuilder.BuildMessageChains(message).ToArray();
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = group,
                messageChain = msgChains
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.sendGroupMessage, request));
            if (((int)json["code"]) == 0)
            {
                return (int)json["messageId"];
            }
            else
            {
                Debug.WriteLine(json["msg"].ToString());
            }
            return 0;
        }
        public static int SendFriendMessage(long QQ, string message)
        {
            MiraiMessageBase[] msgChains = CQCodeBuilder.BuildMessageChains(message).ToArray();
            object request = new
            {
                sessionKey = MiraiAdapter.Instance.SessionKey_Message,
                target = QQ,
                messageChain = msgChains.ToJson()
            };
            JObject json = JObject.Parse(MiraiAdapter.Instance.CallMiraiAPI(MiraiApiType.sendFriendMessage, request));
            if (((int)json["code"]) == 0)
            {
                return (int)json["messageId"];
            }
            return 0;
        }
    }
}
