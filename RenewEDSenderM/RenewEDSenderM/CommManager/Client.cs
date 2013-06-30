using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Threading;
using System.Runtime.InteropServices;
using System.Data;
using System.Messaging;



namespace RenewEDSenderM.CommManager
{
    class Client
    {
        /// <summary>
        /// Socket成员对象
        /// </summary>
        private static Socket m_socket = null;
        private static int m_timeout = 0;
        private static string m_verifyStr = "";
        private static string m_xmlStr = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><root><common><project_id><!-- 项目编号 --></project_id><gateway_id><!-- 采集装置编号 --></gateway_id><type>request</type></common></root>";
        /// <summary>
        /// 项目编号
        /// </summary>
        private static string m_project_id;
        /// <summary>
        /// 采集装置编号
        /// </summary>
        private static string m_gateway_id;
        private static string m_input_sequence = "000000";
        /// <summary>
        /// 向数据中心发送的数据经过采集装置解析
        /// </summary>
        private static string m_input_parse = "yes";
		/// <summary>
		/// 重试次数
		/// </summary>
        private static int try_count = 0;
        /// <summary>
        /// 配置项成员对象
        /// </summary>
        private static Configuration m_config = null;
        private static bool m_isPassAuthentication = false;
        private static bool m_isConnected = false;
        private static bool m_isCreatThread = false;
        private static bool m_isCreatReport = false;

        private static bool m_TryFirtst10Fail = true;
        /// <summary>
        /// 互斥信号量成员
        /// </summary>
        private static Mutex m_mutex = new Mutex();
        /// <summary>
        /// 消息队列对象
        /// </summary>
        private static Support.MsgQueManager m_msgq;
        /// <summary>
        /// 重连计时
        /// </summary>
        private static int m_tryTimes = 0;
        /// <summary>
        /// 失败重传log标记，避免重复写log
        /// </summary>
        private static int m_isRereport = 1;
        /// <summary>
        /// 确定是否有失败数据
        /// </summary>
        private static bool m_isHasFailedData = true;

        //imports SetLocalTime function from kernel32.dll 
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SetLocalTime(ref SystemTime lpSystemTime);
		[DllImport("kernel32.dll")]
        private static extern void Sleep(int dwMilliseconds);

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

