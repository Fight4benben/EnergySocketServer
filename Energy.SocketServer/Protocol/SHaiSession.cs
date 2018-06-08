using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.SocketServer.Protocol
{
    public class SHaiSession:AppSession<SHaiSession,SHaiRequestInfo>
    {

        //protected override int GetMaxRequestLength()
        //{
            
        //    return 409600;
        //}

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
            Console.WriteLine("{0} -> 会话已关闭，关闭原因为：" + reason.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }
    }
}
