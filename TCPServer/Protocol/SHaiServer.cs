using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Protocol
{
    public class SHaiServer:AppServer<SHaiSession,SHaiRequestInfo>
    {
        public SHaiServer()
            : base(new DefaultReceiveFilterFactory<SHaiReceiveFilter,SHaiRequestInfo>())
        { }

        protected override void OnNewSessionConnected(SHaiSession session)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff -> " + session.RemoteEndPoint.Address + ":" + session.RemoteEndPoint.Port));
        }
    }
}
