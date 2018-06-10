using System;
using Energy.Common.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TCPServerTest.SettingsTest
{
    [TestClass]
    public class SettingsTest
    {
        [TestMethod]
        public void TestCreateDBndTable()
        {
            SettingsHelper.CreateSettingsDBTable();

            Console.WriteLine("创建表与库成功！");
        }
        [TestMethod]
        public void TestInsertValue()
        {
            bool result = SettingsHelper.SetSettingValue("Server","127.0.0.1");

            if (result)
                Console.WriteLine("插入数据成功");
            else
                Console.WriteLine("插入失败");
        }

        [TestMethod]
        public void TestGetValueByKey()
        {
            string result = SettingsHelper.GetSettingValue("");

            Console.WriteLine("Server:{0}",result);
        }

    }
}
