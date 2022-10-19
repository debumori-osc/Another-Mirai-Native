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
        private static readonly Encoding GB18030 = Encoding.GetEncoding("GB18030");
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
