using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiMessageEventArgs
{
    public class GroupMessage
    {
        public string type { get; set; }
        public Sender sender { get; set; }
        public object[] messageChain { get; set; }
        public class Sender
        {
            public int id { get; set; }
            public string memberName { get; set; }
            public string specialTitle { get; set; }
            public string permission { get; set; }
            public int joinTimestamp { get; set; }
            public int lastSpeakTimestamp { get; set; }
            public int muteTimeRemaining { get; set; }
            public Group group { get; set; }
        }
        public class Group
        {
            public int id { get; set; }
            public string name { get; set; }
            public string permission { get; set; }
        }
    }
}
