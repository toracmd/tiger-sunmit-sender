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

namespace RenewEDSenderWin
{
    /// <summary>
    /// T.B.D.
    /// 1、监视进程，一旦失败更改UI
    /// 2、后台程序情况
    /// 3、当前网络状态：连接可用\不可用 http://social.msdn.microsoft.com/Forums/zh-CN/visualcshartzhchs/thread/c89be7e2-592e-4fec-8b72-e2f22f52319b/：
    /// 4、MsgQue还有退出不彻底
    /// 5、退出时其他线程的退出
    /// 6、并发问题
    /// </summary>
    public partial class SenderUI : Form
    {
        /// <summary>
        /// 发送程序的可执行文件名，也是进程名称
        /// </summary>
        // T.B.D. 文件名读配置或者写定
        private static string m_fileNameDesp = "RenewEDSenderM";
        // T.B.D. 文件路径在程序打包后的
        private static string m_filePath = "D:\\workspace\\sunmit\\svn\\RenewEDSenderM\\RenewEDSenderM\\bin\\Debug";
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

        private System.Timers.Timer m_moniter_timer = new System.Timers.Timer();

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
            //初始按钮状态
            btnSenderStop.Enabled = false;
            btnSenderRestart.Enabled = false;

            //Computer c = new Computer();
            //isNetAvailable = c.Network.IsAvailable;
            txtBoxNetState.Text = NetWorkState;
            //c.Network.NetworkAvailabilityChanged +=new NetworkAvailableEventHandler(Network_NetworkAvailabilityChanged);

            m_moniter_timer.Elapsed +=new ElapsedEventHandler(MoniterTimer_Elapsed);
            m_moniter_timer.Interval = 5 * 1000;    //5秒一次
            m_moniter_timer.Enabled = true;

            try
            {
                m_MsgQueManager = new MsgQueManager();
            }
            catch (MessageQueueException mqex)
            {
                MessageBox.Show("消息队列尚未启动:" + mqex);
            }
            MsgQueueRecv();
            //建立消息队列接收线程
            //m_thread_recv_queue = new Thread(new ThreadStart(MsgQueueRecv));
            //启动消息队列接收线程
            //m_thread_recv_queue.Start();

            //方法一：不进行跨线程安全检查  
            //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;  
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
                        //监控发送服务进程状态
                        //m_thread_monitor = new Thread(new ThreadStart(MonitorSendProc));
                        //启动后台服务监视线程
                        //m_thread_monitor.Start();
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
            //SenderUI构造函数里是否要初始内容
            SetConfig setconfig = new SetConfig();
            Configuration config = setconfig.ReadConfig();
            //Configuration config = new Configuration();
            m_project_id = txtBoxProId.Text;
            m_gateway_id = txtBoxGateId.Text;
            m_Ip = txtBoxIP.Text;
            m_Port = txtBoxPort.Text;
            m_AesKey = txtBoxAesKey.Text;
            m_AesIV = txtBoxAesIV.Text;
            m_Md5Key = txtBoxMd5Key.Text;

            config.project_id = m_project_id;
            config.gateway_id = m_gateway_id;
            config.ip = m_Ip;
            config.port = m_Port;
            config.key = m_AesKey;
            config.iv = m_AesIV;
            config.md5 = m_Md5Key;
            //T.B.D. 单独设定
            setconfig.WriteConfig(config);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
