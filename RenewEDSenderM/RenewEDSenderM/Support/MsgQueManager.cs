using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Threading;

namespace RenewEDSenderM.Support
{
    /// <summary>
    /// T.B.D.
    /// windows 2003：控制面板---添加/删除程序---添加/删除windows组件---应用程序服务器--勾选 消息队列
    /// win7：控制面板---程序和功能---打开或关闭windows功能---勾选 Microsoft Message Queue (MSMQ)服务器
    /// XP profession：控制面板---添加/删除程序----添加/删除windows组件---勾选 消息队列
    /// </summary>
    public class MsgQueManager
    {
        /// <summary>
        /// Message Queue 路径
        /// </summary>
        public static string MessageQueuePath = @".\private$\RenewableQueue";

        /// <summary>
        /// 最大并发线程数T.B.D.
        /// </summary>
        private static int MAX_WORKER_THREADS = 30;
        /// <summary>
        /// Message Queue 实体
        /// </summary>
        private static MessageQueue instance = getInstance();

        /// <summary>
        /// T.B.D.
        /// </summary>
        private static WaitHandle[] waitHandleArray;// = new WaitHandle[MAX_WORKER_THREADS];

        private static string MsgLabel = String.Format("Network state");
        /// <summary>
        /// 获得消息队列实体
        /// </summary>
        /// <returns></returns>
        public static MessageQueue getInstance()
        {
            if (MessageQueue.Exists(MessageQueuePath))
            {
                //如果已存在则返回已有消息队列
                instance = new MessageQueue(MessageQueuePath);
            }
            else
            {
                //否则新建消息队列
                instance = MessageQueue.Create(MessageQueuePath);
            }
            //返回实例
            return instance;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public static void Dispose()
        {
            getInstance().Close();
            MessageQueue.Delete(MessageQueuePath);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// 
        public void SendMsg(MsgBody msg)
        {
            //序列化
            getInstance().Formatter = new XmlMessageFormatter(
                    new Type[] { typeof(MsgBody) }
                    );
            //发送消息到消息队列
            getInstance().Send(msg, MsgLabel);
        }
        /// <summary>
        /// 同步接收消息
        /// </summary>
        /// <returns></returns>
        public MsgBody RecvMsg()
        {
            Message msg = new Message();
            try
            {
                msg = getInstance().Receive();
                msg.Formatter = new XmlMessageFormatter(
                         new Type[]
                        {
                            typeof(MsgBody)
                        } );
            }
            catch(Exception e)
            {
                return null;
            }
            return (MsgBody)msg.Body;
        }
        /// <summary>
        /// T.B.D. 删除
        /// </summary>
        /// <param name="myReceiveCompleted"></param>
        public void MsgQStartListen(ReceiveCompletedEventHandler myReceiveCompleted)
        {
            //while(true)

            getInstance().ReceiveCompleted += new ReceiveCompletedEventHandler(myReceiveCompleted);
            getInstance().BeginReceive();

            //WaitHandle w = getInstance().BeginReceive().AsyncWaitHandle;
            //getInstance().BeginReceive();
            //for (int i = 0; i < MAX_WORKER_THREADS; i++)
            //{
            //    waitHandleArray[i] = getInstance().BeginReceive().AsyncWaitHandle;

            //}
        }
        /// <summary>
        /// T.B.D.删除
        /// </summary>
        public void MsgQStopListen()
        {
            for (int i = 0; i < waitHandleArray.Length; i++)
            {
                try
                {
                    waitHandleArray[i].Close();
                }
                catch(Exception e)
                {
                }
            }
            try
            {
                WaitHandle.WaitAll(waitHandleArray, 1000, false);
            }
            catch (Exception e)
            {
            }
        }
    }
    /// <summary>
    /// 消息体类
    /// </summary>
    public class MsgBody
    {
        /// <summary>
        /// 网络连接状态
        /// </summary>
        public bool isConnected;
        /// <summary>
        /// 认证状态
        /// </summary>
        public bool isVerified;

        /// <summary>
        /// 发送运行阶段
        /// </summary>
        public RUN_PHASE phase;

        /// <summary>
        /// 用于消息队列，必须保留
        /// </summary>
        public MsgBody()
        {
        }
        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="conn">连接状态</param>
        /// <param name="syn">认证状态</param>
        /// <param name="r">运行阶段</param>
        public MsgBody(bool conn, bool syn ,RUN_PHASE r)
        {
            isConnected = conn;
            isVerified = syn;
            phase = r;
        }
        /// <summary>
        /// 构造函数重载
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="r"></param>
        public MsgBody(bool conn, RUN_PHASE r)
        {
            isConnected = conn;
            phase = r;
        }
        
    }
    /// <summary>
    /// 运行状态工具类
    /// </summary>
    public class RUN_STATUS_MEASURE
    {
        private static readonly string CONNECTED = "正在连接...";
        private static readonly string VERIFY = "正在认证...";
        private static readonly string REPORT = "正在上传...";
        private static readonly string HEARTBEAT = "保活连接...";
        private static readonly string INVALID = "无效...";
        
        private static readonly string isConnected = "已连接";
        private static readonly string isDisconnected = "连接已断开";

        public static readonly string[] CONNECT_STATUS = { isConnected, isDisconnected};

        public static readonly string[] RUN_STAGE_ARRAY = { CONNECTED, VERIFY, REPORT, HEARTBEAT, INVALID };
    }
    /// <summary>
    /// 运行阶段枚举类型
    /// </summary>
    public enum RUN_PHASE
    {
        CONNECTED,
        VERIFY,
        REPORT,
        HEARTBEAT,
        INVALID
    }
}
