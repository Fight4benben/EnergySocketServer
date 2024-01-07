using Energy.Common.Entity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.DAL
{
    public class SQLServerHelper
    {
        public static void SaveData2SqlServer(string dataConnString,string hisConnString, MeterList list)
        {
            string time = list.CollectTime.ToString("yyyy-MM-dd HH:mm:00");
            if (list.CollectTime.Minute % 15 == 0)
            {
                int emsCnt = 0;
                StringBuilder energyBuilder = new StringBuilder();

                foreach (var meter in list.Meters)
                {
                    var meterParams = meter.MeterParams.FindAll(x=>x.ParamName.ToUpper() == "EPI" || 
                        x.ParamName.ToUpper() == "EPIJ" ||
                        x.ParamName.ToUpper() == "EPIF" || 
                        x.ParamName.ToUpper() == "EPIP" ||
                        x.ParamName.ToUpper() == "EPIG" ||
                        x.ParamName.ToUpper() == "LJLL");

                    foreach (var meterParam in meterParams)
                    {
                        if (emsCnt >= 800)
                        {
                            string sql1 = energyBuilder.ToString();
                            SaveData(dataConnString, sql1);



                            energyBuilder = energyBuilder.Clear();
                            emsCnt = 0;
                        }

                        if (emsCnt == 0)
                        {
                            energyBuilder.Append(@"INSERT INTO Data_GateWayUpdate(F_BuildID, F_CollectTime, F_ReceiveTime, F_MeterCode, F_ParamCode, F_DisConnect,F_DisConnectTime,F_Value) VALUES ");
                            energyBuilder.Append($"('{list.BuildId}','{time}','{time}','{meter.MeterId}','{meterParam.ParamName}',0, NULL, '{meterParam.ParamValue}')");
                        }
                        else
                        {
                            energyBuilder.Append($",('{list.BuildId}','{time}','{time}','{meter.MeterId}','{meterParam.ParamName}',0, NULL, '{meterParam.ParamValue}')");
                        }

                        emsCnt++;
                    }

                    
                }

                if(energyBuilder.ToString().Length > 0)
                    SaveData(dataConnString, energyBuilder.ToString());
            }

            int hisCnt = 0;
            StringBuilder hisBuilder = new StringBuilder();
            // 保存到HistoryData表
            foreach (var meter in list.Meters)
            {
                foreach (var meterParam in meter.MeterParams)
                {
                    if (hisCnt >= 800)
                    {
                        SaveData(hisConnString, hisBuilder.ToString());
                        hisBuilder = hisBuilder.Clear();
                        hisCnt = 0;
                    }

                    if (hisCnt == 0)
                    {
                        hisBuilder.Append(@"INSERT INTO HistoryGateWayData(F_BuildID,F_MeterCode,F_ParamCode, F_DateTime, F_PV) VALUES");
                        hisBuilder.Append($" ('{list.BuildId}','{meter.MeterId}','{meterParam.ParamName}','{time}',{meterParam.ParamValue})");
                    }
                    else
                    {
                        hisBuilder.Append($" ,('{list.BuildId}','{meter.MeterId}','{meterParam.ParamName}','{time}',{meterParam.ParamValue})");
                    }

                    hisCnt++;
                }
            }

            SaveData(hisConnString, hisBuilder.ToString());
        }

        private static void SaveData(string connString, string sql)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = sql;

                command.ExecuteNonQuery();
            }
        }

        public static int ExecuteStoredProcedure(string connString, string procedureName)
        {
            int cnt = 0;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(procedureName, conn))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    cnt = command.ExecuteNonQuery();
                }
            }

            return cnt;
        }

       
    }
}
