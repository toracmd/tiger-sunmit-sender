using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Timers;


namespace RenewEDSenderM.CommManager
{
    class Client
    {
        private static Socket c = null;
        private static int timeout = 0;
        private static int vtimeout = 0;
        private static string verifyStr = "";
        private static string xmlStr = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><root><common><project_id><!-- 项目编号 --></project_id><gateway_id><!-- 采集装置编号 --></gateway_id><type>request</type></common></root>";
        private static string project_id = "110000015";
        private static string gateway_id = "1100000140202";
        private static string input_sequence = "";
        private static string input_parse = "";
        private static string input_time = "";
        private static int try_count = 0;
        private static Configuration config = null;

        static void Main(string[] args)
        {
            try
            {
                Initial();
                //int port = 13145;
                //string host = "10.6.0.115";
                int port = int.Parse(config.port);
                string host = config.ip;
                project_id = config.project_id;
                gateway_id = config.gateway_id;
                /**/
                ///创建终结点EndPoint
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口转化为IPEndpoint实例

                /**/
                ///创建socket并连接到服务器
                c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建Socket
                Console.WriteLine("Conneting…");
                c.Connect(ipe);//连接到服务器

                //进行发送相关操作
                while (true)
                {

                    if (Verify()) //进行认证
                        Send();//如果认证成功，则进行发送数据，包括心跳数据包等，如果连接失败，则总send状态跳出，重新进行认证
                    else
                    {
                        //先关闭已经打开的链接
                        if (c != null)
                        {
                            c.Close();
                            Console.Write("Closing Used Link....");
                        }
                        ///创建socket并连接到服务器
                        c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建Socket
                        Console.WriteLine("Conneting…");
                        c.Connect(ipe);//连接到服务器
                    }
                }

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("argumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException:{0}", e);
            }
            c.Close();
            Console.ReadLine();
            Console.WriteLine("Press Enter to Exit");
        }
        //进行初始化操作，主要是读取配置文件中的参数
        private static void Initial()
        {
            SetConfig set_config = new SetConfig();
            config = set_config.ReadConfig();
        }

        //解析并判断接收数据是否正确，若正确，则分离出相应的命令和参数
        private static bool Verify()
        {
            //初始化各种参数
            timeout = 0;
            vtimeout = 0;
            verifyStr = "";
            input_sequence = "";
            input_parse = "";
            input_time = "";

            Console.Write("Verifying.....");
            ///向服务器发送信息
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            int bytes;
            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(VerifyEvent);
            aTimer.Interval = 3 * 1000;    //配置文件中配置的秒数
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            XmlProcessManager.XMLRead xmlread = new XmlProcessManager.XMLRead();
            XmlProcessManager.Order order = new XmlProcessManager.Order();

            //包括传输认证数据，超时重传和错误重传

            //发起身份认证
            xmlwrite.Input(xmlStr, project_id, gateway_id);
            xmlwrite.Request();
            verifyStr = xmlwrite.Output();
            aTimer.Enabled = true;


            //接收数据，并进行超时判断
            recvStr = "";
            while (true)
            {
                if (try_count >= 5)
                {
                    Console.Write("Try 5 times!");
                    try_count = 0;
                    return false;
                }

                //进行认证数据的接收，如果接收到数据,进入下一个阶段，并关闭定时器避免重传
                if ((bytes = c.Receive(recvBytes, recvBytes.Length, 0)) != 0)
                {

                    recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                    aTimer.Enabled = false;
                    break;
                }
            }

            //接收到认证信息
            if (bytes != 0)
            {
                int i;
                byte[] rByte = new byte[bytes];
                Array.Copy(recvBytes, rByte, bytes);
                xmlread.BInput(rByte);
                //xmlread.Input(rStr1);
                order = xmlread.Output();
                input_sequence = order.sequence;
            }

            xmlwrite.Input(xmlStr, project_id, gateway_id);
            Support.Encryption.MD5_KEY_STR = config.md5;
            string md5Str = Support.Encryption.getMd5Hash(order.sequence);
            xmlwrite.SendMD5(md5Str);
            verifyStr = xmlwrite.Output();
            SendMsgB(xmlwrite.BOutput());
            aTimer.Enabled = true;


            //接收数据，并进行超时判断
            recvStr = "";
            while (true)
            {
                if (try_count >= 5)
                {
                    Console.Write("Try 5 times!");
                    try_count = 0;
                    return false;
                }
                //进行认证数据的接收，如果接收到数据,进入下一个阶段，并关闭定时器避免重传
                if ((bytes = c.Receive(recvBytes, recvBytes.Length, 0)) != 0)
                {


                    recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                    aTimer.Enabled = false;
                    break;
                }
            }

            //接收到认证信息
            if (bytes != 0)
            {
                int i;
                byte[] rByte1 = new byte[bytes];
                Array.Copy(recvBytes, rByte1, bytes);
                xmlread.BInput(rByte1);
                order = xmlread.Output();
            }

            //判断是否认证成功
            if (order.result == "pass")
                return true;
            else
                return false;

        }

