using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native
{
    public static class Helper
    {
        public static long TimeStamp => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        public static DateTime TimeStamp2DateTime(long timestamp) => new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local).AddSeconds(timestamp);
        public static bool ContainsKey(this JToken json, string key)
        {
            try
            {
                foreach (JProperty item in json.Children())
                {
                    if (item.Name == key) return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