        static void Main(string[] args)
        {
            //DbManager.TestAccessData.Test();
            LogManager.Logger.FuncEntryLog(args);
            try
            {
                m_msgq = new Support.MsgQueManager();
            }
            catch (MessageQueueException mqex)
            {
                LogManager.Logger.WriteErrorLog("001001:Message Queue Service is not started:{0}", mqex);
            }
            while (true)
            {
                //初始化各种参数，避免错误
                m_timeout = 0;
                try_count = 0;
                m_isHasFailedData = true;

                try
                {
                    // 发送读取配置消息
                    try
                    {
                        m_msgq.SendMsg(new Support.MsgBody(false, false, Support.RUN_PHASE.READCONFIG));
                    }
                    catch (MessageQueueException mqex)
                    {
                        LogManager.Logger.WriteWarnLog("001002:尚未设置 Path 属性或访问消息队列方法时出错:{0}", mqex);
                    }
                    // 读取解析配置文件
                    if (InitConfig())
                    {
                        LogManager.Logger.WriteInfoLog("001003:Complete reading the configuration!");
                    }
                    else
                    {
                        LogManager.Logger.WriteWarnLog("001004:Fail to read the configuration!Retrying....");
                        continue;
                    }
                    // 创建定时上传Timer 20130629 首次在这里创建
                    try
                    {
                        // 首次创建后即可
                        if (m_isCreatReport == false && m_config.autoReportTimerFlg)
                        {
                            System.Timers.Timer reportTimer = new System.Timers.Timer();
                            if (reportTimer != null)
                            {
                                reportTimer.Elapsed += new ElapsedEventHandler(ReportEvent);
                                reportTimer.Interval = Convert.ToInt32(m_config.reportTime) * 60 * 1000; //配置文件中配置的秒数,30分钟一次
                                reportTimer.Enabled = true;
                                m_isCreatReport = true;
                                LogManager.Logger.WriteInfoLog("00101101:Timer of fixed timing Report is created!");
                            }
                            else
                            {
                                LogManager.Logger.WriteInfoLog("00101201:Fail to create timer of fixed timing Report! null pointer");
                                continue;
                            }
                        }
                    }
                    catch (ArgumentException e)
                    {
                        LogManager.Logger.WriteWarnLog("001013:Fail to create timer of report! ArgumentException:{0}", e);
                        continue;
                    }
                    catch (ObjectDisposedException e)
                    {
                        LogManager.Logger.WriteWarnLog("001014:Fail to create timer of report! ObjectDisposedException:{0}", e);
                        continue;
                    }
                    catch (Exception e)
                    {
                        LogManager.Logger.WriteWarnLog("001015:Fail to create timer of report! Exception:{0}", e);
                        continue;
                    }
                    // 设置ip地址、端口等信息
                    int port = int.Parse(m_config.port);
                    string host = m_config.ip;
                    m_project_id = m_config.project_id;
                    m_gateway_id = m_config.gateway_id;

                    // 创建终结点EndPoint对象
                    IPAddress ip = IPAddress.Parse(host);
                    IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口转化为IPEndpoint实例

                    // 创建socket并连接到服务器
                    m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建Socket
                    LogManager.Logger.WriteInfoLog("001005:Connect...");
                    // 发送请求连接消息
                    try
                    {
                        m_msgq.SendMsg(new Support.MsgBody(false, false, Support.RUN_PHASE.CONNECTING));
                    }
                    catch (MessageQueueException mqex)
                    {
                        LogManager.Logger.WriteWarnLog("001006:尚未设置 Path 属性或访问消息队列方法时出错:{0}", mqex);
                    }
                    // 建立Socket连接到服务器,重连会有延迟时间,延迟随着重连次数的增多而变大,递增为30秒,最大为5分钟
                    Sleep(m_tryTimes * 1000 * 30);
                    m_tryTimes++;
                    if (m_tryTimes > 10)
                    {
                        m_tryTimes = 10;
                        //发送重试次数超过10次警告 20130530
                        if (m_TryFirtst10Fail)
                        {
                            try
                            {
                                m_msgq.SendMsg(new Support.MsgBody(false, false, Support.RUN_PHASE.CONNECT_RETRY10));
                            }
                            catch (MessageQueueException mqex)
                            {
                                LogManager.Logger.WriteWarnLog("尚未设置 Path 属性或访问消息队列方法时出错:{0}", mqex);
                            }
                            finally
                            {
                                m_TryFirtst10Fail = false;
                            }
                        }
                    }
                    m_socket.Connect(ipe);

                    // m_tryTimes清零 20130530
                    m_tryTimes = 0;
                    m_TryFirtst10Fail = true;
                    //// 创建失败数据重传线程
                    //if (m_isCreatThread == false)
                    //{
                    //    Thread oThread = new Thread(new ThreadStart(ControlRereport));
                    //    //Thread oThread = new Thread(new ThreadStart(Rereport));
                    //    if (oThread != null)
                    //    {
                    //        m_isCreatThread = true;
                    //        try
                    //        {
                    //            oThread.Start();
                    //        }
                    //        catch (ThreadStateException te)
                    //        {
                    //            LogManager.Logger.WriteWarnLog("001007:Fail to create Retransmission process:{0}", te);
                    //            continue;
                    //        }
                    //        catch (OutOfMemoryException oe)
                    //        {
                    //            LogManager.Logger.WriteWarnLog("001008:Fail to create Retransmission process:{0}", oe);
                    //            continue;
                    //        }
                    //        LogManager.Logger.WriteInfoLog("001009:Retransmission process is created!");
                    //    }
                    //    else
                    //    {
                    //        LogManager.Logger.WriteWarnLog("001010:Fail to create Retransmission process! null pointer");
                    //        continue;
                    //    }
                    //}

                    //// 创建定时上传Timer
                    //try
                    //{
                    //    // 首次创建后即可
                    //    if (m_isCreatReport == false)
                    //    {
                    //        System.Timers.Timer reportTimer = new System.Timers.Timer();
                    //        if (reportTimer != null)
                    //        {
                    //            reportTimer.Elapsed += new ElapsedEventHandler(ReportEvent);
                    //            reportTimer.Interval = Convert.ToInt32(m_config.reportTime) * 60 * 1000; //配置文件中配置的秒数,30分钟一次
                    //            reportTimer.Enabled = true;
                    //            m_isCreatReport = true;
                    //            LogManager.Logger.WriteInfoLog("001011:Timer of fixed timing Report is created!");
                    //        }
                    //        else
                    //        {
                    //            LogManager.Logger.WriteInfoLog("001012:Fail to create timer of fixed timing Report! null pointer");
                    //            continue;
                    //        }
                    //    }
                    //}
                    //catch (ArgumentException e)
                    //{
                    //    LogManager.Logger.WriteWarnLog("001013:Fail to create timer of report! ArgumentException:{0}", e);
                    //    continue;
                    //}
                    //catch (ObjectDisposedException e)
                    //{
                    //    LogManager.Logger.WriteWarnLog("001014:Fail to create timer of report! ObjectDisposedException:{0}", e);
                    //    continue;
                    //}
                    //catch (Exception e)
                    //{
                    //    LogManager.Logger.WriteWarnLog("001015:Fail to create timer of report! Exception:{0}", e);
                    //    continue;
                    //}
                   
                    // 开始收发交互
                    while (true)
                    {
                        // 认证不会把异常抛到本层
                        if (Authentication())
                        {
                            // 清零20130530
                            m_tryTimes = 0;
                            m_TryFirtst10Fail = true;

                            //>>>>
                            // 创建失败数据重传线程
                            if (m_isCreatThread == false)
                            {
                                Thread oThread_CtrlRereport = new Thread(new ThreadStart(ControlRereport));
                                //Thread oThread = new Thread(new ThreadStart(Rereport));
                                if (oThread_CtrlRereport != null)
                                {
                                    m_isCreatThread = true;
                                    try
                                    {
                                        oThread_CtrlRereport.Start();
                                    }
                                    catch (ThreadStateException te)
                                    {
                                        LogManager.Logger.WriteWarnLog("001007:Fail to create Retransmission process:{0}", te);
                                        continue;
                                    }
                                    catch (OutOfMemoryException oe)
                                    {
                                        LogManager.Logger.WriteWarnLog("001008:Fail to create Retransmission process:{0}", oe);
                                        continue;
                                    }
                                    LogManager.Logger.WriteInfoLog("001009:Retransmission process is created!");
                                }
                                else
                                {
                                    LogManager.Logger.WriteWarnLog("001010:Fail to create Retransmission process! null pointer");
                                    continue;
                                }
                            }

                            // 创建定时上传Timer 20130629 首次在这里创建
                            try
                            {
                                // 首次创建后即可
                                if (m_isCreatReport == false)
                                {
                                    System.Timers.Timer reportTimer = new System.Timers.Timer();
                                    if (reportTimer != null)
                                    {
                                        reportTimer.Elapsed += new ElapsedEventHandler(ReportEvent);
                                        reportTimer.Interval = Convert.ToInt32(m_config.reportTime) * 60 * 1000; //配置文件中配置的秒数,30分钟一次
                                        reportTimer.Enabled = true;
                                        m_isCreatReport = true;
                                        //>>>> 20130629 修改config中<ReportTimerFlag>
                                        SetConfig setcfg = new SetConfig();
                                        Configuration config = new Configuration();
                                        config.autoReportTimerFlg = true;
                                        setcfg.WriteSpecailConfig(config, Commands.REPORT_TIMER_ENABLE_FLAG);
                                        //<<<<
                                        LogManager.Logger.WriteInfoLog("001011:Timer of fixed timing Report is created!");
                                    }
                                    else
                                    {
                                        LogManager.Logger.WriteInfoLog("001012:Fail to create timer of fixed timing Report! null pointer");
                                        continue;
                                    }
                                }
                            }
                            catch (ArgumentException e)
                            {
                                LogManager.Logger.WriteWarnLog("001013:Fail to create timer of report! ArgumentException:{0}", e);
                                continue;
                            }
                            catch (ObjectDisposedException e)
                            {
                                LogManager.Logger.WriteWarnLog("001014:Fail to create timer of report! ObjectDisposedException:{0}", e);
                                continue;
                            }
                            catch (Exception e)
                            {
                                LogManager.Logger.WriteWarnLog("001015:Fail to create timer of report! Exception:{0}", e);
                                continue;
                            }
                            //<<<<

                            // 认证成功
                            SendCommunication();//如果认证成功，则进行发送数据，包括心跳数据包等，如果连接失败，则从send状态跳出，重新进行认证
                        }

                        // 认证失败后重新连接
                        m_isConnected = false;
						m_isPassAuthentication = false;
                        // 重连时更新失败重传线程状态
                        m_isHasFailedData = true;
                        //初始化各种参数，避免错误
                        m_timeout = 0;
                        try_count = 0;

                        // 发送认证失败消息
                        try
                        {
                            m_msgq.SendMsg(new Support.MsgBody(true, m_isPassAuthentication, Support.RUN_PHASE.VERIFY_FAIL));
                        }
                        catch (MessageQueueException mqex)
                        {
                            LogManager.Logger.WriteWarnLog("尚未设置 Path 属性或访问消息队列方法时出错:{0}", mqex);
                        }
                        //先关闭已经打开的链接
                        if (m_socket != null)
                        {
                            m_socket.Close();
                            Console.Write("Closing Used Link....");
                            LogManager.Logger.WriteInfoLog("Closing Used Link....");
                        }
                        // 重新创建socket并连接到服务器
                        try
                        {
                            m_msgq.SendMsg(new Support.MsgBody(false, m_isPassAuthentication, Support.RUN_PHASE.CONNECTING));
                        }
                        catch (MessageQueueException mqex)
                        {
                            LogManager.Logger.WriteWarnLog("尚未设置 Path 属性或访问消息队列方法时出错:{0}", mqex);
                        }
                        m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建Socket
                        LogManager.Logger.WriteInfoLog("Connect again....");

                        // 重新连接到服务器,重连会有延迟时间,延迟随着重连次数的增多而变大,最大为5分钟
                        Sleep(m_tryTimes * 1000 * 30);
                        m_tryTimes++;
                        if (m_tryTimes > 10)
                        {
                            m_tryTimes = 10;
                            //发送重试次数超过10次警告 20130530
                            if (m_TryFirtst10Fail)
                            {
                                try
                                {
                                    m_msgq.SendMsg(new Support.MsgBody(false, false, Support.RUN_PHASE.AUTHENTICATION_RETRY10));
                                }
                                catch (MessageQueueException mqex)
                                {
                                    LogManager.Logger.WriteWarnLog("尚未设置 Path 属性或访问消息队列方法时出错:{0}", mqex);
                                }
                                finally
                                {
                                    m_TryFirtst10Fail = false;
                                }
                            }
                        }
                        m_socket.Connect(ipe);//连接到服务器

                    }

                }
                catch (ArgumentNullException e)
                {
                    LogManager.Logger.WriteWarnLog(" 001020:Fail! argumentNullException: {0}", e);
                    Console.WriteLine("Fail !argumentNullException: {0}", e);
                    m_isConnected = false;
                    Support.MsgBody msgb = new Support.MsgBody(false, m_isPassAuthentication, Support.RUN_PHASE.INVALID);
                    try
                    {
                        m_msgq.SendMsg(msgb);
                    }
                    catch (MessageQueueException mqex)
                    {
                        LogManager.Logger.WriteWarnLog("001021:{0}", mqex);
                    }
                }
                catch (SocketException e)
                {
                    LogManager.Logger.WriteWarnLog("001022:Fail on SocketException:{0}", e);
                    Console.WriteLine("Fail on SocketException:{0}", e);
                    m_isConnected = false;
                    Support.MsgBody msgb = new Support.MsgBody(false, m_isPassAuthentication, Support.RUN_PHASE.INVALID);
                    try
                    {
                        m_msgq.SendMsg(msgb);
                    }
                    catch (MessageQueueException mqex)
                    {
                        LogManager.Logger.WriteWarnLog("001023:{0}", mqex);
                    }
                }
                catch (Exception e)
                {
                    LogManager.Logger.WriteWarnLog("001024:Fail on Exception:{0}", e);
                    m_isConnected = false;
                    Support.MsgBody msgb = new Support.MsgBody(false, m_isPassAuthentication, Support.RUN_PHASE.INVALID);
                    try
                    {
                        m_msgq.SendMsg(msgb);
                    }
                    catch (MessageQueueException mqex)
                    {
                        LogManager.Logger.WriteWarnLog("001025:{0}", mqex);
                    }
                }
                if (m_socket != null)
                {
                    m_socket.Close();
                }
            }
            //Console.ReadLine();
            Console.WriteLine("Press Enter to Exit");
            LogManager.Logger.FuncExitLog();
        }

