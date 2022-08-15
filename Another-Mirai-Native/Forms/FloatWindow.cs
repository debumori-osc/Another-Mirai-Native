using Another_Mirai_Native.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Another_Mirai_Native.Forms
{
    public partial class FloatWindow : Form
    {
        public FloatWindow()
        {
            InitializeComponent();
        }
        #region 拖动无窗体的控件
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        #endregion
        /// <summary>
        /// 圆形图片框, 来自CSDN
        /// </summary>
        public class RoundPictureBox : PictureBox
        {
            protected override void OnCreateControl()
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(this.ClientRectangle);
                Region region = new Region(gp);
                this.Region = region;
                base.OnCreateControl();
            }
        }

        private static string DefaultIcon = "/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAMCAgMCAgMDAwMEAwMEBQgFBQQEBQoHBwYIDAoMDAsKCwsNDhIQDQ4RDgsLEBYQERMUFRUVDA8XGBYUGBIUFRT/2wBDAQMEBAUEBQkFBQkUDQsNFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBT/wAARCAA4AEMDASIAAhEBAxEB/8QAGwAAAwEBAQEBAAAAAAAAAAAAAAcIBgUDAQn/xAA+EAAABAQDBAYHBAsAAAAAAAABAwQFAAIGEQcTFAghIzESFUNRU3MWIjNBYWPwFzSBoSQyRHGRlKOxs8Hj/8QAGgEAAgMBAQAAAAAAAAAAAAAAAAYBBAUHAv/EACYRAAIBAwQBAwUAAAAAAAAAAAABAwQFEQITITEUBiIjMjNBQ1L/2gAMAwEAAhEDEQA/AP1QgHlBcO+MnWlcttFsaxydJjQKLAeESXmGm28IvtIANKM9xGw/nAE4994SDBtMU1WxSn0ZROK9am9qlUpdJpvME3lHaDFJ9CQOlTcg/uXf8oxpLpTRSbcmsuaKWSRcIaQF333j2llCULBCQctqqkGF7lY10jsLjyNKRoDVWm83LvaHMlVyLUsikmYDiTJOmWYHLfF6CaKZZieSo4nG+Sfseqzrmmms01ApaqZazJdKWuWXUr9Tm8LSkle18qNXs74eVTQlGTem1VONTVE5GapSKoy5ST5RXy49toDDaocR6Rbi6UeE7HUDQ5pnVCerKzSc0oeRoRkV2P1e0q0rlTzhC5ZSP7ysKe2wlMIeKXmqr5cWyChQkG3dBCJZtrWmVLWmNV0vXyBRPJedN6EOg5Y919LBAA8REcsYS9UrvtBTlT9dAgbpfX0BfFNM83LH+lDXqFZomZco8Mq8SjSiYRkL9f6GEX1Fd/AjjjX7Bis9u8xSSfwbF0dDKPqmm0rVTRz2ldjRSqV5BWn6tKL8XM7PiwxJAL04mhy+G+M1TwijKAoRHl3xpUywZAt7oQnUxyvLNXXFtI4znRzGULoZJIjs5m5qkC0QG5nCyo4mGGJb7PU7nTbpTbinaUSnJRPStSWGqKzcoqxXu+vLjZO5MrunyjJzAD5e4YXjqHUTmgMTqDJpylBXu5Rv0NxcMi23wU9VNvRlMSetfd6v94n9HRRWImOdS+maLUk0+pTG06lNHhAVpSs0y3mw97nCllGT2tolTFOjMd6jdU9SNTPSjC7Ik2WV1fUio3Vd5RpWgsaX8I6unlZFVlZaYvuGCJMQbeLO2oyktS0JWyCoCg6C9M002qVJSzg/XAs3K9cL++CJIKadEuublJIBcDCuQxHoFLmA04vQ6p0aOEak7UxN8r5sWlJJ0AALfCJF2yyxok+m6qZJVM1VObmlYUreBvBXZpna/DKA3+PmQpXq0q6RZX1o3rTW+K8Sfk69G18hfUma3rs7objSwGxpXwNK7KNskfQEsN1/xhKP6duJXj6VMZjC8mhcxwSmmJjf5oqOIU0U0l4oVnVBpXhnVKqyo5s6B0vxyDnq1Qzr2jqqbE9Aw5SeYzrB2MD9Fakv3oyMzRzcpqapULYoMAxfOq61XZPYh4Y/4vo2FjU1Ul0HSy93pilBNzTSkqlUA7zOLws003imxWmDGFgYesMkzipBe/KrGqlmX74ZLLaZdcm7J9swq2pjptG3G+zyx5xbadnvC17rx+RrF7O0TJswtvkLMUhmnFpgHiDYd5ofnDalAJQsG4InbbXpYyuNnKp6dnbnlxlWHIM1JT6EVys0C1yY2xRWaVfl9Wiio6iuhMfeQ6EEEEGSD5YLcomXaFcErjtM7OrAsATSRdHRfl9+U1qv9wQR7AotexonYsZFacs78Iz8mE9KlGibIxpgMHvCCCKjSfaJTa6F7tYN6Zt2eal05BZcshqAbcv28mHan6IlSDYL2ggizjgG89nrBBBHggIIIIAP/9k=";
        private void FloatWindow_Load(object sender, EventArgs e)
        {
            if (File.Exists(LogHelper.GetLogFilePath()) is false)
            {
                LogHelper.CreateDB();
            }
            //设置窗口透明色, 实现窗口背景透明
            this.TransparencyKey = Color.Gray;
            this.BackColor = Color.Gray;
            //读取配置
            string[] position = ConfigHelper.GetConfig<string>("FloatWindow_Location").Split(',');
            if (position.Length == 2)
            {
                Left = int.Parse(position[0]);
                Top = int.Parse(position[1]);
            }
            if(ConfigHelper.ConfigHasKey("FloatWindow_Visible"))
                Visible = ConfigHelper.GetConfig<bool>("FloatWindow_Visible");
            TopMost = ConfigHelper.GetConfig<bool>("FloatWindow_TopMost");
            //日志窗口初始化
            LogForm logForm = new();
            logForm.Text = $"运行日志 - {Helper.QQ}";
            logForm.Show();
            logForm.Visible = false;
            //初始化托盘
            //NotifyIconHelper._NotifyIcon = notifyIcon;
            //NotifyIconHelper.Init();
            //NotifyIconHelper.ShowNotifyIcon();
            //载入插件
            //pluginManagment = new PluginManagment();
            //Thread thread = new Thread(() =>
            //{
            //    pluginManagment.Init();
            //});
            //thread.Start();
            //将托盘右键菜单复制一份
            //pictureBox_Main.ContextMenu = notifyIcon.ContextMenu;
            //默认头像,防止网络问题造成空头像出现
            Image image = Helper.Base642Image(DefaultIcon);
            try
            {
                var streamTask = Helper.Get($"http://q1.qlogo.cn/g?b=qq&nk={Helper.QQ}&s=640");
                streamTask.Wait();
                if (streamTask.Result.Length > 0)//下载成功
                    image = Image.FromStream(streamTask.Result);
            }
            catch
            {
                //网络异常,图片使用默认头像
                LogHelper.WriteLog("下载头像超时，重新启动程序可能解决这个问题");
            }
            RoundPictureBox RoundpictureBox = new()
            {
                Size = new Size(50, 50),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Left = -1,
                Top = 0,
                ContextMenu = notifyIcon.ContextMenu
            };
            if (image != null)
                RoundpictureBox.Image = image;
            //添加拖动事件
            RoundpictureBox.MouseDown += RoundpictureBox_MouseDown;
            this.LocationChanged += FloatWindow_LocationChanged;
            //显示控件, 置顶
            this.Controls.Add(RoundpictureBox);
            RoundpictureBox.BringToFront();
        }
        //防抖
        Timer Debounce = null;
        private void FloatWindow_LocationChanged(object sender, EventArgs e)
        {
            Debounce = new();
            Debounce.Interval = 500;
            Debounce.Tick += (a, b) =>
            {
                ConfigHelper.SetConfig("FloatWindow_Location", this.Location.X + "," + this.Location.Y);
            };
            Debounce.Start();
        }

        private void RoundpictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
            }
        }
    }
}
