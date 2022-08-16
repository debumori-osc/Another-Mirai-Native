using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class FriendInputStatusChangedEvent
    {
        public string type { get; set; }
        public Friend friend { get; set; }
        public bool inputting { get; set; }
        public class Friend
        {
            public int id { get; set; }
            public string nickname { get; set; }
            public string remark { get; set; }
        }
    }
}
