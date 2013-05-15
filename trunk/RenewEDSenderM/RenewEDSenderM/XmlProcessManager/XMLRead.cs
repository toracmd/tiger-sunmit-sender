using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Xml;


namespace RenewEDSenderM.XmlProcessManager
{
    class XMLRead
    {
        private static string sequence = "";
        private static string result = "";
        private static string time = "";
        private static string heart_result = "";
        private static string beginTime = "";
        private static string endTime = "";
        private static string period = "";
        private static string keytype = "";
        private static string key = "";
        private static string type = "";

        private static XmlDocument xmlDoc;

        public void BInput(byte[] rByte)
        {
            Support.DataPackage dp = new Support.DataPackage() { Package = rByte };
            Support.Encryption.AES_KEY = Encoding.ASCII.GetBytes("0000000000123456");
            Support.Encryption.AES_IV = Encoding.ASCII.GetBytes("0000000000123456");
            string rStr = Support.Encryption.DecryptStringFromBytes_Aes(dp.DataBlock);
            string rStr1 = Support.Encryption.RemoveZeroPaddings(rStr);
            Input(rStr1);
        }
        public void Input(string str)
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
                    XmlNodeList cList = xr.ChildNodes;
                    foreach (XmlNode xc in cList)
                    {
                        if (xc.Name == "type")
                        {
                            type = xc.InnerText;
                            break;
                        }
                    }
                }

                //解析为sequence，则获取sequence
                if (type == "sequence" && xr.Name == "id_validate")
                {
                    XmlNodeList iList = xr.ChildNodes;
                    foreach (XmlNode xi in iList)
                    {
                        if (xi.Name == "sequence")
                        {
                            sequence = xi.InnerText;
                            break;
                        }
                    }
                }

                //获取身份认证结果
                if (type == "result" && xr.Name == "id_validate")
                {
                    XmlNodeList iList = xr.ChildNodes;
                    foreach (XmlNode xi in iList)
                    {
                        if (xi.Name == "result")
                            result = xi.InnerText;
                        if (xi.Name == "time")
                            time = xi.InnerText;
                    }
                }

                //获取心跳结果
                if (type == "heart_result" && xr.Name == "id_validate")
                {
                    XmlNodeList iList = xr.ChildNodes;
                    foreach (XmlNode xi in iList)
                    {
                        if (xi.Name == "heart_result")
                            heart_result = xi.InnerText;
                    }
                }

                //获取查询参数
                if (type == "query" && xr.Name == "data")
                {
                    XmlNodeList iList = xr.ChildNodes;
                    foreach (XmlNode xi in iList)
                    {
                        if (xi.Name == "beginTime")
                            beginTime = xi.InnerText;
                        if (xi.Name == "endTime")
                            endTime = xi.InnerText;
                    }
                }


                //获取周期参数
                if (type == "period" && xr.Name == "config ")
                {
                    XmlNodeList iList = xr.ChildNodes;
                    foreach (XmlNode xi in iList)
                    {
                        if (xi.Name == "period")
                            period = xi.InnerText;
                    }
                }

                //获取密钥信息
                if (type == "setkey" && xr.Name == "stand")
                {
                    XmlNodeList iList = xr.ChildNodes;
                    foreach (XmlNode xi in iList)
                    {
                        if (xi.Name == "type")
                            keytype = xi.InnerText;
                        if (xi.Name == "key")
                            key = xi.InnerText;
                    }
                }
            }

        }


        public Order Output()
        {
            Order order = new Order();
            order.sequence = sequence;
            order.result = result;
            order.time = time;
            order.heart_result = heart_result;
            order.beginTime = beginTime;
            order.endTime = endTime;
            order.period = period;
            order.keytype = keytype;
            order.key = key;
            order.order = type;

            return order;
        }
    }
}
