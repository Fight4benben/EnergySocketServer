using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Energy.Common.Entity;
using MongoDB.Bson;
using Energy.Common.DAL;

namespace Energy.Common.Utils
{
    public class XMLHelper
    {
        /// <summary>
        /// 解析XML报文，获取报文的头信息
        /// </summary>
        /// <param name="source">xml字符串</param>
        /// <returns>MessageInfo对象</returns>
        public static MessageInfo GetMessageInfo(string source)
        {
            MessageInfo info = new MessageInfo();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(source);

            info.BuildID = doc.SelectSingleNode("/root/common/building_id").InnerText;
            info.GatewayID = doc.SelectSingleNode("/root/common/gateway_id").InnerText;
            info.MessageType = doc.SelectSingleNode("/root/common/type").InnerText;

            if (info.MessageType == "energy_data")
                info.MessageAttribute = doc.SelectSingleNode("/root/data").Attributes["operation"].Value;
            else
                info.MessageAttribute = doc.SelectSingleNode("/root/" + info.MessageType).Attributes["operation"].Value;
                

            switch (info.MessageAttribute) 
            {
                case "md5":
                    info.MessageContent = doc.SelectSingleNode("/root/"+info.MessageType+"/md5").InnerText;
                    break;
                case "report":
                case "continuous":
                    info.MessageContent = doc.SelectSingleNode("/root/data/time").InnerText;
                    break;
            }

            return info;
        }

        /// <summary>
        /// 根据报文头信息，生成发送包的xml字节数组
        /// </summary>
        /// <param name="info">接收到的报文信息</param>
        /// <param name="type">类型</param>
        /// <param name="operation">类型的属性</param>
        /// <param name="value">类型的子节点的值</param>
        /// <param name="args">附加参数</param>
        /// <returns></returns>
        public static byte[] GetXmlBytes(MessageInfo info,string type,string operation,string value,params string[] args)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0","utf-8","yes");
            doc.AppendChild(declaration);

            XmlNode rootNode = doc.CreateElement("root");

            XmlNode commonNode = doc.CreateElement("common");

             XmlNode typeNode;
             XmlNode valueNode;

             if (type != "energy_data")
             {
                 typeNode = doc.CreateElement(type);
                 valueNode = doc.CreateElement(operation);
             }
             else
             {
                 valueNode = doc.CreateElement("time");
                 typeNode = doc.CreateElement("data");
             }

             valueNode.InnerText = value;
             typeNode.AppendChild(valueNode);
             if (args.Length > 0)
             {
                 XmlNode ackNode = doc.CreateElement("ack");
                 ackNode.InnerText = args[0];
                 typeNode.AppendChild(ackNode);
             }

            XmlAttribute operationAttr = doc.CreateAttribute("operation");
            operationAttr.Value = operation;
            typeNode.Attributes.Append(operationAttr);

            

            XmlNode buildIdNode = doc.CreateElement("building_id");
            buildIdNode.InnerText = info.BuildID;
            XmlNode gatewayNode = doc.CreateElement("gateway_id");
            gatewayNode.InnerText = info.GatewayID;
            XmlNode commonTypeNode = doc.CreateElement("type");
            commonTypeNode.InnerText = type;

            commonNode.AppendChild(buildIdNode);
            commonNode.AppendChild(gatewayNode);
            commonNode.AppendChild(commonTypeNode);

            rootNode.AppendChild(commonNode);
            rootNode.AppendChild(typeNode);

            doc.AppendChild(rootNode);

            return Encoding.UTF8.GetBytes(doc.InnerXml);
        }

