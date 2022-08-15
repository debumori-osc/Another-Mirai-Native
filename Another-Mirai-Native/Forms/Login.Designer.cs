namespace Another_Mirai_Native
{
    partial class Login
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.WSUrl = new System.Windows.Forms.TextBox();
            this.AutoLoginCheck = new System.Windows.Forms.CheckBox();
            this.LoginBtn = new System.Windows.Forms.Button();
            this.AuthKeyText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.QQText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "ws:";
            // 
            // WSUrl
            // 
            this.WSUrl.Location = new System.Drawing.Point(86, 41);
            this.WSUrl.Name = "WSUrl";
            this.WSUrl.Size = new System.Drawing.Size(178, 23);
            this.WSUrl.TabIndex = 1;
            // 
            // AutoLoginCheck
            // 
            this.AutoLoginCheck.AutoSize = true;
            this.AutoLoginCheck.Location = new System.Drawing.Point(26, 101);
            this.AutoLoginCheck.Name = "AutoLoginCheck";
            this.AutoLoginCheck.Size = new System.Drawing.Size(75, 21);
            this.AutoLoginCheck.TabIndex = 3;
            this.AutoLoginCheck.Text = "自动连接";
            this.AutoLoginCheck.UseVisualStyleBackColor = true;
            // 
            // LoginBtn
            // 
            this.LoginBtn.Location = new System.Drawing.Point(189, 99);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(75, 23);
            this.LoginBtn.TabIndex = 4;
            this.LoginBtn.Text = "连接";
            this.LoginBtn.UseVisualStyleBackColor = true;
            this.LoginBtn.Click += new System.EventHandler(this.LoginBtn_Click);
            // 
            // AuthKeyText
            // 
            this.AuthKeyText.Location = new System.Drawing.Point(86, 70);
            this.AuthKeyText.Name = "AuthKeyText";
            this.AuthKeyText.Size = new System.Drawing.Size(178, 23);
            this.AuthKeyText.TabIndex = 2;
            this.AuthKeyText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AuthKeyText_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "AuthKey:";
            // 
            // QQText
            // 
            this.QQText.Location = new System.Drawing.Point(86, 12);
            this.QQText.Name = "QQText";
            this.QQText.Size = new System.Drawing.Size(178, 23);
            this.QQText.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "QQ:";
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 134);
            this.Controls.Add(this.QQText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AuthKeyText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.LoginBtn);
            this.Controls.Add(this.AutoLoginCheck);
            this.Controls.Add(this.WSUrl);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "连接配置";
            this.Load += new System.EventHandler(this.Login_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox WSUrl;
        private CheckBox AutoLoginCheck;
        private Button LoginBtn;
        private TextBox AuthKeyText;
        private Label label2;
        private TextBox QQText;
        private Label label3;
    }
}