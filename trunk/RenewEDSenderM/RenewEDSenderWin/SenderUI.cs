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

namespace RenewEDSenderWin
{
    /// <summary>
    /// T.B.D.
    /// 1、监视进程，一旦失败更改UI
    /// 2、后台程序情况
    /// 3、当前网络状态 http://social.msdn.microsoft.com/Forums/zh-CN/visualcshartzhchs/thread/c89be7e2-592e-4fec-8b72-e2f22f52319b/
    /// 4、MsgQue还有退出不彻底
    /// </summary>
    public partial class SenderUI : Form
    {
        /// <summary>
        /// 发送程序的可执行文件名
        /// </summary>
        private static string fileNameDesp = "RenewEDSenderM";

        private static string filePath = "D:\\workspace\\sunmit\\svn\\RenewEDSenderM\\RenewEDSenderM\\bin\\Debug";
        /// <summary>
        /// 发送服务的进程ID
        /// </summary>
        private static int pid_sender;
        /// <summary>
        /// 发送服务启动状态
        /// </summary>
        private static bool isStart = false;
        /// <summary>
        /// 项目编号
        /// </summary>
        private static string project_id;
        /// <summary>
        /// 采集装置编号
        /// </summary>
        private static string gateway_id;
        /// <summary>
        /// 服务器IP
        /// </summary>
        private static string strIp;
        /// <summary>
        /// 服务器端口
        /// </summary>
        private static string strPort;
        /// <summary>
        /// AES密钥
        /// </summary>
        private static string strAesKey;
        /// <summary>
        /// AES偏移量
        /// </summary>
        private static string strAesIV;
        /// <summary>
        /// MD5密钥
        /// </summary>
        private static string strMd5Key;
        
        // 新建的代理
        private delegate void btnDisableDelegate();


        public SenderUI()
        {
            InitializeComponent();
            btnSenderStop.Enabled = false;
            btnSenderRestart.Enabled = false;

            //建立消息队列
            //MessageQueue myQueue = new MessageQueue(RenewEDSenderM.Support.MsgQueManager.MessageQueueName);
            //建立消息队列接收线程，另准备消息队列接收完成事件
            Thread thread_recv_queue = new Thread(new ThreadStart(MsgQueueRecv));
            thread_recv_queue.Start();
        }

        public void MsgQueueRecv()
        {
            while (true)
            {
                try
                {
                    MessageQueue msgq = MsgQueManager.getInstance();
                    msgq.Formatter = new XmlMessageFormatter(
                            new Type[]
                        {
                            typeof(MsgBody)
                        }
                        );
                    msgq.ReceiveCompleted += MessageArrived;
                    msgq.BeginReceive();
                }
                catch (MessageQueueException ex)
                {
                    MsgQueManager.Dispose();
                    return;
                }
            }
        }
        public static void MessageArrived(object sender, ReceiveCompletedEventArgs e)
        {
            MessageQueue msgq = (MessageQueue)sender;
            System.Messaging.Message msg = msgq.EndReceive(e.AsyncResult);
            object o = msg.Body;
            //string s = o.ToString();
            MessageBox.Show(msg.Label + o);
        }

