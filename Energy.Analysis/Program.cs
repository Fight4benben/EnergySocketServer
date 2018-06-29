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
            ShowLog("数据解析应用程序启动，线程ID：{0}.", Thread.CurrentThread.ManagedThreadId.ToString());

            ShowLog("检查是否包含settings文件...");
            SettingsHelper.CreateSettingsDBTable();

            ShowLog("检查是否配置MySql连接字符串,服务器地址是否存在？" );
            if (SettingsHelper.GetSettingValue("MySqlServer") == "")
                SettingsHelper.SetSettingValue("MySqlServer","127.0.0.1");
            ShowLog("检查是否配置MySql连接字符串,服务器端口是否存在？");
            if (SettingsHelper.GetSettingValue("MySqlPort") == "")
                SettingsHelper.SetSettingValue("MySqlPort", "3306");
            ShowLog("检查是否配置MySql连接字符串,数据库名称是否存在？");
            if (SettingsHelper.GetSettingValue("DatabaseNameDB") == "")
                SettingsHelper.SetSettingValue("DatabaseNameDB", "energydb");
            ShowLog("检查是否配置MySql连接字符串,用户名是否存在？");
            if (SettingsHelper.GetSettingValue("MySqlUid") == "")
                SettingsHelper.SetSettingValue("MySqlUid", "root");
            ShowLog("检查是否配置MySql连接字符串,密码是否存在？");
            if (SettingsHelper.GetSettingValue("MySqlPwd") == "")
                SettingsHelper.SetSettingValue("MySqlPwd", "Fight4benben");

            Thread transThread = new Thread(SaveDataToMysql);

            transThread.Start();

            ShowLog("退出应用程序请输入quit.");

            string endString;
            while ((endString = Console.ReadLine()) != "quit")
            {

            }

        }

        /// <summary>
        /// 一直运行，将有新的数据存储到MySql数据库中
        /// </summary>
        private static void SaveDataToMysql()
        {
            ShowLog("数据转存线程启动，线程ID：{0}.",Thread.CurrentThread.ManagedThreadId.ToString());

            while (true)
            {
                if (DateTime.Now.Minute % 5 == 1)
                {
                    ShowLog("检查是否有需要存储的数据？");
                    List<SourceDataHeader> headerList = Energy.Common.DAL.SQLiteHelper.GetUnStoreList();
                    if (headerList.Count < 1000 && headerList.Count >= 0)
                        ShowLog("共有{0}个报文需要存储到数据中.", headerList.Count.ToString());
                    else
                        ShowLog("当前需要存储1000个报文.");

                    ShowLog("开始存储文件...");
                    foreach (SourceDataHeader header in headerList)
                    {
                        SourceData source = SQLiteHelper.GetSourceByHeader(header);

                        MeterList meterList = JsonConvert.DeserializeObject<MeterList>(source.JsonData);

                        try
                        {
                            SaveDataFromSqliteToMySql.ExecuteInsertTransactions(meterList, header);
                        }
                        catch (Exception e)
                        {
                            ShowLog("错误信息:{0}", e.Message);
                        }

                    }
                    ShowLog("完成当前周期文件的存储。");

                    System.Threading.Thread.Sleep(1000 * 60);
                }
                else
                {
                    ShowLog("当前无数据需要处理，进行下一个周期的数据扫描...");
                    System.Threading.Thread.Sleep(1000*60); 
                }

                
            }

        }

        /// <summary>
        /// 计算能耗值
        /// </summary>
        private static void CalculateEnergyData()
        {
            ShowLog("能耗计算线程已启动。");
            while (true)
            {
                if (DateTime.Now.Minute % 5 == 2)
                {
                    List<DateTime> times = MySQLHelper.GetUnCalculatedDataTimeList(Runtime.MySqlConnectString);

                    //获取

                }
            }
        }

        private static void ShowLog(string text,params string[] values)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")+"<"+Thread.CurrentThread.ManagedThreadId.ToString()+"> -> "+text,values);
        }
    }
}
