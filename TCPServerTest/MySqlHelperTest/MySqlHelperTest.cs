﻿using System;
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

        [TestMethod]
        public void TestGetUnCalculateDataList()
        {
            List<DateTime> times = Energy.Common.DAL.MySQLHelper.GetUnCalculatedDataTimeList("Server=127.0.0.1;Port=3306;Database=energydb;Uid=root;Pwd=Fight4benben");

            List<OriginEnergyData> list = Energy.Common.DAL.MySQLHelper.GetUnCalcedEnergyDataList("Server=127.0.0.1;Port=3306;Database=energydb;Uid=root;Pwd=Fight4benben",times[0],times[times.Count-1]);

            Console.WriteLine(list.Count);
        }

        [TestMethod]
        public void TestGetNextTime()
        {
            //List<DateTime> times = Energy.Common.DAL.MySQLHelper.GetUnCalculatedDataTimeList("Server=127.0.0.1;Port=3306;Database=energydb;Uid=root;Pwd=Fight4benben");
            List<DateTime> times = new List<DateTime>();

            times.Add(new DateTime(2018,6,20,10,10,0));
            DateTime nextTime = times.Find(t=> t > times[0]);

            Console.WriteLine(new DateTime().ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine(nextTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
