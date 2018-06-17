using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.Common.Utils;

namespace Energy.Analysis
{
    public class Runtime
    {
        public static string MySqlConnectString = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};",
            SettingsHelper.GetSettingValue("MySqlServer"),SettingsHelper.GetSettingValue("MySqlPort"),
            SettingsHelper.GetSettingValue("DatabaseNameDB"),SettingsHelper.GetSettingValue("MySqlUid"),
            SettingsHelper.GetSettingValue("MySqlPwd"));
    }
}
