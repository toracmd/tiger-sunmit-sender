namespace RenewEDSenderM
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
            this.btnStartSender = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnStopSender = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartSender
            // 
            this.btnStartSender.Location = new System.Drawing.Point(571, 27);
            this.btnStartSender.Name = "btnStartSender";
            this.btnStartSender.Size = new System.Drawing.Size(94, 50);
            this.btnStartSender.TabIndex = 0;
            this.btnStartSender.Text = "启动发送任务";
            this.btnStartSender.UseVisualStyleBackColor = true;
            this.btnStartSender.Click += new System.EventHandler(this.btnStartSender_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(100, 152);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(447, 200);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // btnStopSender
            // 
            this.btnStopSender.Location = new System.Drawing.Point(571, 119);
            this.btnStopSender.Name = "btnStopSender";
            this.btnStopSender.Size = new System.Drawing.Size(94, 50);
            this.btnStopSender.TabIndex = 2;
            this.btnStopSender.Text = "停止发送任务";
            this.btnStopSender.UseVisualStyleBackColor = true;
            this.btnStopSender.Click += new System.EventHandler(this.btnStopSender_Click);
            // 
            // SenderUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 410);
            this.Controls.Add(this.btnStopSender);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnStartSender);
            this.Name = "SenderUI";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartSender;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btnStopSender;
    }
}

