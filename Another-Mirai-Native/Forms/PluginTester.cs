using Another_Mirai_Native.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Another_Mirai_Native.Adapter.CQCode.Model;
using System.Diagnostics;
using System.IO;

namespace Another_Mirai_Native.Forms
{
    public partial class PluginTester : Form
    {
        public PluginTester()
        {
            InitializeComponent();
            Instance = this;
        }
        public static PluginTester Instance { get; set; }
        public CQPlugin TestingPlugin { get; set; }
        private void PluginTester_Load(object sender, EventArgs e)
        {
            if(TestingPlugin == null)
            {
                MessageBox.Show("插件信息无效");
                Close();
                return;
            }
            PluginName.Text = TestingPlugin.appinfo.Name;
            long groupId = ConfigHelper.GetConfig<long>("Tester_GroupID");
            long QQId = ConfigHelper.GetConfig<long>("Tester_QQID");
            Random rd = new();
            if (groupId == 0) groupId = rd.Next(100000, 1000000000);
            if (QQId == 0) QQId = rd.Next(100000, 1000000000);

            GroupID.Text = groupId.ToString();
            QQID.Text = QQId.ToString();

            PluginTestHelper.Instance.EnableTest(TestingPlugin);
            PluginTestHelper.Instance.OnPluginSendMsg -= PluginTestHelper_OnPluginSendMsg;
            PluginTestHelper.Instance.OnPluginChanged -= Instance_OnPluginChanged;
            PluginTestHelper.Instance.OnPluginSendMsg += PluginTestHelper_OnPluginSendMsg;
            PluginTestHelper.Instance.OnPluginChanged += Instance_OnPluginChanged;
        }

        private void Instance_OnPluginChanged(CQPlugin plugin)
        {
            TestingPlugin = plugin;
            PluginName.Text = TestingPlugin.appinfo.Name;
            MessageBox.Show($"测试插件已变更，新的测试插件为: {plugin.appinfo.Name}");
        }

        private void PluginTestHelper_OnPluginSendMsg(string msg)
        {
            AddChatBlock(msg, true);
        }

