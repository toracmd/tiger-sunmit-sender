using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Messaging;
using RenewEDSenderM.CommManager;
using RenewEDSenderM.Support;
using System.Runtime.InteropServices;
using System.Timers;
//using System.Configuration;

namespace RenewEDSenderWin
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SenderUI : Form
    {
        /// <summary>
        /// 发送程序的可执行文件名，也是进程名称
        /// </summary>
        // T.B.D. 文件名读配置或者写定
        //private static string m_fileNameDesp = "RenewEDSenderM";
        private static string m_fileNameDesp = System.Configuration.ConfigurationManager.AppSettings["EXE_FILE_NAME"];
        
        // T.B.D. 文件路径在程序打包后的
        //private static string m_filePath = "D:\\workspace\\sunmit\\svn\\RenewEDSenderM\\RenewEDSenderM\\bin\\Debug";
        private static string m_filePath = System.Configuration.ConfigurationManager.AppSettings["EXE_FILE_PATH"];
        /// <summary>
        /// 发送服务启动后获得的进程ID
        /// </summary>
        private static int m_pid_sender;
        /// <summary>
        /// 发送服务启动状态
        /// </summary>
        private static bool m_isStart = false;
        /// <summary>
        /// 项目编号
        /// </summary>
        private static string m_project_id;
        /// <summary>
        /// 采集装置编号
        /// </summary>
        private static string m_gateway_id;
        /// <summary>
        /// 服务器IP
        /// </summary>
        private static string m_Ip;
        /// <summary>
        /// 服务器端口
        /// </summary>
        private static string m_Port;
        /// <summary>
        /// AES密钥
        /// </summary>
        private static string m_AesKey;
        /// <summary>
        /// AES偏移量
        /// </summary>
        private static string m_AesIV;
        /// <summary>
        /// MD5密钥
        /// </summary>
        private static string m_Md5Key;
        
        /// <summary>
        /// 按钮切换委托的类型定义
        /// </summary>
        private delegate void btnSwitchDelegateDef();

        /// <summary>
        /// 消息处理委托的类型定义
        /// </summary>
        /// <param name="msg">从消息队列里获得,并经过序列化后的消息体</param>
        private delegate void MsgProcDelegateDef(MsgBody msg);

        /// <summary>
        /// 消息队列管理器
        /// </summary>
        private static MsgQueManager m_MsgQueManager;

        private static Configuration config;

        private System.Timers.Timer m_moniter_timer = new System.Timers.Timer();

        private bool isCompleteCheck = true;

        private List<Control> listContrl = new List<Control>() ;

        public string NetWorkState
        {
            get
            {
                if (IsConnected())
                {
                    return "网络连接可用";
                }
                else
                {
                    return "网络连接不可用";
                }
            }
        }
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        public static bool IsConnected()
        {

            int desc = 0;

            bool state = InternetGetConnectedState(out desc, 0);

            return state;

        }
        /// <summary>
        /// SenderUI构造函数
        /// </summary>
        public SenderUI()
        {
            InitializeComponent();
            InitMember();
            //建立消息队列接收线程
            //m_thread_recv_queue = new Thread(new ThreadStart(MsgQueueRecv));
            //启动消息队列接收线程
            //m_thread_recv_queue.Start();

            //方法一：不进行跨线程安全检查  
            //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;  
        }
        /// <summary>
        /// 初始化成员
        /// </summary>
        private void InitMember()
        {
            //初始按钮状态
            btnSenderStop.Enabled = false;
            btnSenderRestart.Enabled = false;

            //Computer c = new Computer();
            //isNetAvailable = c.Network.IsAvailable;
            txtBoxNetState.Text = NetWorkState;
            //c.Network.NetworkAvailabilityChanged +=new NetworkAvailableEventHandler(Network_NetworkAvailabilityChanged);

            m_moniter_timer.Elapsed += new ElapsedEventHandler(MoniterTimer_Elapsed);
            m_moniter_timer.Interval = 5 * 1000;    //5秒一次
            m_moniter_timer.Enabled = true;

            try
            {
                m_MsgQueManager = new MsgQueManager();
            }
            catch (Exception ex)
            {
                MessageBox.Show("消息队列尚未启动:" /*+ ex*/ + "试运行命令再执行：net start MSMQ");
                
                return;
            }
            MsgQueueRecv();

            //添加配置项控件到集合
            listContrl.Add(txtBoxAesIV);
            listContrl.Add(txtBoxAesKey);
            listContrl.Add(txtBoxAreaCode);
            listContrl.Add(txtBoxGateId);
            listContrl.Add(txtBoxGen1);
            listContrl.Add(txtBoxGen2);
            listContrl.Add(txtBoxIP);
            listContrl.Add(txtBoxMd5Key);
            listContrl.Add(txtBoxOuter1);
            listContrl.Add(txtBoxOuter2);
            listContrl.Add(txtBoxPort);
            listContrl.Add(txtBoxProCode);
            listContrl.Add(txtBoxProId);
            listContrl.Add(txtBoxSun1);
            listContrl.Add(txtBoxSun2);
            listContrl.Add(txtBoxSunGen1);
            listContrl.Add(txtBoxSunGen2);
            listContrl.Add(txtBoxSysCode);
            listContrl.Add(txtBoxTechCode);

            SetConfig cfg = new SetConfig();
            config = cfg.ReadConfig();
            if (config != null)
            {
                txtBoxAesIV.Text = config.iv;
                txtBoxAesKey.Text = config.key;
                txtBoxGateId.Text = config.gateway_id;
                txtBoxIP.Text = config.ip;
                txtBoxMd5Key.Text = config.md5;
                txtBoxPort.Text = config.port;
                txtBoxProId.Text = config.project_id;

                txtBoxProCode.Text = config.programid;
                txtBoxAreaCode.Text = config.areacode;
                txtBoxTechCode.Text = config.techtype;
                txtBoxSysCode.Text = config.syscode;

                txtBoxGen1.Text = config.meterInfo.MD_Code1;
                txtBoxGen2.Text = config.meterInfo.MD_Code2;
                txtBoxOuter1.Text = config.meterInfo.MB_Code1;
                txtBoxOuter2.Text = config.meterInfo.MB_Code2;
                txtBoxSun1.Text = config.meterInfo.MA_Code1;
                txtBoxSun2.Text = config.meterInfo.MB_Code2;
                txtBoxSunGen1.Text = config.meterInfo.MC_Code1;
                txtBoxSunGen2.Text = config.meterInfo.MC_Code2;
                //T.B.D. 1、其他需要编辑的; 2、设置的； 3、验证格式
            }
        }
        /// <summary>
        /// 消息队列异步接收函数
        /// </summary>
        public void MsgQueueRecv()
        {
            if (true)
            {
                try
                {
                    //获得消息队列
                    MessageQueue msgq = MsgQueManager.getInstance();
                    //绑定消息接收完成事件的处理函数
                    msgq.ReceiveCompleted += MsgQReceiveCompleted;
                    //msgq.BeginReceive();
                    //等待异步完成T.B.D. 要不要关 或者 使用 和 句柄函数用BeginReceive搭配
                    WaitHandle waitHandle = msgq.BeginReceive().AsyncWaitHandle;
                }
                catch (MessageQueueException ex)
                {
                    //T.B.D.异常初始方式
                    MsgQueManager.Dispose();
                    return;
                }
            }
        }
        /// <summary>
        /// 异步消息接收完成事件处理函数
        /// </summary>
        /// <param name="sender">消息队列</param>
        /// <param name="e">事件</param>
        private void MsgQReceiveCompleted(object sender, System.Messaging.ReceiveCompletedEventArgs e)
        {
            //获得当前消息队列
            MessageQueue msgq = (MessageQueue)sender;
            try
            {
                //取得消息
                System.Messaging.Message msg = msgq.EndReceive(e.AsyncResult);
                //对消息进行序列化
                msg.Formatter = new XmlMessageFormatter(
                    new Type[] { typeof(MsgBody) }
                    );
                //取得消息内容
                MsgBody msgbody = (MsgBody)msg.Body;
                //MessageBox.Show(msg.Label + msgbody);
                //消息处理代理执行消息处理函数
                MsgProcDelegate(msgbody);
            }
            catch (Exception ex)
            {
                //T.B.D.
            }
            finally
            {
                //继续异步接收消息
                msgq.BeginReceive();
            }
        }
        private void MoniterTimer_Elapsed(object sender, EventArgs e)
        {
            if (m_isStart)
            {
                if (!CheckProcessExists())
                {
                    ManageControllerDelegate(SetBtnStopDisable);
                }
            }
            ManageControllerDelegate(setTxtNetState);
        }
        /// <summary>
        /// 启动按钮按下事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSenderStart_Click(object sender, EventArgs e)
        {
            //启动发送服务进程
            StartProcessSend();
        }
        /// <summary>
        /// 启动发送服务进程
        /// </summary>
        private void StartProcessSend()
        {
            try
            {
                //判断该进程是否已经启动
                if (!CheckProcessExists())
                {
                    bool b = false;
                    //实例化一个进程对象
                    Process p = new Process();
                    //设置该进程对象关联的文件为发送服务
                    //p.StartInfo.FileName = System.IO.Path.Combine(Application.StartupPath, fileName);
                    //T.B.D. For Test
                    p.StartInfo.FileName = System.IO.Path.Combine(m_filePath, m_fileNameDesp);

                    //设置使用操作系统外壳程序启动
                    p.StartInfo.UseShellExecute = true;
                    //不在新窗口启动
                    p.StartInfo.CreateNoWindow = true;

                    //设置进程退出时触发此事件
                    p.EnableRaisingEvents = true;
                    //添加进程退出消息处理函数
                    p.Exited += new EventHandler(SendProc_Exited);
                    //启动进程
                    b = p.Start();
                    if (b == true)
                    {
                        //启动成功后，获取发送服务的进程号
                        m_pid_sender = p.Id;
                        //设置发送服务进程状态
                        m_isStart = true;
                        //设置UI按钮状态
                        btnSenderStart.Enabled = false;
                        btnSenderStop.Enabled = true;
                        btnSenderRestart.Enabled = true;
                        foreach (Control c in listContrl)
                        {
                            c.Enabled = false;
                        }
                    }
                }
                else
                {
                    m_isStart = true;
                    ManageControllerDelegate(SetBtnStartDisable);
                }

            }
            catch (ArgumentOutOfRangeException argex)
            {
            }
            catch (ThreadStateException threadex)
            {
            }
            catch (OutOfMemoryException ooe)
            {
            }
            catch (Exception ex)
            {
                //T.B.D.进程启动异常信息
                MessageBox.Show("Exception:" + ex.Source + " " + ex.Message);
            }
        }

        /// <summary>
        /// 后台发送服务进程监视函数.正常
        /// </summary>
        private void MonitorSendProc()
        {
            int second = 5;
            while (true)
            {
                //5秒查看一次
                Thread.Sleep(second * 100);
                if (!CheckProcessExists())
                {
                    //T.B.D.这里可能得并发
                    m_isStart = false;
                    //后台服务进程失效则切换按钮状态
                    ManageControllerDelegate(SetBtnStopDisable);
                }
                else
                {
                    m_isStart = true;
                    ManageControllerDelegate(SetBtnStartDisable);
                }
            }
        }
        /// <summary>
        /// 判断指定进程是否存在
        /// </summary>
        /// <returns>true/false</returns>
        private bool CheckProcessExists()
        {
            //获取指定文件名的所有进程对象
            Process[] processes = Process.GetProcessesByName(m_fileNameDesp);
            string fileName = m_fileNameDesp + ".exe";    // >>>> revision 20130517
            //遍历该进程对象集合
            foreach (Process p in processes)
            {
                //根据关联进程的主模块信息，判定该文件名进程是否启动
                if (p.ProcessName == m_fileNameDesp)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 发送停止按钮按下事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSenderStop_Click(object sender, EventArgs e)
        {
            //停止发送服务进程
            StopProcessSend();
        }
        /// <summary>
        /// 停止指定进程号[发送服务]的进程
        /// </summary>
        private void StopProcessSend()
        {
            try
            {
                //获取指定文件名的所有进程对象
                Process[] processes = Process.GetProcessesByName(m_fileNameDesp);
                foreach (Process p in processes)
                {
                    //根据关联进程的主模块信息，判定该文件名进程是否启动
                    if (p.ProcessName == m_fileNameDesp)
                    {
                        p.Kill();
                        p.Close();
                        m_isStart = false;
                        //m_thread_monitor.Abort();
                        //m_thread_monitor = null;
                    }
                }
            }
            catch (ThreadAbortException threadabortex)
            { 
            }
            catch (ArgumentException argex)
            {
            }
            catch (Exception ex)
            {
                //进程关闭异常信息
                //MessageBox.Show("Exception:" + ex.Source + " " + ex.Message);
            }
            finally
            {
                //正确关闭后设置相关信息
                if (!CheckProcessExists())
                {
                    m_isStart = false;
                    //Thread.Sleep(500);
                    btnSenderStart.Enabled = true;
                    btnSenderStop.Enabled = false;
                    btnSenderRestart.Enabled = false;
                    txtBoxConnStatus.Text = RUN_STATUS_MEASURE.CONNECT_STATUS[1];
                    txtBoxRunPhase.Text = RUN_STATUS_MEASURE.RUN_STAGE_ARRAY[(int)RUN_PHASE.INVALID];

                    foreach (Control c in listContrl)
                    {
                        c.Enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// 进程退出消息处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendProc_Exited(object sender, System.EventArgs e)
        {
            //T.B.D.
            //isStart = false;
            //btnStopDisableDelegate(btnStopDisable);
        }
        /// <summary>
        /// 按钮开关切换代理函数，跨线程操作安全考虑
        /// </summary>
        /// <param name="ExecuteFun"></param>
        private void ManageControllerDelegate(btnSwitchDelegateDef ExecuteFun)
        {
            //控件是否需要invoke
            if (this.InvokeRequired)
            {
                this.Invoke(ExecuteFun);
            }
            else
            {
                ExecuteFun();
            }
        }
        /// <summary>
        /// 消息处理代理函数，跨线程操作安全考虑
        /// </summary>
        /// <param name="msg"></param>
        private void MsgProcDelegate(MsgBody msg)
        {
            // 判断是否需要Invoke，多线程时需要
            if (InvokeRequired)
            {
                // 通过委托调用写主线程控件的程序，传递参数放在object数组中
                Invoke(new MsgProcDelegateDef(MsgProcDelegate), new object[] { msg });
            }
            else
            {
                MsgProcess(msg);
            }
        }
        /// <summary>
        /// 按钮开关切换函数，由代理完成
        /// </summary>
        private void SetBtnStopDisable()
        {

            //设置按钮状态
            btnSenderStart.Enabled = true;
            btnSenderStop.Enabled = false;
            btnSenderRestart.Enabled = false;
            //设置后台服务状态
            txtBoxConnStatus.Text = RUN_STATUS_MEASURE.CONNECT_STATUS[1];
            txtBoxRunPhase.Text = RUN_STATUS_MEASURE.RUN_STAGE_ARRAY[(int)RUN_PHASE.INVALID];

            //T.B.D.提示框？
        }
        private void SetBtnStartDisable()
        {
            btnSenderStart.Enabled = false;
            btnSenderStop.Enabled = true;
            btnSenderRestart.Enabled = true;
        }

        private void setTxtNetState()
        {
            txtBoxNetState.Text = NetWorkState;
        }
        /// <summary>
        /// 消息处理函数，由代理完成
        /// </summary>
        /// <param name="msg"></param>
        private void MsgProcess(MsgBody msg)
        {
            //根据设置连接状态框
            if (msg.isConnected)
            {
                //正常连接情况
                txtBoxConnStatus.Text = RUN_STATUS_MEASURE.CONNECT_STATUS[0];
            }
            else
            {
                //不正常连接情况
                txtBoxConnStatus.Text = RUN_STATUS_MEASURE.CONNECT_STATUS[1];
            }
            //后台进程运行阶段显示
            txtBoxRunPhase.Text = RUN_STATUS_MEASURE.RUN_STAGE_ARRAY[(int)msg.phase];
        }
        /// <summary>
        /// 重启按钮按下事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSenderRestart_Click(object sender, EventArgs e)
        {
            //重启后台服务进程
            RestartProcessSend();
        }
        /// <summary>
        /// 重启发送服务进程
        /// </summary>
        private void RestartProcessSend()
        {
            object o = new object();
            Monitor.Enter(o);
            //关闭发送服务进程
            StopProcessSend();
            //启动发送服务进程
            StartProcessSend();
            Monitor.Exit(o);
        }
        /// <summary>
        /// 主窗口退出时，自动停止未关闭的进程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SenderUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            //T.B.D.是否可用sender_id来
            if (CheckProcessExists())
            {
                StopProcessSend();
            }
            //T.B.D.
            MsgQueManager.Dispose();
        }
        /// <summary>
        /// 更新按钮按下事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //T.B.D. m_isStart读是否需要加锁
            if (m_isStart)
            {
                MessageBox.Show("修改密钥请停止发送服务");
                return;
            }
            if (!isCompleteCheck)
            {
                isCompleteCheck = true;
                return;
            }
            bool ret = true;
            //SenderUI构造函数里是否要初始内容
            SetConfig setconfig = new SetConfig();
            Configuration config = setconfig.ReadConfig();
            //Configuration config = new Configuration();
            //m_project_id = txtBoxProId.Text;
            //m_gateway_id = txtBoxGateId.Text;
            //m_Ip = txtBoxIP.Text;
            //m_Port = txtBoxPort.Text;
            //m_AesKey = txtBoxAesKey.Text;
            //m_AesIV = txtBoxAesIV.Text;
            //m_Md5Key = txtBoxMd5Key.Text;

            //config.project_id = m_project_id;
            //config.gateway_id = m_gateway_id;
            //config.ip = m_Ip;
            //config.port = m_Port;
            //config.key = m_AesKey;
            //config.iv = m_AesIV;
            //config.md5 = m_Md5Key;

            config.iv = txtBoxAesIV.Text;
            config.key = txtBoxAesKey.Text;
            config.gateway_id = txtBoxGateId.Text;
            config.ip = txtBoxIP.Text;
            config.md5 = txtBoxMd5Key.Text;
            config.port = txtBoxPort.Text;
            config.project_id = txtBoxProId.Text;
            
            config.programid = txtBoxProCode.Text;
            config.techtype = txtBoxTechCode.Text;
            config.syscode = txtBoxSysCode.Text;
            
            config.meterInfo.MD_Code1 = txtBoxGen1.Text;
            config.meterInfo.MD_Code2 = txtBoxGen2.Text;
            config.meterInfo.MB_Code1 = txtBoxOuter1.Text;
            config.meterInfo.MB_Code2 = txtBoxOuter2.Text;
            config.meterInfo.MA_Code1 = txtBoxSun1.Text;
            config.meterInfo.MB_Code2 = txtBoxSun2.Text;
            config.meterInfo.MC_Code1 = txtBoxSunGen1.Text;
            config.meterInfo.MC_Code2 = txtBoxSunGen2.Text;	
            //T.B.D. 单独设定
            ret &= setconfig.WriteConfig(config);
            
            if (ret)
            {
                MessageBox.Show("变更已保存");
            }
            else
            {
                MessageBox.Show("变更保存失败");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void txtBoxAreaCode_Validated(object sender, EventArgs e)
        {
            bool ismatch = false;
            ismatch = MatchedValidateRegex_Controller(txtBoxAreaCode.Text, RegexPattern.REGEX_AREA_CODE);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxAreaCode, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxAreaCode.Focus();
            }
        }

        /// <summary>
        /// 项目编号验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxProId_Validated(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// 采集装置编号验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxGateId_Validated(object sender, EventArgs e)
        {

        }

        private void txtBoxIP_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxIP.Text.Trim(), RegexPattern.REGEX_IP);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxIP, ErrorMessage.ERR_VALIDATE_IP_PATTERN);
                txtBoxIP.Focus();
            }
        }

        private void txtBoxPort_Validated(object sender, EventArgs e)
        {
            int port = 0;
            bool isNum = false;
            isNum = int.TryParse(txtBoxPort.Text, out port);
            if (port > 0 && port <= 65535)
            {
                isCompleteCheck &= true;
                errorProvider1.Clear();
            }
            else
            {
                isCompleteCheck &= false;
                errorProvider1.SetError(txtBoxPort, ErrorMessage.ERR_VALIDATE_PORT_PATTERN);
                txtBoxPort.Focus();
            }
        }

        private void txtBoxAesKey_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxAesKey.Text.Trim(), RegexPattern.REGEX_16CH);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxAesKey, ErrorMessage.ERR_VALIDATE_KEY_PATTERN);
                txtBoxAesKey.Focus();
            }
        }

        private void txtBoxAesIV_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxAesIV.Text.Trim(), RegexPattern.REGEX_16CH);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxAesIV, ErrorMessage.ERR_VALIDATE_KEY_PATTERN);
                txtBoxAesIV.Focus();
            }
        }

        private void txtBoxMd5Key_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxMd5Key.Text.Trim(), RegexPattern.REGEX_16CH);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxMd5Key, ErrorMessage.ERR_VALIDATE_KEY_PATTERN);
                txtBoxMd5Key.Focus();
            }
        }

        private void txtBoxProCode_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxProCode.Text.Trim(), RegexPattern.REGEX_PROJECT_CODE);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxProCode, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxProCode.Focus();
            }
        }

        private void txtBoxTechCode_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxTechCode.Text.Trim(), RegexPattern.REGEX_TECH_TYPE);
            isCompleteCheck &= ismatch;
            if(ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxTechCode, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxTechCode.Focus();
            }
        }

        private void txtBoxSysCode_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxSysCode.Text.Trim(), RegexPattern.REGEX_SYS_CODE);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxSysCode, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxSysCode.Focus();
            }

        }

        private void txtBoxSun1_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxSun1.Text.Trim(), RegexPattern.REGEX_COLLECT_DEVICE);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxSun1, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxSun1.Focus();
            }
        }

        private void txtBoxOuter1_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxOuter1.Text.Trim(), RegexPattern.REGEX_COLLECT_DEVICE);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxOuter1, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxOuter1.Focus();
            }
        }

        private void txtBoxSunGen1_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxSunGen1.Text.Trim(), RegexPattern.REGEX_COLLECT_DEVICE);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxSunGen1, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxSunGen1.Focus();
            }
        }

        private void txtBoxGen1_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxGen1.Text.Trim(), RegexPattern.REGEX_COLLECT_DEVICE);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxGen1, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxGen1.Focus();
            }
        }

        private void txtBoxSun2_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxSun2.Text.Trim(), RegexPattern.REGEX_COLLECT_POINT);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxSun2, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxSun2.Focus();
            }
        }

        private void txtBoxOuter2_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxOuter2.Text.Trim(), RegexPattern.REGEX_COLLECT_POINT);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxOuter2, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxOuter2.Focus();
            }
        }

        private void txtBoxSunGen2_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxSunGen2.Text.Trim(), RegexPattern.REGEX_COLLECT_POINT);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxSunGen2, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxSunGen2.Focus();
            }
        }

        private void txtBoxGen2_Validated(object sender, EventArgs e)
        {
            bool ismatch = MatchedValidateRegex_Controller(txtBoxGen2.Text.Trim(), RegexPattern.REGEX_COLLECT_POINT);
            isCompleteCheck &= ismatch;
            if (ismatch)
            {
                errorProvider1.Clear();
            }
            else
            {
                errorProvider1.SetError(txtBoxGen2, ErrorMessage.ERR_VALIDATE_CODE_PATTERN);
                txtBoxGen2.Focus();
            }
        }
        private bool MatchedValidateRegex_Controller(string parms, string regex)
        {
            if (parms == null || parms.Trim().Length == 0)
            {
                return false;
            }
            System.Text.RegularExpressions.Regex check = new System.Text.RegularExpressions.Regex(regex);
            return check.IsMatch(parms);
        }

        private void txtBoxProId_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxGateId_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxAreaCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxProCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxTechCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxSysCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxSun1_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxOuter1_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxSunGen1_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxGen1_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxSun2_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxOuter2_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxSunGen2_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }

        private void txtBoxGen2_KeyPress(object sender, KeyPressEventArgs e)
        {
            AllowNumInput(sender, e);
        }
        /// <summary>
        /// 限制只允许输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllowNumInput(object sender, KeyPressEventArgs e)
        {
            //判断按键是不是要输入的类型。
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8/* && (int)e.KeyChar != 46*/)
            {
                e.Handled = true;
            }
        }

        private void btnKeyUpdate_Click(object sender, EventArgs e)
        {
            if (!isCompleteCheck)
            {
                isCompleteCheck = true;
                return;
            }
            SetConfig setcfg = new SetConfig();
            Configuration config = new Configuration();
            config.key = txtBoxAesKey.Text;
            config.iv = txtBoxAesIV.Text;
            config.md5 = txtBoxMd5Key.Text;

            bool ret = true;
            ret &= setcfg.WriteSpecailConfig(config, Commands.KEY);
            ret &= setcfg.WriteSpecailConfig(config, Commands.IV);
            ret &= setcfg.WriteSpecailConfig(config, Commands.MD5);
            if (ret)
            {
                MessageBox.Show("更新写入成功");
            }
            else
            {
                MessageBox.Show("更新写入失败");
            }
        }
    }
}
