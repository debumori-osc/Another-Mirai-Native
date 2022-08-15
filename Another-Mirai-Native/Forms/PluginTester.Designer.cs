namespace Another_Mirai_Native.Forms
{
    partial class PluginTester
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ChatTextBox = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.PluginName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.MsgToSend = new System.Windows.Forms.TextBox();
            this.SendMsg = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.GroupID = new System.Windows.Forms.TextBox();
            this.QQID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ChatTextBox
            // 
            this.ChatTextBox.Location = new System.Drawing.Point(6, 55);
            this.ChatTextBox.Name = "ChatTextBox";
            this.ChatTextBox.ReadOnly = true;
            this.ChatTextBox.Size = new System.Drawing.Size(764, 325);
            this.ChatTextBox.TabIndex = 0;
            this.ChatTextBox.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.PluginName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.ChatTextBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 386);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "插件对话窗口";
            // 
            // PluginName
            // 
            this.PluginName.AutoSize = true;
            this.PluginName.Location = new System.Drawing.Point(103, 29);
            this.PluginName.Name = "PluginName";
            this.PluginName.Size = new System.Drawing.Size(56, 17);
            this.PluginName.TabIndex = 2;
            this.PluginName.Text = "插件名称";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "目前测试的插件：";
            // 
            // MsgToSend
            // 
            this.MsgToSend.Location = new System.Drawing.Point(18, 442);
            this.MsgToSend.Name = "MsgToSend";
            this.MsgToSend.Size = new System.Drawing.Size(683, 23);
            this.MsgToSend.TabIndex = 2;
            // 
            // SendMsg
            // 
            this.SendMsg.Location = new System.Drawing.Point(707, 443);
            this.SendMsg.Name = "SendMsg";
            this.SendMsg.Size = new System.Drawing.Size(75, 23);
            this.SendMsg.TabIndex = 3;
            this.SendMsg.Text = "发送";
            this.SendMsg.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 414);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "消息来源的群号";
            // 
            // GroupID
            // 
            this.GroupID.Location = new System.Drawing.Point(113, 411);
            this.GroupID.Name = "GroupID";
            this.GroupID.Size = new System.Drawing.Size(177, 23);
            this.GroupID.TabIndex = 5;
            this.GroupID.Text = "0";
            // 
            // QQID
            // 
            this.QQID.Location = new System.Drawing.Point(417, 412);
            this.QQID.Name = "QQID";
            this.QQID.Size = new System.Drawing.Size(177, 23);
            this.QQID.TabIndex = 7;
            this.QQID.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(311, 415);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "消息来源的QQ号";
            // 
            // PluginTester
            // 
            this.AcceptButton = this.SendMsg;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 478);
            this.Controls.Add(this.QQID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.GroupID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SendMsg);
            this.Controls.Add(this.MsgToSend);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "PluginTester";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "插件事件测试";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox ChatTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label PluginName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox MsgToSend;
        private System.Windows.Forms.Button SendMsg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox GroupID;
        private System.Windows.Forms.TextBox QQID;
        private System.Windows.Forms.Label label3;
    }
}