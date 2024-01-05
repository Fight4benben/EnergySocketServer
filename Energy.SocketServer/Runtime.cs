using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Energy.Common.Entity;
using Energy.SocketServer.Protocol;
using Energy.Common.Utils;
using NLog;
using Energy.Common.DAL;

namespace Energy.SocketServer
{
    public class Runtime
    {
        public static string MySqlConnectString = "";

        public static Dictionary<string, string> m_GateWayInfo = new Dictionary<string, string>();
        public static string m_AESValue = "1234567890123456";

        public static Logger m_Logger = LogManager.GetLogger("SocketServer");

        public static IBootstrap m_Bootstrap;

        /// <summary>
        /// 是否存入原始文件进入数据库，true表示存入数据库，false表示存入文件目录
        /// </summary>
        public static bool SaveToDataBase = false;

    }
}