        private void btnSenderStart_Click(object sender, EventArgs e)
        {
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
                    //实例化一个进程对象
                    Process p = new Process();
                    //设置该进程对象关联的文件为发送服务
                    //p.StartInfo.FileName = System.IO.Path.Combine(Application.StartupPath, fileName);
                    //T.B.D. For Test
                    p.StartInfo.FileName = System.IO.Path.Combine(filePath, fileNameDesp);

                    //设置使用操作系统外壳程序启动
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.CreateNoWindow = true;
  
                    //设置进程退出时触发此事件
                    p.EnableRaisingEvents = true;
                    //添加进程退出消息处理函数
                    p.Exited += new EventHandler(myProcess_Exited);
                    //启动进程
                    p.Start();
                    //p.WaitForInputIdle(200);
                    
                    //启动成功后，获取发送服务的进程号
                    pid_sender = p.Id;
                    //设置发送服务进程状态
                    isStart = true;
                    //设置UI按钮状态
                    btnSenderStart.Enabled = false;
                    btnSenderStop.Enabled = true;
                    btnSenderRestart.Enabled = true;
                }
                if (!CheckProcessExists())
                {
                    MessageBox.Show("启动失败");
                    isStart = false;
                    btnSenderStart.Enabled = true;
                    btnSenderStop.Enabled = false;
                    btnSenderRestart.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                //进程启动异常信息
                MessageBox.Show("Exception:" + ex.Source + " " + ex.Message);
            }
        }
        /// <summary>
        /// 判断指定进程是否存在
        /// </summary>
        /// <returns>true/false</returns>
        private bool CheckProcessExists()
        {
            //获取指定文件名的所有进程对象
            Process[] processes = Process.GetProcessesByName(fileNameDesp);
            string fileName = fileNameDesp + ".exe";    // >>>> revision 20130517
            //遍历该进程对象集合
            foreach (Process p in processes)
            {
                //根据关联进程的主模块信息，判定该文件名进程是否启动
                if (System.IO.Path.Combine(filePath, fileName) == p.MainModule.FileName)
                    return true;

            }
            return false;
        }

        private void btnSenderStop_Click(object sender, EventArgs e)
        {
            StopProcessSend();
        }
        /// <summary>
        /// 关闭指定进程号[发送服务]的进程
        /// </summary>
        private void StopProcessSend()
        {
            try
            {
                //根据进程号关闭该进程
                Process p = Process.GetProcessById(pid_sender);
                //向该进程对象发送kill信号
                p.Kill();
                //释放资源
                p.Close();
                
            }
            catch (Exception ex)
            {
                //进程关闭异常信息
                MessageBox.Show("Exception:" + ex.Source + " " + ex.Message);
            }
            //正确关闭后设置相关信息
            if (!CheckProcessExists())
            {
                isStart = false;
                btnSenderStart.Enabled = true;
                btnSenderStop.Enabled = false;
                btnSenderRestart.Enabled = false;
            }
        }

        /// <summary>
        /// 进程退出消息处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            isStart = false;
            btnStopDisableDelegate(btnStopDisable);
        }
        /// <summary>
        /// 按钮开关切换代理函数
        /// </summary>
        /// <param name="myDelegate"></param>
        private void btnStopDisableDelegate(btnDisableDelegate myDelegate)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(myDelegate);
            }
            else
            {
                myDelegate();
            }
        }
        /// <summary>
        /// 按钮开关切换函数
        /// </summary>
        private void btnStopDisable()
        {
            btnSenderStart.Enabled = true;
            btnSenderStop.Enabled = false;
            btnSenderRestart.Enabled = false;
        }

        private void btnSenderRestart_Click(object sender, EventArgs e)
        {
            RestartProcessSend();
        }
        /// <summary>
        /// 重启发送服务进程
        /// </summary>
        private void RestartProcessSend()
        {
            //关闭发送服务进程
            StopProcessSend();
            //启动发送服务进程
            StartProcessSend();
        }
        /// <summary>
        /// 主窗口退出时，自动停止未关闭的进程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SenderUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (CheckProcessExists())
            {
                StopProcessSend();
            }
            MsgQueManager.Dispose();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (isStart)
            {
                MessageBox.Show("修改密钥请停止发送服务");
                return;
            }
            SetConfig setconfig = new SetConfig();
            Configuration config = setconfig.ReadConfig();
            //Configuration config = new Configuration();
            project_id = txtBoxProId.Text;
            gateway_id = txtBoxGateId.Text;
            strIp = txtBoxIP.Text;
            strPort = txtBoxPort.Text;
            strAesKey = txtBoxAesKey.Text;
            strAesIV = txtBoxAesIV.Text;
            strMd5Key = txtBoxMd5Key.Text;

            config.project_id = project_id;
            config.gateway_id = gateway_id;
            config.ip = strIp;
            config.port = strPort;
            config.key = strAesKey;
            config.iv = strAesIV;
            config.md5 = strMd5Key;

            setconfig.WriteConfig(config);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
