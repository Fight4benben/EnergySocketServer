using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Energy.Common.DAL;

namespace Energy.Common.Utils
{
    public class SettingsHelper
    {
        public static string GetSettingValue(string key)
        {
            string sql = "select Value from {0} where Key=@Key";
            SQLiteParameter parameter = new SQLiteParameter("@Key",key);

            object value = SQLiteHelper.ExecuteScalar("settings", "appsettings",sql,parameter);


            return value==null ? "":value.ToString();
        }

        public static bool SetSettingValue(string key, string value)
        {
            string sql = string.Format("insert into appsettings values('{0}','{1}');",key,value);

            int count = SQLiteHelper.ExecuteNonQuery("settings",sql);

            return count > 0 ? true : false;
        }

        public static void CreateSettingsDBTable()
        {
            CreateSettingsDB();
            CreateAppSettingsTable();
        }

        /// <summary>
        /// 创建系统配置文件库
        /// </summary>
        private static void CreateSettingsDB()
        {
            SQLiteHelper.CreateLocalDB("settings");
        }

        /// <summary>
        /// 创建配置文件表
        /// </summary>
        private static void CreateAppSettingsTable()
        {
            string sql = @"create table appsettings (
                        Key varchar(20) primary key not null,
                        Value varchar(50) not null
                    );";
            SQLiteHelper.CreateTableInDB("settings", "appsettings",sql);
            
        }
    }
}
