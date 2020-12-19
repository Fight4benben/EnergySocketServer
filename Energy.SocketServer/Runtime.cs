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
        public static string MySqlConnectString = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};",
            SettingsHelper.GetSettingValue("MySqlServer"), SettingsHelper.GetSettingValue("MySqlPort"),
            SettingsHelper.GetSettingValue("DatabaseNameDB"), SettingsHelper.GetSettingValue("MySqlUid"),
            SettingsHelper.GetSettingValue("MySqlPwd"));


        public static Dictionary<string, string> m_GateWayInfo = new Dictionary<string, string>();
        public static string m_AESValue = "1234567890123456";

        public static Logger m_Logger = LogManager.GetLogger("SocketServer");

        public static IBootstrap m_Bootstrap;

    }
}
