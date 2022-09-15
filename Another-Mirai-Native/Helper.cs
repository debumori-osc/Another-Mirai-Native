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
using System.Windows.Forms;

namespace Another_Mirai_Native
{
    /// <summary>
    /// 公用静态函数集
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// 框架登录中的QQ号
        /// </summary>
        public static string QQ { get; set; }
        /// <summary>
        /// 框架登录中的昵称
        /// </summary>
        public static string NickName { get; set; }
        /// <summary>
        /// 连接MHA的URL
        /// </summary>
        public static string WsURL { get; set; }
        /// <summary>
        /// 连接MHA的SecretKey
        /// </summary>
        public static string WsAuthKey { get; set; }
        /// <summary>
        /// 最大获取或显示日志数量
        /// </summary>
        // TODO: 设置为配置
        public static int MaxLogCount { get; set; } = 500;
        public static List<DateTime> MsgSpeed { get; set; } = new();
        public static DateTime StartUpTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 当前时间戳
        /// </summary>
        public static long TimeStamp => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        /// <summary>
        /// 时间戳转换为DateTime
        /// </summary>
        public static DateTime TimeStamp2DateTime(long timestamp) => new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local).AddSeconds(timestamp);
        public static long DateTime2TimeStamp(DateTime datetime) => (long)(datetime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        /// <summary>
        /// JToken中是否含有某个键
        /// </summary>
        /// <param name="json"></param>
        /// <param name="key">需要判断的键名</param>
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
        public static async Task<Stream> GetData(string url)
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
        public static async Task<string> GetString(string url)
        {
            try
            {
                using var http = new HttpClient();
                var r = http.GetAsync(url);
                return await r.Result.Content.ReadAsStringAsync();
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
        /// <summary>
        /// Base64转图片
        /// </summary>
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
        /// <summary>
        /// 从文本转换为枚举对象
        /// </summary>
        /// <typeparam name="T">待转换枚举</typeparam>
        /// <param name="value">待转换文本</param>
        /// <returns>枚举对象</returns>
        public static T String2Enum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
        /// <summary>
        /// 对象转Json文本
        /// </summary>
        public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj);
        /// <summary>
        /// 从cqimg中获取图片URL
        /// </summary>
        /// <param name="cqimg"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 图片转Base64
        /// </summary>
        /// <param name="picPath">图片路径</param>
        public static string ParsePic2Base64(string picPath)
        {
            if (File.Exists(picPath) is false)
            {
                return "";
            }
            var buffer = File.ReadAllBytes(picPath);
            return Convert.ToBase64String(buffer);
        }
        /// <summary>
        /// 重启应用
        /// </summary>
        public static void RestartApplication()
        {
            string path = typeof(Login).Assembly.Location;
            Process.Start(path, $"-r");
            NotifyIconHelper.HideNotifyIcon();
            Environment.Exit(0);
        }
        public static string[] Split(this string message, string pattern)
        {
            List<string> p = new();// 记录下文本与CQ码的位置关系
            string tmp = "";
            for (int i = 0; i < message.Length; i++)// 将消息中的CQ码与文本分离开
            {
                tmp += message[i];// 文本
                if (tmp == pattern)// 此消息中没有其他文本, 只有CQ码
                {
                    p.Add(pattern);
                    tmp = "";
                }
                else if (tmp.EndsWith(pattern))// 消息以CQ码结尾
                {
                    p.Add(tmp[..^10]);// 记录文本位置
                    p.Add(pattern);// 记录CQ码位置
                    tmp = "";
                }
            }
            if (tmp != "")// 文本中没有CQ码, 或不以CQ码结尾
                p.Add(tmp);
            return p.ToArray();
        }
    }
}
