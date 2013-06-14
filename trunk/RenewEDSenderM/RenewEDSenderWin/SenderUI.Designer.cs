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
            this.components = new System.ComponentModel.Container();
            this.btnSenderStart = new System.Windows.Forms.Button();
            this.btnSenderStop = new System.Windows.Forms.Button();
            this.btnSenderRestart = new System.Windows.Forms.Button();
            this.groupBoxCtrl = new System.Windows.Forms.GroupBox();
            this.groupBoxKey = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.txtBoxSun2 = new System.Windows.Forms.TextBox();
            this.txtBoxOuter2 = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.txtBoxSunGen2 = new System.Windows.Forms.TextBox();
            this.txtBoxGen2 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.txtBoxSun1 = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtBoxOuter1 = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.txtBoxSunGen1 = new System.Windows.Forms.TextBox();
            this.txtBoxGen1 = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtBoxSysCode = new System.Windows.Forms.TextBox();
            this.txtBoxTechCode = new System.Windows.Forms.TextBox();
            this.txtBoxProCode = new System.Windows.Forms.TextBox();
            this.txtBoxAreaCode = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtBoxProId = new System.Windows.Forms.TextBox();
            this.txtBoxGateId = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBoxPort = new System.Windows.Forms.TextBox();
            this.txtBoxIP = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnKeyUpdate = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.txtBoxMd5Key = new System.Windows.Forms.TextBox();
            this.txtBoxAesIV = new System.Windows.Forms.TextBox();
            this.txtBoxAesKey = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAesKey = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBoxNetState = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtBoxRunPhase = new System.Windows.Forms.TextBox();
            this.txtBoxConnStatus = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBoxCtrl.SuspendLayout();
            this.groupBoxKey.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSenderStart
            // 
            this.btnSenderStart.Location = new System.Drawing.Point(35, 26);
            this.btnSenderStart.Name = "btnSenderStart";
            this.btnSenderStart.Size = new System.Drawing.Size(134, 30);
            this.btnSenderStart.TabIndex = 0;
            this.btnSenderStart.Text = "启动发送服务";
            this.btnSenderStart.UseVisualStyleBackColor = true;
            this.btnSenderStart.Click += new System.EventHandler(this.btnSenderStart_Click);
            // 
            // btnSenderStop
            // 
            this.btnSenderStop.Location = new System.Drawing.Point(35, 78);
            this.btnSenderStop.Name = "btnSenderStop";
            this.btnSenderStop.Size = new System.Drawing.Size(134, 27);
            this.btnSenderStop.TabIndex = 1;
            this.btnSenderStop.Text = "停止发送服务";
            this.btnSenderStop.UseVisualStyleBackColor = true;
            this.btnSenderStop.Click += new System.EventHandler(this.btnSenderStop_Click);
            // 
            // btnSenderRestart
            // 
            this.btnSenderRestart.Location = new System.Drawing.Point(35, 122);
            this.btnSenderRestart.Name = "btnSenderRestart";
            this.btnSenderRestart.Size = new System.Drawing.Size(134, 27);
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
            this.groupBoxCtrl.Size = new System.Drawing.Size(247, 175);
            this.groupBoxCtrl.TabIndex = 1;
            this.groupBoxCtrl.TabStop = false;
            this.groupBoxCtrl.Text = "数据发送服务";
            // 
            // groupBoxKey
            // 
            this.groupBoxKey.Controls.Add(this.groupBox3);
            this.groupBoxKey.Controls.Add(this.groupBox2);
            this.groupBoxKey.Controls.Add(this.label25);
            this.groupBoxKey.Controls.Add(this.label24);
            this.groupBoxKey.Controls.Add(this.label23);
            this.groupBoxKey.Controls.Add(this.label15);
            this.groupBoxKey.Controls.Add(this.txtBoxSysCode);
            this.groupBoxKey.Controls.Add(this.txtBoxTechCode);
            this.groupBoxKey.Controls.Add(this.txtBoxProCode);
            this.groupBoxKey.Controls.Add(this.txtBoxAreaCode);
            this.groupBoxKey.Controls.Add(this.label13);
            this.groupBoxKey.Controls.Add(this.label12);
            this.groupBoxKey.Controls.Add(this.label11);
            this.groupBoxKey.Controls.Add(this.label10);
            this.groupBoxKey.Controls.Add(this.txtBoxProId);
            this.groupBoxKey.Controls.Add(this.txtBoxGateId);
            this.groupBoxKey.Controls.Add(this.label6);
            this.groupBoxKey.Controls.Add(this.label5);
            this.groupBoxKey.Controls.Add(this.txtBoxPort);
            this.groupBoxKey.Controls.Add(this.txtBoxIP);
            this.groupBoxKey.Controls.Add(this.label4);
            this.groupBoxKey.Controls.Add(this.label3);
            this.groupBoxKey.Controls.Add(this.btnKeyUpdate);
            this.groupBoxKey.Controls.Add(this.btnUpdate);
            this.groupBoxKey.Controls.Add(this.txtBoxMd5Key);
            this.groupBoxKey.Controls.Add(this.txtBoxAesIV);
            this.groupBoxKey.Controls.Add(this.txtBoxAesKey);
            this.groupBoxKey.Controls.Add(this.label2);
            this.groupBoxKey.Controls.Add(this.label1);
            this.groupBoxKey.Controls.Add(this.lblAesKey);
            this.groupBoxKey.Location = new System.Drawing.Point(283, 13);
            this.groupBoxKey.Name = "groupBoxKey";
            this.groupBoxKey.Size = new System.Drawing.Size(935, 488);
            this.groupBoxKey.TabIndex = 2;
            this.groupBoxKey.TabStop = false;
            this.groupBoxKey.Text = "配置管理";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.label29);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.label28);
            this.groupBox3.Controls.Add(this.txtBoxSun2);
            this.groupBox3.Controls.Add(this.txtBoxOuter2);
            this.groupBox3.Controls.Add(this.label27);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.label26);
            this.groupBox3.Controls.Add(this.txtBoxSunGen2);
            this.groupBox3.Controls.Add(this.txtBoxGen2);
            this.groupBox3.Location = new System.Drawing.Point(619, 160);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(279, 163);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "数据采集识别码";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 22);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(71, 12);
            this.label22.TabIndex = 30;
            this.label22.Text = "太阳辐照度:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 94);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(131, 12);
            this.label20.TabIndex = 32;
            this.label20.Text = "光伏组件背面表面温度:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(208, 134);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(59, 12);
            this.label29.TabIndex = 41;
            this.label29.Text = "(2位数字)";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 135);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(47, 12);
            this.label19.TabIndex = 33;
            this.label19.Text = "发电量:";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(208, 99);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(59, 12);
            this.label28.TabIndex = 40;
            this.label28.Text = "(2位数字)";
            // 
            // txtBoxSun2
            // 
            this.txtBoxSun2.Location = new System.Drawing.Point(140, 19);
            this.txtBoxSun2.Name = "txtBoxSun2";
            this.txtBoxSun2.Size = new System.Drawing.Size(54, 21);
            this.txtBoxSun2.TabIndex = 12;
            this.txtBoxSun2.Text = "110000015";
            this.txtBoxSun2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxSun2_KeyPress);
            this.txtBoxSun2.Validated += new System.EventHandler(this.txtBoxSun2_Validated);
            // 
            // txtBoxOuter2
            // 
            this.txtBoxOuter2.Location = new System.Drawing.Point(140, 58);
            this.txtBoxOuter2.Name = "txtBoxOuter2";
            this.txtBoxOuter2.Size = new System.Drawing.Size(54, 21);
            this.txtBoxOuter2.TabIndex = 14;
            this.txtBoxOuter2.Text = "110000015";
            this.txtBoxOuter2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxOuter2_KeyPress);
            this.txtBoxOuter2.Validated += new System.EventHandler(this.txtBoxOuter2_Validated);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(208, 61);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(59, 12);
            this.label27.TabIndex = 39;
            this.label27.Text = "(2位数字)";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 61);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(59, 12);
            this.label21.TabIndex = 31;
            this.label21.Text = "室外温度:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(208, 22);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(59, 12);
            this.label26.TabIndex = 38;
            this.label26.Text = "(2位数字)";
            // 
            // txtBoxSunGen2
            // 
            this.txtBoxSunGen2.Location = new System.Drawing.Point(140, 90);
            this.txtBoxSunGen2.Name = "txtBoxSunGen2";
            this.txtBoxSunGen2.Size = new System.Drawing.Size(54, 21);
            this.txtBoxSunGen2.TabIndex = 16;
            this.txtBoxSunGen2.Text = "110000015";
            this.txtBoxSunGen2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxSunGen2_KeyPress);
            this.txtBoxSunGen2.Validated += new System.EventHandler(this.txtBoxSunGen2_Validated);
            // 
            // txtBoxGen2
            // 
            this.txtBoxGen2.Location = new System.Drawing.Point(140, 131);
            this.txtBoxGen2.Name = "txtBoxGen2";
            this.txtBoxGen2.Size = new System.Drawing.Size(54, 21);
            this.txtBoxGen2.TabIndex = 18;
            this.txtBoxGen2.Text = "110000015";
            this.txtBoxGen2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxGen2_KeyPress);
            this.txtBoxGen2.Validated += new System.EventHandler(this.txtBoxGen2_Validated);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label33);
            this.groupBox2.Controls.Add(this.txtBoxSun1);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.txtBoxOuter1);
            this.groupBox2.Controls.Add(this.label32);
            this.groupBox2.Controls.Add(this.txtBoxSunGen1);
            this.groupBox2.Controls.Add(this.txtBoxGen1);
            this.groupBox2.Controls.Add(this.label30);
            this.groupBox2.Controls.Add(this.label31);
            this.groupBox2.Location = new System.Drawing.Point(317, 160);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(296, 163);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "数据采集装置识别编码";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(15, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 12);
            this.label14.TabIndex = 20;
            this.label14.Text = "太阳辐照度:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 61);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(59, 12);
            this.label16.TabIndex = 21;
            this.label16.Text = "室外温度:";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(217, 134);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(59, 12);
            this.label33.TabIndex = 45;
            this.label33.Text = "(2位数字)";
            // 
            // txtBoxSun1
            // 
            this.txtBoxSun1.Location = new System.Drawing.Point(147, 19);
            this.txtBoxSun1.Name = "txtBoxSun1";
            this.txtBoxSun1.Size = new System.Drawing.Size(54, 21);
            this.txtBoxSun1.TabIndex = 11;
            this.txtBoxSun1.Text = "110000015";
            this.txtBoxSun1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxSun1_KeyPress);
            this.txtBoxSun1.Validated += new System.EventHandler(this.txtBoxSun1_Validated);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 94);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(131, 12);
            this.label17.TabIndex = 22;
            this.label17.Text = "光伏组件背面表面温度:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(15, 135);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(47, 12);
            this.label18.TabIndex = 23;
            this.label18.Text = "发电量:";
            // 
            // txtBoxOuter1
            // 
            this.txtBoxOuter1.Location = new System.Drawing.Point(147, 58);
            this.txtBoxOuter1.Name = "txtBoxOuter1";
            this.txtBoxOuter1.Size = new System.Drawing.Size(54, 21);
            this.txtBoxOuter1.TabIndex = 13;
            this.txtBoxOuter1.Text = "110000015";
            this.txtBoxOuter1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxOuter1_KeyPress);
            this.txtBoxOuter1.Validated += new System.EventHandler(this.txtBoxOuter1_Validated);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(217, 96);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(59, 12);
            this.label32.TabIndex = 44;
            this.label32.Text = "(2位数字)";
            // 
            // txtBoxSunGen1
            // 
            this.txtBoxSunGen1.Location = new System.Drawing.Point(147, 90);
            this.txtBoxSunGen1.Name = "txtBoxSunGen1";
            this.txtBoxSunGen1.Size = new System.Drawing.Size(54, 21);
            this.txtBoxSunGen1.TabIndex = 15;
            this.txtBoxSunGen1.Text = "110000015";
            this.txtBoxSunGen1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxSunGen1_KeyPress);
            this.txtBoxSunGen1.Validated += new System.EventHandler(this.txtBoxSunGen1_Validated);
            // 
            // txtBoxGen1
            // 
            this.txtBoxGen1.Location = new System.Drawing.Point(147, 131);
            this.txtBoxGen1.Name = "txtBoxGen1";
            this.txtBoxGen1.Size = new System.Drawing.Size(54, 21);
            this.txtBoxGen1.TabIndex = 17;
            this.txtBoxGen1.Text = "110000015";
            this.txtBoxGen1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxGen1_KeyPress);
            this.txtBoxGen1.Validated += new System.EventHandler(this.txtBoxGen1_Validated);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(217, 22);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(59, 12);
            this.label30.TabIndex = 42;
            this.label30.Text = "(2位数字)";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(217, 61);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(59, 12);
            this.label31.TabIndex = 43;
            this.label31.Text = "(2位数字)";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(592, 120);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(59, 12);
            this.label25.TabIndex = 32;
            this.label25.Text = "(2位数字)";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(592, 93);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(59, 12);
            this.label24.TabIndex = 31;
            this.label24.Text = "(3位数字)";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(592, 64);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(95, 12);
            this.label23.TabIndex = 30;
            this.label23.Text = "(3位数字或字母)";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(592, 29);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(59, 12);
            this.label15.TabIndex = 29;
            this.label15.Text = "(6位数字)";
            // 
            // txtBoxSysCode
            // 
            this.txtBoxSysCode.Location = new System.Drawing.Point(392, 117);
            this.txtBoxSysCode.Name = "txtBoxSysCode";
            this.txtBoxSysCode.Size = new System.Drawing.Size(182, 21);
            this.txtBoxSysCode.TabIndex = 10;
            this.txtBoxSysCode.Text = "110000015";
            this.txtBoxSysCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxSysCode_KeyPress);
            this.txtBoxSysCode.Validated += new System.EventHandler(this.txtBoxSysCode_Validated);
            // 
            // txtBoxTechCode
            // 
            this.txtBoxTechCode.Location = new System.Drawing.Point(392, 90);
            this.txtBoxTechCode.Name = "txtBoxTechCode";
            this.txtBoxTechCode.Size = new System.Drawing.Size(182, 21);
            this.txtBoxTechCode.TabIndex = 9;
            this.txtBoxTechCode.Text = "110000015";
            this.txtBoxTechCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxTechCode_KeyPress);
            this.txtBoxTechCode.Validated += new System.EventHandler(this.txtBoxTechCode_Validated);
            // 
            // txtBoxProCode
            // 
            this.txtBoxProCode.Location = new System.Drawing.Point(392, 58);
            this.txtBoxProCode.Name = "txtBoxProCode";
            this.txtBoxProCode.Size = new System.Drawing.Size(182, 21);
            this.txtBoxProCode.TabIndex = 8;
            this.txtBoxProCode.Text = "110000015";
            this.txtBoxProCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxProCode_KeyPress);
            this.txtBoxProCode.Validated += new System.EventHandler(this.txtBoxProCode_Validated);
            // 
            // txtBoxAreaCode
            // 
            this.txtBoxAreaCode.Location = new System.Drawing.Point(392, 26);
            this.txtBoxAreaCode.Name = "txtBoxAreaCode";
            this.txtBoxAreaCode.Size = new System.Drawing.Size(182, 21);
            this.txtBoxAreaCode.TabIndex = 7;
            this.txtBoxAreaCode.Text = "110000015";
            this.txtBoxAreaCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxAreaCode_KeyPress);
            this.txtBoxAreaCode.Validated += new System.EventHandler(this.txtBoxAreaCode_Validated);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(315, 122);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 12);
            this.label13.TabIndex = 19;
            this.label13.Text = "系统编码:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(315, 93);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 12);
            this.label12.TabIndex = 18;
            this.label12.Text = "技术类型:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(315, 61);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 17;
            this.label11.Text = "项目编码:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(315, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 12);
            this.label10.TabIndex = 16;
            this.label10.Text = "行政区编码:";
            // 
            // txtBoxProId
            // 
            this.txtBoxProId.Location = new System.Drawing.Point(104, 26);
            this.txtBoxProId.Name = "txtBoxProId";
            this.txtBoxProId.Size = new System.Drawing.Size(182, 21);
            this.txtBoxProId.TabIndex = 3;
            this.txtBoxProId.Text = "110000015";
            this.txtBoxProId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxProId_KeyPress);
            this.txtBoxProId.Validated += new System.EventHandler(this.txtBoxProId_Validated);
            // 
            // txtBoxGateId
            // 
            this.txtBoxGateId.Location = new System.Drawing.Point(104, 61);
            this.txtBoxGateId.Name = "txtBoxGateId";
            this.txtBoxGateId.Size = new System.Drawing.Size(182, 21);
            this.txtBoxGateId.TabIndex = 4;
            this.txtBoxGateId.Text = "1100000140202";
            this.txtBoxGateId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxGateId_KeyPress);
            this.txtBoxGateId.Validated += new System.EventHandler(this.txtBoxGateId_Validated);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 12);
            this.label6.TabIndex = 13;
            this.label6.Text = "采集装置编号:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "项目编号:";
            // 
            // txtBoxPort
            // 
            this.txtBoxPort.Location = new System.Drawing.Point(104, 144);
            this.txtBoxPort.Name = "txtBoxPort";
            this.txtBoxPort.Size = new System.Drawing.Size(182, 21);
            this.txtBoxPort.TabIndex = 6;
            this.txtBoxPort.Text = "13145";
            this.txtBoxPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBoxPort_KeyPress);
            this.txtBoxPort.Validated += new System.EventHandler(this.txtBoxPort_Validated);
            // 
            // txtBoxIP
            // 
            this.txtBoxIP.Location = new System.Drawing.Point(104, 113);
            this.txtBoxIP.Name = "txtBoxIP";
            this.txtBoxIP.Size = new System.Drawing.Size(182, 21);
            this.txtBoxIP.TabIndex = 5;
            this.txtBoxIP.Text = "210.77.14.218";
            this.txtBoxIP.Validated += new System.EventHandler(this.txtBoxIP_Validated);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "Server IP:";
            // 
            // btnKeyUpdate
            // 
            this.btnKeyUpdate.Location = new System.Drawing.Point(211, 407);
            this.btnKeyUpdate.Name = "btnKeyUpdate";
            this.btnKeyUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnKeyUpdate.TabIndex = 23;
            this.btnKeyUpdate.Text = "更新密钥";
            this.btnKeyUpdate.UseVisualStyleBackColor = true;
            this.btnKeyUpdate.Click += new System.EventHandler(this.btnKeyUpdate_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(104, 407);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 22;
            this.btnUpdate.Text = "更新配置";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // txtBoxMd5Key
            // 
            this.txtBoxMd5Key.Location = new System.Drawing.Point(104, 239);
            this.txtBoxMd5Key.Name = "txtBoxMd5Key";
            this.txtBoxMd5Key.PasswordChar = '*';
            this.txtBoxMd5Key.Size = new System.Drawing.Size(182, 21);
            this.txtBoxMd5Key.TabIndex = 21;
            this.txtBoxMd5Key.Text = "0000000000123456";
            this.txtBoxMd5Key.Validated += new System.EventHandler(this.txtBoxMd5Key_Validated);
            // 
            // txtBoxAesIV
            // 
            this.txtBoxAesIV.Location = new System.Drawing.Point(104, 208);
            this.txtBoxAesIV.Name = "txtBoxAesIV";
            this.txtBoxAesIV.PasswordChar = '*';
            this.txtBoxAesIV.Size = new System.Drawing.Size(182, 21);
            this.txtBoxAesIV.TabIndex = 20;
            this.txtBoxAesIV.Text = "0000000000123456";
            this.txtBoxAesIV.Validated += new System.EventHandler(this.txtBoxAesIV_Validated);
            // 
            // txtBoxAesKey
            // 
            this.txtBoxAesKey.Location = new System.Drawing.Point(104, 179);
            this.txtBoxAesKey.Name = "txtBoxAesKey";
            this.txtBoxAesKey.PasswordChar = '*';
            this.txtBoxAesKey.Size = new System.Drawing.Size(182, 21);
            this.txtBoxAesKey.TabIndex = 19;
            this.txtBoxAesKey.Text = "0000000000123456";
            this.txtBoxAesKey.Validated += new System.EventHandler(this.txtBoxAesKey_Validated);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBoxNetState);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtBoxRunPhase);
            this.groupBox1.Controls.Add(this.txtBoxConnStatus);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Location = new System.Drawing.Point(26, 194);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 307);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "服务状态";
            // 
            // txtBoxNetState
            // 
            this.txtBoxNetState.Enabled = false;
            this.txtBoxNetState.Location = new System.Drawing.Point(88, 91);
            this.txtBoxNetState.Name = "txtBoxNetState";
            this.txtBoxNetState.Size = new System.Drawing.Size(146, 21);
            this.txtBoxNetState.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(23, 94);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 12);
            this.label9.TabIndex = 4;
            this.label9.Text = "网络状态:";
            // 
            // txtBoxRunPhase
            // 
            this.txtBoxRunPhase.Enabled = false;
            this.txtBoxRunPhase.Location = new System.Drawing.Point(88, 57);
            this.txtBoxRunPhase.Name = "txtBoxRunPhase";
            this.txtBoxRunPhase.Size = new System.Drawing.Size(146, 21);
            this.txtBoxRunPhase.TabIndex = 3;
            // 
            // txtBoxConnStatus
            // 
            this.txtBoxConnStatus.Enabled = false;
            this.txtBoxConnStatus.Location = new System.Drawing.Point(87, 25);
            this.txtBoxConnStatus.Name = "txtBoxConnStatus";
            this.txtBoxConnStatus.Size = new System.Drawing.Size(146, 21);
            this.txtBoxConnStatus.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 34);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "连接状态:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "运行阶段:";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // SenderUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1230, 554);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxKey);
            this.Controls.Add(this.groupBoxCtrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SenderUI";
            this.Text = "太阳能光伏发送控制窗口";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SenderUI_FormClosed);
            this.groupBoxCtrl.ResumeLayout(false);
            this.groupBoxKey.ResumeLayout(false);
            this.groupBoxKey.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
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
        private System.Windows.Forms.Button btnKeyUpdate;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.TextBox txtBoxPort;
        private System.Windows.Forms.TextBox txtBoxIP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBoxProId;
        private System.Windows.Forms.TextBox txtBoxGateId;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtBoxRunPhase;
        private System.Windows.Forms.TextBox txtBoxConnStatus;
        private System.Windows.Forms.TextBox txtBoxNetState;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtBoxSun1;
        private System.Windows.Forms.TextBox txtBoxSysCode;
        private System.Windows.Forms.TextBox txtBoxTechCode;
        private System.Windows.Forms.TextBox txtBoxProCode;
        private System.Windows.Forms.TextBox txtBoxAreaCode;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtBoxGen1;
        private System.Windows.Forms.TextBox txtBoxSunGen1;
        private System.Windows.Forms.TextBox txtBoxOuter1;
        private System.Windows.Forms.TextBox txtBoxGen2;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtBoxSunGen2;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtBoxOuter2;
        private System.Windows.Forms.TextBox txtBoxSun2;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

