using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

namespace RenewEDSenderM.Support
{
    /// <summary>
    /// 问题：收到数据包、发送数据包，透明比特流与有意义的字段转换
    /// 目的：类似C语言结构体类型强转
    /// 方案：用属性的特殊控制手段，此种情况下类的特殊初始化方法
    /// </summary>
    class DataPackage
    {
        /// <summary>
        /// 用于头部校验
        /// </summary>
        private static byte[] Verify_Head = { 0x68, 0x68, 0x16, 0x16 };
        /// <summary>
        /// 用于尾部校验
        /// </summary>
        private static byte[] Verify_Tail = { 0x55, 0xAA, 0x55, 0xAA };
        /// <summary>
        /// 数据包长度下限18bytes，用于数据包校验
        /// </summary>
        private static uint PkgLengthMin = 18;
        /// <summary>
        /// 数据包长度上限，用于数据包校验
        /// </summary>
        private static uint PkgLengthMax = uint.MaxValue ;
        private byte[] m_head = { 0x68, 0x68, 0x16, 0x16 } ;	//缺省值
        private byte[] m_dataLength = new byte[sizeof(uint)];   //m_dataLength = sizeof(m_seq) + sizeof(m_data)
        private byte[] m_seq = new byte[sizeof(uint)];  //
        private byte[] m_data;
        private byte[] m_dummy; //T.B.D. 不考虑？
        private byte[] m_crc16 = new byte[sizeof(UInt16)];
        private byte[] m_tail = { 0x55, 0xAA, 0x55, 0xAA };		//缺省值
        #region 打包
        /// <summary>
        /// 收到的字节流进行解析验证
        /// 验证失败抛出异常
        /// </summary>        
        public byte[] Package
        {
            //收到一个包之后，映射到各字段
            set
            {
                //try
                //{
                //解析收到的包，验证包长度
                if (value.Length < PkgLengthMin/*  || value.Length > PkgLengthMax*/)
                    throw new DataPackageException(DataPackageException.ex_msg7_pkg);
                //解析出头部
                Array.Copy(value, m_head, 4);
                //校验头部
                
                //if (!Comparer.Equals(Verify_Head, m_head))
                    //throw new DataPackageException(DataPackageException.ex_msg1_head);
                //解析出有效数据长度字段
                Array.Copy(value, 4, m_dataLength, 0, 4);
                //解析出序号字段
                Array.Copy(value, 8, m_seq, 0, 4);
                //校验有效数据段长度与数据实际长度一致性，PkgLengthMin为除数据段以外的数据大小
                if((DataLength - 4) != (value.Length - PkgLengthMin))
                    throw new DataPackageException(DataPackageException.ex_msg6_data);
                //解析出有效数据段数据,DataLength为AES加密后的指令内容与指令序号的总长度
                m_data = new byte[DataLength - 4];
                Array.Copy(value, 12, m_data, 0, DataLength - 4);
                byte[] data_crc = new byte[DataLength + 10];
                //解析得到待校验数据[包头到有效数据段]，AES加密后的指令内容：CRC16校验码
                Array.Copy(value, 0, data_crc, 0, DataLength + 10);
                //CRC校验
                byte tmp;
                tmp = data_crc[data_crc.Length - 1];
                data_crc[data_crc.Length - 1] = data_crc[data_crc.Length - 2];
                data_crc[data_crc.Length - 2] = tmp;
                if (Encryption.CRC16(data_crc) != 0)
                    throw new DataPackageException(DataPackageException.ex_msg4_crc);
                Array.Copy(value, 12 + DataLength - 4, m_crc16, 0, 2);
                Array.Copy(value, 12 + DataLength - 4 + 2, m_tail, 0, 2);
            }
            //发送一个包时，发送整个段
            get
            {
                ArrayList arraylist = new ArrayList();
                byte[] bytes;
                arraylist.AddRange(m_head);
                arraylist.AddRange(m_dataLength);
                arraylist.AddRange(m_seq);
                arraylist.AddRange(m_data);
                arraylist.AddRange(m_crc16);
                arraylist.AddRange(m_tail);
                bytes = arraylist.ToArray(typeof(byte)) as byte[];
                return bytes;
            }
        }
        /// <summary>
        /// 数据包头
        /// </summary>
        public byte[] Head
        {
            set
            {
                //T.B.D. 需要校验头部否？
                //byte[] h = { 0x68, 0x68, 0x16, 0x16 };
                m_head = value;
            }
            get
            {
                return m_head;
            }
        }
        /// <summary>
        /// 数据包长度
        /// </summary>
        public uint DataLength
        {
            get
            {
                if (m_dataLength.Length != 4)
                {
                   // LogManager.Logger.WriteWarnLog("数据长度字段出错{0} != 4", m_dataLength.Length);
                    return 0;
                }
                return BitConverter.ToUInt32(m_dataLength, 0);
            }
            set
            {
                m_dataLength = BitConverter.GetBytes(value);
            }
        }
        /// <summary>
        /// 数据包序号
        /// </summary>
        public uint Seq
        {
            get
            {
                if (m_seq.Length != 4)
                {
                   // LogManager.Logger.WriteWarnLog("序号长度字段出错{0} != 4", m_seq.Length);
                }
                return BitConverter.ToUInt32(m_seq, 0);
            }
            set
            {
                m_seq = BitConverter.GetBytes(value);
            }
        }
        /// <summary>
        /// 数据包数据字段
        /// </summary>
        public byte[] DataBlock
        {
            get
            {
                return m_data;
            }
            set
            {
                if ((value.Length) != DataLength - 4)
                { 
                  //  LogManager.Logger.WriteErrorLog("数据长度字段为{0}, 数据部分大小为{1} ", DataLength, m_data.Length);
                    return;
                }
                m_data = value;
            }
        }
        public byte[] Dummy
        {
            set
            {

            }
        }
        /// <summary>
        /// CRC16校验码
        /// </summary>
        public UInt16 CRC
        {
            get
            {
                if (m_crc16.Length != 2)
                {
                    //LogManager.Logger.WriteWarnLog("CRC字段出错{0} != 2", m_crc16.Length);
                }
                return BitConverter.ToUInt16(m_crc16, 0);
            }
            set
            {
                m_crc16 = BitConverter.GetBytes(value);
            }
        }
        /// <summary>
        /// 数据包尾
        /// </summary>
		public byte[] Tail
        {
            set
            {
                //T.B.D. 需要校验头部否？
                byte[] h = { 0x68, 0x68, 0x16, 0x16 };
                m_tail = h;
            }
            get
            {
                return m_tail;
            }
        } 
        #endregion
        #region 解包
        #endregion
        /// <summary>
        /// 将十六进制字符串转换为字节序列
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="discarded"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            string newString = "";
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (Uri.IsHexDigit(c))
                    newString += c;
                else
                    discarded++;
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = newString.Substring(0, newString.Length - 1);
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { newString[j], newString[j + 1] });
                bytes[i] = Convert.ToByte(hex, 16);
                j = j + 2;
            }
            return bytes;
        }
    }
}
