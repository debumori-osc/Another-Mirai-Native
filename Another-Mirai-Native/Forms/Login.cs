using Another_Mirai_Native.Forms;
using Another_Mirai_Native.Native;
using System.Diagnostics;

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
                MessageBox.Show("ÇëÊäÈëËùÓÐ×Ö¶Î");
                return;
            }
            try
            {
                LoginBtn.Enabled = false;
                ConfigHelper.WriteConfig("AutoLogin", AutoLoginCheck.Checked);
                ConfigHelper.WriteConfig("QQ", QQText.Text);
                ConfigHelper.WriteConfig("Ws_Url", WSUrl.Text);
                ConfigHelper.WriteConfig("Ws_AuthKey", AuthKeyText.Text);
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
            
                if (status)
                {
                    Hide();
                    new FloatWindow().Show();
                }
                else
                {
                    MessageBox.Show(msg);
                }
                LoginBtn.Invoke(() =>
            {LoginBtn.Enabled = true;
            });
        }

        private void Login_Load(object sender, EventArgs e)
        {
            AutoLoginCheck.Checked = ConfigHelper.GetConfig<bool>("AutoLogin");
            QQText.Text = ConfigHelper.GetConfig<string>("QQ");
            WSUrl.Text = ConfigHelper.GetConfig<string>("Ws_Url");
            AuthKeyText.Text = ConfigHelper.GetConfig<string>("Ws_AuthKey");
            if (AutoLoginCheck.Checked)
            {
                LoginBtn.PerformClick();
            }
            Instance_Handle = this.Handle;
            new Thread(() => Hide()).Start();
        }

        private void AuthKeyText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                LoginBtn.PerformClick();
        }
    }
}