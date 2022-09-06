using Another_Mirai_Native.Adapter.CQCode.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native
{
    public static class Helper
    {
        public static string QQ { get; set; }
        public static string NickName { get; set; }
        public static string WsURL { get; set; }
        public static string WsAuthKey { get; set; }
        public static int MaxLogCount { get; set; } = 500;
        public static long TimeStamp => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        public static DateTime TimeStamp2DateTime(long timestamp) => new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local).AddSeconds(timestamp);
        public static bool ContainsKey(this JToken json, string key)
        {
            try
            {
                return json[key] != null;
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
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="path">目标文件夹</param>
        /// <param name="overwrite">重复时是否覆写</param>
        /// <returns></returns>
        public static async Task<bool> DownloadFile(string url, string fileName, string path, bool overwrite = false)
        {
            using var http = new HttpClient();
            try
            {
                if (string.IsNullOrWhiteSpace(url)) return false;
                if (!overwrite && File.Exists(Path.Combine(path, fileName))) return true;
                var r = await http.GetAsync(url);
                byte[] buffer = await r.Content.ReadAsByteArrayAsync();
                Directory.CreateDirectory(path);
                File.WriteAllBytes(Path.Combine(path, fileName), buffer);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
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
        public static T String2Enum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
        public static T Int2Enum<T>(int value)
        {
            return (T)Enum.Parse(typeof(T), Enum.GetName(typeof(T), value));
        }
        public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj);
        public static string GetPicUrlFromCQImg(string cqimg)
        {
            string picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", cqimg);
            if (!File.Exists(picPath))
            {
                string picUrl = File.ReadAllText(picPath + ".cqimg");
                picUrl = picUrl.Split('\n').Last().Replace("url=", "");
                return picUrl;
            }
            return "";
        }
        /// <summary>
        /// 读取指针内所有的字节数组并编码为指定字符串
        /// </summary>
        /// <param name="strPtr">字符串的 <see cref="IntPtr"/> 对象</param>
        /// <param name="encoding">目标编码格式</param>
        /// <returns></returns>
        public static string ToString(this IntPtr strPtr, Encoding encoding = null)
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

        public static string ToString(this JToken json, Encoding encoding)
        {
            string c = json.ToString();
            var b = Encoding.UTF8.GetBytes(c);
            byte[] messageBytes = Encoding.Convert(Encoding.UTF8, encoding, b);
            var messageIntptr = Marshal.AllocHGlobal(messageBytes.Length + 1);
            Marshal.Copy(messageBytes, 0, messageIntptr, messageBytes.Length);
            return ToString(messageIntptr, encoding);
        }

        public static string ParsePic2Base64(string picPath)
        {
            if(File.Exists(picPath) is false)
            {
                return "";
            }
            var buffer = File.ReadAllBytes(picPath);
            return Convert.ToBase64String(buffer);
        }
        public static void RestartApplication()
        {
            string path = typeof(Login).Assembly.Location;
            Process.Start(path, $"-r");
            //NotifyIconHelper.HideNotifyIcon();
            Environment.Exit(0);
        }
    }
}
