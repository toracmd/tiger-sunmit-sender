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
    class Testt
    {
        /// <summary>
        /// 测试初始化1
        /// </summary>
        public static void testInit()
        {
            // 收到包
            byte[] c = { 0x01, 0x02 };
            
            { 
            }
            //Test 初始化

            DataPackage dp = new DataPackage() { Package = c };

        }
        /// <summary>
        /// 测试初始化2
        /// </summary>
        public static void testInit2()
        {
            //发送包
            int outi;
            string str = "AD4E729DCD81551ABC7C2983C86D3C6A58A862EAB7762371A0F6A3C83FBDC8EC79D54004F3249D83EE1D8BBE3C906270C99C44D6CF4C7B049B643A7DA312DCC0C5600F18FBB91ADF460802532B9FD2C766EC60477242D78F9CAA0092DA2F1B0492D92951E9EF1E68096B7EA76C41ABF30EB177D316196693D66C3C8AB2C767ED461BABF65A472C57DE9C972B10EDE61E916F38D678C48ED447928E77EF77798D71F71B29AA4A2542C866B4A623FDC6F8838B2D3AD851EC9091C029072A49BB9DDFCCF2831858E339FE73534A65432E5AC65F3E0DF735510BCD64F8B7C2B1F264";
            DataPackage dp = new DataPackage() { DataLength = 0x000000e4, Seq = 0x1692ec42, CRC=0x6441, DataBlock= DataPackage.GetBytes(str, out outi)};
            byte [] package = dp.Package;
        }
        
    }
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
                if (Support.Encryption.CRC16(data_crc) != 0)
                    throw new DataPackageException(DataPackageException.ex_msg4_crc);
                    Array.Copy(value, 12 + DataLength - 4, m_crc16, 0, 2);
                    Array.Copy(value, 12 + DataLength - 4 + 2, m_tail, 0, 2);
                //}
                //catch (ArgumentException aex)
                //{
                //    throw 
                //}
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
        public uint DataLength
        {
            get
            {
                if (m_dataLength.Length != 4)
                {
                    LogManager.Logger.WriteWarnLog("数据长度字段出错{0} != 4", m_dataLength.Length);
                    return 0;
                }
                return BitConverter.ToUInt32(m_dataLength, 0);
            }
            set
            {
                m_dataLength = BitConverter.GetBytes(value);
            }
        }
        public uint Seq
        {
            get
            {
                if (m_seq.Length != 4)
                {
                    LogManager.Logger.WriteWarnLog("序号长度字段出错{0} != 4", m_seq.Length);
                }
                return BitConverter.ToUInt32(m_seq, 0);
            }
            set
            {
                m_seq = BitConverter.GetBytes(value);
            }
        }
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
                    LogManager.Logger.WriteErrorLog("数据长度字段为{0}, 数据部分大小为{1} ", DataLength, m_data.Length);
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
        public UInt16 CRC
        {
            get
            {
                if (m_crc16.Length != 2)
                {
                    LogManager.Logger.WriteWarnLog("CRC字段出错{0} != 2", m_crc16.Length);
                }
                return BitConverter.ToUInt16(m_crc16, 0);
            }
            set
            {
                m_crc16 = BitConverter.GetBytes(value);
            }
        }
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
        public static void test()
        {
            byte[] m = { 0xe4, 0x00, 0x00, 0x00 };
            uint u = BitConverter.ToUInt32(m, 0);
            //初始化方式
            DataPackage dp = new DataPackage() { Head = new byte[4]};
            //string s = m.ToString();
            //UInt32 u = Convert.ToUInt32(s, 10);
            //uint uu = u;
        }
    }
    /// <summary>
    /// 发送数据包格式
    /// </summary>
    [Serializable]
    class DataPackage1
    {
        #region field
        private byte[] m_header = { 0x68, 0x68, 0x16, 0x16 };
        private uint m_dataLength;
        private uint m_seq;
        private byte[] m_data;
        private byte[] m_dummy;
        //private byte[] m_dummy2 = { 0x00, 0x00 }; 
        private UInt16 m_crc16;
        private byte[] m_tail = {0x55,0xAA,0x55,0xAA };
        #endregion
        #region gets and sets
        /// <summary>
        /// 有效数据长度(byte)
        /// </summary>
        public uint DataLength
        {
            get
            {
                return m_dataLength;
            }
            set
            {
                m_dataLength = value;
            }
        }
        
        //public uint DataBlockLength
        //{
        //    get
        //    {
        //        if ((m_dataLength & 0x00000004) == 0)
        //        {
        //            return m_dataLength;
        //        }
        //        else
        //        {
        //            return (m_dataLength & 0xFFFFFFFC) + 4;
        //        }
        //        /*return sizeof(uint) - m_dataLength % sizeof(uint) */
        //    }
        //    set
        //    {
        //    }
        //}
        /// <summary>
        /// 序号
        /// </summary>
        public uint Sequence
        {
            get
            {
                return m_seq;
            }
            set
            {
                m_seq = value;
            }
        }
        /// <summary>
        /// 数据段
        /// </summary>
        public byte[] DataBlock
        {
            set
            {
                m_data = value;
                //DataLength = (uint)m_data.Length;
                //设置padding的长度
                if ((DataLength & 0x00000004) == 0)
                {
                    uint nPadding = sizeof(uint) - DataLength % sizeof(uint) + 2;
                    m_dummy = new byte[nPadding];
                }
            }
            get
            {
                return m_data;
            }
        }
        public UInt16 CRC16
        {
            set
            {
                m_crc16 = value;
            }
            get
            {
                return m_crc16;
            }
        }
        #endregion
        #region Serialize
        public static byte[] Serialize(DataPackage1 dp)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            
            formatter.Serialize(ms, dp);
            byte[] b = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(b, 0, b.Length);
            ms.Close();
            return b;
        }

        public static DataPackage1 Deserialize(byte[] byteArray)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            ms.Write(byteArray, 0, byteArray.Length);
            ms.Position = 0;
            DataPackage1 dp = formatter.Deserialize(ms) as DataPackage1;
            return dp;
        }
        #endregion
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
    class TestDp
    {
        public static void TestPack()
        {
            
            //IsHexDigit('c');
            //T.B.D. 1 判断Datablock长度是否等于DataLength
            //T.B.D. 2 数据字符集 ASCII？Encoding将字符串以二进制显示，但是这本来就是二进制串->实质是二进制输入问题
            #region Test Encoding
            byte[] x = { 0xAD, 0x4E };
            byte[] z = Encoding.ASCII.GetBytes("AD4E");
            int l;
            byte[] y = DataPackage1.GetBytes("AD4E", out l);//T.B.D. 大小写呢
            #endregion
            byte[] block = { 0xAD, 0x4E, 0xCD, 0xDE };
            DataPackage1 dp = new DataPackage1()
            {
                DataLength = 0x00000008,
                Sequence = 0x42ec9216,
                CRC16 = 0x6441,
                DataBlock = block,
            };
            byte[] output = DataPackage1.Serialize(dp);
            
            LogManager.Logger.WriteDebugLog("{0}", output);    
        }
        public static void TestUnPack()
        {
        }
    }
}
