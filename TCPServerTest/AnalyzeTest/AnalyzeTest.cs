using System;
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

            

            Console.WriteLine(data.JsonData);
        }
    }
}
