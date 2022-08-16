using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class NudgeEvent
    {
        public string type { get; set; }
        public long fromId { get; set; }
        public Subject subject { get; set; }
        public string action { get; set; }
        public string suffix { get; set; }
        public long target { get; set; }
        public class Subject
        {
            public long id { get; set; }
            public string kind { get; set; }
        }

    }
}
