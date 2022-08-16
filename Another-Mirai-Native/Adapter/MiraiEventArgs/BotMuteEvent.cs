using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class BotMuteEvent
    {
        public string type { get; set; }
        public int durationSeconds { get; set; }
        public Operator _operator { get; set; }
        public class Operator
        {
            public int id { get; set; }
            public string memberName { get; set; }
            public string permission { get; set; }
            public string specialTitle { get; set; }
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
