using System;
using System.Collections.Generic;
using Energy.Common.Entity;
using Energy.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace TCPServerTest.MySqlHelperTest
{
    [TestClass]
    public class MySqlHelperTest
    {
        [TestMethod]
        public void TestConnectToMySql()
        {
            string connectString = "Server=127.0.0.1;Port=3306;Database=test;Uid=root;Pwd=Fight4benben";
            using (MySqlConnection connection = new MySqlConnection(connectString))
            {
                connection.Open();

                Console.WriteLine("Open energydb Success!");
            }
        }

        [TestMethod]
        public void TestTransaction()
        {
            MeterList source;

            List<SourceDataHeader> headerList = Energy.Common.DAL.SQLiteHelper.GetUnStoreList();

            SourceData data = Energy.Common.DAL.SQLiteHelper.GetSourceByHeader(headerList[0]);

            source = JsonConvert.DeserializeObject<MeterList>(data.JsonData);

            SaveDataFromSqliteToMySql.ExecuteInsertTransactions(source,headerList[0]);
        }

        [TestMethod]
        public void TestGetUnCalculateTimeList()
        {
            List<DateTime> times = Energy.Common.DAL.MySQLHelper.GetUnCalculatedDataTimeList("Server=127.0.0.1;Port=3306;Database=energydb;Uid=root;Pwd=Fight4benben");

            foreach (var item in times)
            {
                Console.WriteLine(item.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
    }
}
