using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class BotLeaveEventDisband
    {
        public string type { get; set; }
        public Group group { get; set; }
        public object _operator { get; set; }
        public class Group
        {
            public long id { get; set; }
            public string name { get; set; }
            public string permission { get; set; }
        }
    }
}
