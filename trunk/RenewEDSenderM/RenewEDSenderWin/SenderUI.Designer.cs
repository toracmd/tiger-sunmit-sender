namespace RenewEDSenderWin
{
    partial class SenderUI
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSenderStart = new System.Windows.Forms.Button();
            this.btnSenderStop = new System.Windows.Forms.Button();
            this.btnSenderRestart = new System.Windows.Forms.Button();
            this.groupBoxCtrl = new System.Windows.Forms.GroupBox();
            this.groupBoxKey = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.txtBoxMd5Key = new System.Windows.Forms.TextBox();
            this.txtBoxAesIV = new System.Windows.Forms.TextBox();
            this.txtBoxAesKey = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAesKey = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBoxIP = new System.Windows.Forms.TextBox();
            this.txtBoxPort = new System.Windows.Forms.TextBox();
            this.groupBoxCtrl.SuspendLayout();
            this.groupBoxKey.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSenderStart
            // 
            this.btnSenderStart.Location = new System.Drawing.Point(35, 17);
            this.btnSenderStart.Name = "btnSenderStart";
            this.btnSenderStart.Size = new System.Drawing.Size(121, 56);
            this.btnSenderStart.TabIndex = 0;
            this.btnSenderStart.Text = "启动发送服务";
            this.btnSenderStart.UseVisualStyleBackColor = true;
            this.btnSenderStart.Click += new System.EventHandler(this.btnSenderStart_Click);
            // 
            // btnSenderStop
            // 
            this.btnSenderStop.Location = new System.Drawing.Point(35, 116);
            this.btnSenderStop.Name = "btnSenderStop";
            this.btnSenderStop.Size = new System.Drawing.Size(121, 56);
            this.btnSenderStop.TabIndex = 1;
            this.btnSenderStop.Text = "停止发送服务";
            this.btnSenderStop.UseVisualStyleBackColor = true;
            this.btnSenderStop.Click += new System.EventHandler(this.btnSenderStop_Click);
            // 
            // btnSenderRestart
            // 
            this.btnSenderRestart.Location = new System.Drawing.Point(35, 231);
            this.btnSenderRestart.Name = "btnSenderRestart";
            this.btnSenderRestart.Size = new System.Drawing.Size(121, 56);
            this.btnSenderRestart.TabIndex = 2;
            this.btnSenderRestart.Text = "重启发送服务";
            this.btnSenderRestart.UseVisualStyleBackColor = true;
            this.btnSenderRestart.Click += new System.EventHandler(this.btnSenderRestart_Click);
            // 
            // groupBoxCtrl
            // 
            this.groupBoxCtrl.Controls.Add(this.btnSenderRestart);
            this.groupBoxCtrl.Controls.Add(this.btnSenderStop);
            this.groupBoxCtrl.Controls.Add(this.btnSenderStart);
            this.groupBoxCtrl.Location = new System.Drawing.Point(30, 13);
            this.groupBoxCtrl.Name = "groupBoxCtrl";
            this.groupBoxCtrl.Size = new System.Drawing.Size(212, 316);
            this.groupBoxCtrl.TabIndex = 3;
            this.groupBoxCtrl.TabStop = false;
            this.groupBoxCtrl.Text = "数据发送服务";
            // 
            // groupBoxKey
            // 
            this.groupBoxKey.Controls.Add(this.txtBoxPort);
            this.groupBoxKey.Controls.Add(this.txtBoxIP);
            this.groupBoxKey.Controls.Add(this.label4);
            this.groupBoxKey.Controls.Add(this.label3);
            this.groupBoxKey.Controls.Add(this.btnCancel);
            this.groupBoxKey.Controls.Add(this.btnUpdate);
            this.groupBoxKey.Controls.Add(this.txtBoxMd5Key);
            this.groupBoxKey.Controls.Add(this.txtBoxAesIV);
            this.groupBoxKey.Controls.Add(this.txtBoxAesKey);
            this.groupBoxKey.Controls.Add(this.label2);
            this.groupBoxKey.Controls.Add(this.label1);
            this.groupBoxKey.Controls.Add(this.lblAesKey);
            this.groupBoxKey.Location = new System.Drawing.Point(283, 13);
            this.groupBoxKey.Name = "groupBoxKey";
            this.groupBoxKey.Size = new System.Drawing.Size(321, 315);
            this.groupBoxKey.TabIndex = 4;
            this.groupBoxKey.TabStop = false;
            this.groupBoxKey.Text = "配置管理";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(163, 282);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "取消更新";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(82, 282);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 6;
            this.btnUpdate.Text = "更新配置";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // txtBoxMd5Key
            // 
            this.txtBoxMd5Key.Location = new System.Drawing.Point(104, 239);
            this.txtBoxMd5Key.Name = "txtBoxMd5Key";
            this.txtBoxMd5Key.Size = new System.Drawing.Size(182, 21);
            this.txtBoxMd5Key.TabIndex = 5;
            // 
            // txtBoxAesIV
            // 
            this.txtBoxAesIV.Location = new System.Drawing.Point(104, 208);
            this.txtBoxAesIV.Name = "txtBoxAesIV";
            this.txtBoxAesIV.Size = new System.Drawing.Size(182, 21);
            this.txtBoxAesIV.TabIndex = 4;
            // 
            // txtBoxAesKey
            // 
            this.txtBoxAesKey.Location = new System.Drawing.Point(104, 179);
            this.txtBoxAesKey.Name = "txtBoxAesKey";
            this.txtBoxAesKey.Size = new System.Drawing.Size(182, 21);
            this.txtBoxAesKey.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 242);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "MD5 Key:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 211);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "AES IV:";
            // 
            // lblAesKey
            // 
            this.lblAesKey.AutoSize = true;
            this.lblAesKey.Location = new System.Drawing.Point(23, 182);
            this.lblAesKey.Name = "lblAesKey";
            this.lblAesKey.Size = new System.Drawing.Size(53, 12);
            this.lblAesKey.TabIndex = 0;
            this.lblAesKey.Text = "AES Key:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "Server IP:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "Server Port:";
            // 
            // txtBoxIP
            // 
            this.txtBoxIP.Location = new System.Drawing.Point(104, 113);
            this.txtBoxIP.Name = "txtBoxIP";
            this.txtBoxIP.Size = new System.Drawing.Size(182, 21);
            this.txtBoxIP.TabIndex = 10;
            // 
            // txtBoxPort
            // 
            this.txtBoxPort.Location = new System.Drawing.Point(104, 144);
            this.txtBoxPort.Name = "txtBoxPort";
            this.txtBoxPort.Size = new System.Drawing.Size(182, 21);
            this.txtBoxPort.TabIndex = 11;
            // 
            // SenderUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 366);
            this.Controls.Add(this.groupBoxKey);
            this.Controls.Add(this.groupBoxCtrl);
            this.Name = "SenderUI";
            this.Text = "太阳能光伏发送控制窗口";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SenderUI_FormClosed);
            this.groupBoxCtrl.ResumeLayout(false);
            this.groupBoxKey.ResumeLayout(false);
            this.groupBoxKey.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSenderStart;
        private System.Windows.Forms.Button btnSenderStop;
        private System.Windows.Forms.Button btnSenderRestart;
        private System.Windows.Forms.GroupBox groupBoxCtrl;
        private System.Windows.Forms.GroupBox groupBoxKey;
        private System.Windows.Forms.TextBox txtBoxMd5Key;
        private System.Windows.Forms.TextBox txtBoxAesIV;
        private System.Windows.Forms.TextBox txtBoxAesKey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblAesKey;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.TextBox txtBoxPort;
        private System.Windows.Forms.TextBox txtBoxIP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}

