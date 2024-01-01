using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.Common.Entity;
using Energy.Common.Utils;
using NLog;

namespace Energy.Analysis
{
    public class Runtime
    {
        public static string MySqlConnectString = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};SslMode=None;",
            SettingsHelper.GetSettingValue("MySqlServer"),SettingsHelper.GetSettingValue("MySqlPort"),
            SettingsHelper.GetSettingValue("DatabaseNameDB"),SettingsHelper.GetSettingValue("MySqlUid"),
            SettingsHelper.GetSettingValue("MySqlPwd"));


        public static string MysqlConn { get; set; }

        public static Logger m_Logger = LogManager.GetLogger("Analysis");
        /// <summary>
        /// 生成5分钟表的插入代码
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GenerateMinuteSQL(List<CalcEnergyData> list)
        {
            return GenerateEnergyValueSQL(list,"Minute");
        }

        public static string GenreateHourSQL(List<CalcEnergyData> list)
        {
            return GenerateEnergyValueSQL(list,"Hour");
        }

        public static string GenreateDaySQL(List<CalcEnergyData> list)
        {
            return GenerateEnergyValueSQL(list, "Day");
        }

        private static string GenerateEnergyValueSQL(List<CalcEnergyData> list,string type)
        {
            string tableName="t_ec_minutevalue";
            string expression = "F_Value+VALUES(F_Value)";
            string timeFormat = "yyyy-MM-dd HH:mm:ss";
            switch (type)
            {
                case "Minute":
                    tableName = "t_ec_minutevalue";
                    expression = "VALUES(F_Value)";
                    timeFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                case "Hour":
                    tableName = "t_ec_hourvalue";
                    expression = "F_Value+VALUES(F_Value)";
                    timeFormat = "yyyy-MM-dd HH:00:00";
                    break;
                case "Day":
                    tableName = "t_ec_dayvalue";
                    expression = "F_Value+VALUES(F_Value)";
                    timeFormat = "yyyy-MM-dd 00:00:00";
                    break; 
            }

            StringBuilder builder = new StringBuilder();

            if (list.Count > 0)
            {
                builder.Append(string.Format("INSERT INTO {0} (F_BuildID,F_MeterCode,F_Time,F_Value) VALUES",tableName));
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == 0)
                        builder.Append(string.Format("('{0}','{1}','{2}',{3})", list[i].BuildID, list[i].MeterCode,
                            list[i].Time.ToString(timeFormat), list[i].Value));
                    else
                        builder.Append(string.Format(",('{0}','{1}','{2}',{3})", list[i].BuildID, list[i].MeterCode,
                            list[i].Time.ToString(timeFormat), list[i].Value));
                }

                builder.Append(string.Format("  ON DUPLICATE KEY UPDATE F_Value={0};",expression));
            }

            return builder.ToString();
        }

        public static string GenerateOrigStateSQL(DateTime time)
        {
            return string.Format("UPDATE t_data_originenergyvalue SET F_Calced=1 WHERE F_Time='{0}'", time.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
