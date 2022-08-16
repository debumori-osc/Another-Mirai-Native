using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class MemberPermissionChangeEvent
    {
        public string type { get; set; }
        public string origin { get; set; }
        public string current { get; set; }
        public Member member { get; set; }
        public class Member
        {
            public int id { get; set; }
            public string memberName { get; set; }
            public string permission { get; set; }
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
