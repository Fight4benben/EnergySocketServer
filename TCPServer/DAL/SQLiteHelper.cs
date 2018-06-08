using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using TCPServer.Entity;
using TCPServer.Utils;

namespace TCPServer.DAL
{
    public class SQLiteHelper
    {
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbName"></param>
        public static void CreateLocalDB(string dbName)
        {
            //判断是否存在当前数据库，若不存在则创建，存在则不执行任何操作
            if (!File.Exists(dbName))
            {
                SQLiteConnection.CreateFile(dbName);
            }
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="dbName">数据库名</param>
        /// <param name="tableName">表名</param>
        /// <param name="createSQL">创建SQL语句</param>
        private static void CreateTableInDB(string dbName,string tableName,string createSQL)
        {
            //如果不包含dbName则，推出该方法
            if (!File.Exists(dbName))
                return;

            using (SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;",dbName)))
            {
                connection.Open();
                //1. 判断数据库中是否包含该表
                if (IsTableExists(connection, tableName))
                {
                    connection.Close();
                    return;
                }

                //2.不存在则创建表
                string sql = string.Format(createSQL,tableName);
                try
                {
                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                catch
                {

                }
                
            }
        }

        public static void CreateTempDataTable(string dbName)
        {
            string sql = @"create table {0}(
                           BuildID VARCHAR(15) NOT NULL,
                           GatewayID VARCHAR(30) NOT NULL,
                           CollectTime DATETIME NOT NULL,
                           DatagramType VARCHAR(10) NOT NULL,
                           Status INT DEFAULT 0,
                           JsonData TEXT NOT NULL,
                           primary key(BuildID,GatewayID,CollectTime));";

            CreateTableInDB("TempData","GatewayData",sql);
        }

        /// <summary>
        /// 向临时中专表中插入网关上传的数据
        /// </summary>
        /// <param name="info"></param>
        /// <param name="JsonSource"></param>
        /// <returns></returns>
        public static bool InsertGatewayDataToDB(MessageInfo info,string JsonSource)
        {
            string sql = string.Format(@"insert into GatewayData(BuildID,GatewayID,CollectTime,DatagramType,Status,JsonData)
                            values('{0}','{1}','{2}','{3}',{4},'{5}');",info.BuildID,info.GatewayID, ToolUtil.GetDateTimeFromString(info.MessageContent).ToString("yyyy-MM-dd HH:mm:ss"),
                            info.MessageAttribute,0, JsonSource);

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=TempData;Version=3;"))
            {
                connection.Open();
                try
                {
                    SQLiteCommand command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }
                catch
                {
                    return false;
                } 
            }

            return true;
        }

        public static SourceData GetFirstGatewayData()
        {
            string sql = @"select * from {0} where Status=0 order by CollectTime ASC limit 0,1;";

            SourceData sourceData = new SourceData();

            SQLiteDataReader reader = ExecuteReader("TempData","GatewayData",sql);

            while (reader.Read())
            {
                sourceData.BuildID = reader["BuildID"].ToString();
                sourceData.GatewayID = reader["GatewayID"].ToString();
                sourceData.CollectTime = reader["CollectTime"].ToString();
                sourceData.DatagramType = reader["DatagramType"].ToString();
                sourceData.Status = Convert.ToInt32(reader["Status"]);
                sourceData.JsonData = reader["JsonData"].ToString();
            }

            reader.Close();

            return sourceData;
        }

        public static SourceData GetSourceByHeader(SourceDataHeader header)
        {
            string sql ="select * from {0}"+ string.Format(@" where BuildID='{0}' 
                                    and GatewayID='{1}' and CollectTime='{2}' 
                                    and DatagramType='{3}' and Status={4};",header.BuildID,header.GatewayID,header.CollectTime,header.DatagramType,header.Status);

            SourceData sourceData = new SourceData();
            SQLiteDataReader reader = ExecuteReader("TempData", "GatewayData", sql);

            while (reader.Read())
            {
                sourceData.BuildID = reader["BuildID"].ToString();
                sourceData.GatewayID = reader["GatewayID"].ToString();
                sourceData.CollectTime = reader["CollectTime"].ToString();
                sourceData.DatagramType = reader["DatagramType"].ToString();
                sourceData.Status = Convert.ToInt32(reader["Status"]);
                sourceData.JsonData = reader["JsonData"].ToString();
            }

            reader.Close();

            return sourceData;
        }

        public static List<SourceDataHeader> GetUnStoreList()
        {
            string sql = @"select BuildID,GatewayID,CollectTime,DatagramType,Status from {0} where Status=0 order by CollectTime ASC limit 0,1000;";

            SQLiteDataReader reader = ExecuteReader("TempData","GatewayData",sql);

            List<SourceDataHeader> headerList = new List<SourceDataHeader>();

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

            reader.Close();

            return headerList;
        }

        

        private static int ExecuteNonQuery(string dbName,string sql)
        {
            int result = 0;
            using (SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;",dbName)))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(sql,connection);
                result = command.ExecuteNonQuery();
            }

            return result;
        }

        private static SQLiteDataReader ExecuteReader(string dbName,string tableName,string sql)
        {
            string sql1 = string.Format(sql,tableName);
            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbName));

            try
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(sql1, connection);
                return command.ExecuteReader();
            }
            catch (Exception)
            {
                throw;
            }
          
        }

        private static bool IsTableExists(SQLiteConnection connection,string tableName)
        {
            string sql1 = string.Format(@"select count(*) from sqlite_master
                            where type='table' and name ='{0}';",tableName);

            SQLiteCommand command = new SQLiteCommand(sql1,connection);
            if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                return false;
            else
                return true;
        }
    }
}
