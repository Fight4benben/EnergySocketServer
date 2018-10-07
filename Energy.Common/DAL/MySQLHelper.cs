using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Energy.Common.Entity;
using Energy.Common.Utils;
using MySql.Data.MySqlClient;

namespace Energy.Common.DAL
{
    public class MySQLHelper
    {

        private static MySqlCommand InsertValueTransaction(string connectString)
        {
            string sql1 = "insert into user(user_name) values('zhangsan');";

            MySqlConnection connection = new MySqlConnection(string.Format(connectString, "test"));

            MySqlCommand command = new MySqlCommand(sql1,connection);

            return command;
        }

        private static MySqlCommand InsertBuildingValue(string connectString)
        {
            string sql = "insert into user values(1,'test');";

            MySqlConnection connection = new MySqlConnection(string.Format(connectString,"test1"));

            MySqlCommand command = new MySqlCommand(sql,connection);

            return command;
        }

        /// <summary>
        /// 将数据通过事务插入到能源数据表中
        /// </summary>
        /// <param name="connectString">连接字符串</param>
        /// <param name="sqls">多个能源的sql语句，分钟、小时、天</param>
        /// <returns></returns>
        public static bool InsertDataTable(string connectString,List<string> sqls)
        {
            using (MySqlConnection connection = new MySqlConnection(connectString))
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();
                MySqlCommand command = new MySqlCommand();

                command.Connection = connection;
                command.CommandTimeout = 0;
                command.Transaction = transaction;
                
                try
                {
                    for (int i = 0; i < sqls.Count; i++)
                    {
                        //当计算后List为空的情况下，生成的SQL为"",此时不应该执行语句
                        if (sqls[i] == "")
                            continue;

                        string sql = sqls[i];
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "<" + Thread.CurrentThread.ManagedThreadId.ToString() + "> -> " + "执行SQL计算事务...");

                    return true;
                }
                catch (Exception e)
                {

                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "<" + Thread.CurrentThread.ManagedThreadId.ToString() + "> -> " + e.Message);
                    transaction.Rollback();
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="connectString"></param>
        /// <param name="lists"></param>
        public static List<string> GetInsertSqls(params object[] lists)
        {
            List<string> sqls = new List<string>();
            foreach (var item in lists)
            {
                if (!item.GetType().IsGenericType)
                    continue;

                string name =item.GetType().GenericTypeArguments[0].FullName;

                string sql = GenerateSQLByList(name, item);
                if (!string.IsNullOrEmpty(sql))
                {
                    sqls.Add(sql);
                }

            }

            return sqls;
        }

        private static string GenerateSQLByList(string typeName,object item)
        {
            StringBuilder builder = new StringBuilder();
            //List<Energy.Common.Entity.HistoryBaseData> list;
            switch (typeName)
            {
                case "Energy.Common.Entity.OriginEnergyData":
                    List<Energy.Common.Entity.OriginEnergyData> list = (List<Energy.Common.Entity.OriginEnergyData>)item;
                    if (list.Count > 0)
                    {
                        builder.Append("insert into t_ov_origvalue ");
                        for (int i = 0; i < list.Count; i++)
                        {
                            Energy.Common.Entity.OriginEnergyData originEnergyData = list[i];
                            if (i == 0)
                                builder.Append(string.Format(" values('{0}','{1}','{2}',{3},{4})", originEnergyData.BuildID,
                                    originEnergyData.MeterCode, originEnergyData.Time.ToString("yyyy-MM-dd HH:mm:ss"), originEnergyData.Value,
                                    (originEnergyData.Calced == true) ? 1 : 0));
                            else
                                builder.Append(string.Format(",('{0}','{1}','{2}',{3},{4})", originEnergyData.BuildID,
                                    originEnergyData.MeterCode, originEnergyData.Time.ToString("yyyy-MM-dd HH:mm:ss"), originEnergyData.Value,
                                    (originEnergyData.Calced == true) ? 1 : 0));
                        }
                    }
                    break;
                case "Energy.Common.Entity.VoltageData":
                    List<Energy.Common.Entity.VoltageData> listVolatage = (List<Energy.Common.Entity.VoltageData>)item;
                    if (listVolatage.Count > 0)
                    {
                        builder.Append("insert into t_data_voltagevalue ");
                        for (int i = 0; i < listVolatage.Count; i++)
                        {
                            Entity.VoltageData voltageData = listVolatage[i];
                            if (i == 0)
                                builder.Append(string.Format(" values('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9})",
                                    voltageData.BuildID, voltageData.MeterCode, voltageData.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Utils.ToolUtil.JudgeParamValue(voltageData.U), Utils.ToolUtil.JudgeParamValue(voltageData.Ua),
                                    Utils.ToolUtil.JudgeParamValue(voltageData.Ub), Utils.ToolUtil.JudgeParamValue(voltageData.Uc),
                                    Utils.ToolUtil.JudgeParamValue(voltageData.Uab), Utils.ToolUtil.JudgeParamValue(voltageData.Ubc),
                                    Utils.ToolUtil.JudgeParamValue(voltageData.Uca)));
                            else
                                builder.Append(string.Format(" ,('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9})",
                                    voltageData.BuildID, voltageData.MeterCode, voltageData.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Utils.ToolUtil.JudgeParamValue(voltageData.U), Utils.ToolUtil.JudgeParamValue(voltageData.Ua),
                                    Utils.ToolUtil.JudgeParamValue(voltageData.Ub), Utils.ToolUtil.JudgeParamValue(voltageData.Uc),
                                    Utils.ToolUtil.JudgeParamValue(voltageData.Uab), Utils.ToolUtil.JudgeParamValue(voltageData.Ubc),
                                    Utils.ToolUtil.JudgeParamValue(voltageData.Uca)));

                        }
                    }
                    break;
                case "Energy.Common.Entity.CurrentData":
                    List<Energy.Common.Entity.CurrentData> listCurrent = (List<Energy.Common.Entity.CurrentData>)item;
                    if (listCurrent.Count > 0)
                    {
                        builder.Append("insert into t_data_currentvalue ");
                        for (int i = 0; i < listCurrent.Count; i++)
                        {
                            Entity.CurrentData currentData = listCurrent[i];
                            if (i == 0)
                                builder.Append(string.Format(" values('{0}','{1}','{2}',{3},{4},{5},{6})",
                                    currentData.BuildID, currentData.MeterCode, currentData.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Utils.ToolUtil.JudgeParamValue(currentData.I), Utils.ToolUtil.JudgeParamValue(currentData.Ia),
                                    Utils.ToolUtil.JudgeParamValue(currentData.Ib), Utils.ToolUtil.JudgeParamValue(currentData.Ic)));
                            else
                                builder.Append(string.Format(" ,('{0}','{1}','{2}',{3},{4},{5},{6})",
                                    currentData.BuildID, currentData.MeterCode, currentData.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Utils.ToolUtil.JudgeParamValue(currentData.I), Utils.ToolUtil.JudgeParamValue(currentData.Ia),
                                    Utils.ToolUtil.JudgeParamValue(currentData.Ib), Utils.ToolUtil.JudgeParamValue(currentData.Ic)));
                        }
                    }
                    
                    break;
            }
            return builder.ToString();
        }

        /// <summary>
        /// 获取前100个未处理数据的时间
        /// </summary>
        /// <returns></returns>
        public static List<DateTime> GetUnCalculatedDataTimeList(string connectString)
        {
            List<DateTime> timeList = new List<DateTime>();
            string sql = @"select F_Time FROM t_ov_origvalue
                           where F_Calced = 0 group by F_Time 
                           order by F_Time ASC limit 100; ";

            using (MySqlConnection connection = new MySqlConnection(connectString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(sql,connection);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    timeList.Add(Convert.ToDateTime(reader["F_Time"].ToString()));
                }
            } 

            return timeList;
        }

        public static List<OriginEnergyData> GetUnCalcedEnergyDataList(string connectString,DateTime startTime,DateTime endTime)
        {
            List<OriginEnergyData> list = new List<OriginEnergyData>();

            string sql = string.Format(@"select * from t_ov_origvalue
                                        where F_Time between '{0}' and '{1}'
                                        and F_Calced = 0",startTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                        endTime.ToString("yyyy-MM-dd HH:mm:ss"));

            using (MySqlConnection connection = new MySqlConnection(connectString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(sql, connection);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OriginEnergyData originEnergyData = new OriginEnergyData();
                    originEnergyData.BuildID = reader["F_BuildID"].ToString();
                    originEnergyData.MeterCode = reader["F_MeterCode"].ToString();
                    originEnergyData.Time = Convert.ToDateTime(reader["F_Time"].ToString());
                    originEnergyData.Value = Convert.ToSingle(reader["F_Value"]);
                    originEnergyData.Calced = Convert.ToInt32(reader["F_Calced"])==0?false:true;
                    list.Add(originEnergyData);
                }
            }

            return list;
        }

    }
}
