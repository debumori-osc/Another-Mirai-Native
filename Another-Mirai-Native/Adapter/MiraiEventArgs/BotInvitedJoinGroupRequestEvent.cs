using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class BotInvitedJoinGroupRequestEvent
    {
        public string type { get; set; }
        public int eventId { get; set; }
        public int fromId { get; set; }
        public int groupId { get; set; }
        public string groupName { get; set; }
        public string nick { get; set; }
        public string message { get; set; }
    }
}
