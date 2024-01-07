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
using Energy.Common.Utils;
using Energy.Common.Settings;

namespace Energy.SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===========================启动数据接收服务=======================================");
            Runtime.m_Logger.Info("===========================启动数据接收服务=======================================");
            FileHelper.CheckDirExistAndCreate(FileHelper.RealTimeDataPath);

            Runtime.SaveToMongodb = Convert.ToBoolean(JsonConfigHelper.Instance.GetValue("SaveToMongodb"));
            

            Runtime.MySqlConnectString = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};SslMode=None;",
            JsonConfigHelper.Instance.GetValue("MySqlServer"), JsonConfigHelper.Instance.GetValue("MySqlPort"),
            JsonConfigHelper.Instance.GetValue("DatabaseNameDB"), JsonConfigHelper.Instance.GetValue("MySqlUid"),
            JsonConfigHelper.Instance.GetValue("MySqlPwd"));

            var bootstrap = BootstrapFactory.CreateBootstrap();

            if (!bootstrap.Initialize())
            {
                Console.WriteLine("{0} -> Failed to initialize!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Runtime.m_Logger.Error("初始化数据接受服务失败！");
                Console.ReadKey();
                return;
            }

            var result = bootstrap.Start();

            Console.WriteLine("{1} -> Start result: {0}!", result, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            Runtime.m_Logger.Info(string.Format("{1} -> Start result: {0}!", result, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));


            if (result == StartResult.Failed)
            {
                Console.WriteLine("{0} -> Failed to start!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Console.ReadKey();
                return;
            }

            Console.WriteLine("===========================启动数据接收服务成功=======================================");
            Runtime.m_Logger.Info("===========================启动数据接收服务成功=======================================");

            while (true)
            {
                Thread.Sleep(1);
            }

        }
    }
}