        /// <summary>
        /// 获取xml对应的对象的json
        /// </summary>
        /// <param name="xmlSource">xml</param>
        /// <returns></returns>
        public static string GetMeterList(string xmlSource,string mysqlString,string reportType)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlSource);

            string buildId = doc.SelectSingleNode("/root/common/building_id").InnerText;
            string gatewayId = doc.SelectSingleNode("/root/common/gateway_id").InnerText;
            string type = doc.SelectSingleNode("/root/common/type").InnerText;
            DateTime collectTime = ToolUtil.GetDateTimeFromString(doc.SelectSingleNode("/root/data/time").InnerText);
            bool status = false;
            List<Meter> meterList = new List<Meter>();

            List<MeterStatus> statusList = new List<MeterStatus>();

            XmlNodeList nodeList = doc.SelectNodes("/root/data/meters/meter");

            //List<BsonDocument> bsonList = new List<BsonDocument>();

            foreach (XmlNode item in nodeList)
            {
                //BsonDocument document = new BsonDocument();

                List<MeterParam> paramList = new List<MeterParam>();
                string meterid = item.Attributes["id"].Value;
                //document.Add("F_BuildId",buildId);
                //document.Add("F_GatewayId", gatewayId);
                //document.Add("F_MeterCode", meterid);
                //document.Add("F_Time",collectTime);
                
                XmlNodeList nodeParams = item.SelectNodes("function");
                bool meterStatus = true;
                foreach (XmlNode paramNode in nodeParams)
                {
                    string paramName = paramNode.Attributes["id"].Value;
                    string paramError = paramNode.Attributes["error"].Value.Trim();

                    float paramValue;
                    if (paramNode.InnerText != "")
                    {
                        paramValue = Convert.ToSingle(paramNode.InnerText);
                        //document.Add(paramName,paramValue);
                        paramList.Add(new MeterParam(paramName, paramError, paramValue));
                    }
                    else
                    {
                        paramList.Add(new MeterParam(paramName, paramError, null));
                    }

                }
                //bsonList.Add(document);
                meterList.Add(new Meter(meterStatus,meterid,paramList));

                if (reportType == "report" && nodeParams.Count > 0)
                {
                    if(string.IsNullOrEmpty(nodeParams[0].Attributes["error"].Value.Trim()))
                        statusList.Add(new MeterStatus() { BuildId = buildId, GatewayId = gatewayId, MeterCode = meterid, Status=1, XmlTime = collectTime, LastUploadTime = DateTime.Now });
                    else
                        statusList.Add(new MeterStatus() { BuildId = buildId, GatewayId = gatewayId, MeterCode = meterid, Status=0, XmlTime = collectTime, LastUploadTime = DateTime.Now });
                }
                    
            }

            //try
            //{
            //    MongoHelper helper = MongoHelper.GetInstance();
            //    var collection = helper.GetCollection("HistoryData");
            //   collection.InsertMany(bsonList);
            //}
            //catch 
            //{ 
            //}
            if(reportType == "report")
                MySQLHelper.InsertMeterStatusByList(mysqlString,statusList);
            

            return JsonConvert.SerializeObject(new MeterList(buildId, gatewayId, collectTime, status, meterList)).ToString();
        }


        public static List<YangZhuChangMeter> GetYangZhuChangMeters(string xmlSource)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlSource);

            string buildId = doc.SelectSingleNode("/root/common/building_id").InnerText;
            string gatewayId = doc.SelectSingleNode("/root/common/gateway_id").InnerText;
            string type = doc.SelectSingleNode("/root/common/type").InnerText;
            DateTime collectTime = ToolUtil.GetDateTimeFromString(doc.SelectSingleNode("/root/data/time").InnerText);
      
            List<YangZhuChangMeter> meterList = new List<YangZhuChangMeter>();

            XmlNodeList nodeList = doc.SelectNodes("/root/data/meters/meter");

            foreach (XmlNode item in nodeList)
            {
                YangZhuChangMeter tempMeter = new YangZhuChangMeter();
                
                string meterid = item.Attributes["id"].Value;
                
                XmlNodeList nodeParams = item.SelectNodes("function");
                int meterStatus = 1;
                foreach (XmlNode paramNode in nodeParams)
                {
                    string paramName = paramNode.Attributes["id"].Value;
                    string paramError = paramNode.Attributes["error"].Value.Trim();

                    if (paramName == "EPI" || paramName == "LJLL")
                    {
                        meterStatus = string.IsNullOrEmpty(paramError) ? 1 : 0;

                        decimal paramValue;
                        paramValue = Convert.ToDecimal(paramNode.InnerText);
                        tempMeter.BuildId = buildId;
                        tempMeter.GatewayName = gatewayId;
                        tempMeter.MeterCode = meterid;
                        tempMeter.Time = collectTime;
                        tempMeter.Status = meterStatus;
                        tempMeter.Value = paramValue;
                    }
                }

                meterList.Add(tempMeter);
            }

            return meterList;
        }

    }
}
