using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCPServer.Entity;
using TCPServer.Protocol;
using TCPServer.Utils;

namespace TCPServer
{
    public class Runtime
    {
        public static Dictionary<string, string> m_GateWayInfo = new Dictionary<string, string>();
        public static string m_AESValue = "1234567890123456";


        public static void RequestProcess(MessageInfo message, SHaiSession session)
        {
            string key = message.BuildID + "_" + message.GatewayID;
            if (message.MessageAttribute == "request")
            {
                Console.WriteLine("接收到" + session.RemoteEndPoint.Address + ":" + session.RemoteEndPoint.Port + "身份认证请求，请求信息为：" + message.BuildID + "," + message.GatewayID);
                Guid guid = Guid.NewGuid();
                string sequence = guid.ToString();
                if (Runtime.m_GateWayInfo.ContainsKey(key))
                    Runtime.m_GateWayInfo[key] = MD5Helper.GetMD5(Runtime.m_AESValue + sequence);
                else
                    Runtime.m_GateWayInfo.Add(key, MD5Helper.GetMD5(Runtime.m_AESValue + sequence));

                byte[] data = XMLHelper.GetXmlBytes(message, "id_validate", "sequence", sequence);
                if (session != null && data.Length > 0)
                {
                    session.Send(data, 0, data.Length);
                }

            }
            else if (message.MessageAttribute == "md5")
            {
                byte[] data;
                if (Runtime.m_GateWayInfo[key] == message.MessageContent)
                {
                    Console.WriteLine("MD5信息比对成功：" + message.BuildID + "," + message.GatewayID);
                    data = XMLHelper.GetXmlBytes(message, "id_validate", "result", "pass");
                    session.Send(data, 0, data.Length);
                }
                else
                {
                    Console.WriteLine("MD5信息比对失败：" + message.BuildID + "," + message.GatewayID);
                    data = XMLHelper.GetXmlBytes(message, "id_validate", "result", "fail");
                    session.Send(data, 0, data.Length);
                }
            }
            else if (message.MessageAttribute == "notify")
            {
                Console.WriteLine("接收到心跳信息。");
                //byte[] data = XMLHelper.GetXmlBytes(message, "heart_beat", "time", "");
                //session.Send(data, 0, data.Length);
            }
            else if (message.MessageAttribute == "report")
            {
                Console.WriteLine("接收到" + message.GatewayID + "网关发来的建筑" + message.BuildID + "的定时上报的能耗数据，数据时间为：" + message.MessageContent);
                //保存数据到文件中

                //回送接收到数据
                byte[] data = XMLHelper.GetXmlBytes(message, "energy_data", message.MessageAttribute, message.MessageContent, "OK");
                session.Send(data, 0, data.Length);
            }
            else if (message.MessageAttribute == "continuous")
            {
                Console.WriteLine("接收到" + message.GatewayID + "网关发来的建筑" + message.BuildID + "的断点续传的能耗数据,数据时间为：" + message.MessageContent);
                //回送接收到数据
                byte[] data = XMLHelper.GetXmlBytes(message, "energy_data", message.MessageAttribute, message.MessageContent, "OK");
                session.Send(data, 0, data.Length);
            }
            else if (message.MessageAttribute == "finish")
            {
                Console.WriteLine(message.GatewayID + "网关发来的建筑" + message.BuildID + "断点续传完成标记。");

                //回送接收到数据
                byte[] data = XMLHelper.GetXmlBytes(message, "energy_data", message.MessageAttribute, message.MessageContent, "OK");
                session.Send(data, 0, data.Length);
            }
            
        }
    }
}
