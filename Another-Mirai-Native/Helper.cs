using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native
{
    public static class Helper
    {
        public static string QQ { get; set; }
        public static string WsURL { get; set; }
        public static string WsAuthKey { get; set; }
        public static int MaxLogCount { get; set; } = 500;
        public static long TimeStamp => (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)).TotalSeconds;
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
        public static async Task<Stream> Get(string url)
        {
            try
            {
                using var http = new HttpClient();
                var r = http.GetAsync(url);
                return await r.Result.Content.ReadAsStreamAsync();
            }
            catch
            {
                return null;
            }
        }
        public static Image Base642Image(string base64)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                MemoryStream ms = new(bytes);
                Image image = Image.FromStream(ms);
                return image;
            }
            catch
            {
                return null;
            }
        }
    }
}
