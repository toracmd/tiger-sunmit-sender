using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;

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
        public static string MessageQueueName = @".\private$\RenewableQueue";

        private static MessageQueue instance;

        private MsgQueManager()
        {

        }

        public static MessageQueue getInstance()
        {
            if (MessageQueue.Exists(MessageQueueName))
                instance = new MessageQueue(MessageQueueName);
            else
                instance = MessageQueue.Create(MessageQueueName);
            //MsgQueManager msgm = new MsgQueManager();
            return instance;
        }
        public static void Dispose()
        {
            getInstance().Close();
            MessageQueue.Delete(MessageQueueName);
        }
        public static void SendMsg(MsgBody msg, string label)
        {
            getInstance().Send(msg, label);
        }
        public static MsgBody RecvMsg(MessageQueue sender, ReceiveCompletedEventArgs e)
        {
            MessageQueue msgq = (MessageQueue)sender;
            System.Messaging.Message msg = msgq.EndReceive(e.AsyncResult);
            return (MsgBody)msg.Body;
        }
        public static MsgBody RecvMsg()
        {
            MessageQueue msgq = MsgQueManager.getInstance();
            msgq.Formatter = new XmlMessageFormatter(
                    new Type[]
                        {
                            typeof(MsgBody)
                        }
                );
            Message msg = msgq.Receive();

            return (MsgBody)msg.Body;
        }
    }
    public class MsgBody
    {
        public bool isConnected;

        /// <summary>
        /// 运行到第几步了
        /// </summary>
        //public int Step;
    }
}
