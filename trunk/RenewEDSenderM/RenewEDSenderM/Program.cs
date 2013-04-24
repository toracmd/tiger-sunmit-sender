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
                Support.TestEncrpt.TestGetKeyMd5("40dfbc87592be8a");
                Support.TestEncrpt.TestGetMd5("40dfbc87592be8a0000000000123456");
                Support.TestEncrpt.TestGetKeyMd5_Error("40dfbc87592be8a");
                Support.Encryption.testcDes_aes();

                int i;
                //s vs 6441
                string s = "68681616e40000001692ec42AD4E729DCD81551ABC7C2983C86D3C6A58A862EAB7762371A0F6A3C83FBDC8EC79D54004F3249D83EE1D8BBE3C906270C99C44D6CF4C7B049B643A7DA312DCC0C5600F18FBB91ADF460802532B9FD2C766EC60477242D78F9CAA0092DA2F1B0492D92951E9EF1E68096B7EA76C41ABF30EB177D316196693D66C3C8AB2C767ED461BABF65A472C57DE9C972B10EDE61E916F38D678C48ED447928E77EF77798D71F71B29AA4A2542C866B4A623FDC6F8838B2D3AD851EC9091C029072A49BB9DDFCCF2831858E339FE73534A65432E5AC65F3E0DF735510BCD64F8B7C2B1F2646441";
                string ss = "68681616e40000001692ec42AD4E729DCD81551ABC7C2983C86D3C6A58A862EAB7762371A0F6A3C83FBDC8EC79D54004F3249D83EE1D8BBE3C906270C99C44D6CF4C7B049B643A7DA312DCC0C5600F18FBB91ADF460802532B9FD2C766EC60477242D78F9CAA0092DA2F1B0492D92951E9EF1E68096B7EA76C41ABF30EB177D316196693D66C3C8AB2C767ED461BABF65A472C57DE9C972B10EDE61E916F38D678C48ED447928E77EF77798D71F71B29AA4A2542C866B4A623FDC6F8838B2D3AD851EC9091C029072A49BB9DDFCCF2831858E339FE73534A65432E5AC65F3E0DF735510BCD64F8B7C2B1F26";
                string p = s + "55aa55aa";
                byte[] b = Support.DataPackage.GetBytes(s, out i);
                byte[] bb = Support.DataPackage.GetBytes(ss, out i);
                byte[] pp = Support.DataPackage.GetBytes(p, out i);
                Support.Encryption.CRC16(b);
                Support.Encryption.CRC16(bb);
                Support.DataPackage dp = new Support.DataPackage() { Package = pp };
                //Support.Testt.testInit();
                //Support.Testt.testInit2();
                //Support.DataPackage.test();
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
