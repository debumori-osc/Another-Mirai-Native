using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Adapter
{
    public static class Cache
    {
        public static Dictionary<long, (long, string)> FriendRequset { get; set; } = new();
        public static Dictionary<long, (long, string, long, string)> GroupRequset { get; set; } = new();
        public static Dictionary<long, JArray> GroupList { get; set; } = new();
        public static Dictionary<(long, long), JObject> GroupMemberInfo { get; set; } = new();
    }
}
