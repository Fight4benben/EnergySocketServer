using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.SocketServer
{
    public class Startup
    {
        public static void StartSocket()
        {
            Runtime.m_Bootstrap = BootstrapFactory.CreateBootstrap();

            if (!Runtime.m_Bootstrap.Initialize())
            {
                Console.WriteLine("{0} -> Failed to initialize!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Console.ReadKey();
                return;
            }

            var result = Runtime.m_Bootstrap.Start();

            Console.WriteLine("{1} -> Start result: {0}!", result, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            if (result == StartResult.Failed)
            {
                Console.WriteLine("{0} -> Failed to start!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Console.ReadKey();
                return;
            }
        }

        public static void StopSocket()
        {

        }
    }
}
