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

        static void ClientMain(string[] args)
        {
            try
            {
                int port = 13145;
                string host = "183.243.189.162";
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

        //向服务器端传输数据
        public static void Send()
        {
            ///向服务器发送信息
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            int bytes;
            //心跳数据包，30秒发一次
            Timer notifyTimer = new Timer();
            notifyTimer.Elapsed += new ElapsedEventHandler(NotifyEvent);
            notifyTimer.Interval = 30 * 1000;    //配置文件中配置的秒数,30秒一次

            //定时上报数据，30分钟发一次
            Timer reportTimer = new Timer();
            reportTimer.Elapsed += new ElapsedEventHandler(ReportEvent);
            reportTimer.Interval = 30 * 60 * 1000; //配置文件中配置的秒数,30分钟一次
            reportTimer.Enabled = true;


            XmlProcessManager.XMLRead xmlread = new XmlProcessManager.XMLRead();
            XmlProcessManager.Order order= new XmlProcessManager.Order();


            //包括传输数据，超时重传和错误重传
            while (true)
            {
                notifyTimer.Enabled = true;
                recvStr = "";
                //接收数据，并进行超时判断
                while (true)
                {
                    if(timeout==1)
                        break;
                    //进行数据的接收，如果接收到数据则下一个阶段，并关闭定时器避免重传
                    if((bytes = c.Receive(recvBytes, recvBytes.Length, 0)) != 0 )
                    {
                        recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                        notifyTimer.Enabled = false;
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
                xmlread.Input(recvStr);
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
            xmlwrite.Input(xmlStr,project_id,gateway_id);
            xmlwrite.Request();
            verifyStr = xmlwrite.Output();
            verifyStr = "68681616e40000001692ec42AD4E729DCD81551ABC7C2983C86D3C6A58A862EAB7762371A0F6A3C83FBDC8EC79D54004F3249D83EE1D8BBE3C906270C99C44D6CF4C7B049B643A7DA312DCC0C5600F18FBB91ADF460802532B9FD2C766EC60477242D78F9CAA0092DA2F1B0492D92951E9EF1E68096B7EA76C41ABF30EB177D316196693D66C3C8AB2C767ED461BABF65A472C57DE9C972B10EDE61E916F38D678C48ED447928E77EF77798D71F71B29AA4A2542C866B4A623FDC6F8838B2D3AD851EC9091C029072A49BB9DDFCCF2831858E339FE73534A65432E5AC65F3E0DF735510BCD64F8B7C2B1F264416455AA55AA";
            SendMsgB(xmlwrite.BOutput());
            //SendMsg(verifyStr);
            //aTimer.Enabled = true;


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
                //if (vtimeout == 1)
                //    break;
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
                Array.Copy(recvBytes,rByte,bytes);
                xmlread.BInput(rByte);
                //xmlread.Input(rStr1);
                order = xmlread.Output();
                input_sequence = order.sequence;
            }

            xmlwrite.Input(xmlStr,project_id,gateway_id);
            //这里的order是错误的用法，应该是md5值在order中，这里缺失本地序列进行md5加密
            //xmlwrite.SendMD5(order);
            string md5Str = Support.Encryption.getMd5Hash(order.sequence);
            xmlwrite.SendMD5(md5Str);
            verifyStr = xmlwrite.Output();
            SendMsgB(xmlwrite.BOutput());
            //SendMsg(verifyStr);
           // aTimer.Enabled = true;


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
                //if (vtimeout == 1)
                //    break;
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
                //xmlread.Input(rStr1);
                order = xmlread.Output();
            }

            //判断是否认证成功
            if (order.result == "pass")
                return true;
            else
                return false;
            
        }

        ////**测试用：解析并判断接收数据是否正确，若正确，则分离出相应的命令和参数
        //private static bool Verify(int process,string recvStr)
        //{   
        //    if (recvStr == "yes")
        //        return true;
        //    else
        //        return false;
        //}

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
            byte[] dp = Support.DataPackage1.GetBytes(TestStr,out i);
            byte[] bs = Encoding.ASCII.GetBytes(TestStr);//把字符串编码为字节
            Console.WriteLine("Send Message");

            //c.Send(bs, bs.Length, 0);//发送信息
            c.Send(dp, dp.Length, 0);//发送信息
        }

        ////产生下一级别的发送信息并封装成xml且进行了加密打包等操作
        ////其中Order是一个类，用于储存服务器端传来的各种命令及参数
        //private static void CreateNextMsg(int process,Order order)
        //{
        //    XMLWrite xmlwrite=new XMLWrite();

        //    switch(process)
        //    {
        //        case 0: xmlwrite.Request();
        //            break;
        //        case 1: xmlwrite.SendMD5(order.MD5);
        //            break;
        //        default:break;
        //    }
        //}

        ////**测试用：产生下一级别的发送信息并封装成xml且进行了加密打包等操作
        ////其中Order是一个类，用于储存服务器端传来的各种命令及参数
        //private static void CreateNextMsg()
        //{
        //    Console.Write("Create next level！");
        //    sendStr=sendStr+"2";
        //}


        //定时器事件，到时间就将timeout设置为1，表明超时
        //private static void SendEvent(object sender, ElapsedEventArgs e)
        //{
        //    timeout = 1;
        //    Console.Write("Time out!");
        //    SendMsg();
        //}

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
        }

        //定时发送数据，需要收集数据
        private static void ReportEvent(object sender, ElapsedEventArgs e)
        {
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            xmlwrite.Input(xmlStr, project_id, gateway_id);
            //xmlwrite.Report();
            string reports = xmlwrite.Output();
            SendMsg(reports);
        }

        //应答查询操作，与数据库交互
        private static void Reply(XmlProcessManager.Order order)
        {
            XmlProcessManager.XMLWrite xmlwerite = new XmlProcessManager.XMLWrite();


        }

        //发送周期配置应答信息
        private static void Period_ack(XmlProcessManager.Order order)
        {
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            xmlwrite.Input(xmlStr,project_id,gateway_id);
            xmlwrite.Period_Ack();
            string period_string = xmlwrite.Output();
            SendMsg(period_string);
        }

        //通过获取的服务器的设置密钥命令来进行密钥的配置
        private static void SetKey(XmlProcessManager.Order order)
        {

        }

    }
}
