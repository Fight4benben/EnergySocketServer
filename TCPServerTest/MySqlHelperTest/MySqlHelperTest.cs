using System;
using System.Collections.Generic;
using Energy.Common.Entity;
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

            List<OriginEnergyData> origDataList = new List<OriginEnergyData>();

            foreach (var item in source.Meters)
            {
                MeterParam param = item.MeterParams.Find(meterParam => meterParam.ParamName.ToLower() == "EPI".ToLower() || meterParam.ParamName == "LJLL".ToLower()
                                                || meterParam.ParamName.ToLower() == "LLHeat".ToLower());

                if (param == null)
                    continue;

                origDataList.Add(new OriginEnergyData()
                {
                    BuildID = source.BuildId,
                    MeterCode = item.MeterId,
                    Time = source.CollectTime,
                    Value = param.ParamValue,
                    Calced = false
                });
            }
            string connectString = "Server=127.0.0.1;Port=3306;Database={0};Uid=root;Pwd=Fight4benben";
            Energy.Common.DAL.MySQLHelper.ExecuteTransactionScope(connectString,origDataList);
        }
    }
}
