using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Energy.Common.DAL;
using Energy.Common.Entity;
using Energy.Common.Utils;
using Newtonsoft.Json;

namespace Energy.Analysis
{
    class Program
    {
        public static void Main(string[] args)
        {
            LogDBHelper.CreateLogDbTable();

            Runtime.m_Logger.Info("数据解析应用程序启动，线程ID：{0}.", Thread.CurrentThread.ManagedThreadId.ToString());
            ShowLog("数据解析应用程序启动，线程ID：{0}.", Thread.CurrentThread.ManagedThreadId.ToString());

            Runtime.m_Logger.Info("检查是否包含settings文件...");
            ShowLog("检查是否包含settings文件...");

            SettingsHelper.CreateSettingsDBTable();

            ShowLog("检查是否配置MySql连接字符串,服务器地址是否存在？" );
            Runtime.m_Logger.Info("检查是否配置MySql连接字符串,服务器地址是否存在？");
            if (SettingsHelper.GetSettingValue("MySqlServer") == "")
                SettingsHelper.SetSettingValue("MySqlServer","127.0.0.1");
            ShowLog("检查是否配置MySql连接字符串,服务器端口是否存在？");
            Runtime.m_Logger.Info("检查是否配置MySql连接字符串,服务器端口是否存在？");
            if (SettingsHelper.GetSettingValue("MySqlPort") == "")
                SettingsHelper.SetSettingValue("MySqlPort", "3306");
            ShowLog("检查是否配置MySql连接字符串,数据库名称是否存在？");
            Runtime.m_Logger.Info("检查是否配置MySql连接字符串,数据库名称是否存在？");
            if (SettingsHelper.GetSettingValue("DatabaseNameDB") == "")
                SettingsHelper.SetSettingValue("DatabaseNameDB", "energydb");
            ShowLog("检查是否配置MySql连接字符串,用户名是否存在？");
            Runtime.m_Logger.Info("检查是否配置MySql连接字符串,用户名是否存在？");
            if (SettingsHelper.GetSettingValue("MySqlUid") == "")
                SettingsHelper.SetSettingValue("MySqlUid", "root");
            ShowLog("检查是否配置MySql连接字符串,密码是否存在？");
            Runtime.m_Logger.Info("检查是否配置MySql连接字符串,密码是否存在？");
            if (SettingsHelper.GetSettingValue("MySqlPwd") == "")
                SettingsHelper.SetSettingValue("MySqlPwd", "Fight4benben");

            //Console.WriteLine("TEST:"+ Runtime.MySqlConnectString);
            Runtime.MysqlConn = Runtime.MySqlConnectString;

            Thread transThread = new Thread(SaveDataToMysql);
            Thread calcThread = new Thread(CalculateEnergyData);
            Thread deleteThread = new Thread(DeleteProcessedData);

            transThread.Start();
            ShowLog("已启动转发线程");
            Runtime.m_Logger.Info("已启动转发线程");
            calcThread.Start();
            ShowLog("已启动计算线程");
            Runtime.m_Logger.Info("已启动计算线程");
            deleteThread.Start();
            ShowLog("已启动清除线程");
            Runtime.m_Logger.Info("已启动清除线程");

            ShowLog("退出应用程序请输入quit.");
            Runtime.m_Logger.Info("退出应用程序请输入quit.");

            //string endString;

            //while ((endString = Console.ReadLine()) != "quit")
            //{

            //}

            while (true)
            {
                Thread.Sleep(1);
            }

        }

        /// <summary>
        /// 一直运行，将有新的数据存储到MySql数据库中
        /// </summary>
        private static void SaveDataToMysql()
        {
            ShowLog("数据转存线程启动，线程ID：{0}.",Thread.CurrentThread.ManagedThreadId.ToString());
            Runtime.m_Logger.Info("数据转存线程启动，线程ID：{0}.", Thread.CurrentThread.ManagedThreadId.ToString());

            while (true)
            {
                if (DateTime.Now.Minute % 5 == 4)
                {
                    ShowLog("检查是否有需要存储的数据？");
                    Runtime.m_Logger.Info("检查是否有需要存储的数据？");
                    //Runtime.m_Logger.Info("获取连接字符串："+Runtime.MySqlConnectString);
                    //List<SourceDataHeader> headerList = Energy.Common.DAL.SQLiteHelper.GetUnStoreList();
                    //需要将此处Sqlite 替换为mysql
                    List<SourceDataHeader> headerList = Energy.Common.DAL.MySQLHelper.GetUnStoreList(Runtime.MysqlConn);
                    if (headerList.Count < 1000 && headerList.Count >= 0)
                    {
                        ShowLog("共有{0}个报文需要存储到数据中.", headerList.Count.ToString());
                        Runtime.m_Logger.Info("共有{0}个报文需要存储到数据中.", headerList.Count.ToString());
                    }   
                    else
                    {
                        ShowLog("当前需要存储1000个报文.");
                        Runtime.m_Logger.Info("当前需要存储1000个报文.");
                    }
                        

                    ShowLog("开始存储文件...");
                    Runtime.m_Logger.Info("开始存储文件...");
                    foreach (SourceDataHeader header in headerList)
                    {
                        ShowLog("开始存储{0}.", header.CollectTime + "_" + header.BuildID + "_" + header.GatewayID);
                        //SourceData source = SQLiteHelper.GetSourceByHeader(header);

                        SourceData source = MySQLHelper.GetSourceByHeader(header, Runtime.MysqlConn);

                        if (source == null)
                            continue;

                        MeterList meterList = JsonConvert.DeserializeObject<MeterList>(source.JsonData);

                        try
                        {
                            SaveDataFromSqliteToMySql.ExecuteInsertTransactions(meterList, header);
                        }
                        catch (Exception e)
                        {
                            ShowLog("错误信息:{0},附加信息：{1}.", e.Message,header.CollectTime+"_"+header.BuildID+"_"+header.GatewayID);
                            ShowLog("错误堆栈:{0}.", e.StackTrace);
                            Runtime.m_Logger.Error("错误信息:{0}", e.Message);
                        }

                    }
                    ShowLog("完成当前周期文件的存储。");
                    Runtime.m_Logger.Info("完成当前周期文件的存储。");

                    Thread.Sleep(1000 * 60);
                }
                else
                {
                    ShowLog("当前无数据需要处理，进行下一个周期的数据扫描...");
                    Runtime.m_Logger.Info("当前无数据需要处理，进行下一个周期的数据扫描...");
                    Thread.Sleep(1000*60); 
                }

                
            }

        }

