using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.Common.Entity;
using Energy.Common.Utils;

namespace Energy.Analysis
{
    public class Runtime
    {
        public static string MySqlConnectString = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};",
            SettingsHelper.GetSettingValue("MySqlServer"),SettingsHelper.GetSettingValue("MySqlPort"),
            SettingsHelper.GetSettingValue("DatabaseNameDB"),SettingsHelper.GetSettingValue("MySqlUid"),
            SettingsHelper.GetSettingValue("MySqlPwd"));

        public static string GenerateMinuteSQL(List<CalcEnergyData> list)
        {
            StringBuilder builder = new StringBuilder();

            if(list.Count>0)
            {
                builder.Append("INSERT INTO t_ec_minutevalue(F_BuildID,F_MeterCode,F_Time,F_Value) VALUES");
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == 0)
                        builder.Append(string.Format("('{0}','{1}','{2}',{3})", list[i].BuildID, list[i].MeterCode,
                            list[i].Time.ToString("yyyy-MM-dd HH:mm:ss"), list[i].Value));
                    else
                        builder.Append(string.Format(",('{0}','{1}','{2}',{3})", list[i].BuildID, list[i].MeterCode,
                            list[i].Time.ToString("yyyy-MM-dd HH:mm:ss"), list[i].Value));
                }

                builder.Append("  ON DUPLICATE KEY UPDATE F_Value=VALUES(F_Value);");
            }

            return builder.ToString();
        }

        public static string GenerateOrigStateSQL(DateTime time)
        {
            return "";
        }
    }
}
