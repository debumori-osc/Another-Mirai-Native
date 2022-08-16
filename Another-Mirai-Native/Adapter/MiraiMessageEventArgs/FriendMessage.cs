using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiMessageEventArgs
{
    public class FriendMessage
    {
        public string type { get; set; }
        public Sender sender { get; set; }
        public object[] messageChain { get; set; }
        public class Sender
        {
            public long id { get; set; }
            public string nickname { get; set; }
            public string remark { get; set; }
        }
    }
}
