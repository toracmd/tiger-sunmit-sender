using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenewEDSenderWin
{
    public class ErrorMessage
    {
        public static readonly string ERR_VALIDATE_KEY_PATTERN = @"提示：密钥需要16字节字符";

        public static readonly string ERR_VALIDATE_IV_PATTERN = @"提示：向量需要16字节字符";

        public static readonly string ERR_VALIDATE_IP_PATTERN = @"提示：IP地址格式不正确";

        public static readonly string ERR_VALIDATE_PORT_PATTERN = @"提示：端口介于0~65535";

        public static readonly string ERR_VALIDATE_CODE_PATTERN = @"提示：编码不正确";
    }
    public class RegexPattern
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public static readonly string REGEX_IP = @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$";

        /// <summary>
        /// 密钥、IV格式
        /// </summary>
        public static readonly string REGEX_16CH = @"^[0-9a-zA-Z]{16}$";

        /// <summary>
        /// 行政区域码
        /// </summary>
        public static readonly string REGEX_AREA_CODE = @"^[0-9]{6}$";

        /// <summary>
        /// 项目编码
        /// </summary>
        public static readonly string REGEX_PROJECT_CODE = @"^[0-9a-zA-Z]{3}$";

        /// <summary>
        /// 计数类型编码
        /// </summary>
        public static readonly string REGEX_TECH_TYPE = @"^[0-9]{3}$";

        /// <summary>
        /// 系统编码
        /// </summary>
        public static readonly string REGEX_SYS_CODE = @"^[0-9]{2}$";

        /// <summary>
        /// 采集指标编码
        /// </summary>
        public static readonly string REGEX_COLLECT_TARGET = @"^[0-9]{2}$";

        /// <summary>
        /// 装置识别编码
        /// </summary>
        public static readonly string REGEX_COLLECT_DEVICE = @"^[0-9]{2}$";

        /// <summary>
        /// 采集点识别编码
        /// </summary>
        public static readonly string REGEX_COLLECT_POINT = @"^[0-9]{2}$";
    }
}
