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
    //读取配置文件中的配置参数，并可以实时修改配置参数
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
        //private static string config_path = "./Config/Config.xml";
        private static string config_path = "../../../RenewEDSenderM/bin/Debug/Config/Config.xml";
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
                        if (xs.Name == "IP")
                        {
                            m_server_ip = xs.InnerText;
                            config.ip = m_server_ip;
                            continue;
                        }

                        //获取服务器的端口
                        if (xs.Name == "Port")
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
                        if (xc.Name == "Project_id")
                        {
                            m_client_project_id = xc.InnerText;
                            config.project_id = m_client_project_id;
                            continue;
                        }

                        //获取客户端的gateway_id
                        if (xc.Name == "Gateway_id")
                        {
                            m_client_gateway_id = xc.InnerText;
                            config.gateway_id = m_client_gateway_id;
                            continue;
                        }

                        //获取客户端的心跳数据间隔
                        if (xc.Name == "NotifyTime")
                        {
                            m_client_notifyTime = xc.InnerText;
                            config.notifyTime = m_client_notifyTime;
                            continue;
                        }

                        //获取客户端数据发送间隔
                        if (xc.Name == "ReportTime")
                        {
                            m_client_reportTime = xc.InnerText;
                            config.reportTime = m_client_reportTime;
                            continue;
                        }

                        //获取客户端认证超时
                        if (xc.Name == "VerifyTime")
                        {
                            m_client_verifyTime = xc.InnerText;
                            config.verifyTime = m_client_verifyTime;
                            continue;
                        }

                        //获取客户端失败尝试次数
                        if (xc.Name == "Times")
                        {
                            m_client_times = xc.InnerText;
                            config.times = m_client_times;
                            continue;
                        }

                        //获取客户端的周期参数
                        if (xc.Name == "Period")
                        {
                            m_client_period = xc.InnerText;
                            config.period = m_client_period;
                            continue;
                        }

                        //获取客户端AES密钥
                        if (xc.Name == "Key")
                        {
                            m_client_key = xc.InnerText;
                            config.key = m_client_key;
                            continue;
                        }

                        //获取客户端MD5 值
                        if (xc.Name == "MD5")
                        {
                            m_client_md5 = xc.InnerText;
                            config.md5 = m_client_md5;
                            continue;
                        }

                        //获取客户端AES初始向量
                        if (xc.Name == "IV")
                        {
                            m_client_iv = xc.InnerText;
                            config.iv = m_client_iv;
                            continue;
                        }
                    }
                }
                
            }
            return config;
        }

        public void WriteConfig(Configuration config)
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
                        if (xs.Name == "IP")
                        {
                            XmlElement xst = (XmlElement)xs;
                            m_server_ip = config.ip;
                            xst.InnerText = m_server_ip;
                            continue;
                        }

                        //设置服务器的端口
                        if (xs.Name == "Port")
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
                        if (xc.Name == "Project_id")
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_project_id = config.project_id;
                            xct.InnerText = m_client_project_id;
                            continue;
                        }

                        //设置客户端的gateway_id
                        if (xc.Name == "Gateway_id")
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_gateway_id = config.gateway_id;
                            xct.InnerText = m_client_gateway_id;
                            continue;
                        }

                        //设置客户端的心跳数据间隔
                        if (xc.Name == "NotifyTime")
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_notifyTime = config.notifyTime;
                            xct.InnerText = m_client_notifyTime;
                            continue;
                        }

                        //设置客户端数据发送间隔
                        if (xc.Name == "ReportTime")
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_reportTime = config.reportTime;
                            xct.InnerText = m_client_reportTime;
                            continue;
                        }

                        //设置客户端认证超时
                        if (xc.Name == "VerifyTime")
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_verifyTime = config.verifyTime;
                            xct.InnerText = m_client_verifyTime;
                            continue;
                        }

                        //设置客户端失败尝试次数
                        if (xc.Name == "Times")
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_times = config.times;
                            xct.InnerText = m_client_times;
                            continue;
                        }

                        //设置客户端的周期参数
                        if (xc.Name == "Period")
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_period = config.period;
                            xct.InnerText = m_client_period;
                            continue;
                        }

                        //设置客户端AES密钥
                        if (xc.Name == "Key")
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_key = config.key;
                            xct.InnerText = m_client_key;
                            continue;
                        }

                        //设置客户端MD5值
                        if (xc.Name == "MD5")
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_md5 = config.md5;
                            xct.InnerText = m_client_md5;
                            continue;
                        }

                        //设置客户端AES初始向量
                        if (xc.Name == "IV")
                        {
                            XmlElement xct = (XmlElement)xc;
                            m_client_iv = config.iv;
                            xct.InnerText = m_client_iv;
                            continue;
                        }
                    }
                }

            }
            //修改configuration后，进行保存
            xmlDoc.Save(config_path);
        }

        public void WriteSpecailConfig(Configuration config, string valueName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(config_path);
            XmlNode root = xmlDoc.SelectSingleNode("config");
            XmlNodeList rList = root.ChildNodes;

            if (valueName == "ip"|| valueName =="port")
            {
                foreach (XmlNode xr in rList)
                {
                    if (xr.Name == "Server_Information")
                    {
                        XmlNodeList sList = xr.ChildNodes;
                        foreach (XmlNode xs in sList)
                        {
                            // 只修改ip
                            if (valueName == "ip" && xs.Name == "IP")
                            {
                                XmlElement xst = (XmlElement)xs;
                                m_server_ip = config.ip;
                                xst.InnerText = m_server_ip;
                                xmlDoc.Save(config_path);
                                return;
                            }
                            // 只修改port
                            if (valueName == "port" && xs.Name == "Port")
                            {
                                XmlElement xst = (XmlElement)xs;
                                m_server_ip = config.ip;
                                xst.InnerText = m_server_ip;
                                xmlDoc.Save(config_path);
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (XmlNode xr in rList)
                {
                    if (xr.Name == "Client_Information")
                    {
                        XmlNodeList cList = xr.ChildNodes;
                        foreach (XmlNode xc in cList)
                        {
                            // 只修改project_id
                            if (valueName == "project_id" && xc.Name == "Project_id")
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_project_id = config.project_id;
                                xct.InnerText = m_client_project_id;
                                xmlDoc.Save(config_path);
                                return;
                            }

                            // 只修改gateway_id
                            if (valueName == "gateway_id" && xc.Name == "Gateway_id")
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_gateway_id = config.gateway_id;
                                xct.InnerText = m_client_gateway_id;
                                xmlDoc.Save(config_path);
                                return;
                            }

                            // 只修改notifytime
                            if (valueName == "notifytime" && xc.Name == "NotifyTime")
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_notifyTime = config.notifyTime;
                                xct.InnerText = m_client_notifyTime;
                                xmlDoc.Save(config_path);
                                return;
                            }

                            // 只修改reporttime
                            if (valueName == "reporttime" && xc.Name == "ReportTime")
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_notifyTime = config.notifyTime;
                                xct.InnerText = m_client_notifyTime;
                                xmlDoc.Save(config_path);
                                return;
                            }
                            // 只修改verifytime
                            if (valueName == "verifytime" && xc.Name == "VerifyTime")
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_verifyTime = config.verifyTime;
                                xct.InnerText = m_client_verifyTime;
                                xmlDoc.Save(config_path);
                                return;
                            }

                            // 只修改times
                            if (valueName == "times" && xc.Name == "Times")
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_times = config.times;
                                xct.InnerText = m_client_times;
                                xmlDoc.Save(config_path);
                                return;
                            }

                            // 只修改period
                            if (valueName == "period" && xc.Name == "Period")
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_period = config.period;
                                xct.InnerText = m_client_period;
                                xmlDoc.Save(config_path);
                                return;
                            }

                            // 只修改key
                            if (valueName == "key" && xc.Name == "Key")
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_key = config.key;
                                xct.InnerText = m_client_key;
                                xmlDoc.Save(config_path);
                                return;
                            }

                            // 只修改MD5
                            if (valueName == "md5" && xc.Name == "MD5")
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_md5 = config.md5;
                                xct.InnerText = m_client_md5;
                                xmlDoc.Save(config_path);
                                return;
                            }

                            // 只修改key
                            if (valueName == "iv" && xc.Name == "IV")
                            {
                                XmlElement xct = (XmlElement)xc;
                                m_client_iv = config.iv;
                                xct.InnerText = m_client_iv;
                                xmlDoc.Save(config_path);
                                return;
                            }
                        }
                    }
                }
                   
            }

            //如果走到这一步，则代表没有相应的配置值，可以写入log
            Console.Write("There is no such Configuration Value ！");
        }


    }

    public class Configuration
    {
        public string ip="";
        public string port="";
        public string project_id="";
        public string gateway_id="";
        public string notifyTime="";
        public string reportTime="";
        public string verifyTime="";
        public string times="";
        public string period="";
        public string key="";
        public string md5 = "";
        public string iv = "";
    }
}
