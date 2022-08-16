using Another_Mirai_Native.Adapter.CQCode.Model;
using Another_Mirai_Native.Enums;
using System;
using System.Collections.Generic;
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
                messageChain = msgChains.ToJson()
            };
            MiraiAdapter.Instance.CallMiraiAPI(request);
            return 0;
        }
        public static int SendFriendMessage(long QQ, string message)
        {
            return 0;
        }
    }
}
