using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Pipes;
using System.Diagnostics;
using System.Threading;

namespace RenewEDSenderM
{
    public partial class SenderUI : Form
    {
        /// <summary>
        /// 创建发送数据工作线程对象，未启动
        /// </summary>
        private static CtrlManager.SendWorker sendWorkerObj = new CtrlManager.SendWorker();
        private static Thread sendThread = new Thread(sendWorkerObj.DoWork);
        public SenderUI()
        {
            InitializeComponent();
        }

        private void btnStartSender_Click(object sender, EventArgs e)
        {
            LogManager.Logger.FuncEntryLog(sender, e);
            sendThread.IsBackground = true;
            //启动收集线程
            sendThread.Start();
            //等待线程启动成功
            while (!sendThread.IsAlive) ;
            LogManager.Logger.FuncExitLog();
        }

        private void btnStopSender_Click(object sender, EventArgs e)
        {
            sendWorkerObj.RequestStop();
        }
    }
}
