using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace RenewEDSenderWin
{
    public partial class SenderUI : Form
    {
        /// <summary>
        /// 发送程序的可执行文件名
        /// </summary>
        private static string fileName = "test.txt";
        /// <summary>
        /// 发送服务的进程ID
        /// </summary>
        private static int pid_sender;
        /// <summary>
        /// 发送服务启动状态
        /// </summary>
        private static bool isStart = false;
        public SenderUI()
        {
            InitializeComponent();
            btnSenderStop.Enabled = false;
            btnSenderRestart.Enabled = false;
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
                    p.StartInfo.FileName = System.IO.Path.Combine(Application.StartupPath, fileName);
                    //设置使用操作系统外壳程序启动
                    p.StartInfo.UseShellExecute = true;
                    //p.WaitForInputIdle();
                    //启动进程
                    p.Start();
                    //启动成功后，获取发送服务的进程号
                    pid_sender = p.Id;
                    //设置发送服务进程状态
                    isStart = true;
                    //设置UI按钮状态
                    btnSenderStart.Enabled = false;
                    btnSenderStop.Enabled = true;
                    btnSenderRestart.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                //进程启动异常信息
                MessageBox.Show(ex.Source + " " + ex.Message);
            }
        }
        /// <summary>
        /// 判断指定进程是否存在
        /// </summary>
        /// <returns>true/false</returns>
        private bool CheckProcessExists()
        {
            //获取指定文件名的所有进程对象
            Process[] processes = Process.GetProcessesByName(fileName);
            //遍历该进程对象集合
            foreach (Process p in processes)
            {
                //根据关联进程的主模块信息，判定该文件名进程是否启动
                if (System.IO.Path.Combine(Application.StartupPath, fileName) == p.MainModule.FileName)
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
            }
            catch (Exception ex)
            {
                //进程关闭异常信息
                MessageBox.Show(ex.Source + " " + ex.Message);
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
        }
    }
}
