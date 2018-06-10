using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// 执行事务
        /// </summary>
        /// <param name="connectString"></param>
        /// <param name="lists"></param>
        public static void ExecuteTransactionScope(string connectString,params object[] lists)
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

            Console.WriteLine(sqls);

            //下面代码下次废弃
            MySqlCommand command1 = InsertValueTransaction(connectString);
            MySqlCommand command2 = InsertBuildingValue(connectString);

            command1.Connection.Open();
            command2.Connection.Open();
            MySqlTransaction transaction1 = command1.Connection.BeginTransaction();
            MySqlTransaction transaction2 = command2.Connection.BeginTransaction();

            try
            {
                command1.ExecuteNonQuery();
                command2.ExecuteNonQuery();
            } catch (MySqlException ex)
            {
                transaction1.Rollback();
                transaction2.Rollback();
                command1.Connection.Close();
                command2.Connection.Close();

                return;
            }

            transaction1.Commit();
            transaction2.Commit();

            command1.Connection.Close();
            command2.Connection.Close();
        }

        private static string GenerateSQLByList(string typeName,object item)
        {
            StringBuilder builder = new StringBuilder();
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
                    
                    break;
                case "Energy.Common.Entity.CurrentData":
                    break;
            }
            return builder.ToString();
        }

    }
}
