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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ScrollPanel = new System.Windows.Forms.Panel();
            this.ChatPanel = new System.Windows.Forms.Panel();
            this.PluginName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.MsgToSend = new System.Windows.Forms.TextBox();
            this.SendMsg = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.GroupID = new System.Windows.Forms.TextBox();
            this.QQID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.isPrivateMsg = new System.Windows.Forms.CheckBox();
            this.ShowHandleMsg = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.ScrollPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ScrollPanel);
            this.groupBox1.Controls.Add(this.PluginName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(776, 386);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "插件对话窗口";
            // 
            // ScrollPanel
            // 
            this.ScrollPanel.AutoScroll = true;
            this.ScrollPanel.Controls.Add(this.ChatPanel);
            this.ScrollPanel.Location = new System.Drawing.Point(8, 44);
            this.ScrollPanel.Name = "ScrollPanel";
            this.ScrollPanel.Size = new System.Drawing.Size(762, 336);
            this.ScrollPanel.TabIndex = 3;
            // 
            // ChatPanel
            // 
            this.ChatPanel.Location = new System.Drawing.Point(16, 12);
            this.ChatPanel.Name = "ChatPanel";
            this.ChatPanel.Size = new System.Drawing.Size(726, 321);
            this.ChatPanel.TabIndex = 0;
            // 
            // PluginName
            // 
            this.PluginName.AutoSize = true;
            this.PluginName.Location = new System.Drawing.Point(103, 29);
            this.PluginName.Name = "PluginName";
            this.PluginName.Size = new System.Drawing.Size(53, 12);
            this.PluginName.TabIndex = 2;
            this.PluginName.Text = "插件名称";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "目前测试的插件：";
            // 
            // MsgToSend
            // 
            this.MsgToSend.Location = new System.Drawing.Point(18, 442);
            this.MsgToSend.Multiline = true;
            this.MsgToSend.Name = "MsgToSend";
            this.MsgToSend.Size = new System.Drawing.Size(683, 21);
            this.MsgToSend.TabIndex = 2;
            this.MsgToSend.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MsgToSend_KeyUp);
            // 
            // SendMsg
            // 
            this.SendMsg.Location = new System.Drawing.Point(707, 443);
            this.SendMsg.Name = "SendMsg";
            this.SendMsg.Size = new System.Drawing.Size(75, 23);
            this.SendMsg.TabIndex = 3;
            this.SendMsg.Text = "发送";
            this.SendMsg.UseVisualStyleBackColor = true;
            this.SendMsg.Click += new System.EventHandler(this.SendMsg_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 414);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "消息来源的群号";
            // 
            // GroupID
            // 
            this.GroupID.Location = new System.Drawing.Point(113, 411);
            this.GroupID.Name = "GroupID";
            this.GroupID.Size = new System.Drawing.Size(177, 21);
            this.GroupID.TabIndex = 5;
            this.GroupID.Text = "0";
            // 
            // QQID
            // 
            this.QQID.Location = new System.Drawing.Point(417, 412);
            this.QQID.Name = "QQID";
            this.QQID.Size = new System.Drawing.Size(177, 21);
            this.QQID.TabIndex = 7;
            this.QQID.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(311, 415);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "消息来源的QQ号";
            // 
            // isPrivateMsg
            // 
            this.isPrivateMsg.AutoSize = true;
            this.isPrivateMsg.Location = new System.Drawing.Point(601, 414);
            this.isPrivateMsg.Name = "isPrivateMsg";
            this.isPrivateMsg.Size = new System.Drawing.Size(48, 16);
            this.isPrivateMsg.TabIndex = 8;
            this.isPrivateMsg.Text = "私聊";
            this.isPrivateMsg.UseVisualStyleBackColor = true;
            // 
            // ShowHandleMsg
            // 
            this.ShowHandleMsg.AutoSize = true;
            this.ShowHandleMsg.Checked = true;
            this.ShowHandleMsg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowHandleMsg.Location = new System.Drawing.Point(655, 414);
            this.ShowHandleMsg.Name = "ShowHandleMsg";
            this.ShowHandleMsg.Size = new System.Drawing.Size(120, 16);
            this.ShowHandleMsg.TabIndex = 9;
            this.ShowHandleMsg.Text = "显示插件处理结果";
            this.ShowHandleMsg.UseVisualStyleBackColor = true;
            // 
            // PluginTester
            // 
            this.AcceptButton = this.SendMsg;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 478);
            this.Controls.Add(this.ShowHandleMsg);
            this.Controls.Add(this.isPrivateMsg);
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PluginTester_FormClosing);
            this.Load += new System.EventHandler(this.PluginTester_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ScrollPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label PluginName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox MsgToSend;
        private System.Windows.Forms.Button SendMsg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox GroupID;
        private System.Windows.Forms.TextBox QQID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel ScrollPanel;
        private System.Windows.Forms.Panel ChatPanel;
        private System.Windows.Forms.CheckBox isPrivateMsg;
        private System.Windows.Forms.CheckBox ShowHandleMsg;
    }
}