        public class ChatBox : PictureBox
        {
            int padding = 5;
            protected override void OnCreateControl()
            {
                Font font = new("微软雅黑", 10);
                string text = Tag.ToString().Replace("\r\n", "\n");
                if (text.Length > 30)
                {
                    var lines = text.Split('\n');
                    text = "";
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Length > 30)
                        {
                            for (int j = 30; j < lines[i].Length; j+=30)
                            {
                                lines[i] = lines[i].Insert(j - 1, "\n");                                
                            }
                        }
                        text += lines[i] + "\n";
                    }
                }
                Label textLabel = new()
                {
                    AutoSize = true,
                    Text = text,
                    Location = new(padding, padding),
                    Font = font,
                };
                Controls.Add(textLabel);
                var path = DrawRoundRect(textLabel.Width + padding * 2, textLabel.Height + padding * 2, 5);
                Region = new(path);
                Size = new Size(textLabel.Width + padding * 2, textLabel.Height + padding * 2);
                ContextMenu = new ContextMenu();
                ContextMenu.MenuItems.Add(new MenuItem() { Text = "复制" });
                ContextMenu.MenuItems.Add(new MenuItem() { Text = "+1" });
                ContextMenu.MenuItems.Add(new MenuItem() { Text = "-" });
                ContextMenu.MenuItems.Add(new MenuItem() { Text = "读取图片" });
                ContextMenu.MenuItems[0].Click += (a, b) => Clipboard.SetText(Tag.ToString());
                ContextMenu.MenuItems[1].Click += (a, b) => { Instance.MsgToSend.Text = Tag.ToString(); Instance.SendMsg.PerformClick(); };
                ContextMenu.MenuItems[3].Click += (a, b) => 
                {
                    string img = Tag.ToString();
                    var c = CQCodeModel.Parse(img);
                    if (c.Any(x => x.IsImageCQCode))
                    {
                        var img_CQCode = c.First(x => x.IsImageCQCode);
                        string img_file = img_CQCode.Items["file"];
                        string img_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", img_file);
                        if (File.Exists(img_path))
                        {
                            Process.Start(img_path);
                        }
                        else
                        {
                            string picTmp = File.ReadAllText(img_path + ".cqimg");
                            picTmp = picTmp.Split('\n').Last().Replace("url=", "");
                            Process.Start(picTmp);
                        }
                    }
                };
                base.OnCreateControl();
            }
        }
        Color plugin = Color.White;
        Color client = Color.FromArgb(149, 236, 105);
        public static GraphicsPath DrawRoundRect(int width, int height, int borderRadius)
        {
            GraphicsPath path = new();
            //left top
            path.AddArc(new Rectangle(new Point(0, 0), new Size(borderRadius * 2, borderRadius * 2)), 180, 90);
            path.AddLine(new Point(borderRadius - 1, 0), new Point(width - borderRadius + 1, 0));

            //right top
            path.AddArc(new Rectangle(new Point(width - borderRadius * 2, 0), new Size(borderRadius * 2, borderRadius * 2)), 270, 90);
            path.AddLine(new Point(width, borderRadius - 1), new Point(width, height - borderRadius + 1));

            //right bottom
            path.AddArc(new Rectangle(new Point(width - borderRadius * 2, height - borderRadius * 2), new Size(borderRadius * 2, borderRadius * 2)), 0, 90);
            path.AddLine(new Point(borderRadius - 1, height), new Point(width - borderRadius + 1, height));

            //left bottom
            path.AddArc(new Rectangle(new Point(0, height - borderRadius * 2), new Size(borderRadius * 2, borderRadius * 2)), 90, 90);
            path.AddLine(new Point(0, borderRadius - 1), new Point(0, height - borderRadius + 1));

            return path;
        }

        int lastY = 0;
        static Encoding GB18030 = Encoding.GetEncoding("GB18030");
        private void SendMsg_Click(object sender, EventArgs e)
        {
            string text = MsgToSend.Text;
            AddChatBlock(text, false);
            MsgToSend.Text = "";
            long groupId = Convert.ToInt64(GroupID.Text);
            long QQId = Convert.ToInt64(QQID.Text);

            new Thread(() =>
            {
                bool result;
                if(PluginTestHelper.Instance.CheckPlugin() is false)
                {
                    PluginTestHelper.Instance.EnableTest(TestingPlugin);
                }
                if (isPrivateMsg.Checked)
                {
                    result = PluginTestHelper.Instance.SendPrivateMsg(text, QQId);
                }
                else
                {
                    result = PluginTestHelper.Instance.SendGroupMsg(text, groupId, QQId);
                }
                Instance.Invoke(() =>
                {
                    if (ShowHandleMsg.Checked == false) return;
                    string msg = "插件放行了请求";
                    if (result)
                        msg = "插件结束了请求";
                    AddChatBlock(msg, true);
                });
            }).Start();
        }
        List<string> msgSave { get; set; } = new();
        int msgIndex = 0;
        public void AddChatBlock(string text, bool isPlugin)
        {
            Instance.Invoke(() => 
            {
                if (!isPlugin) msgSave.Add(text);
                msgIndex = msgSave.Count;
                var c = new ChatBox
                {
                    Tag = text,
                    BackColor = isPlugin ? plugin : client
                };
                ChatPanel.Controls.Add(c);
                c.Location = new Point(isPlugin ? 0 : ChatPanel.Width - c.Size.Width, lastY);
                lastY += c.Size.Height + 5;

                if (lastY > ChatPanel.Size.Height)
                {
                    ChatPanel.Size = new Size(ChatPanel.Size.Width, lastY + 10);
                }
                ScrollPanel.ScrollControlIntoView(c);
            });           
        }

        private void MsgToSend_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (msgIndex > 0)
                {
                    msgIndex--;
                    MsgToSend.Text = msgSave[msgIndex];
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (msgIndex < msgSave.Count)
                {
                    msgIndex++;
                    if(msgIndex == msgSave.Count)
                    {
                        MsgToSend.Text = "";
                    }
                    else
                    {
                        MsgToSend.Text = msgSave[msgIndex];
                    }
                }
            }
        }

        private void PluginTester_FormClosing(object sender, FormClosingEventArgs e)
        {
            TestingPlugin.Testing = false;
        }
    }
}
