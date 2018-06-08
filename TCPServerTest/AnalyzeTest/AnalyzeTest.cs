using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TCPServer.Entity;

namespace TCPServerTest.AnalyzeTest
{
    [TestClass]
    public class AnalyzeTest
    {
        [TestMethod]
        public void TestJsonConvert()
        {
            MeterList source;

            List<SourceDataHeader> headerList = TCPServer.DAL.SQLiteHelper.GetUnStoreList();

            SourceData data = TCPServer.DAL.SQLiteHelper.GetSourceByHeader(headerList[0]);

            source = JsonConvert.DeserializeObject<MeterList>(data.JsonData);

            Console.WriteLine(data.JsonData);
        }
    }
}
