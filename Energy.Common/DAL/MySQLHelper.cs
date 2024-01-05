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
        /// 向临时中专表中插入网关上传的数据
        /// </summary>
        /// <param name="info"></param>
        /// <param name="JsonSource"></param>
        /// <returns></returns>
        public static bool InsertGatewayDataToDB(MessageInfo info, string JsonSource, string connectString)
        {
            string sql = string.Format(@"insert into gatewaydata(BuildID,GatewayID,CollectTime,DatagramType,Status,JsonData)
                            values('{0}','{1}','{2}','{3}',{4},'{5}');", info.BuildID, info.GatewayID, ToolUtil.GetDateTimeFromString(info.MessageContent).ToString("yyyy-MM-dd HH:mm:ss"),
                            info.MessageAttribute, 0, JsonSource);

            using (MySqlConnection connection = new MySqlConnection(connectString))
            {
                connection.Open();
                try
                {
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetUpdateStatusSQL(SourceDataHeader header)
        {
            string sql = string.Format(@"update gatewaydata set Status=1 where BuildID='{0}' and GatewayID = '{1}' 
                            and CollectTime='{2}' and DatagramType='{3}';", header.BuildID, header.GatewayID,
                            header.CollectTime, header.DatagramType);

            return sql;
        }

        public static List<SourceDataHeader> GetUnStoreList(string connectionString)
        {
            string sql = @"select BuildID,GatewayID,CollectTime,DatagramType,Status from gatewaydata where Status=0 order by CollectTime ASC limit 0,1000;";

            List<SourceDataHeader> headerList = new List<SourceDataHeader>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(sql,connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    SourceDataHeader header = new SourceDataHeader();
                    header.BuildID = reader["BuildID"].ToString();
                    header.GatewayID = reader["GatewayID"].ToString();
                    header.CollectTime = DateTime.Parse(reader["CollectTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    header.DatagramType = reader["DatagramType"].ToString();
                    header.Status = Convert.ToInt32(reader["Status"]);

                    headerList.Add(header);
                }
            }

            return headerList;
        }

        //public static List<RentEnergyData> GetRentEnergyDatum()
        //{
        //    string sql = @"select F_BuildID BuildID,F_MeterCode MeterCode,F_Day Day,F_StartValue StartValue,F_EndValue EndValue,F_Calced Calced 
        //            from t_rent_daysatrtendvalue where f_day = {0}";
        //}

        public static SourceData GetSourceByHeader(SourceDataHeader header,string connectionString)
        {
            string sql = "select * from gatewaydata" + string.Format(@" where BuildID='{0}' 
                                    and GatewayID='{1}' and CollectTime='{2}' 
                                    and DatagramType='{3}' and Status={4};", header.BuildID, header.GatewayID, header.CollectTime, header.DatagramType, header.Status);

            SourceData sourceData = new SourceData();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //SourceDataHeader header = new SourceDataHeader();
                    sourceData.BuildID = reader["BuildID"].ToString();
                    sourceData.GatewayID = reader["GatewayID"].ToString();
                    sourceData.CollectTime = DateTime.Parse(reader["CollectTime"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                    sourceData.DatagramType = reader["DatagramType"].ToString();
                    sourceData.Status = Convert.ToInt32(reader["Status"]);
                    sourceData.JsonData = reader["JsonData"].ToString();
                }
            }

            return sourceData;
        }

        /// <summary>
        /// 删除已经存储并解析到mysql数据的原始报文数据
        /// </summary>
        /// <returns></returns>
        public static int DeleteStoredGatewayDataFromDB(string connectionString)
        {
            string sql = "delete from gatewaydata where Status=1";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    int result = command.ExecuteNonQuery();

                    return result;
                }
                catch
                {
                    return -9999;
                }
            }
        }

        public static int DeleteOriginData(string connectionString)
        {
            try
            {
                string sql = "delete from t_data_originenergyvalue where F_Calced=1 or F_Time<DATE_ADD(NOW(),INTERVAL -5 day)";

                //string sql = "delete from t_ov_origvalue where F_Calced=1 or F_Time<DATE_ADD(NOW(),INTERVAL -10 day)";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    try
                    {
                        MySqlCommand command = new MySqlCommand(sql, connection);
                        int result = command.ExecuteNonQuery();

                        return result;
                    }
                    catch
                    {
                        return -9999;
                    }

                    connection.Close();
                }
            }
            catch
            {
                return -10001;
            }

        }

        /// <summary>
        /// 如果解析数据并且插入成功，则将Status标记为1,作为事务使用
        /// </summary>
        /// <returns></returns>
        //public static string GetUpdateStatusSQL(SourceDataHeader header)
        //{
        //    string sql = string.Format(@"update gatewaydata set Status=1 where BuildID='{0}' and GatewayID = '{1}' 
        //                    and CollectTime='{2}' and DatagramType='{3}';", header.BuildID, header.GatewayID,
        //                    header.CollectTime, header.DatagramType);

        //    return sql;
        //}

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
                MySqlCommand command = connection.CreateCommand();
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

        public static int InsertGatewayStatusByList(string conn, List<GatewayStatus> list)
        {
            if (list.Count == 0)
                return -1;

            StringBuilder builder = new StringBuilder();
            builder.Append("replace into t_status_gateway(F_BuildID,F_GatewayID,F_LastUploadTime,F_Status,F_XmlTime) ");

            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    builder.Append(string.Format("values('{0}','{1}','{2}',{3},'{4}')", list[i].BuildId, list[i].GatewayId,
                        list[i].LastUploadTime.ToString("yyyy-MM-dd HH:mm:ss"), list[i].Status, list[i].XmlTime.ToString("yyyy-MM-dd HH:mm:ss")));
                }
                else
                {
                    builder.Append(string.Format(",('{0}','{1}','{2}',{3},'{4}')", list[i].BuildId, list[i].GatewayId,
                        list[i].LastUploadTime.ToString("yyyy-MM-dd HH:mm:ss"), list[i].Status, list[i].XmlTime.ToString("yyyy-MM-dd HH:mm:ss")));
                }
            }

            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand();

                command.Connection = connection;
                command.CommandTimeout = 0;

                try
                {
            
                    command.CommandText = builder.ToString();
                    int cnt =command.ExecuteNonQuery();

                    return cnt;
                  
                }
                catch
                {
                    return -2;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static int InsertMeterStatusByList(string conn, List<MeterStatus> list)
        {
            if (list.Count == 0)
                return -1;

            StringBuilder builder = new StringBuilder();
            builder.Append("replace into t_status_meter(F_BuildID,F_GatewayID,F_MeterCode,F_Status,F_XmlTime,F_LastUploadTime) ");

            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    builder.Append(string.Format("values('{0}','{1}','{2}',{3},'{4}','{5}')", list[i].BuildId, list[i].GatewayId,
                        list[i].MeterCode, list[i].Status, list[i].XmlTime.ToString("yyyy-MM-dd HH:mm:ss"), list[i].LastUploadTime.ToString("yyyy-MM-dd HH:mm:ss")));
                }
                else
                {
                    builder.Append(string.Format(" ,('{0}','{1}','{2}',{3},'{4}','{5}')", list[i].BuildId, list[i].GatewayId,
                        list[i].MeterCode, list[i].Status, list[i].XmlTime.ToString("yyyy-MM-dd HH:mm:ss"), list[i].LastUploadTime.ToString("yyyy-MM-dd HH:mm:ss")));
                }
            }

            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand();

                command.Connection = connection;
                command.CommandTimeout = 0;

                try
                {

                    command.CommandText = builder.ToString();
                    int cnt = command.ExecuteNonQuery();

                    return cnt;

                }
                catch
                {
                    return -2;
                }
                finally
                {
                    connection.Close();
                }
            }
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
                        builder.Append("insert into t_data_originenergyvalue ");
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
                case "Energy.Common.Entity.PowerData":
                    List<Energy.Common.Entity.PowerData> listPower = (List<Energy.Common.Entity.PowerData>)item;
                    if (listPower.Count > 0)
                    {
                        builder.Append("insert into t_data_power ");
                        for (int i = 0; i < listPower.Count; i++)
                        {
                            Energy.Common.Entity.PowerData powerData = listPower[i];
                            if (i == 0)
                                builder.Append(string.Format(" values('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14})",
                                    powerData.BuildID,powerData.MeterCode, powerData.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Utils.ToolUtil.JudgeParamValue(powerData.P), Utils.ToolUtil.JudgeParamValue(powerData.Pa), Utils.ToolUtil.JudgeParamValue(powerData.Pb), Utils.ToolUtil.JudgeParamValue(powerData.Pc),
                                    Utils.ToolUtil.JudgeParamValue(powerData.Q), Utils.ToolUtil.JudgeParamValue(powerData.Qa), Utils.ToolUtil.JudgeParamValue(powerData.Qb), Utils.ToolUtil.JudgeParamValue(powerData.Qc),
                                    Utils.ToolUtil.JudgeParamValue(powerData.S), Utils.ToolUtil.JudgeParamValue(powerData.Sa), Utils.ToolUtil.JudgeParamValue(powerData.Sb), Utils.ToolUtil.JudgeParamValue(powerData.Sc)));
                            else
                                builder.Append(string.Format(" ,('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14})",
                                    powerData.BuildID, powerData.MeterCode, powerData.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Utils.ToolUtil.JudgeParamValue(powerData.P), Utils.ToolUtil.JudgeParamValue(powerData.Pa), Utils.ToolUtil.JudgeParamValue(powerData.Pb), Utils.ToolUtil.JudgeParamValue(powerData.Pc),
                                    Utils.ToolUtil.JudgeParamValue(powerData.Q), Utils.ToolUtil.JudgeParamValue(powerData.Qa), Utils.ToolUtil.JudgeParamValue(powerData.Qb), Utils.ToolUtil.JudgeParamValue(powerData.Qc),
                                    Utils.ToolUtil.JudgeParamValue(powerData.S), Utils.ToolUtil.JudgeParamValue(powerData.Sa), Utils.ToolUtil.JudgeParamValue(powerData.Sb), Utils.ToolUtil.JudgeParamValue(powerData.Sc)));
                        }
                    }
                    break;
                case "Energy.Common.Entity.BaseElecData":
                    List<Energy.Common.Entity.BaseElecData> listBaseElec = (List<Energy.Common.Entity.BaseElecData>)item;
                    if (listBaseElec.Count > 0)
                    {
                        builder.Append("insert into t_data_baseelecparam ");
                        for (int i = 0; i < listBaseElec.Count; i++)
                        {
                            Energy.Common.Entity.BaseElecData baseElecData = listBaseElec[i];
                            if (i == 0)
                            {
                                builder.Append(string.Format(" values('{0}','{1}','{2}',{3},{4})",
                                   baseElecData.BuildID, baseElecData.MeterCode, baseElecData.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                   Utils.ToolUtil.JudgeParamValue(baseElecData.PF), Utils.ToolUtil.JudgeParamValue(baseElecData.Fr)));
                            }
                            else
                            {
                                builder.Append(string.Format(" ,('{0}','{1}','{2}',{3},{4})",
                                   baseElecData.BuildID, baseElecData.MeterCode, baseElecData.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                                   Utils.ToolUtil.JudgeParamValue(baseElecData.PF), Utils.ToolUtil.JudgeParamValue(baseElecData.Fr)));
                            }
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
            string sql = @"select F_Time FROM t_data_originenergyvalue
                           where F_Time > DATE_ADD(now(), INTERVAL -5 day)
                           AND F_Calced = 0 group by F_Time 
                           order by F_Time ASC limit 100; ";
            //string sql = @"select F_Time FROM t_ov_origvalue
            //               where F_Time > DATE_ADD(now(), INTERVAL -10 day)
            //               AND F_Calced = 0 group by F_Time 
            //               order by F_Time ASC limit 100; ";

            using (MySqlConnection connection = new MySqlConnection(connectString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(sql,connection);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    timeList.Add(Convert.ToDateTime(reader["F_Time"].ToString()));
                }

                connection.Close();
            } 

            return timeList;
        }

        public static List<OriginEnergyData> GetUnCalcedEnergyDataList(string connectString,DateTime startTime,DateTime endTime)
        {
            List<OriginEnergyData> list = new List<OriginEnergyData>();

            string sql = string.Format(@"select * from t_data_originenergyvalue
                                        where F_Time between '{0}' and '{1}'
                                        and F_Calced = 0", startTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                        endTime.ToString("yyyy-MM-dd HH:mm:ss"));

            //string sql = string.Format(@"select * from t_ov_origvalue
            //                            where F_Time between '{0}' and '{1}'
            //                            and F_Calced = 0", startTime.ToString("yyyy-MM-dd HH:mm:ss"),
            //                            endTime.ToString("yyyy-MM-dd HH:mm:ss"));

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
                connection.Close();
            }

            return list;
        }

        public static int InsertRealTimeValues(string connectString,List<YangZhuChangMeter> list)
        {
            if (list.Count == 0)
                return 0;

            StringBuilder builder = new StringBuilder();
            StringBuilder builder1 = new StringBuilder();
            builder.Append("replace into t_data_realtimevalue(BuildId,GatewayName,MeterCode,Time,Value,Status)");
            builder1.Append("replace into t_data_historyvalue(BuildId,GatewayName,MeterCode,Time,Value,Status)");
            for (int i = 0; i < list.Count; i++)
            {
                YangZhuChangMeter temp = list[i];
                if (i == 0)
                {
                    builder.Append(string.Format(" values('{0}','{1}','{2}','{3}',{4},'{5}')", temp.BuildId, temp.GatewayName, temp.MeterCode,
                        temp.Time.ToString("yyyy-MM-dd HH:mm:00"), temp.Value, temp.Status));
                    builder1.Append(string.Format(" values('{0}','{1}','{2}','{3}',{4},'{5}')", temp.BuildId, temp.GatewayName, temp.MeterCode,
                        temp.Time.ToString("yyyy-MM-dd HH:mm:00"), temp.Value, temp.Status));
                }
                else
                {
                    builder.Append(string.Format(" ,('{0}','{1}','{2}','{3}',{4},'{5}')", temp.BuildId, temp.GatewayName, temp.MeterCode,
                        temp.Time.ToString("yyyy-MM-dd HH:mm:00"), temp.Value, temp.Status));
                    builder1.Append(string.Format(" ,('{0}','{1}','{2}','{3}',{4},'{5}')", temp.BuildId, temp.GatewayName, temp.MeterCode,
                         temp.Time.ToString("yyyy-MM-dd HH:mm:00"), temp.Value, temp.Status));
                }
            }
            int cnt = 0;
            using (MySqlConnection connection = new MySqlConnection(connectString))
            {
                connection.Open();
                try
                {
                    MySqlCommand command = new MySqlCommand(builder.ToString(), connection);
                    command.CommandText = builder.ToString();
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    cnt += command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            using (MySqlConnection connection = new MySqlConnection(connectString))
            {
                connection.Open();
                try
                {
                    MySqlCommand command = new MySqlCommand(builder1.ToString(), connection);
                    command.CommandText = builder1.ToString();
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    cnt += command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return cnt;
        }
    }
}
