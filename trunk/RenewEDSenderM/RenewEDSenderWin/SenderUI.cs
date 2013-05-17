using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using RenewEDSenderM;

using System.Runtime.InteropServices;

namespace RenewEDSenderWin
{
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
                    //p.StartInfo.FileName = System.IO.Path.Combine(Application.StartupPath, fileName);
                    //T.B.D. For Test
                    p.StartInfo.FileName = System.IO.Path.Combine(filePath, fileNameDesp);

                    //设置使用操作系统外壳程序启动
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.CreateNoWindow = true;
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (isStart)
            {
                MessageBox.Show("修改密钥请停止发送服务");
                return;
            }
            strIp = txtBoxIP.Text;
            strPort = txtBoxPort.Text;
            strAesKey = txtBoxAesKey.Text;
            strAesIV = txtBoxAesIV.Text;
            strMd5Key = txtBoxMd5Key.Text;
        }
    }
}
