/*  
 *  Configuration --- 读取配置文件Config.xml中的各种配置参数，可以手动修改，或者通过
 *                    从服务器端接收到的命令来进行配置和修改。
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;


namespace RenewEDSenderM.CommManager
{   
    /// <summary>
    /// 读取配置文件中的配置参数，并可以实时修改配置参数
    /// </summary>

    public class SetConfig
    {
        
        private static string m_server_ip = "";
        private static string m_server_port = "";
        private static string m_client_project_id = "";
        private static string m_client_gateway_id = "";
        private static string m_client_notifyTime = "";
        private static string m_client_reportTime = "";
        private static string m_client_verifyTime = "";
        private static string m_client_times = "";
        private static string m_client_period = "";
        private static string m_client_key = "";
        private static string m_client_md5 = "";
        private static string m_client_iv = "";
		private static string m_client_areacode = "";
        private static string m_client_programid = "";
        private static string m_client_techtype = "";
        private static string m_client_syscode = "";
        
       
        private static string config_path = "./Config/Config.xml";
		//T.B.D. 路径问题
        //private static string config_path = "../../../RenewEDSenderM/bin/Debug/Config/Config.xml";
        
        public Configuration ReadConfig()
        {
            Configuration config = new Configuration();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(config_path);
            XmlNode root = xmlDoc.SelectSingleNode("config");
            XmlNodeList rList = root.ChildNodes;


            //读取各级参数
            foreach(XmlNode xr in rList)
            {
                //读取服务器相关参数
                if (xr.Name == "Server_Information")
                {
                    XmlNodeList sList = xr.ChildNodes;
                    foreach (XmlNode xs in sList)
                    {
                        //获取服务器的IP地址
                        if (xs.Name== Commands.IP)
                        {
                            m_server_ip = xs.InnerText;
                            config.ip = m_server_ip;
                            continue;
                        }

                        //获取服务器的端口
                        if (xs.Name == Commands.PORT)
                        {
                            m_server_port = xs.InnerText;
                            config.port = m_server_port;
                            continue;
                        }
                    }
                }

                if (xr.Name == "Client_Information")
                {
                    XmlNodeList cList = xr.ChildNodes;
                    foreach (XmlNode xc in cList)
                    {
                        //获取客户端的project_id
                        if (xc.Name == Commands.PROJECT_ID)
                        {
                            m_client_project_id = xc.InnerText;
                            config.project_id = m_client_project_id;
                            continue;
                        }

                        //获取客户端的gateway_id
                        if (xc.Name == Commands.GATEWAY_ID)
                        {
                            m_client_gateway_id = xc.InnerText;
                            config.gateway_id = m_client_gateway_id;
                            continue;
                        }

                        //获取客户端的心跳数据间隔
                        if (xc.Name == Commands.NOTIFY_TIME)
                        {
                            m_client_notifyTime = xc.InnerText;
                            config.notifyTime = m_client_notifyTime;
                            continue;
                        }

                        //获取客户端数据发送间隔
                        if (xc.Name == Commands.REPORT_TIME)
                        {
                            m_client_reportTime = xc.InnerText;
                            config.reportTime = m_client_reportTime;
                            continue;
                        }

                        //获取客户端认证超时
                        if (xc.Name == Commands.VERIFY_TIME)
                        {
                            m_client_verifyTime = xc.InnerText;
                            config.verifyTime = m_client_verifyTime;
                            continue;
                        }

                        //获取客户端失败尝试次数
                        if (xc.Name == Commands.TIMES)
                        {
                            m_client_times = xc.InnerText;
                            config.times = m_client_times;
                            continue;
                        }

                        //获取客户端的周期参数
                        if (xc.Name == Commands.PERIOD)
                        {
                            m_client_period = xc.InnerText;
                            config.period = m_client_period;
                            continue;
                        }

                        //获取客户端AES密钥
                        if (xc.Name == Commands.KEY)
                        {
                            m_client_key = xc.InnerText;
                            config.key = m_client_key;
                            continue;
                        }

                        //获取客户端MD5 值
                        if (xc.Name == Commands.MD5)
                        {
                            m_client_md5 = xc.InnerText;
                            config.md5 = m_client_md5;
                            continue;
                        }

                        //获取客户端AES初始向量
                        if (xc.Name == Commands.IV)
                        {
                            m_client_iv = xc.InnerText;
                            config.iv = m_client_iv;
                            continue;
                        }

                        //获取客户端行政区编码
                        if (xc.Name == Commands.AREA_CODE)
                        {
                            m_client_areacode = xc.InnerText;
                            config.areacode = m_client_areacode;
                            continue;
                        }

                        //获取客户端项目编码
                        if (xc.Name == Commands.PROGRAM_ID)
                        {
                            m_client_programid = xc.InnerText;
                            config.programid = m_client_programid;
                            continue;
                        }

                        //获取客户端技术类型
                        if (xc.Name == Commands.TECH_TYPE)
                        {
                            m_client_techtype = xc.InnerText;
                            config.techtype = m_client_techtype;
                            continue;
                        }

                        //获取客户端系统编码
                        if (xc.Name == Commands.SYS_CODE)
                        {
                            m_client_syscode = xc.InnerText;
                            config.syscode = m_client_syscode;
                            continue;
                        }
                        if (xc.Name == Commands.METER)
                        {
                            MeterInfo meterInfo = new MeterInfo();
                            meterInfo.MA_ProgramId = xc.Attributes["MA_ProgramId"].Value;
                            meterInfo.MA_Code1 = xc.Attributes["MA_Code1"].Value;
                            meterInfo.MA_Code2 = xc.Attributes["MA_Code2"].Value;
                            meterInfo.MB_ProgramId = xc.Attributes["MB_ProgramId"].Value;
                            meterInfo.MB_Code1 = xc.Attributes["MB_Code1"].Value;
                            meterInfo.MB_Code2 = xc.Attributes["MB_Code2"].Value;
                            meterInfo.MC_ProgramId = xc.Attributes["MC_ProgramId"].Value;
                            meterInfo.MC_Code1 = xc.Attributes["MC_Code1"].Value;
                            meterInfo.MC_Code2 = xc.Attributes["MC_Code2"].Value;
                            meterInfo.MD_ProgramId = xc.Attributes["MD_ProgramId"].Value;
                            meterInfo.MD_Code1 = xc.Attributes["MD_Code1"].Value;
                            meterInfo.MD_Code2 = xc.Attributes["MD_Code2"].Value;
                            config.meterInfo = meterInfo;
                            continue;
                        }
                    }
                }
                
            }
            return config;
        }

        public bool WriteConfig(Configuration config)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(config_path);
            XmlNode root = xmlDoc.SelectSingleNode("config");
            XmlNodeList rList = root.ChildNodes;


            //读取各级参数
            foreach (XmlNode xr in rList)
            {
                //读取服务器相关参数
                if (xr.Name == "Server_Information")
                {
                    XmlNodeList sList = xr.ChildNodes;
                    foreach (XmlNode xs in sList)
                    {
                        //设置服务器的IP地址
                        if (xs.Name == Commands.IP)
                        {
                            XmlElement xst = (XmlElement)xs;
                            m_server_ip = config.ip;
                            xst.InnerText = m_server_ip;
                            continue;
                        }

                        //设置服务器的端口
                        if (xs.Name == Commands.PORT)
                        {
                            XmlElement xst = (XmlElement)xs;
                            m_server_port = config.port;
                            xst.InnerText = m_server_port;
                            continue;
                        }
                    }
                }

                if (xr.Name == "Client_Information")
                {
                    XmlNodeList cList = xr.ChildNodes;
                    foreach (XmlNode xc in cList)
                    {
                        //设置客户端的project_id
                        if (xc.Name == Commands.PROJECT_ID)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_project_id = config.project_id;
                            xct.InnerText = m_client_project_id;
                            continue;
                        }

                        //设置客户端的gateway_id
                        if (xc.Name == Commands.GATEWAY_ID)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_gateway_id = config.gateway_id;
                            xct.InnerText = m_client_gateway_id;
                            continue;
                        }

                        //设置客户端的心跳数据间隔
                        if (xc.Name == Commands.NOTIFY_TIME)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_notifyTime = config.notifyTime;
                            xct.InnerText = m_client_notifyTime;
                            continue;
                        }

                        //设置客户端数据发送间隔
                        if (xc.Name == Commands.REPORT_TIME)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_reportTime = config.reportTime;
                            xct.InnerText = m_client_reportTime;
                            continue;
                        }

                        //设置客户端认证超时
                        if (xc.Name == Commands.VERIFY_TIME)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_verifyTime = config.verifyTime;
                            xct.InnerText = m_client_verifyTime;
                            continue;
                        }

                        //设置客户端失败尝试次数
                        if (xc.Name == Commands.TIMES)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_times = config.times;
                            xct.InnerText = m_client_times;
                            continue;
                        }

                        //设置客户端的周期参数
                        if (xc.Name ==Commands.PERIOD)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_period = config.period;
                            xct.InnerText = m_client_period;
                            continue;
                        }

                        //设置客户端AES密钥
                        if (xc.Name == Commands.KEY)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_key = config.key;
                            xct.InnerText = m_client_key;
                            continue;
                        }

                        //设置客户端MD5值
                        if (xc.Name == Commands.MD5)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_md5 = config.md5;
                            xct.InnerText = m_client_md5;
                            continue;
                        }

                        //设置客户端AES初始向量
                        if (xc.Name == Commands.IV)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_iv = config.iv;
                            xct.InnerText = m_client_iv;
                            continue;
                        }

                        ////获取客户端行政区编码
                        if (xc.Name == Commands.AREA_CODE)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_areacode = config.areacode;
                            xct.InnerText = m_client_areacode;
                            continue;
                        }

                        //设置客户端项目编码
                        if (xc.Name == Commands.PROGRAM_ID)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_programid = config.programid;
                            xct.InnerText = m_client_programid;
                            continue;
                        }

                        //设置客户端技术类型
                        if (xc.Name == Commands.TECH_TYPE)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_techtype = config.techtype;
                            xct.InnerText = m_client_techtype;
                            continue;
                        }

                        //设置客户端系统编码
                        if (xc.Name == Commands.SYS_CODE)
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_syscode = config.syscode;
                            xct.InnerText = m_client_syscode;
                            continue;
                        }
                        if (xc.Name == Commands.METER)
                        {
                            XmlElement xct = (XmlElement)xc;
                            MeterInfo meterInfo = config.meterInfo;

                            xct.SetAttribute("MA_ProgramId", meterInfo.MA_ProgramId);
                            xct.SetAttribute("MA_Code1", meterInfo.MA_Code1);
                            xct.SetAttribute("MA_Code2", meterInfo.MA_Code2);

                            xct.SetAttribute("MB_ProgramId", meterInfo.MB_ProgramId);
                            xct.SetAttribute("MB_Code1", meterInfo.MB_Code1);
                            xct.SetAttribute("MB_Code2", meterInfo.MB_Code2);

                            xct.SetAttribute("MC_ProgramId", meterInfo.MC_ProgramId);
                            xct.SetAttribute("MC_Code1", meterInfo.MC_Code1);
                            xct.SetAttribute("MC_Code2", meterInfo.MC_Code2);

                            xct.SetAttribute("MD_ProgramId", meterInfo.MD_ProgramId);
                            xct.SetAttribute("MD_Code1", meterInfo.MD_Code1);
                            xct.SetAttribute("MD_Code2", meterInfo.MD_Code2);

                            continue;

                        }
                    }
                }

            }
            //修改configuration后，进行保存
            try
            {
                xmlDoc.Save(config_path);
            }
            catch (XmlException xe)
            {
                return false;
            }
            return true;
        }

        public bool WriteSpecailConfig(Configuration config, string valueName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(config_path);
            XmlNode root = xmlDoc.SelectSingleNode("config");
            XmlNodeList rList = root.ChildNodes;

            if (valueName == Commands.IP|| valueName == Commands.PORT)
            {
                foreach (XmlNode xr in rList)
                {
                    if (xr.Name == "Server_Information")
                    {
                        XmlNodeList sList = xr.ChildNodes;
                        foreach (XmlNode xs in sList)
                        {
                            // 只修改ip
                            if (valueName == Commands.IP && xs.Name == Commands.IP)
                            {
                                XmlElement xst = (XmlElement)xs;
                                m_server_ip = config.ip;
                                xst.InnerText = m_server_ip;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }
                            // 只修改port
                            if (valueName == Commands.PORT && xs.Name == Commands.PORT)
                            {
                                XmlElement xst = (XmlElement)xs;
                                m_server_ip = config.ip;
                                xst.InnerText = m_server_ip;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (XmlNode xr in rList)
                {
                    if (xr.Name == "Client_Information" )
                    {
                        XmlNodeList cList = xr.ChildNodes;
                        foreach (XmlNode xc in cList)
                        {
                            // 只修改project_id
                            if (valueName == Commands.PROJECT_ID && xc.Name == Commands.PROJECT_ID)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_project_id = config.project_id;
                                xct.InnerText = m_client_project_id;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }

                            // 只修改gateway_id
                            if (valueName == Commands.GATEWAY_ID && xc.Name == Commands.GATEWAY_ID)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_gateway_id = config.gateway_id;
                                xct.InnerText = m_client_gateway_id;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }

                            // 只修改notifytime
                            if (valueName == Commands.NOTIFY_TIME  && xc.Name == Commands.NOTIFY_TIME)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_notifyTime = config.notifyTime;
                                xct.InnerText = m_client_notifyTime;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }

                            // 只修改reporttime
                            if (valueName == Commands.REPORT_TIME && xc.Name == Commands.REPORT_TIME)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_notifyTime = config.notifyTime;
                                xct.InnerText = m_client_notifyTime;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }
                            // 只修改verifytime
                            if (valueName == Commands.VERIFY_TIME && xc.Name == Commands.VERIFY_TIME)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_verifyTime = config.verifyTime;
                                xct.InnerText = m_client_verifyTime;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }

                            // 只修改times
                            if (valueName == Commands.TIMES && xc.Name == Commands.TIMES)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_times = config.times;
                                xct.InnerText = m_client_times;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }

                            // 只修改period
                            if (valueName == Commands.PERIOD && xc.Name == Commands.PERIOD)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_period = config.period;
                                xct.InnerText = m_client_period;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }

                            // 只修改key
                            if (valueName == Commands.KEY && xc.Name == Commands.KEY)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_key = config.key;
                                xct.InnerText = m_client_key;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }

                            // 只修改MD5
                            if (valueName == Commands.MD5 && xc.Name == Commands.MD5)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_md5 = config.md5;
                                xct.InnerText = m_client_md5;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }

                            // 只修改key
                            if (valueName == Commands.IV && xc.Name == Commands.IV)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_iv = config.iv;
                                xct.InnerText = m_client_iv;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }

                            // 只修改行政区编码
                            if (valueName == Commands.AREA_CODE && xc.Name == Commands.AREA_CODE)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_areacode = config.areacode;
                                xct.InnerText = m_client_areacode;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }

                            // 只修改行政区编码
                            if (valueName == Commands.PROGRAM_ID && xc.Name == Commands.PROGRAM_ID)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_programid = config.programid;
                                xct.InnerText = m_client_programid;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }


                            // 只修改行技术类型
                            if (valueName == Commands.TECH_TYPE && xc.Name == Commands.TECH_TYPE)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_techtype = config.techtype;
                                xct.InnerText = m_client_techtype;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }


                            // 只修改行系统代码
                            if (valueName == Commands.SYS_CODE && xc.Name == Commands.SYS_CODE)
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_syscode = config.syscode;
                                xct.InnerText = m_client_syscode;
                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }
                            if (valueName == Commands.METER && xc.Name == Commands.METER)
                            {
                                XmlElement xct = (XmlElement)xc;
                                MeterInfo meterInfo = config.meterInfo;

                                xct.SetAttribute("MA_ProgramId", meterInfo.MA_ProgramId);
                                xct.SetAttribute("MA_Code1", meterInfo.MA_Code1);
                                xct.SetAttribute("MA_Code2", meterInfo.MA_Code2);

                                xct.SetAttribute("MB_ProgramId", meterInfo.MB_ProgramId);
                                xct.SetAttribute("MB_Code1", meterInfo.MB_Code1);
                                xct.SetAttribute("MB_Code2", meterInfo.MB_Code2);

                                xct.SetAttribute("MC_ProgramId", meterInfo.MC_ProgramId);
                                xct.SetAttribute("MC_Code1", meterInfo.MC_Code1);
                                xct.SetAttribute("MC_Code2", meterInfo.MC_Code2);

                                xct.SetAttribute("MD_ProgramId", meterInfo.MD_ProgramId);
                                xct.SetAttribute("MD_Code1", meterInfo.MD_Code1);
                                xct.SetAttribute("MD_Code2", meterInfo.MD_Code2);

                                try
                                {
                                    xmlDoc.Save(config_path);
                                }
                                catch (XmlException xe)
                                {
                                    return false;
                                }
                                return true;
                            }
                        }
                    }
                }
                   
            }

            //如果走到这一步，则代表没有相应的配置值，可以写入log
            
            Console.Write("There is no such Configuration Value ！");
            LogManager.Logger.WriteWarnLog("There is no such Configuration Value !");
            return false;
        }


    }

    public class Configuration
    {
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ip="";
        /// <summary>
        /// 服务器端口
        /// </summary>
        public string port="";
        /// <summary>
        /// 项目编号
        /// </summary>
        public string project_id="";
        /// <summary>
        /// 系统装置编号
        /// </summary>
        public string gateway_id="";
        /// <summary>
        /// 心跳数据包发送间隔
        /// </summary>
        public string notifyTime="";
        /// <summary>
        /// 数据包定时发送间隔
        /// </summary>
        public string reportTime="";

        public string verifyTime="";
        /// <summary>
        /// 失败后尝试次数
        /// </summary>
        public string times="";
        public string period="";
        public string key="";
        public string md5 = "";
        public string iv = "";
        /// <summary>
        /// 行政区编码
        /// </summary>
        public string areacode = "";
        public string programid = "";
        public string techtype = "";
        public  string syscode = "";
        public MeterInfo meterInfo;
    }
    public class MeterInfo
    {
        public string MA_ProgramId="";
        public string MA_Code1="";
        public string MA_Code2="";

        public string MB_ProgramId = "";
        public string MB_Code1 = "";
        public string MB_Code2 = "";

        public string MC_ProgramId = "";
        public string MC_Code1 = "";
        public string MC_Code2 = "";

        public string MD_ProgramId = "";
        public string MD_Code1 = "";
        public string MD_Code2 = "";

    }
    public class Commands
    {
        public static  string IP = "IP";
        public static string PORT = "Port";
        public static string PROJECT_ID = "Project_id";
        public static string GATEWAY_ID = "Gateway_id";
        public static string AREA_CODE = "Area_code";
        public static string PROGRAM_ID = "Program_id";
        public static string TECH_TYPE = "Tech_type";
        public static string SYS_CODE = "Sys_code";
        public static string METER = "Meter";
        public static string NOTIFY_TIME = "NotifyTime";
        public static string REPORT_TIME = "ReportTime";
        public static string VERIFY_TIME = "VerifyTime";
        public static string TIMES = "Times";
        public static string PERIOD = "Period";
        public static string KEY = "Key";
        public static string MD5 = "MD5";
        public static string IV = "IV";
    }
}
