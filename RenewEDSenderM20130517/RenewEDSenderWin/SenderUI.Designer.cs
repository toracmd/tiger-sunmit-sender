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
            this.SuspendLayout();
            // 
            // btnSenderStart
            // 
            this.btnSenderStart.Location = new System.Drawing.Point(65, 30);
            this.btnSenderStart.Name = "btnSenderStart";
            this.btnSenderStart.Size = new System.Drawing.Size(121, 56);
            this.btnSenderStart.TabIndex = 0;
            this.btnSenderStart.Text = "启动发送服务";
            this.btnSenderStart.UseVisualStyleBackColor = true;
            this.btnSenderStart.Click += new System.EventHandler(this.btnSenderStart_Click);
            // 
            // btnSenderStop
            // 
            this.btnSenderStop.Location = new System.Drawing.Point(65, 129);
            this.btnSenderStop.Name = "btnSenderStop";
            this.btnSenderStop.Size = new System.Drawing.Size(121, 56);
            this.btnSenderStop.TabIndex = 1;
            this.btnSenderStop.Text = "停止发送服务";
            this.btnSenderStop.UseVisualStyleBackColor = true;
            this.btnSenderStop.Click += new System.EventHandler(this.btnSenderStop_Click);
            // 
            // btnSenderRestart
            // 
            this.btnSenderRestart.Location = new System.Drawing.Point(65, 244);
            this.btnSenderRestart.Name = "btnSenderRestart";
            this.btnSenderRestart.Size = new System.Drawing.Size(121, 56);
            this.btnSenderRestart.TabIndex = 2;
            this.btnSenderRestart.Text = "重启发送服务";
            this.btnSenderRestart.UseVisualStyleBackColor = true;
            this.btnSenderRestart.Click += new System.EventHandler(this.btnSenderRestart_Click);
            // 
            // SenderUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 366);
            this.Controls.Add(this.btnSenderRestart);
            this.Controls.Add(this.btnSenderStop);
            this.Controls.Add(this.btnSenderStart);
            this.Name = "SenderUI";
            this.Text = "太阳能光伏发送控制窗口";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SenderUI_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSenderStart;
        private System.Windows.Forms.Button btnSenderStop;
        private System.Windows.Forms.Button btnSenderRestart;
    }
}

