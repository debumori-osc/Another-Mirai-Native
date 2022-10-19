using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CQP
{
    public static class Helper
    {
        public static string QQ { get; set; }
        public static string NickName { get; set; }
        public static string WsURL { get; set; }
        public static string WsAuthKey { get; set; }
        public static int MaxLogCount { get; set; } = 500;
        public static long TimeStamp => (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
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
        public static T String2Enum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }       
        public static T Int2Enum<T>(int value)
        {
            return (T)Enum.Parse(typeof(T), Enum.GetName(typeof(T), value));
        }
        public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj);
        static Encoding GB18030 = Encoding.GetEncoding("GB18030");
        /// <summary>
		/// 读取指针内所有的字节数组并编码为指定字符串
		/// </summary>
		/// <param name="strPtr">字符串的 <see cref="IntPtr"/> 对象</param>
		/// <param name="encoding">目标编码格式</param>
		/// <returns></returns>
		public static string ToString(this IntPtr strPtr, Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }

            int len = Kernel32.LstrlenA(strPtr);   //获取指针中数据的长度
            if (len == 0)
            {
                return string.Empty;
            }
            byte[] buffer = new byte[len];
            Marshal.Copy(strPtr, buffer, 0, len);
            return encoding.GetString(buffer);
        }
        public static IntPtr ToNative(this JToken json) => Marshal.UnsafeAddrOfPinnedArrayElement(Encoding.Convert(Encoding.Unicode, GB18030, Encoding.Unicode.GetBytes(json.ToString())), 0);
        public static IntPtr ToNative(this string text) => Marshal.UnsafeAddrOfPinnedArrayElement(Encoding.Convert(Encoding.Unicode, GB18030, Encoding.Unicode.GetBytes(text)), 0);
    }
}
