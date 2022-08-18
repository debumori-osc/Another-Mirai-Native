using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class MemberMuteEvent
    {
        public string type { get; set; }
        public long durationSeconds { get; set; }
        public Member member { get; set; }
        [JsonProperty(PropertyName = "operator")]
        public Operator _operator { get; set; }
        public class Member
        {
            public long id { get; set; }
            public string memberName { get; set; }
            public string specialTitle { get; set; }
            public string permission { get; set; }
            public long jolongimestamp { get; set; }
            public long lastSpeakTimestamp { get; set; }
            public long muteTimeRemaining { get; set; }
            public Group group { get; set; }
        }

        public class Group
        {
            public long id { get; set; }
            public string name { get; set; }
            public string permission { get; set; }
        }

        public class Operator
        {
            public long id { get; set; }
            public string memberName { get; set; }
            public string specialTitle { get; set; }
            public string permission { get; set; }
            public long jolongimestamp { get; set; }
            public long lastSpeakTimestamp { get; set; }
            public long muteTimeRemaining { get; set; }
            public Group group { get; set; }
        }
    }
}
