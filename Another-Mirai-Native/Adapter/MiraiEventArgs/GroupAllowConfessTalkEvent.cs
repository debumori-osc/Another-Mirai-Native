using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class GroupAllowConfessTalkEvent
    {
        public string type { get; set; }
        public bool origin { get; set; }
        public bool current { get; set; }
        public Group group { get; set; }
        public bool isByBot { get; set; }
        public class Group
        {
            public long id { get; set; }
            public string name { get; set; }
            public string permission { get; set; }
        }

    }
}
