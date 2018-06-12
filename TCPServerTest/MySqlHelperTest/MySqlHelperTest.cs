using System;
using System.Collections.Generic;
using Energy.Common.Entity;
using EnergyAnalysis;
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

            List<OriginEnergyData> energyDatas = SaveDataFromSqliteToMySql.GetEnergyData(source);
            List<VoltageData> voltageDatas= SaveDataFromSqliteToMySql.GetVoltageData(source);
            List<CurrentData> currentDatas = SaveDataFromSqliteToMySql.GetCurrentData(source);
            
            string connectString = "Server=127.0.0.1;Port=3306;Database={0};Uid=root;Pwd=Fight4benben";

            Energy.Common.DAL.MySQLHelper.ExecuteTransactionScope(connectString, energyDatas, voltageDatas, currentDatas);
        }
    }
}
