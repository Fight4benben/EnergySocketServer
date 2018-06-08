using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.Common.Entity;
using Energy.SocketServer.Protocol;
using Energy.Common.Utils;

namespace Energy.SocketServer.Commands
{
    public class HEARTBEAT : CommandBase<SHaiSession, SHaiRequestInfo>
    {
        public override void ExecuteCommand(SHaiSession session, SHaiRequestInfo requestInfo)
        {
            MessageInfo message = XMLHelper.GetMessageInfo(requestInfo.Data);
            ProcessHeartbeat(message,session);
        }

        void ProcessHeartbeat(MessageInfo message,SHaiSession session)
        {
            if (message.MessageAttribute == "notify")
            {
                Console.WriteLine("{0} -> 接收到网关:{1}上传的心跳信息",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),message.GatewayID);
                byte[] data = XMLHelper.GetXmlBytes(message, "heart_beat", "time", DateTime.Now.ToString("yyyyMMddHHmmss"));
                session.Send(ToolUtil.AppendPacketHeader(2,data), 0, data.Length+7);
            }
        }
    }
}
