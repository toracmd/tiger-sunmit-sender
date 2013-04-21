using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RenewEDSenderM
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Support.DataPackage.test();
                //Support.TestDp.TestPack();
            }
            catch (Exception ex)
            {
                LogManager.Logger.WriteWarnLog("Fail！！！");
            }
            //try
            //{
            //    Support.TestDatagram td = new Support.TestDatagram();
            //    td.ConvertData();
            //}
            //catch (Exception ex)
            //{
            //    LogManager.Logger.WriteWarnLog(ex.ToString());
            //}
            //LogManager.Logger.WriteDebugLog("228的16进制字符串是{0}" ,Convert.ToString(228, 16));
            //DbManager.Connect conn = new DbManager.Connect();
            //try
            //{

            //    String s = LogManager.Test.getConnString();
            //}
            //catch (Exception ex)
            //{
            //    LogManager.Logger.WriteErrorLog("LogManager.Test meet {0}", ex);
            //}
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SenderUI());
        }
    }
}