        /// <summary>
        /// 计算能耗值
        /// </summary>
        private static void CalculateEnergyData()
        {
            ShowLog("能耗计算线程已启动。");
            Runtime.m_Logger.Info("能耗计算线程已启动。");
            while (true)
            {

                if (DateTime.Now.Minute % 5 == 2)
                {
                    try
                    {
                        int delCnt = MySQLHelper.DeleteOriginData(Runtime.MysqlConn);

                        if (delCnt == -10001)
                        {
                            Runtime.m_Logger.Error("删除已计算数据失败。");
                        }

                        List<DateTime> times = new List<DateTime>();
                    
                        times.AddRange( MySQLHelper.GetUnCalculatedDataTimeList(Runtime.MysqlConn));
                    
                        //获取该时间段内未处理的数据
                        if (times.Count <= 1)
                        {
                            Thread.Sleep(1000 * 60);
                            continue;
                        }

                        List<OriginEnergyData> list = MySQLHelper.GetUnCalcedEnergyDataList(Runtime.MysqlConn, times[0],times[times.Count-1]);

                        foreach (DateTime time in times)
                        {
                            DateTime nextTime = times.Find(t => t>time);

                            if (nextTime == null)
                                continue;

                            List<OriginEnergyData> currentList = list.FindAll(data=>data.Time == time);
                            List<OriginEnergyData> nextList = list.FindAll(data => data.Time == nextTime);

                            if (nextList.Count == 0)
                            {
                                continue;
                            }

                            List<CalcEnergyData> CalcedList = new List<CalcEnergyData>();

                            //遍历当前currentList，根据主键内容取出nextList中的数据
                            foreach (var item in currentList)
                            {
                                OriginEnergyData next = nextList.Find(data=>data.BuildID == item.BuildID && data.MeterCode == item.MeterCode);

                                if (next == null)
                                    continue;

                                if (next.Value == null || item.Value == null)
                                    continue;

                                if ((next.Value - item.Value) < 0)
                                    continue;

                                CalcedList.Add(new CalcEnergyData() {
                                    BuildID = item.BuildID,
                                    MeterCode = item.MeterCode,
                                    Time = item.Time,
                                    Value = next.Value-item.Value
                                });
                            }
                            //批量插入CalcedList到数据库T_EC_XXXValue数据库表
                            List<string> sqls = new List<string>();
                            sqls.Add(Runtime.GenerateMinuteSQL(CalcedList));
                            sqls.Add(Runtime.GenreateHourSQL(CalcedList));
                            sqls.Add(Runtime.GenreateDaySQL(CalcedList));
                            sqls.Add(Runtime.GenerateOrigStateSQL(time));

                            bool result = MySQLHelper.InsertDataTable(Runtime.MySqlConnectString,sqls);

                            if (result)
                            {
                                ShowLog(string.Format("能耗值计算成功，数据时间为{0}", time.ToString("yyyy-MM-dd HH:mm:ss")));
                                Runtime.m_Logger.Info(string.Format("能耗值计算成功，数据时间为{0}", time.ToString("yyyy-MM-dd HH:mm:ss")));
                            }
                            else
                            {
                                ShowLog(string.Format("能耗值计算失败，数据时间为{0}", time.ToString("yyyy-MM-dd HH:mm:ss")));
                                Runtime.m_Logger.Info(string.Format("能耗值计算失败，数据时间为{0}", time.ToString("yyyy-MM-dd HH:mm:ss")));
                            }
                            
                        }

                    }
                    catch(Exception er)
                    {
                        Runtime.m_Logger.Error(er.Message);
                        Runtime.m_Logger.Error(er.StackTrace);
                        
                        Thread.Sleep(1000 * 60);
                        continue;
                    }
                }

                ShowLog("当前未到能耗计算时间，请稍后...");
                Runtime.m_Logger.Info("当前未到能耗计算时间，请稍后...");
                System.Threading.Thread.Sleep(1000*60);
            }

        }

        /// <summary>
        /// 线程：定时删除TempData中的存入数据库的数据记录
        /// </summary>
        private static void DeleteProcessedData()
        {
            ShowLog("启动定期清除线程！");
            Runtime.m_Logger.Info("启动定期清除线程！");

            while (true)
            {
                if (DateTime.Now.Minute % 15 == 3)
                {

                    //int count = SQLiteHelper.DeleteStoredGatewayDataFromDB();
                    int count = MySQLHelper.DeleteStoredGatewayDataFromDB(Runtime.MysqlConn);

                    ShowLog("本次共清理{0}条数据", count.ToString());
                    Runtime.m_Logger.Info("本次共清理{0}条数据", count.ToString());

                    Thread.Sleep(1000 * 60);
                }

                Thread.Sleep(1);
            }
        }

        private static void ShowLog(string text,params string[] values)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")+"<"+Thread.CurrentThread.ManagedThreadId.ToString()+"> -> "+text,values);
        }
    }
}
