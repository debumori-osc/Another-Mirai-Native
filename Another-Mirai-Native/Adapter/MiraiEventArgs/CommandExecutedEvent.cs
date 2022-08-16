using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class CommandExecutedEvent
    {
        public string type { get; set; }
        public string name { get; set; }
        public object friend { get; set; }
        public object member { get; set; }
        public Arg[] args { get; set; }
    }

    public class Arg
    {
        public string type { get; set; }
        public string text { get; set; }
    }
}
