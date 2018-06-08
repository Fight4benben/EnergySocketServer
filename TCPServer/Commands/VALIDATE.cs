using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCPServer.Entity;
using TCPServer.Protocol;
using TCPServer.Utils;

namespace TCPServer.Commands
{
    public class VALIDATE:CommandBase<SHaiSession,SHaiRequestInfo>
    {
        public override void ExecuteCommand(SHaiSession session, SHaiRequestInfo requestInfo)
        {
            MessageInfo message = XMLHelper.GetMessageInfo(requestInfo.Data);
            //Runtime.RequestProcess(message,session);
            ProcessValidate(message,session);
        }

        void ProcessValidate(MessageInfo message,SHaiSession session)
        {
            string key = message.BuildID + "_" + message.GatewayID;
            if (message.MessageAttribute == "request")
            {
                Console.WriteLine("{0} -> 接收到：" + session.RemoteEndPoint.Address + ":" + session.RemoteEndPoint.Port + "身份认证请求，请求信息为：" + message.BuildID + "," + message.GatewayID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Guid guid = Guid.NewGuid();
                string sequence = guid.ToString();
                if (Runtime.m_GateWayInfo.ContainsKey(key))
                    Runtime.m_GateWayInfo[key] = MD5Helper.GetMD5(Runtime.m_AESValue + sequence);
                else
                    Runtime.m_GateWayInfo.Add(key, MD5Helper.GetMD5(Runtime.m_AESValue + sequence));

                byte[] data = XMLHelper.GetXmlBytes(message, "id_validate", "sequence", sequence);
                if (session != null && data.Length > 0)
                {
                    session.Send(ToolUtil.AppendPacketHeader(1,data), 0, data.Length+7);
                }

            }
            else if (message.MessageAttribute == "md5")
            {
                byte[] data;
                if (Runtime.m_GateWayInfo[key] == message.MessageContent)
                {
                    Console.WriteLine("{0} -> MD5信息比对成功：" + message.BuildID + "," + message.GatewayID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    data = XMLHelper.GetXmlBytes(message, "id_validate", "result", "pass");
                    session.Send(ToolUtil.AppendPacketHeader(1,data), 0, data.Length+7);
                }
                else
                {
                    Console.WriteLine("{0} -> MD5信息比对失败：" + message.BuildID + "," + message.GatewayID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    data = XMLHelper.GetXmlBytes(message, "id_validate", "result", "fail");
                    session.Send(ToolUtil.AppendPacketHeader(1, data), 0, data.Length+7);
                }
            }
        }
    }
}
