using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

//xml文件的写入类
namespace RenewEDSenderM.XmlProcessManager
{
    class XMLWrite
    {
        private static XmlDocument xmlDoc;
        private static string result;
        
        //读入xml文件
        public  void Input(string str,string project_id,string gatewawy_id)
        {
            xmlDoc = new XmlDocument();
            //判断是否成功
            if (xmlDoc == null)
            {
                Console.Write("Construct the xml document failed!");
                return;
            }
            xmlDoc.LoadXml(str);

            XmlNode root = xmlDoc.SelectSingleNode("root");
            XmlNodeList rList = root.ChildNodes;

            foreach (XmlNode xr in rList)
            {
                if (xr.Name == "common")
                {
                    XmlNodeList common = xr.ChildNodes;
                    foreach (XmlNode xc in common)
                    {
                        if (xc.Name == "project_id")
                        {
                            XmlElement xct = (XmlElement)xc;
                            xct.InnerText = project_id;
                            continue;
                        }
                        if (xc.Name == "gateway_id")
                        {
                            XmlElement xct = (XmlElement)xc;
                            xct.InnerText = gatewawy_id;
                            continue;
                        }
                        //修改type类型内容
                        if (xc.Name == "type")
                        {
                            XmlElement xct = (XmlElement)xc;
                            xct.InnerText = "request";
                            break;
                        }
                    }
                }

            }
            
        }

        //输出形成的xml文件
        public string Output()
        {
            result = xmlDoc.OuterXml;
            return result;
        }
        public byte[] BOutput()
        {
            result = xmlDoc.OuterXml;
            byte[] sendBytes = Support.Encryption.EncryptStringToBytes_Aes(result, Encoding.ASCII.GetBytes("0000000000123456"), Encoding.ASCII.GetBytes("0000000000123456"));
            Support.DataPackage dp = new Support.DataPackage() { DataLength = ((uint)sendBytes.Length + 4), Seq = 0x1692ec43, DataBlock = sendBytes, CRC = Support.Encryption.CRC16(sendBytes) };
            return dp.Package;
        }
        //生成身份认证请求
        public  void Request()
        {
            XmlNode root = xmlDoc.SelectSingleNode("root");
            XmlNodeList rList = root.ChildNodes;

            foreach (XmlNode xr in rList)
            {
                if (xr.Name == "common")
                {
                    XmlNodeList common = xr.ChildNodes;
                    foreach (XmlNode xc in common)
                    {
                        //修改type类型内容
                        if (xc.Name == "type")
                        {
                            XmlElement xct = (XmlElement)xc;
                            xct.InnerText = "request";
                            break;
                        }
                    }
                }
                
            }

            XmlElement id_validate = xmlDoc.CreateElement("id_validate");
            //*******可用log来代替
            if (id_validate == null)
            {
                Console.Write("Create new xml element failed!");
                return;
            }
            id_validate.SetAttribute("operation", "request");
            id_validate.InnerText = "";
            root.AppendChild(id_validate);

        }


        //校验身份，发送md5
        public  void SendMD5(string md5)
        {
            XmlNode root = xmlDoc.SelectSingleNode("root");
            XmlNodeList rList = root.ChildNodes;

            foreach (XmlNode xr in rList)
            {
                if (xr.Name == "common")
                {
                    XmlNodeList common = xr.ChildNodes;
                    foreach (XmlNode xc in common)
                    {
                        //修改type类型内容
                        if (xc.Name == "type")
                        {
                            XmlElement xct = (XmlElement)xc;
                            xct.InnerText = "md5";
                            break;
                        }
                    }
                }
            }

            XmlElement id_validate = xmlDoc.CreateElement("id_validate");
            //*******可用log来代替
            if (id_validate == null)
            {
                Console.Write("Create new xml element failed!");
                return;
            }

            id_validate.SetAttribute("operation", "md5");
            XmlElement newMD5 = xmlDoc.CreateElement("md5");

            //*******可用log来代替
            if (newMD5 == null)
            {
                Console.Write("Create new xml element failed!");
                return;
            }

            newMD5.InnerText = md5;
            id_validate.AppendChild(newMD5);
            root.AppendChild(id_validate);
        }

