using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class MemberJoinRequestEvent
    {
        public string type { get; set; }
        public long eventId { get; set; }
        public long fromId { get; set; }
        public long groupId { get; set; }
        public string groupName { get; set; }
        public string nick { get; set; }
        public string message { get; set; }
    }
}
