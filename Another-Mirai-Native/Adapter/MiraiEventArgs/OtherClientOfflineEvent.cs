using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter.MiraiEventArgs
{
    public class OtherClientOfflineEvent
    {
        public string type { get; set; }
        public Client client { get; set; }
        public class Client
        {
            public int id { get; set; }
            public string platform { get; set; }
        }
    }
}
