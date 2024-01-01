using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Energy.Common.Entity;
using Energy.Common.Utils;

namespace TCPServerTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMeterObject()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"+
                        "<root>"+
                        "<common>"+
                        "<building_id>000001G004</building_id>"+
                        "<gateway_id>TC888</gateway_id>"+
                        "<type>energy_data</type>"+
                        "</common>"+
                        "<data operation=\"report\">"+
                        "<time>20171220122000</time>"+
                        "<energy_items/>"+
                        "<meters total=\"1\">"+
                        "<meter id=\"T201003\" name=\"A\">"+
                        "<function id=\"CT\" error=\"1\">0.0000</function>"+
                        "<function id=\"Ua\" error=\"1\">0.0000</function>"+
                        "<function id=\"Ub\" error=\"1\">0.0000</function>"+
                        "<function id=\"Uc\" error=\"1\">0.0000</function>"+
                        "<function id=\"Ia\" error=\"1\">0.0000</function>"+
                        "<function id=\"Ib\" error=\"1\">0.0000</function>"+
                        "<function id=\"Ic\" error=\"1\">0.0000</function>"+
                        "<function id=\"Pa\" error=\"1\">0.0000</function>"+
                        "<function id=\"Pb\" error=\"1\">0.0000</function>"+
                        "<function id=\"Pc\" error=\"1\">0.0000</function>"+
                        "<function id=\"P\" error=\"\"></function>"+
                        "<function id=\"Q\" error=\"\"></function>"+
                        "<function id=\"S\" error=\"\"></function>"+
                        "<function id=\"PF\" error=\"1\">0.0000</function>"+
                        "<function id=\"EPI\" error=\"1\">0.0000</function>"+
                        "<function id=\"EPE\" error=\"1\">0.0000</function>"+
                        "<function id=\"EQL\" error=\"1\">0.0000</function>"+
                        "<function id=\"EQC\" error=\"1\">0.0000</function>"+
                        "</meter>"+
                        "</meters>"+
                        "</data>"+
                        "</root>";
            //string meters = XMLHelper.GetMeterList(xml);
            //Console.WriteLine(meters);
        }
    }
}
