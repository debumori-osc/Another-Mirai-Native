using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class FriendRecallEvent
    {
        public string type { get; set; }
        public int authorId { get; set; }
        public int messageId { get; set; }
        public int time { get; set; }
        public int _operator { get; set; }
    }
}