        //心跳验证
        public  void Notify()
        {
            XmlNode root = xmlDoc.SelectSingleNode("root");
            XmlNodeList rList = root.ChildNodes;

            foreach (XmlNode xr in rList)
            {
                if (xr.Name == "common")
                {
                    XmlNodeList common = xr.ChildNodes;
                    foreach (XmlNode xc in common)
                    {
                        //修改type类型内容
                        if (xc.Name == "type")
                        {
                            XmlElement xct = (XmlElement)xc;
                            xct.InnerText = "notify";
                            break;
                        }
                    }
                }
            }

            XmlElement heart_beat = xmlDoc.CreateElement("heart_beat");
            //*******可用log来代替
            if (heart_beat == null)
            {
                Console.Write("Create new xml element failed!");
                return;
            }
            heart_beat.SetAttribute("operation", "notify");
            root.AppendChild(heart_beat);
        }

        ////数据定时发送
        //public  void Report(string input_squence, string input_parse, string input_time, Meter[] input_meter, Information[] input_info)
        //{
        //    XmlNode root = xmlDoc.SelectSingleNode("root");
        //    XmlNodeList rList = root.ChildNodes;

        //    foreach (XmlNode xr in rList)
        //    {
        //        if (xr.Name == "common")
        //        {
        //            XmlNodeList common = xr.ChildNodes;
        //            foreach (XmlNode xc in common)
        //            {
        //                //修改type类型内容
        //                if (xc.Name == "type")
        //                {
        //                    XmlElement xct = (XmlElement)xc;
        //                    xct.InnerText = "report";
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    //生成data元素
        //    XmlElement data = xmlDoc.CreateElement("data");
        //    //*******可用log来代替
        //    if (data == null)
        //    {
        //        Console.Write("Create new xml element failed!");
        //        return;
        //    }
        //    data.SetAttribute("operation", "report");

        //    //生成sequence元素
        //    XmlElement sequence = xmlDoc.CreateElement("sequence");
        //    //*******可用log来代替
        //    if (sequence == null)
        //    {
        //        Console.Write("Create new xml element failed!");
        //        return;
        //    }
        //    sequence.InnerText = input_squence;
        //    data.AppendChild(sequence);

        //    //生成parse元素
        //    XmlElement parse = xmlDoc.CreateElement("parse");
        //    //*******可用log来代替
        //    if (parse == null)
        //    {
        //        Console.Write("Create new xml element failed!");
        //        return;
        //    }
        //    parse.InnerText = input_parse;
        //    data.AppendChild(parse);

        //    //生成time元素
        //    XmlElement time = xmlDoc.CreateElement("time");
        //    //*******可用log来代替
        //    if (time == null)
        //    {
        //        Console.Write("Create new xml element failed!");
        //        return;
        //    }
        //    time.InnerText = input_time;
        //    data.AppendChild(time);

        //    //生成具体数据meter，这部分代码不完整
        //    for (int i = 0; i < length(input_meter); i++)
        //    {
        //        XmlElement meter = xmlDoc.CreateElement("meter");
        //        //*******可用log来代替
        //        if (meter == null)
        //        {
        //            Console.Write("Create new xml element failed!");
        //            return;
        //        }
        //        meter.SetAttribute("id", input_meter[i].id);
        //        meter.SetAttribute("conn", input_meter[i].conn);

        //        XmlElement functions = xmlDoc.CreateElement("function");
        //        //*******可用log来代替
        //        if (functions == null)
        //        {
        //            Console.Write("Create new xml element failed!");
        //            return;
        //        }
        //        functions.SetAttribute("id", input_info[i].id);
        //        functions.SetAttribute("coding", input_info[i].coding);
        //        functions.SetAttribute("error", input_info[i].error);
        //        functions.SetAttribute("sample_time", input_info[i].sample_time);
        //        functions.InnerText = input_info[i].data;
        //        meter.AppendChild(functions);

        //        data.AppendChild(meter);
        //    }

        //    root.AppendChild(data);
        //}
        public void Period_Ack()
        {
            XmlNode root = xmlDoc.SelectSingleNode("root");
            XmlNodeList rList = root.ChildNodes;

            foreach (XmlNode xr in rList)
            {
                if (xr.Name == "common")
                {
                    XmlNodeList common = xr.ChildNodes;
                    foreach (XmlNode xc in common)
                    {
                        //修改type类型内容
                        if (xc.Name == "type")
                        {
                            XmlElement xct = (XmlElement)xc;
                            xct.InnerText = "period_ack";
                            break;
                        }
                    }
                }

            }

            XmlElement config = xmlDoc.CreateElement("congif");
            //*******可用log来代替
            if (config == null)
            {
                Console.Write("Create new xml element failed!");
                return;
            }
            config.SetAttribute("operation", "period_ack");
            root.AppendChild(config);
        }
        
    }
}