        //向服务器端传输数据
        public static void Send()
        {
            ///向服务器发送信息
            string recvStr = "";
            byte[] recvBytes = new byte[1024];

            //心跳数据包，30秒发一次
            Timer notifyTimer = new Timer();
            notifyTimer.Elapsed += new ElapsedEventHandler(NotifyEvent);
            notifyTimer.Interval = 60 * 1000;    //配置文件中配置的秒数,60秒一次

            //定时上报数据，30分钟发一次
            Timer reportTimer = new Timer();
            reportTimer.Elapsed += new ElapsedEventHandler(ReportEvent);
            reportTimer.Interval = 30 * 60 * 1000; //配置文件中配置的秒数,30分钟一次
            reportTimer.Enabled = true;

            XmlProcessManager.XMLRead xmlread = new XmlProcessManager.XMLRead();
            XmlProcessManager.Order order;
            int bytes = 0;

            //包括传输数据，超时重传和错误重传
            while (true)
            {
                notifyTimer.Enabled = true;
                recvStr = "";
                //接收数据，并进行超时判断
                while (true)
                {
                    if (timeout == 1)
                        break;
                    //进行数据的接收，如果接收到数据则下一个阶段，并关闭定时器避免重传
                    if ((bytes = c.Receive(recvBytes, recvBytes.Length, 0)) != 0)
                    {
                        recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                        notifyTimer.Enabled = false;
                        timeout = 0;
                        break;
                    }
                }

                //分析接收到的信息
                //如果超时，则跳出循环进入重新认证链接
                if (timeout == 1)
                {
                    reportTimer.Enabled = false;    //心跳包超时，需要重新连接服务器端，停止继续发送数据
                    Console.Write("HeartBeat Time out!");//可以写入log，进行记录
                    return;                         //直接返回，重新认证
                }

                //从接收到的信息中解析出相应的命令
                int i;
                byte[] rByte = new byte[bytes];
                Array.Copy(recvBytes, rByte, bytes);
                xmlread.BInput(rByte);
                order = xmlread.Output();

                //对于服务器端的查询命令进行回应
                if (order.order == "query")
                {
                    Reply(order);
                    continue;
                }

                //对于服务器端的周期配置命令进行回应
                if (order.order == "period")
                {
                    Period_ack(order);
                    continue;
                }

                if (order.order == "setkey")
                {
                    SetKey(order);
                    continue;
                }
            }

        }

        //单纯发送相应的比特流数据
        private static void SendMsgB(byte[] sendByte)
        {
            Console.WriteLine("Send Message");
            c.Send(sendByte, sendByte.Length, 0);//发送信息

        }
        //单纯发送相应的字符串数据
        private static void SendMsg(string sendStr)
        {
            string TestStr = sendStr;
            int i;
            byte[] dp = Support.DataPackage1.GetBytes(TestStr, out i);
            byte[] bs = Encoding.ASCII.GetBytes(TestStr);//把字符串编码为字节
            Console.WriteLine("Send Message");

            c.Send(dp, dp.Length, 0);//发送信息
        }

        private static void VerifyEvent(object sender, ElapsedEventArgs e)
        {
            try_count++;
            vtimeout = 1;
            Console.Write("Time out!");//可以写入log
            SendMsg(verifyStr);
        }

        //心跳操作的超时处理，将timeout设置为1
        private static void NotifyEvent(object sender, ElapsedEventArgs e)
        {
            timeout = 1;
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            xmlwrite.Input(xmlStr, project_id, gateway_id);
            xmlwrite.Notify();
            SendMsgB(xmlwrite.BOutput());
        }

        //定时发送数据，需要收集数据
        private static void ReportEvent(object sender, ElapsedEventArgs e)
        {
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            xmlwrite.Input(xmlStr, project_id, gateway_id);
            //这部分需要采集数据进行数据录入
            DataInfo[] input_info = new DataInfo[2];
    
            xmlwrite.Report(input_sequence, input_parse, input_time, input_info);
            SendMsgB(xmlwrite.BOutput());
        }

        //应答查询操作，与数据库交互
        private static void Reply(XmlProcessManager.Order order)
        {
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            //数据库查询，并输出相关历史数据的参数


            //其中的sequence parse time和input_info应该从数据库中读取
            DataInfo[] input_info = new DataInfo[2];

            xmlwrite.Query(input_sequence, input_parse, input_time, input_info);
            SendMsgB(xmlwrite.BOutput());
        }

        //发送周期配置应答信息
        private static void Period_ack(XmlProcessManager.Order order)
        {
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            xmlwrite.Input(xmlStr, project_id, gateway_id);
            xmlwrite.Period_Ack();
            string period_string = xmlwrite.Output();
            SendMsg(period_string);
        }

        //通过获取的服务器的设置密钥命令来进行密钥的配置
        private static void SetKey(XmlProcessManager.Order order)
        {
            SetConfig set_config = new SetConfig();

            set_config.WriteSpecailConfig(config, order.order);
        }

    }
}
