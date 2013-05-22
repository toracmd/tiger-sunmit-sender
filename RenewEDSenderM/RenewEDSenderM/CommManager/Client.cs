using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Threading;
using System.Runtime.InteropServices;



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
        private static string input_sequence = "000000";
        private static string input_parse = "";
        private static string input_time = "";
        private static int try_count = 0;
        private static Configuration config = null;
        private static bool synchronize = false;
        private static bool isConnected = false;
        private static bool isCreatThread = false;
        private static bool isCreatReport = false;


        static void Main(string[] args)
        {
            DbManager.TestAccessData.Test();
            LogManager.Logger.FuncEntryLog(args);
			while(true)
			{
			//T.B.D. 连接失败后重试停顿点时间
	            try
	            {
	                LogManager.Logger.WriteDebugLog("进入try");
	                Initial();
	                //int port = 13145;
	                //string host = "10.6.0.115";
	                int port = int.Parse(config.port);
                
	                string host = config.ip;
	                LogManager.Logger.WriteDebugLog("准备ip：" + host);
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
                    LogManager.Logger.WriteInfoLog("Conneting…");
	                c.Connect(ipe);//连接到服务器

	                //Rereport();
	                //失败数据重传
                    if (isCreatThread == false)
                    {
                        Thread oThread = new Thread(new ThreadStart(Rereport));
                        isCreatThread = true;
                        oThread.Start();
                    }


	                //定时上报数据，30分钟发一次
                    if (isCreatReport == false)
                    {
                        System.Timers.Timer reportTimer = new System.Timers.Timer();
                        reportTimer.Elapsed += new ElapsedEventHandler(ReportEvent);
                        reportTimer.Interval = Convert.ToInt32(config.reportTime) * 60 * 1000; //配置文件中配置的秒数,30分钟一次
                        reportTimer.Enabled = true;
                        isCreatReport = true;
                    }

	                //进行发送相关操作
	                while (true)
	                {

	                    if (Verify()) //进行认证
	                        Send();//如果认证成功，则进行发送数据，包括心跳数据包等，如果连接失败，则总send状态跳出，重新进行认证
	                    else
	                    {
	                        isConnected = false;
	                        //先关闭已经打开的链接
	                        if (c != null)
	                        {
	                            c.Close();
	                            Console.Write("Closing Used Link....");
                                LogManager.Logger.WriteInfoLog("Closing Used Link....");
	                        }
	                        ///创建socket并连接到服务器
	                        c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建Socket
                            Console.WriteLine("Conneting…");
                            LogManager.Logger.WriteInfoLog("Conneting....");
	                        c.Connect(ipe);//连接到服务器

	                    }
	                }

	            }
	            catch (ArgumentNullException e)
	            {
	                LogManager.Logger.WriteWarnLog("argumentNullException: {0}", e);
	                Console.WriteLine("argumentNullException: {0}", e);
                    LogManager.Logger.WriteWarnLog("argumentNullException: {0}", e);
	            }
	            catch (SocketException e)
	            {
	                LogManager.Logger.WriteWarnLog("SocketException:{0}", e);
	                Console.WriteLine("SocketException:{0}", e);
                    LogManager.Logger.WriteWarnLog("SocketException:{0}", e);
	            }
	            c.Close();
			}
            //Console.ReadLine();
            Console.WriteLine("Press Enter to Exit");
            LogManager.Logger.FuncExitLog();
        }
        //进行初始化操作，主要是读取配置文件中的参数
        private static void Initial()
        {
            SetConfig set_config = new SetConfig();
            config = set_config.ReadConfig();
        }

        //imports SetLocalTime function from kernel32.dll 
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SetLocalTime(ref SystemTime lpSystemTime);

        //struct for date/time apis 
        public struct SystemTime
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
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
            LogManager.Logger.WriteInfoLog("Verifying.....");
            ///向服务器发送信息
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            int bytes;
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(VerifyEvent);
            aTimer.Interval = 3 * 1000;    //配置文件中配置的秒数
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            XmlProcessManager.XMLRead xmlread = new XmlProcessManager.XMLRead();
            XmlProcessManager.Order order = new XmlProcessManager.Order();

            //包括传输认证数据，超时重传和错误重传

            //发起身份认证
            xmlwrite.Input(xmlStr, project_id, gateway_id,config.key,config.iv);
            xmlwrite.Request();
            SendMsgB(xmlwrite.BOutput());
            aTimer.Enabled = true;


            //接收数据，并进行超时判断
            recvStr = "";
            while (true)
            {
                if (try_count >= 5)
                {
                    Console.Write("Try 5 times!");
                    LogManager.Logger.WriteInfoLog("Try 5 times!");
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
                xmlread.BInput(rByte,config.key,config.iv);
                //xmlread.Input(rStr1);
                order = xmlread.Output();
                input_sequence = order.sequence;
            }

            xmlwrite.Input(xmlStr, project_id, gateway_id,config.key,config.iv);
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
                    LogManager.Logger.WriteInfoLog("Try 5 times!");
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
                xmlread.BInput(rByte1,config.key,config.iv);
                order = xmlread.Output();
            }

            //判断是否认证成功
            if (order.result == "pass")
            {
                synchronize = true;
                isConnected = true;
                SystemTime systNew = new SystemTime();
                string years = order.time.Substring(0,4);
                string months = order.time.Substring(4, 2);
                string days = order.time.Substring(6,2);
                string hours = order.time.Substring(8, 2);
                string minutes = order.time.Substring(10, 2);
                string seconds = order.time.Substring(12, 2);
                systNew.wDay = short.Parse(days);
                systNew.wMonth = short.Parse(months);
                systNew.wYear = short.Parse(years);
                systNew.wHour = short.Parse(hours);
                systNew.wMinute = short.Parse(minutes);
                systNew.wSecond = short.Parse(seconds);
                SetLocalTime(ref systNew);
                return true;
            }
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
            System.Timers.Timer notifyTimer = new System.Timers.Timer();
            notifyTimer.Elapsed += new ElapsedEventHandler(NotifyEvent);
            notifyTimer.Interval = 60 * 1000;    //配置文件中配置的秒数,60秒一次


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
                    notifyTimer.Enabled = false;    //心跳包超时，需要重新连接服务器端，停止继续发送数据
                    Console.Write("HeartBeat Time out!");//可以写入log，进行记录
                    LogManager.Logger.WriteInfoLog("HeartBeat Time out!");
                    return;                         //直接返回，重新认证
                }

                //从接收到的信息中解析出相应的命令
                int i;
                byte[] rByte = new byte[bytes];
                Array.Copy(recvBytes, rByte, bytes);
                xmlread.BInput(rByte,config.key,config.iv);
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
		//true 发送成功
		//false 发送失败
		//T.B.D. 发送异常？
        private static bool SendMsgB(byte[] sendByte)
        {
            Console.WriteLine("Send Message");
            LogManager.Logger.WriteInfoLog("Send Message");
            int sendLength;
            sendLength = c.Send(sendByte, sendByte.Length, 0);//发送信息
            if (sendLength == sendByte.Length)
                return true;
            else
                return false;

        }
        //单纯发送相应的字符串数据
        private static void SendMsg(string sendStr)
        {
            string TestStr = sendStr;
            int i;
            byte[] dp = Support.DataPackage.GetBytes(TestStr, out i);
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
            xmlwrite.Input(xmlStr, project_id, gateway_id,config.key,config.iv);
            xmlwrite.Notify();
            if(SendMsgB(xmlwrite.BOutput())== false)
                isConnected = false;
        }

        //定时发送数据，需要收集数据
        private static void ReportEvent(object sender, ElapsedEventArgs e)
        {
            //如果进行了同步则进行相关操作，否则不进行任何操作
            if (synchronize)
            {
                //数据写入数据库

                //如果网络状况良好，则发送数据
                if (isConnected)
                {
                    //XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
                    //xmlwrite.Input(xmlStr, project_id, gateway_id, config.key, config.iv);
                    ////这部分需要采集数据进行数据录入
                    ////>>>> T.B.D. 测试数据
                    ////if(true)
                    ////{
                    //    DateTime date_send = new DateTime(2013, 4, 28, 19, 0, 0);   //2013-04-28 19:00:00
                    //    TimeSpan ts = new TimeSpan(2, 0, 0);    //2小时的间隔
                    //    DbManager.History_Data hd_array;
                    //    DbManager.DataDump.CalculateAverage(date_send, ts);
                    //    string input_parse = "yes";
                    //    DataInfo[] input_info = new DataInfo[4];
                    //    for (int i = 0; i < 4; i++)
                    //        input_info[i] = new DataInfo();
                    //    string[] ids = GenerateFunID(config.areacode, config.programid, config.techtype, config.syscode);
                    //    input_info[0].data = Convert.ToString(hd_array.ValueA);
                    //    input_info[0].mid = ids[0];
                    //    input_info[0].fid = ids[0];
                    //    input_info[1].data = Convert.ToString(hd_array.ValueB);
                    //    input_info[1].mid = ids[1];
                    //    input_info[1].fid = ids[1];
                    //    input_info[2].data = Convert.ToString(hd_array.ValueC);
                    //    input_info[2].mid = ids[2];
                    //    input_info[2].fid = ids[2];
                    //    input_info[3].data = Convert.ToString(hd_array.ValueD);
                    //    input_info[3].mid = ids[3];
                    //    input_info[3].fid = ids[3];
                    ////}
                    ////<<<<
                    //Random random = new Random();
                    //int sequence = random.Next(10000000, 99999999);
                    //string sequenceStr = sequence.ToString();
                    //xmlwrite.Report(sequenceStr, input_parse, hd_array.timestamp_sendCycle.ToString("yyyyMMddHHmmss"), input_info);

                    //if(SendMsgB(xmlwrite.BOutput()))
                    //    DbManager.DataDump.update_Upload(hd_array.id);
                }
  
            }
            else
                return;
        }

        //应答查询操作，与数据库交互
        private static void Reply(XmlProcessManager.Order order)
        {
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            //数据库查询，并输出相关历史数据的参数
            xmlwrite.Input(xmlStr, project_id, gateway_id, config.key, config.iv);
            Support.Encryption.MD5_KEY_STR = config.md5;

            //其中的sequence parse time和input_info应该从数据库中读取
            DataInfo[] input_info = new DataInfo[4];
            for (int i = 0; i < 4; i++)
                input_info[i] = new DataInfo();
            string input_parse = "yes";
            //组合id

            //提取时间
            int start_years = int.Parse(order.beginTime.Substring(0, 4));
            int start_months = int.Parse(order.beginTime.Substring(4, 2));
            int start_days = int.Parse(order.beginTime.Substring(6, 2));
            int start_hours = int.Parse(order.beginTime.Substring(8, 2));
            int start_minutes = int.Parse(order.beginTime.Substring(10, 2));
            int start_seconds = int.Parse(order.beginTime.Substring(12, 2));

            int end_years = int.Parse(order.endTime.Substring(0, 4));
            int end_months = int.Parse(order.endTime.Substring(4, 2));
            int end_days = int.Parse(order.endTime.Substring(6, 2));
            int end_hours = int.Parse(order.endTime.Substring(8, 2));
            int end_minutes = int.Parse(order.endTime.Substring(10, 2));
            int end_seconds = int.Parse(order.endTime.Substring(12, 2));

            DateTime begin_time = new DateTime(start_years, start_months, start_days, start_hours, start_minutes, start_seconds);
            DateTime end_time = new DateTime(end_years, end_months, end_days, end_hours, end_minutes, end_seconds);

            DbManager.History_Data[] hd_array = DbManager.DataDump.FetchDataSuccess(begin_time, end_time);

            if (hd_array != null)
            {
                string [] ids = GenerateFunID(config.areacode,config.programid,config.techtype,config.syscode);
                int sampleCount;
                sampleCount = hd_array.Length;
                for (int i = 0; i < sampleCount; i++)
                {

                    input_info[0].data = Convert.ToString(hd_array[i].ValueA);
                    input_info[0].mid = ids[0];
                    input_info[0].fid = ids[0];
                    input_info[1].data = Convert.ToString(hd_array[i].ValueB);
                    input_info[1].mid = ids[1];
                    input_info[1].fid = ids[1];
                    input_info[2].data = Convert.ToString(hd_array[i].ValueC);
                    input_info[2].mid = ids[2];
                    input_info[2].fid = ids[2];
                    input_info[3].data = Convert.ToString(hd_array[i].ValueD);
                    input_info[3].mid = ids[3];
                    input_info[3].fid = ids[3];
                    Random random = new Random();
                    int sequence = random.Next(10000000, 99999999);
                    string sequenceStr = sequence.ToString();
                    xmlwrite.Query(sequenceStr, input_parse, hd_array[i].timestamp_sendCycle.ToString("yyyyMMddHHmmss"), input_info);
                    if(SendMsgB(xmlwrite.BOutput()))
                        DbManager.DataDump.update_Upload(hd_array[i].id);
                }
            }
        }

        //发送周期配置应答信息
        private static void Period_ack(XmlProcessManager.Order order)
        {
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            xmlwrite.Input(xmlStr, project_id, gateway_id,config.key,config.iv);
            xmlwrite.Period_Ack();
			/*
            string period_string = xmlwrite.Output();
            SendMsg(period_string);
			*/
			SendMsgB(xmlwrite.BOutput());
        }

        //通过获取的服务器的设置密钥命令来进行密钥的配置
        private static void SetKey(XmlProcessManager.Order order)
        {
            SetConfig set_config = new SetConfig();

            set_config.WriteSpecailConfig(config, order.order);
        }

        //失败数据重传
        private static void Rereport()
        {
            while(true)
            {
                //如果网络状况良好则进行重传
                if (/*isConnected*/true)
                {
                    XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
                    xmlwrite.Input(xmlStr,project_id,gateway_id,config.key,config.iv);
                    Support.Encryption.MD5_KEY_STR = config.md5;

                    DataInfo[] input_info = new DataInfo[4];
                    for (int i = 0;i < 4;i++)
                        input_info[i] = new DataInfo();
                    string input_parse = "yes";
                    DateTime begin_time = new DateTime(1900, 1, 1, 1, 1, 1);
                    DateTime end_time = new DateTime(3000, 1, 1, 1, 1, 1);
                    DbManager.History_Data[] hd_array = DbManager.DataDump.FetchDataFail(begin_time, end_time);
                    if (hd_array != null)
                    {
                        string[] ids = GenerateFunID(config.areacode, config.programid, config.techtype, config.syscode);
                        int sampleCount;
                        sampleCount = hd_array.Length;
                        for (int i = 0; i < sampleCount; i++)
                        {
                            input_info[0].data = Convert.ToString(hd_array[i].ValueA);
                            input_info[0].mid = ids[0];
                            input_info[0].fid = ids[0];
                            input_info[1].data = Convert.ToString(hd_array[i].ValueB);
                            input_info[1].mid = ids[1];
                            input_info[1].fid = ids[1];
                            input_info[2].data = Convert.ToString(hd_array[i].ValueC);
                            input_info[2].mid = ids[2];
                            input_info[2].fid = ids[2];
                            input_info[3].data = Convert.ToString(hd_array[i].ValueD);
                            input_info[3].mid = ids[3];
                            input_info[3].fid = ids[3];
                            Random random = new Random();
                            int sequence = random.Next(10000000,99999999);
                            string sequenceStr = sequence.ToString();
                            xmlwrite.Report(sequenceStr, input_parse, hd_array[i].timestamp_sendCycle.ToString("yyyyMMddHHmmss"), input_info);
                            if (SendMsgB(xmlwrite.BOutput()))
                                DbManager.DataDump.update_Upload(hd_array[i].id);
                        }
                    }
                    
                }
            }
        }

        /// <summary>
        /// 采集指标编码-平行于光伏组件的太阳辐照度
        /// </summary>
        public static readonly string COLLECT_FACTOR_CODE_RADIATION = "01";
        /// <summary>
        /// 采集指标编码-室外温度
        /// </summary>
        public static readonly string COLLECT_FACTOR_CODE_AIRTEMP = "02";
        /// <summary>
        /// 采集指标编码-光伏组件背面表面温度
        /// </summary>
        public static readonly string COLLECT_FACTOR_CODE_LANDTEMP = "03";
        /// <summary>
        /// 采集指标编码-发电量
        /// </summary>
        public static readonly string COLLECT_FACTOR_CODE_ELECTRICITY = "14";

        /// <summary>
        /// 生成计量装置的具体采集功能编号
        /// 1 平行于光伏组件的太阳辐照度
        /// 2 室外温度 
        /// 3 光伏组件背面表面温度
        /// 4 发电量
        /// </summary>
        /// <param name="Area_code">行政区编码</param>
        /// <param name="Program_id">项目编码</param>
        /// <param name="Tech_type">技术类型</param>
        /// <param name="Sys_code">系统编码</param>
        /// <returns>计量装置的具体采集功能编号</returns>
        public static string[] GenerateFunID(string Area_code, string Program_id, string Tech_type, string Sys_code)
        {
            string code_prefix = Area_code + Program_id + Tech_type + Sys_code;
            string[] code_array = new string[4];
            code_array[0] = code_prefix + COLLECT_FACTOR_CODE_RADIATION;
            code_array[1] = code_prefix + COLLECT_FACTOR_CODE_AIRTEMP;
            code_array[2] = code_prefix + COLLECT_FACTOR_CODE_LANDTEMP;
            code_array[3] = code_prefix + COLLECT_FACTOR_CODE_ELECTRICITY;
            return code_array;
        }


    }
}
