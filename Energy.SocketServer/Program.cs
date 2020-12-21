using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.Common.DAL;
using Energy.SocketServer.Protocol;
using System.Threading;

namespace Energy.SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("{0} -> Check TempData DB status ...", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            SQLiteHelper.CreateLocalDB("TempData");
            Console.WriteLine("{0} -> Check GatewayData Table In TempData...", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            SQLiteHelper.CreateTempDataTable("TempData");
            Console.WriteLine("{0} -> Finish TempData DB and GatewayData Table Checked.", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            //Console.WriteLine("{0} -> Press any key to start the server!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            //Console.ReadKey();
            //Console.WriteLine();

            var bootstrap = BootstrapFactory.CreateBootstrap();
           

            if (!bootstrap.Initialize())
            {
                Console.WriteLine("{0} -> Failed to initialize!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Console.ReadKey();
                return;
            }

            var result = bootstrap.Start();

            Console.WriteLine("{1} -> Start result: {0}!", result, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            if (result == StartResult.Failed)
            {
                Console.WriteLine("{0} -> Failed to start!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Console.ReadKey();
                return;
            }



            while (true)
            {
                Thread.Sleep(1);
            }
            //Console.WriteLine("Press key 'q' to stop it!");

            //while (Console.ReadKey().KeyChar != 'q')
            //{
            //    Console.WriteLine();
            //    continue;
            //}

            //Console.WriteLine();


            //Stop the appServer
            //bootstrap.Stop();

            //Console.WriteLine("{0} -> The server was stopped!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //Console.ReadKey();

        }
    }
}
