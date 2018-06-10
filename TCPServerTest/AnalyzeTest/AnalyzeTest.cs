﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Energy.Common.Entity;
using Energy.Common.DAL;

namespace TCPServerTest.AnalyzeTest
{
    [TestClass]
    public class AnalyzeTest
    {
        [TestMethod]
        public void TestJsonConvert()
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

                origDataList.Add(new OriginEnergyData() {
                    BuildID = source.BuildId,
                    MeterCode = item.MeterId,
                    Time = source.CollectTime,
                    Value = param.ParamValue,
                    Calced = false
                });
            }

            Console.WriteLine(data.JsonData);
        }
    }
}
