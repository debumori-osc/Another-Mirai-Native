using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class FriendNickChangedEvent
    {
        public string type { get; set; }
        public Friend friend { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public class Friend
        {
            public long id { get; set; }
            public string nickname { get; set; }
            public string remark { get; set; }
        }
    }
}
