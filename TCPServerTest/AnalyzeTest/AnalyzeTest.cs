using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Energy.Common.Entity;
using Energy.Common.DAL;
using System.Text;
using Energy.Analysis;

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

        [TestMethod]
        public void TestInsertData()
        {
            string connString = "Server=127.0.0.1;Port=3306;Database=energydb;Uid=root;Pwd=Fight4benben";
            List<DateTime> times = MySQLHelper.GetUnCalculatedDataTimeList(connString);

            List<OriginEnergyData> list = MySQLHelper.GetUnCalcedEnergyDataList(connString, times[0], times[1]);

            DateTime nextTime = times.Find(t => t > times[0]);


            List<OriginEnergyData> currentList = list.FindAll(data => data.Time == times[0]);
            List<OriginEnergyData> nextList = list.FindAll(data => data.Time == nextTime);

            List<CalcEnergyData> CalcedList = new List<CalcEnergyData>();

            //遍历当前currentList，根据主键内容取出nextList中的数据
            foreach (var item in currentList)
            {
                OriginEnergyData next = nextList.Find(data => data.BuildID == item.BuildID && data.MeterCode == item.MeterCode);

                if (next == null)
                    continue;

                if (next.Value == null || item.Value == null)
                    continue;

                if ((next.Value - item.Value) < 0)
                    continue;

                CalcedList.Add(new CalcEnergyData()
                {
                    BuildID = item.BuildID,
                    MeterCode = item.MeterCode,
                    Time = item.Time,
                    Value = next.Value - item.Value
                });
            }

            Console.WriteLine(CalcedList.Count);

            //生成SQL语句，批量插入
            StringBuilder builder = new StringBuilder();

            if (CalcedList.Count == 0)
            {
                Console.WriteLine(builder.ToString());
            }
            else
            {
                builder.Append("INSERT INTO t_ec_minutevalue(F_BuildID,F_MeterCode,F_Time,F_Value) VALUES");
                for (int i = 0; i < CalcedList.Count; i++)
                {
                    if (i == 0)
                        builder.Append(string.Format("('{0}','{1}','{2}',{3})", CalcedList[i].BuildID, CalcedList[i].MeterCode,
                            CalcedList[i].Time.ToString("yyyy-MM-dd HH:mm:ss"), CalcedList[i].Value));
                    else
                        builder.Append(string.Format(",('{0}','{1}','{2}',{3})", CalcedList[i].BuildID, CalcedList[i].MeterCode,
                            CalcedList[i].Time.ToString("yyyy-MM-dd HH:mm:ss"), CalcedList[i].Value));
                }

                builder.Append("  ON DUPLICATE KEY UPDATE F_Value=VALUES(F_Value);");
            }

            Console.WriteLine(builder.ToString());
        }

        [TestMethod]
        public void TestGenreateSQL()
        {
            string connString = "Server=127.0.0.1;Port=3306;Database=energydb;Uid=root;Pwd=Fight4benben";
            List<DateTime> times = MySQLHelper.GetUnCalculatedDataTimeList(connString);

            List<OriginEnergyData> list = MySQLHelper.GetUnCalcedEnergyDataList(connString, times[0], times[1]);

            DateTime nextTime = times.Find(t => t > times[0]);


            List<OriginEnergyData> currentList = list.FindAll(data => data.Time == times[0]);
            List<OriginEnergyData> nextList = list.FindAll(data => data.Time == nextTime);

            List<CalcEnergyData> CalcedList = new List<CalcEnergyData>();

            //遍历当前currentList，根据主键内容取出nextList中的数据
            foreach (var item in currentList)
            {
                OriginEnergyData next = nextList.Find(data => data.BuildID == item.BuildID && data.MeterCode == item.MeterCode);

                if (next == null)
                    continue;

                if (next.Value == null || item.Value == null)
                    continue;

                if ((next.Value - item.Value) < 0)
                    continue;

                CalcedList.Add(new CalcEnergyData()
                {
                    BuildID = item.BuildID,
                    MeterCode = item.MeterCode,
                    Time = item.Time,
                    Value = next.Value - item.Value
                });
            }

            Console.WriteLine("Minute:"+Runtime.GenerateMinuteSQL(CalcedList));
            Console.WriteLine("Hour:" + Runtime.GenreateHourSQL(CalcedList));
            Console.WriteLine("Day:" + Runtime.GenreateDaySQL(CalcedList));
        }
    }
}
