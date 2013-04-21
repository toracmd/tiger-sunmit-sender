using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RenewEDSenderM.Support
{
    class MessageDatagram : Object
    {
        /// <summary>
        /// 包头(4字节)0x68 0x68 0x16 0x16
        /// </summary>
        private static byte[] m_header = { 0x68,0x68, 0x16, 0x16};
        /// <summary>
        /// 有效数据总长度(4字节) 
        /// </summary>
        private int m_datalen; //T.B.D. 字节序问题,反转
        /// <summary>
        /// 指令序号(4字节)
        /// </summary>
        private int m_seq; //T.B.D. 序号要是超过了呢
        /// <summary>
        /// 指令内容（M字节）根据指令的不同，内容不同，指令内容为经过 AES加密后的 XML 文本
        /// </summary>
        private byte[] m_data = {}; //T.B.D. 补齐问题
        /// <summary>
        /// CRC16 校验（2字节）
        /// </summary>
        private UInt16 m_crc16;
        /// <summary>
        /// 包尾（4字节）0x55 0xAA 0x55 0xAA
        /// </summary>
        static byte[] m_tail = {0x55,0xAA,0x55,0xAA };
        byte[] package = {};
         

        public MessageDatagram()
        {
            
            m_datalen = 0;
            m_seq = 0;
            m_crc16 = 0;
            Array.Clear(package, 0, package.Length);
            
            Array.Copy(m_header, package, 4);
        }
        public void SetSeq(int seq)
        {
            if (seq > int.MaxValue)
            {
                LogManager.Logger.WriteWarnLog(" sequence of datagram is over the allowance ");
            }
            m_seq = seq;
        }
        public void SetData(byte[] data)
        {
            m_data = data;
            m_datalen = m_data.Length;
            // T.B.D. CRC16
            m_crc16 = Support.Encryption.CRC16(data);
        }
        /// <summary>
        /// 数据打包
        /// </summary>
        /// <returns></returns>
        public bool pack()
        {
            Convert.ToString(m_datalen, 16);

            return false;
        }
        /// <summary>
        /// 数据解包
        /// </summary>
        /// <returns></returns>
        public bool unpack()
        {
            string str_datalen;

            
            //Array.Copy(package, 4, str_datalen, 4);//T.B.D. 字节序问题 e4 00 00 00 = 228 
            return false;
        }
    }
    class TestDatagram
    {
        MessageDatagram md = new MessageDatagram();
        public void ConvertData()
        {
            LogManager.Logger.WriteDebugLog(md.ToString());
        }
    }
}
