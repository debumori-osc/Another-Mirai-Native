using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Another_Mirai_Native.Forms
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }
        public static LogForm Instance { get; set; }
        private LogLevel LogPriority { get; set; } = LogLevel.Info;
        private List<LogModel> LogLists { get; set; } = new();
        private bool AutoScroll { get; set; }

        private void LogForm_Load(object sender, EventArgs e)
        {
            Instance = this;
            LogHelper.LogAdded += LogHelper_LogAdded;
            LogHelper.LogStatusUpdated += LogHelper_LogStatusUpdated;
            comboBox_LogLevel.SelectedIndex = 1;
            LoadLogs();
            previewSize = Size;
        }
        private void LoadLogs()
        {
            listView_LogMain.Items.Clear();
            LogLists = LogHelper.GetDisplayLogs((int)LogPriority);
            foreach (var item in LogLists)
            {
                AddItem2ListView(item);
            }
        }
        private void AddItem2ListView(LogModel item)
        {
            ListViewItem listViewItem = new ListViewItem();
            listViewItem.SubItems[0].Text = LogHelper.GetTimeStampString(item.time);
            listViewItem.SubItems.Add(item.source);
            listViewItem.SubItems.Add(item.name);
            listViewItem.SubItems.Add(item.detail);
            listViewItem.SubItems.Add(item.status);
            listViewItem.ForeColor = GetLogColor(item.priority);//消息颜色
            listView_LogMain.Invoke(new MethodInvoker(() =>
            {
                listView_LogMain.Items.Add(listViewItem);
                if (checkBox_Update.Checked)//日志自动滚动
                {
                    listView_LogMain.EnsureVisible(listView_LogMain.Items.Count - 1);
                    listViewItem.Selected = true;
                }
            }));
        }
        private static Color GetLogColor(int value)
        {
            LogLevel loglevel = (LogLevel)value;
            return GetLogColor(loglevel);
        }
        /// <summary>
        /// 获取日志文本颜色
        /// </summary>
        /// <param name="level">日志等级</param>
        /// <returns></returns>
        private static Color GetLogColor(LogLevel level)
        {
            Color LogColor;
            switch (level)
            {
                case LogLevel.Debug:
                    LogColor = Color.Gray;
                    break;
                case LogLevel.Error:
                    LogColor = Color.Red;
                    break;
                case LogLevel.Info:
                    LogColor = Color.Black;
                    break;
                case LogLevel.Fatal:
                    LogColor = Color.DarkRed;
                    break;
                case LogLevel.InfoSuccess:
                    LogColor = Color.Magenta;
                    break;
                case LogLevel.InfoSend:
                    LogColor = Color.Green;
                    break;
                case LogLevel.InfoReceive:
                    LogColor = Color.Blue;
                    break;
                case LogLevel.Warning:
                    LogColor = Color.FromArgb(255, 165, 0);
                    break;
                default:
                    LogColor = Color.Black;
                    break;
            }
            return LogColor;
        }
        private LogLevel GetLogPriority(int selectIndex)
        {
            switch (selectIndex)
            {
                case 0:
                    return LogLevel.Debug;
                case 1:
                    return LogLevel.Info;
                case 2:
                    return LogLevel.Warning;
                case 3:
                    return LogLevel.Error;
                case 4:
                    return LogLevel.Fatal;
                default:
                    return LogLevel.Info;
            }
        }
        private void LogHelper_LogStatusUpdated(int logid, string status)
        {
            try
            {
                LogModel item = LogLists.Find(x => x.id == logid);
                item.status = status;
                listView_LogMain.Invoke(new MethodInvoker(() =>
                {
                    listView_LogMain.Items[LogLists.IndexOf(item)].SubItems[4].Text = status;
                }));
            }
            catch { }
        }

        private void LogHelper_LogAdded(int logid, LogModel log)
        {
            if (log == null || log.priority < (int)LogPriority)
                return;
            if (string.IsNullOrWhiteSpace(log.detail) && !string.IsNullOrWhiteSpace(log.name))
            {
                log.detail = log.name;
                log.name = "异常捕获";
            }
            LogLists.Add(log);
            AddItem2ListView(log);
            if (LogLists.Count >= Helper.MaxLogCount)
            {
                LogLists.RemoveAt(0);
                listView_LogMain.Invoke(new MethodInvoker(() => { listView_LogMain.Items.RemoveAt(0); }));
            }
            try
            {
                switch ((LogLevel)log.priority)
                {
                    case LogLevel.Warning:
                        NotifyIconHelper.Instance.ShowBalloonTip(2000, log.source, log.detail, ToolTipIcon.Warning);
                        break;
                    case LogLevel.Error:
                        NotifyIconHelper.Instance.ShowBalloonTip(2000, log.source, log.detail, ToolTipIcon.Error);
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        private void comboBox_LogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            var t = GetLogPriority((sender as ComboBox).SelectedIndex);
            if (t == LogPriority)
            {
                return;
            }
            else
            {
                LogPriority = t;
            }
            LoadLogs();
        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox_Update_CheckedChanged(object sender, EventArgs e)
        {
            AutoScroll = checkBox_Update.Checked;
            if (AutoScroll)
            {
                Thread thread = new(() =>
                {
                    label_Desc.Invoke(new MethodInvoker(() => { label_Desc.Visible = true; }));
                    listView_LogMain.EnsureVisible(listView_LogMain.Items.Count - 1);
                    listView_LogMain.Items[listView_LogMain.Items.Count - 1].Selected = true;
                    Thread.Sleep(2000);
                    label_Desc.Invoke(new MethodInvoker(() => { label_Desc.Visible = false; }));
                });
                thread.Start();
            }
        }

        private void checkBox_AboveAll_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = checkBox_AboveAll.Checked;
        }

        private void listView_LogMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (listView_LogMain.SelectedItems.Count != 0 && e.Button == MouseButtons.Right)
            {
                label_Desc.Text = "已复制日志内容";
                label_Desc.Visible = true;
                string text = listView_LogMain.SelectedItems[0].SubItems[3].Text;
                Clipboard.SetText(text);
                new Thread(() =>
                {
                    Thread.Sleep(2000);
                    label_Desc.Invoke(new MethodInvoker(() => { label_Desc.Text = "日志列表将实时滚动"; label_Desc.Visible = false; }));
                }).Start();
            }
        }

        private void listView_LogMain_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            ToolTip toolTip = new();
            string itemInfor = e.Item.SubItems[3].Text;
            toolTip.SetToolTip((e.Item).ListView, itemInfor);
        }
        Size previewSize;
        private void LogForm_SizeChanged(object sender, EventArgs e)
        {
            if (Size.Width < 815 || Size.Height < 259) return;
            int widthOffset = Size.Width - previewSize.Width, heightOffset = Size.Height - previewSize.Height;
            listView_LogMain.Size = new(listView_LogMain.Width + widthOffset, listView_LogMain.Height + heightOffset);
            leftPanel.Location = new(leftPanel.Left, leftPanel.Top + heightOffset);
            rightPanel.Location = new(rightPanel.Left + widthOffset, rightPanel.Top + heightOffset);
            previewSize = Size;
        }
    }
}