        /// <summary>
        /// 初始化各配置项
        /// </summary>
        /// <returns>初始成功与否</returns>
        private static bool InitConfig()
        {
            SetConfig set_config = new SetConfig();
            if (set_config != null)
            {
                m_config = set_config.ReadConfig();
                return true;
            }
            else
            {
                return false;
            }
        }

 

        /// <summary>
        /// 解析并判断接收数据是否正确，若正确，则分离出相应的命令和参数
        /// </summary>
        /// <returns></returns>
        private static bool Authentication()
        {
            //初始化各种参数
            m_timeout = 0;
            m_verifyStr = "";
            m_input_sequence = "";

            Console.Write("Verifying.....");
            LogManager.Logger.WriteInfoLog("001026:Begin to request the authentication...");
            ///向服务器发送信息
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            int bytes;

            // 创建xml读取对象、xml写对象、命令xml对象
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            XmlProcessManager.XMLRead xmlread = new XmlProcessManager.XMLRead();
            XmlProcessManager.Order order = new XmlProcessManager.Order();
			
			//xml相关处理对象无法建立，则退出认证操作
			if(xmlwrite  == null || xmlread == null || order == null)
			{
                LogManager.Logger.WriteWarnLog("001027:Fail to create xmlwrite ,xmlread or order!");
				return false;
			}

            // 发起身份认证
            xmlwrite.Input(m_xmlStr, m_project_id, m_gateway_id,m_config.key,m_config.iv);
            xmlwrite.Request();
            Support.MsgBody msgb = new Support.MsgBody(true, false, Support.RUN_PHASE.VERIFY);
            try
            {
                m_msgq.SendMsg(msgb);
            }
            catch (MessageQueueException mqex)
            {
                LogManager.Logger.WriteWarnLog("001028:尚未设置 Path 属性或访问消息队列方法时出错:{0}", mqex);
            }
            if (SendMsgB(xmlwrite.BOutput()) == false)
            {
                LogManager.Logger.WriteWarnLog("001029:Fail to send request about authentication!");
                return false;
            }
            LogManager.Logger.WriteInfoLog("001030:Success to send request about authentication");

            //超时定时器，保证网络阻塞时不会无限期等待
            try
            {
                System.Timers.Timer checkTimer = new System.Timers.Timer();
                if (checkTimer == null)
                {
                    LogManager.Logger.WriteWarnLog("001031:Fail to create Timer Check!");
                    return false;
                }
                checkTimer.Elapsed += new ElapsedEventHandler(checkEvent);
                checkTimer.Interval = 5 * 1000; //定时时间为5秒
                checkTimer.Enabled = true;
                LogManager.Logger.WriteInfoLog("001032:Timer of fixed timing Check is created!");
                


                //接收数据，并进行超时判断
                recvStr = "";
                while (true)
                {
                    if (try_count >= Convert.ToInt32(m_config.times))
                    {
                        LogManager.Logger.WriteWarnLog("001033:Try 5 times to wait sequence,Time Out!");
						checkTimer.Enabled = false;
                        try_count = 0;
                        return false;
                    }

                    //进行认证数据的接收，如果接收到数据,进入下一个阶段
                    if ((bytes = m_socket.Receive(recvBytes, recvBytes.Length, 0)) != 0)
                    {

                        recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                        LogManager.Logger.WriteInfoLog("001034:Success to receive the response about authentication");
                        checkTimer.Enabled = false;
                        try_count = 0;
                        break;
                    }
                }

                //接收到认证信息
                if (bytes != 0)
                {
                    byte[] rByte = new byte[bytes];
                    Array.Copy(recvBytes, rByte, bytes);
                    if (xmlread.BInput(rByte, m_config.key, m_config.iv) == false)
				    {
                        return false;
                    }
                    order = xmlread.Output();
                    m_input_sequence = order.sequence;
                }

                // 发送收到的随机序列的MD5值
                xmlwrite.Input(m_xmlStr, m_project_id, m_gateway_id,m_config.key,m_config.iv);
                Support.Encryption.MD5_KEY_STR = m_config.md5;
                string md5Str = Support.Encryption.getMd5Hash(order.sequence);
                msgb = new Support.MsgBody(true, false, Support.RUN_PHASE.VERIFY_MD5);
                try
                {
                    m_msgq.SendMsg(msgb);
                }
                catch (MessageQueueException mqex)
                {
                    LogManager.Logger.WriteWarnLog("001035:尚未设置 Path 属性或访问消息队列方法时出错:{0}", mqex);
                }
                xmlwrite.SendMD5(md5Str);
                m_verifyStr = xmlwrite.Output();
                if (SendMsgB(xmlwrite.BOutput()) == false)
                {
                    LogManager.Logger.WriteWarnLog("001036:Fail to send MD5 sequence about authentication!");
                    return false;
                }
                checkTimer.Enabled = true;


                //接收数据，并进行超时判断
                recvStr = "";
                while (true)
                {
                    if (try_count >= Convert.ToInt32(m_config.times))
                    {
                        LogManager.Logger.WriteWarnLog("001037:Try 5 times to wait authentication result,Time Out!");	
						checkTimer.Enabled = false;
                        try_count = 0;
                        return false;
                    }
                    //进行认证数据的接收，如果接收到数据,进入下一个阶段，并关闭定时器避免重传
                    if ((bytes = m_socket.Receive(recvBytes, recvBytes.Length, 0)) != 0)
                    {
                        recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                        checkTimer.Enabled = false;
                        try_count = 0;
                        break;
                    }
                }

                //接收到认证信息
                if (bytes != 0)
                {
                    int i;
                    byte[] rByte1 = new byte[bytes];
                    Array.Copy(recvBytes, rByte1, bytes);
                    if(xmlread.BInput(rByte1,m_config.key,m_config.iv)==false)
				    {
                        return false ;
                    }
				    order = xmlread.Output();
                }

                //判断是否认证成功
                if (order.result == "pass")
                {
                    m_isPassAuthentication = true;
                    m_isConnected = true;
                    try
				    {
					    m_msgq.SendMsg(new Support.MsgBody(true, true, Support.RUN_PHASE.VERIFY_PASS) );
                    }
				    catch(MessageQueueException mqex)
				    {
                        LogManager.Logger.WriteWarnLog("001038:{0}", mqex);
				    }
                    if (order.time.Length == 14)
                    {
                        SystemTime systNew = new SystemTime();
                        string years = order.time.Substring(0, 4);
                        string months = order.time.Substring(4, 2);
                        string days = order.time.Substring(6, 2);
                        string hours = order.time.Substring(8, 2);
                        string minutes = order.time.Substring(10, 2);
                        string seconds = order.time.Substring(12, 2);
                        systNew.wDay = short.Parse(days);
                        systNew.wMonth = short.Parse(months);
                        systNew.wYear = short.Parse(years);
                        systNew.wHour = short.Parse(hours);
                        systNew.wMinute = short.Parse(minutes);
                        systNew.wSecond = short.Parse(seconds);
                        // 认证成功后进行系统授时
                        SetLocalTime(ref systNew);
                    }
                    else
                    {
                        LogManager.Logger.WriteWarnLog("001073: Fail to SetLocalTime, There's no information system setting time");
                    }
                    LogManager.Logger.WriteInfoLog("001039:Authentication process is successful!");
                    return true;
                }
                else
                {
                    LogManager.Logger.WriteWarnLog("001040:Authentication process is failed!");
                    // 发送认证失败消息
                    try
                    {
                        m_msgq.SendMsg(new Support.MsgBody(true, m_isPassAuthentication, Support.RUN_PHASE.VERIFY_FAIL));
                    }
                    catch (MessageQueueException mqex)
                    {
                        LogManager.Logger.WriteWarnLog("001041:尚未设置 Path 属性或访问消息队列方法时出错:{0}", mqex);
                    }
                    return false;
                }
            }
            catch (ArgumentException e)
            {
                LogManager.Logger.WriteWarnLog("001042:Fail to create timer of report! ArgumentException:{0}", e);
                return false;
            }
            catch (ObjectDisposedException e)
            {
                LogManager.Logger.WriteWarnLog("001043:Fail to create timer of report! ObjectDisposedException:{0}", e);
                return false;
            }
            catch (Exception e)
            {
                LogManager.Logger.WriteWarnLog("001044:Fail to create timer of report! Exception:{0}", e);
                return false;
            }
        }

