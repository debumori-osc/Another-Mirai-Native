using Another_Mirai_Native.Adapter;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Forms;
using Another_Mirai_Native.Native;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Another_Mirai_Native
{
    public partial class Login : Form
    {
        public static Login Instance { get; set; }
        public static IntPtr Instance_Handle { get; set; }
        public Login()
        {
            InitializeComponent();
            Instance = this;
        }
        private static MiraiAdapter adapter { get; set; }
        private void LoginBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(WSUrl.Text) || string.IsNullOrWhiteSpace(AuthKeyText.Text)
                || string.IsNullOrWhiteSpace(QQText.Text))
            {
                MessageBox.Show("请完成所有字段");
                return;
            }
            try
            {
                LoginBtn.Enabled = false;
                ConfigHelper.SetConfig("AutoLogin", AutoLoginCheck.Checked);
                ConfigHelper.SetConfig("QQ", QQText.Text);
                ConfigHelper.SetConfig("Ws_Url", WSUrl.Text);
                ConfigHelper.SetConfig("Ws_AuthKey", AuthKeyText.Text);
                adapter = new(WSUrl.Text, QQText.Text, AuthKeyText.Text);
                adapter.ConnectedStateChanged += Adapter_ConnectedStateChanged;
                adapter.Connect();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                LoginBtn.Enabled = true;
            }
        }

        private void Adapter_ConnectedStateChanged(bool status, string msg)
        {
            LoginBtn.BeginInvoke(new MethodInvoker(() =>
            {
                if (status)
                {
                    Hide();
                    new FloatWindow().Show();
                    //不直接给Visable赋值是因为外部调用Show函数会覆盖对Visable的赋值
                    //所以在调用Show之后需要用配置恢复对Visable的变化值
                    if (ConfigHelper.ConfigHasKey("FloatWindow_Visible"))
                        FloatWindow.Instance.Visible = ConfigHelper.GetConfig<bool>("FloatWindow_Visible");
                }
                else
                {
                    MessageBox.Show(msg);
                }
                LoginBtn.Enabled = true;
            }));
        }

        private void Login_Load(object sender, EventArgs e)
        {
            AutoLoginCheck.Checked = ConfigHelper.GetConfig<bool>("AutoLogin");
            QQText.Text = ConfigHelper.GetConfig<string>("QQ");
            WSUrl.Text = ConfigHelper.GetConfig<string>("Ws_Url");
            AuthKeyText.Text = ConfigHelper.GetConfig<string>("Ws_AuthKey");
            Helper.QQ = QQText.Text;
            Helper.WsURL = WSUrl.Text;
            Helper.WsAuthKey = AuthKeyText.Text;
            Helper.MaxLogCount = ConfigHelper.GetConfig<int>("MaxLogCount");
            if(Helper.MaxLogCount == 0)
            {
                Helper.MaxLogCount = 500;
                ConfigHelper.SetConfig("MaxLogCount", 500);
            }
            if (AutoLoginCheck.Checked)
            {
                LoginBtn.PerformClick();
            }
            int wsServerPort = ConfigHelper.GetConfig<int>("Ws_ServerPort");
            if(wsServerPort == 0)
            {
                wsServerPort = 30303;
                ConfigHelper.SetConfig("Ws_ServerPort", 30303);
            }
            try
            {
                WsServer.Init(wsServerPort);
            }
            catch
            {
                MessageBox.Show($"WebSocket服务器端口({wsServerPort})被占用，请更改为其他端口");
                Environment.Exit(0);
            }
            Directory.CreateDirectory("conf");
            Directory.CreateDirectory("logs");
            Directory.CreateDirectory("data"); 
            Directory.CreateDirectory(@"data/app");
            Directory.CreateDirectory(@"data/plugins");
            Directory.CreateDirectory(@"data/plugins/tmp");
            Directory.CreateDirectory(@"data/image");
            Directory.CreateDirectory(@"data/record");
           
            Instance_Handle = this.Handle;
        }

        private void AuthKeyText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoginBtn.PerformClick();
        }
    }
}