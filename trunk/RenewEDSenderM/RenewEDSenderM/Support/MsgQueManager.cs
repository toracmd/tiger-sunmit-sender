using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Threading;

namespace RenewEDSenderM.Support
{
    /// <summary>
    /// 消息队列类
    /// </summary>
    public class MsgQueManager
    {
        /// <summary>
        /// Message Queue 路径
        /// </summary>
        public static string MessageQueuePath = @".\private$\RenewableQueue";

        /// <summary>
        /// 最大并发线程数
        /// </summary>
        private static int MAX_WORKER_THREADS = 30;
        /// <summary>
        /// Message Queue 实体
        /// </summary>
        private static MessageQueue instance = getInstance();

        /// <summary>
        /// 句柄
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
        private static readonly string READCONFIG = @"读配置...";
        private static readonly string CONNECTING = @"读配置成功,正在连接...";
        private static readonly string VERIFY = @"连接已成功,发送认证请求...";
        private static readonly string VERIFY_MD5 = @"收到随机序列,发送MD5认证";
        private static readonly string VERIFY_FAIL = @"认证失败!";
        private static readonly string VERIFY_PASS = @"认证已通过,准备发送数据...";
        private static readonly string REPORT = @"正在上传...";
        private static readonly string REUPLOAD = @"正在重传...";
        private static readonly string HEARTBEAT = @"保活连接,发送心跳数据...";
        private static readonly string INVALID = @"连接无效...";
        private static readonly string REPLY_ACK = @"回复应答ACK...";
        private static readonly string CONNECT_RETRY10 = @"重试连接已超过10次...请检查服务器信息配置是否正确";
        private static readonly string AUTHENTICATION_RETRY10 = @"认证重试超过10次...请检查项目编号密钥等认证信息是否正确";
        
        private static readonly string isConnected = "已连接";
        private static readonly string isDisconnected = "连接已断开";

        public static readonly string[] CONNECT_STATUS = { isConnected, isDisconnected};

        public static readonly string[] RUN_STAGE_ARRAY = { READCONFIG, CONNECTING, VERIFY, VERIFY_MD5, VERIFY_FAIL, VERIFY_PASS, REPORT, REUPLOAD, HEARTBEAT, INVALID, REPLY_ACK, CONNECT_RETRY10, AUTHENTICATION_RETRY10 };
    }
    /// <summary>
    /// 运行阶段枚举类型
    /// </summary>
    public enum RUN_PHASE
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        READCONFIG,
        /// <summary>
        /// 正在连接
        /// </summary>
        CONNECTING,
        /// <summary>
        /// 发送认证请求
        /// </summary>
        VERIFY,
        /// <summary>
        /// MD5认证
        /// </summary>
        VERIFY_MD5,
        /// <summary>
        /// 认证失败
        /// </summary>
        VERIFY_FAIL,
        /// <summary>
        /// 认证通过
        /// </summary>
        VERIFY_PASS,
        /// <summary>
        /// 正在上传数据
        /// </summary>
        REPORT,
        REUPLOAD,
        /// <summary>
        /// 发送心跳数据包
        /// </summary>
        HEARTBEAT,
        /// <summary>
        /// 无效
        /// </summary>
        INVALID,
        REPLY_ACK,
        CONNECT_RETRY10,
        AUTHENTICATION_RETRY10
    }
}