        /// <summary>
        /// 认证成功后与服务器进行数据交互。正常Continue 否则return
        /// </summary>
        public static void SendCommunication()
        {
            ///向服务器发送信息
            string recvStr = "";
            byte[] recvBytes = new byte[1024];

            //心跳数据包Timer
            System.Timers.Timer HeartbeatNotifyTimer = new System.Timers.Timer();
			if(HeartbeatNotifyTimer == null)
			{
                LogManager.Logger.WriteWarnLog("001045:Fail to create Time Heartbeat in SendCommunication!");
				return ;
			}
            
            HeartbeatNotifyTimer.Elapsed += new ElapsedEventHandler(HeartbeatNotifyEvent);
            try
            {
                HeartbeatNotifyTimer.Interval = Convert.ToInt32(m_config.notifyTime) * 60 * 1000;    //配置文件中配置的分钟数
            }
            catch(Exception e)
            {
                HeartbeatNotifyTimer.Interval = 60 * 1000;   //60秒一次
            }

            XmlProcessManager.XMLRead xmlread = new XmlProcessManager.XMLRead();
			if(xmlread == null)
			{
                LogManager.Logger.WriteWarnLog("001046:Fail to create xmlread in SendCommunication!");
				return ;
			}
			
            XmlProcessManager.Order order;
            int bytes = 0;

            //包括传输数据，超时重传和错误重传
            while (true)
            {
                HeartbeatNotifyTimer.Enabled = true;
                recvStr = "";
                //接收数据，并进行超时判断
                while (true)
                {
                    //在两个心跳数据包的发送间隔内没有收到回应，则认为超时
                    if (m_timeout > 1||m_isConnected == false)
                    {
                        m_timeout = 0;
                        break;
                    }

                    //进行数据的接收，如果接收到数据则下一个阶段，并关闭定时器避免重传
                    if ((bytes = m_socket.Receive(recvBytes, recvBytes.Length, 0)) != 0)
                    {
                        //>>>> Test
                        if (false)
                        {
                            int i;
                            //string teststr = "68681616240100007f315d7eAD4E729DCD81551ABC7C2983C86D3C6A58A862EAB7762371A0F6A3C83FBDC8EC0D81C9EDEC3688819457963564538A9AF2BC710625D1F700F80ACEDE47DE81ED6EE95AEF6FC54D21A610835918DB817793A4CF35A7A85CAB9AD90E3EFDC0E89654F722AE201D63B365D492CC6FCE9280F45406C71E468DDD814B73D69D95217CEAB762C6945EEA4FB3D437B83B502446A65AF7B448C2C8F17FE31FBD59DB478917D5B6CD13C47E1E6D7CBB87DFD49C821B84C17D88499A61E0E45F2F94A8DC1E9371E432BCDFAB54A6DD304C193C71E2E4A1C7F0B49CC16F8CC0FF8B5A5EC6978A4D190F3020ED8D38471B746B78743949E49FF9C53DF2056193FF434A187F055B26A6C7DDEDC1A760E0531BEBF1E9FB60C5B0BD0E2CB597AACAFB65629A25A4711d55aa55aa";
                            //string teststr = "68681616f400000028f5e770AD4E729DCD81551ABC7C2983C86D3C6A58A862EAB7762371A0F6A3C83FBDC8EC0D81C9EDEC3688819457963564538A9AF2BC710625D1F700F80ACEDE47DE81ED6EE95AEF6FC54D21A610835918DB817793A4CF35A7A85CAB9AD90E3EFDC0E89654F722AE201D63B365D492CC6FCE9280F45406C71E468DDD814B73D69D95217C175A2C49521EA689A07062D925732ACB4545841451CD92F7E1573206C6D62430073295FF6588F37CDDBBF8A4E7A5F3FE9C21ED7630A917473F8E1A82E199F272C8CAD5616F65B223B12369B19A39CFDC590F9EE46BF043FCCA9D42EF2D372F26DD2C827861945AE9B7535F2766773CEC6b9055aa55aa";
                            string teststr = "6868161604010000271d004cAD4E729DCD81551ABC7C2983C86D3C6A58A862EAB7762371A0F6A3C83FBDC8EC0D81C9EDEC3688819457963564538A9AF2BC710625D1F700F80ACEDE47DE81ED6EE95AEF6FC54D21A610835918DB817793A4CF35A7A85CAB9AD90E3EFDC0E89654F722AE201D63B365D492CC6FCE9280F45406C71E468DDD814B73D69D95217C84330F51CB6DD9710786C73A46506EC6D632582E7EA6556F5FDEA1E0422CBD0D4C46D17449E948F5B9E7374B5B037391E663AFD28028A9D4C8202A1762FFA240066257CA8EC976F77542FD2C69D19A33BF2B140744CC6A8378D4EB7F5E5A3E1B501737429F06AEF0BA9664F5A9F5CB00B14A9B5BC4DD5879DCE1444CE041EACDdcdc55aa55aa";
                            byte[] test = Support.DataPackage.GetBytes(teststr, out i);
                            Array.Clear(recvBytes, 0, bytes);
                            bytes = test.Length;
                            Array.Copy(test, recvBytes, bytes);
                        }
                        //<<<<
                        recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
                        HeartbeatNotifyTimer.Enabled = false;
                        m_timeout = 0;
                        break;
                    }
                }

                //分析接收到的信息
                //如果超时，则跳出循环进入重新认证链接
                if (m_timeout > 1||m_isConnected == false)
                {
                    HeartbeatNotifyTimer.Enabled = false;    //心跳包超时，需要重新连接服务器端，停止继续发送数据
                    Console.Write("HeartBeat Time out!");//可以写入log，进行记录
                    LogManager.Logger.WriteInfoLog("001047:HeartBeat Time out!");
                    return;                         //直接返回，重新进行连接和认证
                }

                //从接收到的信息中解析出相应的命令
                byte[] rByte = new byte[bytes];
				if(rByte == null)
				{
                    LogManager.Logger.WriteWarnLog("001048:Fail to create byte array during SendCommunication!");
					return ;
				}
                Array.Copy(recvBytes, rByte, bytes);
                if(xmlread.BInput(rByte,m_config.key,m_config.iv)== false)
				{
                    return ;
                }
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

        /// <summary>
        /// 发送相应的比特流数据
        /// </summary>
        /// <param name="sendByte"></param>
        /// <returns></returns>
        private static bool SendMsgB(byte[] sendByte)
        {
            Console.WriteLine("Send Message");
            LogManager.Logger.WriteInfoLog("001049:Send Message");
            int sendLength;
            try
            {
                sendLength = m_socket.Send(sendByte, sendByte.Length, SocketFlags.None);//发送信息
            }
            catch (Exception e)
            {
                LogManager.Logger.WriteWarnLog("001050:Fail to send message to server:{0}", e);
                return false;
            }
            if (sendLength == sendByte.Length)
            {
                LogManager.Logger.WriteInfoLog("001051:Message is sent!");
                return true;
            }
            else
            {
                LogManager.Logger.WriteWarnLog("001052:Message is not sent!");
                return false;
            }

        }

        /// <summary>
        /// 发送相应的字符串数据
        /// </summary>
        /// <param name="sendStr"></param>
        private static void SendMsg(string sendStr)
        {
            string TestStr = sendStr;
            int i;
            byte[] dp = Support.DataPackage.GetBytes(TestStr, out i);
            byte[] bs = Encoding.ASCII.GetBytes(TestStr);//把字符串编码为字节
            Console.WriteLine("Send Message");
            try
            {
                m_socket.Send(dp, dp.Length, 0);//发送信息
            }
            catch (Exception e)
            {
                LogManager.Logger.WriteWarnLog("001053:Fail to send message to server: {0}", e);
                //m_isConnected = false;
            }
        }

        private static void checkEvent(object sender, ElapsedEventArgs e)
        {
            try_count++;
            LogManager.Logger.WriteWarnLog("001054:Try Times:{0}", try_count);
        }

        /// <summary>
        /// 心跳操作的超时处理，将timeout设置为1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HeartbeatNotifyEvent(object sender, ElapsedEventArgs e)
        {
            m_timeout++;
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
			if(xmlwrite == null)
			{
                LogManager.Logger.WriteWarnLog("001055:Fail to create xmlwrite during Heartbeat!");
                m_isConnected = false;
				return ; 
			}
            xmlwrite.Input(m_xmlStr, m_project_id, m_gateway_id,m_config.key,m_config.iv);
            xmlwrite.Notify();
            //发送心跳数据包失败，则返回失败，并报告连接断开
            if (SendMsgB(xmlwrite.BOutput()) == false)
            {
                LogManager.Logger.WriteWarnLog("001056:Fail to send notify during Heartbeat!");
                m_isConnected = false;
                return;
            }
            else
            {
                Support.MsgBody msg = new Support.MsgBody(true, true, Support.RUN_PHASE.HEARTBEAT);
                try
                {
                    m_msgq.SendMsg(msg);
                }
                catch (MessageQueueException ex)
                {
                }
            }
        }

        //定时发送数据，需要收集数据
        private static void ReportEvent(object sender, ElapsedEventArgs e)
        {
            //如果通过了认证则进行相关操作，否则不进行任何操作
            //if (m_isPassAuthentication)
            {
                //数据写入数据库

                //如果网络状况良好，则发送数据
                //if (m_isConnected)
                //{
                    //这部分需要采集数据进行数据录入
                    DateTime date_send = DateTime.Now.ToLocalTime();
                    //DateTime date_send = new DateTime(2013, 4, 28, 19, 0, 0);   //2013-04-28 19:00:00
                    //TimeSpan ts = new TimeSpan(2, 0, 0); //T.B.D.测试方便用
                    TimeSpan ts = new TimeSpan(0, int.Parse(m_config.reportTime), 0);    //config.xml的时间间隔
                    DbManager.History_Data hd_array;
                    //DataRow[] dr = DbManager.DataDump.CalculateAverage(date_send, ts);
                    Single [] dr = DbManager.DataDump.CalculateAverage(date_send, ts);
                    if (dr == null)
                    {
                        LogManager.Logger.WriteWarnLog("001057:Report Process: No data fetched from the database!");
                        return;
                    }

                    XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
                    if (xmlwrite == null)
                    {
                        LogManager.Logger.WriteWarnLog("001058:Fail to create xmlwrite in Report!");
                        DbManager.DataDump.WriteToHisDb(date_send, dr, out hd_array);
                        return;
                    }

                    xmlwrite.Input(m_xmlStr, m_project_id, m_gateway_id, m_config.key, m_config.iv);



                    DataInfo[] input_info = new DataInfo[4];
                    for (int i = 0; i < 4; i++)
                    {
                        input_info[i] = new DataInfo();
                        input_info[i].sample_time = date_send.ToString("yyyyMMddHHmmss");
                    }

                    string[] fids = GenerateFunID();
                    string[] mids = GenerateMeterID();
                    input_info[0].data = Convert.ToString(dr[0]);
                    input_info[0].mid = mids[0];
                    input_info[0].fid = fids[0];
                    input_info[1].data = Convert.ToString(dr[1]);
                    input_info[1].mid = mids[1];
                    input_info[1].fid = fids[1];
                    input_info[2].data = Convert.ToString(dr[2]);
                    input_info[2].mid = mids[2];
                    input_info[2].fid = fids[2];
                    input_info[3].data = Convert.ToString(dr[3] + dr[4] + dr[5]);
                    input_info[3].mid = mids[3];
                    input_info[3].fid = fids[3];

                    Random random = new Random();
                    int sequence = random.Next(10000000, 99999999);
                    string sequenceStr = sequence.ToString();
                    xmlwrite.Report(sequenceStr, m_input_parse, date_send.ToString("yyyyMMddHHmmss"), input_info);

                    if (m_isPassAuthentication && m_isConnected && SendMsgB(xmlwrite.BOutput()))
                    {
                        try
                        {
                            m_msgq.SendMsg(new Support.MsgBody(true, true, Support.RUN_PHASE.REPORT));
                        }
                        catch (MessageQueueException ex)
                        {
                        }
                        m_mutex.WaitOne();
                        DbManager.DataDump.WriteToHisDb(date_send, dr, out hd_array);
                        DbManager.DataDump.update_Upload(hd_array.id);
                        m_mutex.ReleaseMutex();
                        LogManager.Logger.WriteInfoLog("001059:The report is sent!");
                    }
                    else
                    {
                        DbManager.DataDump.WriteToHisDb(date_send, dr, out hd_array);
                        LogManager.Logger.WriteWarnLog("001060:The report is failed to send!");
                        return;
                    }
                //}

            }
            //else
            {
                //return;
            }
        }

		///
        /// <summary>应答查询操作，与数据库交互</summary>
		///
        private static void Reply(XmlProcessManager.Order order)
        {
            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            if (xmlwrite == null)
            {
                LogManager.Logger.WriteWarnLog("001061:Fail to create xmlwrite in Reply!");
                return;
            }

            //数据库查询，并输出相关历史数据的参数
            xmlwrite.Input(m_xmlStr, m_project_id, m_gateway_id, m_config.key, m_config.iv);
            Support.Encryption.MD5_KEY_STR = m_config.md5;

            //其中的sequence parse time和input_info应该从数据库中读取
            DataInfo[] input_info = new DataInfo[4];
            for (int i = 0; i < 4; i++)
                input_info[i] = new DataInfo();
            if (input_info == null)
                return;
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

            if (hd_array == null)
            {
                LogManager.Logger.WriteWarnLog("001062:Reply Process: No data fetched from the database!");
                return;
            }

            if (hd_array != null)
            {
                string[] fids = GenerateFunID();
                string[] mids = GenerateMeterID();
                int sampleCount;
                sampleCount = hd_array.Length;
                for (int i = 0; i < sampleCount; i++)
                {
                    string sample_time = hd_array[i].timestamp_sendCycle.ToString("yyyyMMddHHmmss");

                    input_info[0].data = Convert.ToString(hd_array[i].ValueA);
                    input_info[0].mid = mids[0];
                    input_info[0].fid = fids[0];
                    input_info[0].sample_time = sample_time;

                    input_info[1].data = Convert.ToString(hd_array[i].ValueB);
                    input_info[1].mid = mids[1];
                    input_info[1].fid = fids[1];
                    input_info[1].sample_time = sample_time;

                    input_info[2].data = Convert.ToString(hd_array[i].ValueC);
                    input_info[2].mid = mids[2];
                    input_info[2].fid = fids[2];
                    input_info[2].sample_time = sample_time;

                    input_info[3].data = Convert.ToString(hd_array[i].ValueD);
                    input_info[3].mid = mids[3];
                    input_info[3].fid = fids[3];
                    input_info[3].sample_time = sample_time;

                    Random random = new Random();
                    int sequence = random.Next(10000000, 99999999);
                    string sequenceStr = sequence.ToString();
                    xmlwrite.Query(sequenceStr, m_input_parse, hd_array[i].timestamp_sendCycle.ToString("yyyyMMddHHmmss"), input_info);
                    if (SendMsgB(xmlwrite.BOutput()))
                    {
                        try
                        {
                            m_msgq.SendMsg(new Support.MsgBody(true, true, Support.RUN_PHASE.REPORT));
                        }
                        catch (MessageQueueException ex)
                        {
                        }
                        DbManager.DataDump.update_Upload(hd_array[i].id);
                        LogManager.Logger.WriteInfoLog("001063:The reply is sent!");
                    }
                    else
                        LogManager.Logger.WriteWarnLog("001064:The reply is failed to send!");
                    Sleep(20);
                }
            }
        }
		///
        /// <summary>发送周期配置应答信息</summary>
		///
        private static void Period_ack(XmlProcessManager.Order order)
        {
            //更新内存中的配置参数并写入配置文件
            m_config.period = order.period;
            SetConfig set_config = new SetConfig();
            set_config.WriteSpecailConfig(m_config, "Period");

            XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
            if (xmlwrite == null)
            {
                LogManager.Logger.WriteWarnLog("001065:Fail to create xmlwrite in Period Ask!");
                return;
            }

            xmlwrite.Input(m_xmlStr, m_project_id, m_gateway_id,m_config.key,m_config.iv);
            xmlwrite.Period_Ack();
            if (SendMsgB(xmlwrite.BOutput()) == false)
            {
                LogManager.Logger.WriteWarnLog("001066:Fail to send period ask !");
                return;
            }
            try
            {
                m_msgq.SendMsg(new Support.MsgBody(true, true, Support.RUN_PHASE.REPLY_ACK));
            }
            catch (MessageQueueException e)
            {
            }
        }

		///
        /// <summary>通过获取的服务器的设置密钥命令来进行密钥的配置</summary>
		///
        private static void SetKey(XmlProcessManager.Order order)
        {
            if (order.keytype == "0")
            {
                order.order = "MD5";
                m_config.md5 = order.key;
            }
            else if (order.keytype == "1")
            {
                order.order = "Key";
                m_config.key = order.key;
            }
            else if (order.keytype == "2")
            {
                order.order = "IV";
                m_config.iv = order.key;
            }

            SetConfig set_config = new SetConfig();

            set_config.WriteSpecailConfig(m_config, order.order);
        }

        /// <summary>
        /// 控制失败数据重传线程
        /// </summary>
        private static void ControlRereport()
        {
            Thread oThread_Rereport = null;
            while (true)
            {
                if (oThread_Rereport == null)
                    oThread_Rereport = new Thread(new ThreadStart(Rereport));
				if (m_isConnected && m_isPassAuthentication)
				{
                	 
                    //if(oThread == null)
                    //    oThread = new Thread(new ThreadStart(Rereport));
                    
                	//如果线程建立成功，且有失败数据则开始数据重传
                 	if (oThread_Rereport != null && m_isHasFailedData)
                 	{
                    		m_isCreatThread = true;
                            if (!oThread_Rereport.IsAlive)
                            {
                                try
                                {
                                    //>>>> 20130629
                                    if(oThread_Rereport.ThreadState == ThreadState.Stopped)
                                    {
                                        oThread_Rereport = new Thread(new ThreadStart(Rereport));
                                    }
                                    //<<<<
                                    oThread_Rereport.Start();
                                    
                                }
                                catch (ThreadStateException te)
                                {
                                    LogManager.Logger.WriteWarnLog("001007:Fail to create Retransmission process:{0}", te);
                                    continue;
                                }
                                catch (OutOfMemoryException oe)
                                {
                                    LogManager.Logger.WriteWarnLog("001008:Fail to create Retransmission process:{0}", oe);
                                    continue;
                                }
                                LogManager.Logger.WriteInfoLog("001009:Retransmission process is created!");
                            }
                   	}
                 	else if (oThread_Rereport == null)
                 	{
                    		LogManager.Logger.WriteWarnLog("001010:Fail to create Retransmission process! null pointer");
                    		continue;
                 	}
                 	else if(oThread_Rereport.IsAlive) //如果没有失败数据则关闭失败重传线程
                 	{
                     		oThread_Rereport.Abort();
                     		oThread_Rereport.Join();
                            oThread_Rereport = null;
                            //GC.Collect();
                 	}
	            }
                Sleep(400);
            }
        }
        /// <summary>
        /// 失败数据重传
        /// </summary>
        private static void Rereport()
        {
            while(true)
            {
                //如果网络状况良好则进行重传
                if (m_isConnected && m_isPassAuthentication)
                {
                    XmlProcessManager.XMLWrite xmlwrite = new XmlProcessManager.XMLWrite();
                    if (xmlwrite == null)
                    {
                        LogManager.Logger.WriteWarnLog("001067:Fail to create xmlwrite in Rereport!");
                        continue;
                    }
                    xmlwrite.Input(m_xmlStr,m_project_id,m_gateway_id,m_config.key,m_config.iv);
                    Support.Encryption.MD5_KEY_STR = m_config.md5;

                    DataInfo[] input_info = new DataInfo[4];
                    if (input_info == null)
                        continue;

                    for (int i = 0;i < 4;i++)
                        input_info[i] = new DataInfo();

                    DateTime begin_time = new DateTime(1900, 1, 1, 1, 1, 1);
                    DateTime end_time = new DateTime(3000, 1, 1, 1, 1, 1);

                    m_mutex.WaitOne();
                    DbManager.History_Data[] hd_array = DbManager.DataDump.FetchDataFail(begin_time, end_time);
                    m_mutex.ReleaseMutex();

                    if (hd_array == null)
                    {
                        //log标记，避免重复写log，只有在真正需要写log的时候写
                        if (m_isRereport == 1)
                        {
                            LogManager.Logger.WriteWarnLog("001068:未取到失败须重传数据{0}-{1}", begin_time, end_time);
                            m_isRereport = 0;
                            
                        }
                        m_isHasFailedData = false;
                        return;
                    }

                    if (hd_array != null)
                    {
                        //如果有失败数据，将log标记记为1
                        m_isRereport = 1;

                        string[] fids = GenerateFunID();
                        string[] mids = GenerateMeterID();
                        int sampleCount;
                        sampleCount = hd_array.Length;
                        for (int i = 0; i < sampleCount; i++)
                        {
                            string sample_time = hd_array[i].timestamp_sendCycle.ToString("yyyyMMddHHmmss");

                            input_info[0].data = Convert.ToString(hd_array[i].ValueA);
                            input_info[0].mid = mids[0];
                            input_info[0].fid = fids[0];
                            input_info[0].sample_time = sample_time;

                            input_info[1].data = Convert.ToString(hd_array[i].ValueB);
                            input_info[1].mid = mids[1];
                            input_info[1].fid = fids[1];
                            input_info[1].sample_time = sample_time;

                            input_info[2].data = Convert.ToString(hd_array[i].ValueC);
                            input_info[2].mid = mids[2];
                            input_info[2].fid = fids[2];
                            input_info[2].sample_time = sample_time;

                            input_info[3].data = Convert.ToString(hd_array[i].ValueD);
                            input_info[3].mid = mids[3];
                            input_info[3].fid = fids[3];
                            input_info[3].sample_time = sample_time;

                            Random random = new Random();
                            int sequence = random.Next(10000000,99999999);
                            string sequenceStr = sequence.ToString();
                            xmlwrite.Report(sequenceStr, m_input_parse, hd_array[i].timestamp_sendCycle.ToString("yyyyMMddHHmmss"), input_info);
                            if (SendMsgB(xmlwrite.BOutput()))
                            {
                                try
                                {
                                    m_msgq.SendMsg(new Support.MsgBody(true, true, Support.RUN_PHASE.REUPLOAD));
                                }
                                catch (MessageQueueException e)
                                {
                                }
                                DbManager.DataDump.update_Upload(hd_array[i].id);
                                LogManager.Logger.WriteInfoLog("001069:The reReport is sent!");
                            }
                            else
                                LogManager.Logger.WriteWarnLog("001070:The reReport is failed to send!");
                        }
                    }
                    
                }
                Sleep(20);
            }
        }



		/// <summary>计量装置的具体采集功能编号</summary>
        /// <returns>计量装置的具体采集功能编号15位</returns>
        public static string[] GenerateMeterID()
        {
            string[] code_array = new string[4];
            if (code_array == null)
            {
                LogManager.Logger.WriteWarnLog("001071:Failed to generate Meter ID");
                return null;
            }

            string code_perfix = m_config.areacode + m_config.programid;
            code_array[0] = code_perfix + m_config.meterInfo.MA_ProgramId + m_config.meterInfo.MA_Code1 + m_config.meterInfo.MA_Code2;

            code_array[1] = code_perfix + m_config.meterInfo.MB_ProgramId + m_config.meterInfo.MB_Code1 + m_config.meterInfo.MB_Code2;

            code_array[2] = code_perfix + m_config.meterInfo.MC_ProgramId + m_config.meterInfo.MC_Code1 + m_config.meterInfo.MC_Code2;

            code_array[3] = code_perfix + m_config.meterInfo.MD_ProgramId + m_config.meterInfo.MD_Code1 + m_config.meterInfo.MD_Code2;
            return code_array;
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
        public static readonly string COLLECT_FACTOR_CODE_ELECTRICITY = "15";
        //从配置文件中合成FunctionID
        /// <summary>
        /// 生成计量装置的具体采集功能编号
        /// 1 平行于光伏组件的太阳辐照度
        /// 2 室外温度 
        /// 3 光伏组件背面表面温度
        /// 4 发电量
        /// </summary>
        public static string[] GenerateFunID()
        {
            string[] code_array = new string[4];
            if (code_array == null)
            {
                LogManager.Logger.WriteWarnLog("001072:Failed to generate Function ID");
                return null;
            }
            string code_perfix = m_config.areacode + m_config.programid;
            code_array[0] = code_perfix + m_config.techtype + m_config.syscode + COLLECT_FACTOR_CODE_RADIATION;

            code_array[1] = code_perfix + m_config.techtype + m_config.syscode + COLLECT_FACTOR_CODE_AIRTEMP;

            code_array[2] = code_perfix + m_config.techtype + m_config.syscode + COLLECT_FACTOR_CODE_LANDTEMP;

            code_array[3] = code_perfix + m_config.techtype + m_config.syscode + COLLECT_FACTOR_CODE_ELECTRICITY;
            return code_array;
        }

    }
}
