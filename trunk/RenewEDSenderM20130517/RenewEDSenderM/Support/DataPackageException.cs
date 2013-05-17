using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenewEDSenderM.Support
{
    class DataPackageException : ApplicationException
    {
        string _message;
        /// <summary>
        /// 数据包Head不合法
        /// </summary>
        public static readonly string ex_msg1_head = "数据包Head不合法";
        /// <summary>
        /// 数据包Tail不合法
        /// </summary>
        public static readonly string ex_msg2_tail = "数据包Tail不合法";
        /// <summary>
        /// 数据包CRC格式不合法
        /// </summary>
        public static readonly string ex_msg3_crc = "数据包CRC格式不合法";
        /// <summary>
        /// 数据包CRC数据校验出错
        /// </summary>
        public static readonly string ex_msg4_crc = "数据包CRC数据校验出错";
        /// <summary>
        /// 数据包序号段不合法
        /// </summary>
        public static readonly string ex_msg5_seq = "数据包序号段不合法";
        /// <summary>
        /// 数据包数据段出错
        /// </summary>
        public static readonly string ex_msg6_data = "数据包数据段出错";
        /// <summary>
        /// 数据包长度出错
        /// </summary>
        public static readonly string ex_msg7_pkg = "数据包长度出错";
        //获取父类的错误信息内容
        public DataPackageException()
        {
            _message = base.Message;
        }
        public DataPackageException(string str)
        {
            _message = str;
        }
        //重写message属性
        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }
}
