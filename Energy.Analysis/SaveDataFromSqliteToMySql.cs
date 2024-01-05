using Energy.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Energy.Common.DAL;
using System.Data.SQLite;
using System.Threading;

namespace Energy.Analysis
{
    public class SaveDataFromSqliteToMySql
    {
        /// <summary>
        /// 事务：将实时数据按照要求存入mysql数据库中，存储成功，则更新原始数据
        /// </summary>
        /// <param name="meters"></param>
        /// <param name="header"></param>
        public static void ExecuteInsertTransactions(MeterList meters,SourceDataHeader header)
        {
            // 先移除不使用,GetVoltageData(meters),GetCurrentData(meters),GetPowerData(meters),GetBaseElecData(meters)
            object[] list = new object[]{ GetEnergyData(meters) };
            List<string> insertSqls = Energy.Common.DAL.MySQLHelper.GetInsertSqls(list);

            int count = 0;
            MySqlConnection connection = new MySqlConnection(Runtime.MysqlConn);
            //SQLiteConnection sqliteConnection = new SQLiteConnection("Data Source=TempData;Version=3;");
            //SQLiteCommand sQLiteCommand = new SQLiteCommand(SQLiteHelper.GetUpdateStatusSQL(header),sqliteConnection);
            // MySqlCommand updateCommand = new MySqlCommand(MySQLHelper.GetUpdateStatusSQL(header),connection);

            connection.Open();
            //sqliteConnection.Open();

            //SQLiteTransaction sqliteTrans = sqliteConnection.BeginTransaction();
            MySqlTransaction mySqlTrans = connection.BeginTransaction();
            try
            {
                foreach (var sql in insertSqls)
                {
                    MySqlCommand mySqlCommand = new MySqlCommand(sql, connection, mySqlTrans);

                    count += mySqlCommand.ExecuteNonQuery();
                }
                // count += updateCommand.ExecuteNonQuery();

                //sqliteTrans.Commit();
                mySqlTrans.Commit();

                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "<" + Thread.CurrentThread.ManagedThreadId.ToString() + "> -> " + "事务执行成功，共影响{0}条数据。",count);
            }
            catch (Exception e)
            {
                Runtime.m_Logger.Error(e,e.Message); 
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "<" + Thread.CurrentThread.ManagedThreadId.ToString() + "> -> " + "事务执行失败，正在回滚操作，失败内容为：{0}。",e.Message);
                //sqliteTrans.Rollback();
                mySqlTrans.Rollback();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    // updateCommand.ExecuteNonQuery();
                    mySqlTrans.Commit();
                }

                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "<" + Thread.CurrentThread.ManagedThreadId.ToString() + "> -> " + "事务回滚成功！");
            }
            finally
            {
                
                //sQLiteCommand.Connection.Close();
                connection.Close();
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "<" + Thread.CurrentThread.ManagedThreadId.ToString() + "> -> " + "已关闭数据库连接！");
            }
        }
        /// <summary>
        /// 获取能耗值List
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<OriginEnergyData> GetEnergyData(MeterList data)
        {
            List<OriginEnergyData> energyDataList = new List<OriginEnergyData>();
            foreach (var item in data.Meters)
            {
                MeterParam param = item.MeterParams.Find(meterParam => meterParam.ParamName.ToLower() == "EPI".ToLower() || meterParam.ParamName.ToLower() == "LJLL".ToLower()
                                                || meterParam.ParamName.ToLower() == "LLHeat".ToLower()||meterParam.ParamName == "01");

                if (param == null)
                    continue;

                //判断param.ParamError是否为错误状态，如果为错误状态则表示当前仪表通讯中断

                energyDataList.Add(new OriginEnergyData()
                {
                    BuildID = data.BuildId,
                    MeterCode = item.MeterId,
                    Time = data.CollectTime,
                    Value = param.ParamValue,
                    Calced = false
                });
            }

            return energyDataList;
        }

        /// <summary>
        /// 获取电压数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<VoltageData> GetVoltageData(MeterList data)
        {
            List<VoltageData> voltageDataList = new List<VoltageData>();

            foreach (var item in data.Meters)
            {
                VoltageData voltage = new VoltageData();
                voltage.BuildID = data.BuildId;
                voltage.MeterCode = item.MeterId;
                voltage.Time = data.CollectTime;

                MeterParam u = item.MeterParams.Find(m=>m.ParamName.ToLower() == "U".ToLower());
                MeterParam ua = item.MeterParams.Find(m => m.ParamName.ToLower() == "Ua".ToLower());
                MeterParam ub = item.MeterParams.Find(m => m.ParamName.ToLower() == "Ub".ToLower());
                MeterParam uc = item.MeterParams.Find(m => m.ParamName.ToLower() == "Uc".ToLower());
                MeterParam uab = item.MeterParams.Find(m => m.ParamName.ToLower() == "Uab".ToLower());
                MeterParam ubc = item.MeterParams.Find(m => m.ParamName.ToLower() == "Ubc".ToLower());
                MeterParam uca = item.MeterParams.Find(m => m.ParamName.ToLower() == "Uca".ToLower());

                if (u != null)
                    voltage.U = u.ParamValue;

                if (ua != null)
                    voltage.Ua = ua.ParamValue;

                if (ub != null)
                    voltage.Ub = ub.ParamValue;

                if (uc != null)
                    voltage.Uc = uc.ParamValue;

                if (uab != null)
                    voltage.Uab = uab.ParamValue;

                if (ubc != null)
                    voltage.Ubc = ubc.ParamValue;

                if (uca != null)
                    voltage.Uca = uca.ParamValue;

                if(u != null || ua != null || ub != null || uc != null || uab != null || ubc != null || uca != null)
                    voltageDataList.Add(voltage);
            }

            return voltageDataList;
        }

        /// <summary>
        /// 获取电流数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<CurrentData> GetCurrentData(MeterList data)
        {
            List<CurrentData> currentDataList = new List<CurrentData>();

            foreach (var item in data.Meters)
            {
                CurrentData current = new CurrentData();
                current.BuildID = data.BuildId;
                current.MeterCode = item.MeterId;
                current.Time = data.CollectTime;

                MeterParam i = item.MeterParams.Find(m => m.ParamName.ToLower() == "I".ToLower());
                MeterParam ia = item.MeterParams.Find(m => m.ParamName.ToLower() == "Ia".ToLower());
                MeterParam ib = item.MeterParams.Find(m => m.ParamName.ToLower() == "Ib".ToLower());
                MeterParam ic = item.MeterParams.Find(m => m.ParamName.ToLower() == "Ic".ToLower());

                if (i != null)
                    current.I = i.ParamValue;

                if (ia != null)
                    current.Ia = ia.ParamValue;

                if (ib != null)
                    current.Ib = ib.ParamValue;

                if (ic != null)
                    current.Ic = ic.ParamValue;

                if(i != null || ia != null || ib != null || ic != null)
                     currentDataList.Add(current);
            }

            return currentDataList;
        }

        /// <summary>
        /// 解析功率数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<PowerData> GetPowerData(MeterList data)
        {
            List<PowerData> powerDataList = new List<PowerData>();

            foreach (var item in data.Meters)
            {
                PowerData power = new PowerData();
                power.BuildID = data.BuildId;
                power.MeterCode = item.MeterId;
                power.Time = data.CollectTime;

                MeterParam p = item.MeterParams.Find(m => m.ParamName.ToLower() == "P".ToLower());
                MeterParam pa = item.MeterParams.Find(m => m.ParamName.ToLower() == "Pa".ToLower());
                MeterParam pb = item.MeterParams.Find(m => m.ParamName.ToLower() == "Pb".ToLower());
                MeterParam pc = item.MeterParams.Find(m => m.ParamName.ToLower() == "Pc".ToLower());

                MeterParam q = item.MeterParams.Find(m => m.ParamName.ToLower() == "Q".ToLower());
                MeterParam qa = item.MeterParams.Find(m => m.ParamName.ToLower() == "Qa".ToLower());
                MeterParam qb = item.MeterParams.Find(m => m.ParamName.ToLower() == "Qb".ToLower());
                MeterParam qc = item.MeterParams.Find(m => m.ParamName.ToLower() == "Qc".ToLower());

                MeterParam s = item.MeterParams.Find(m => m.ParamName.ToLower() == "S".ToLower());
                MeterParam sa = item.MeterParams.Find(m => m.ParamName.ToLower() == "Sa".ToLower());
                MeterParam sb = item.MeterParams.Find(m => m.ParamName.ToLower() == "Sb".ToLower());
                MeterParam sc = item.MeterParams.Find(m => m.ParamName.ToLower() == "Sc".ToLower());

                if (p != null)
                    power.P = p.ParamValue;
                if (pa != null)
                    power.Pa = pa.ParamValue;
                if (pb != null)
                    power.Pb = pb.ParamValue;
                if (pc != null)
                    power.Pc = pc.ParamValue;

                if (q != null)
                    power.Q = q.ParamValue;
                if (qa != null)
                    power.Qa = qa.ParamValue;
                if (qb != null)
                    power.Qb = qb.ParamValue;
                if (qc != null)
                    power.Qc = qc.ParamValue;

                if (s != null)
                    power.S = s.ParamValue;
                if (sa != null)
                    power.Sa = sa.ParamValue;
                if (sb != null)
                    power.Sb = sb.ParamValue;
                if (sc != null)
                    power.Sc = sc.ParamValue;

                if(p != null || pa != null || pb != null || pc != null ||
                    qa != null || qb != null ||qc != null || q != null ||
                    s != null || sa != null || sb != null || sc != null)
                    powerDataList.Add(power);
            }

            return powerDataList;
        }

        /// <summary>
        /// 解析PF，Fr参数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<BaseElecData> GetBaseElecData(MeterList data)
        {
            List<BaseElecData> baseElecDataList = new List<BaseElecData>();

            foreach (var item in data.Meters)
            {
                BaseElecData baseElec = new BaseElecData();
                baseElec.BuildID = data.BuildId;
                baseElec.MeterCode = item.MeterId;
                baseElec.Time = data.CollectTime;

                MeterParam pf = item.MeterParams.Find(m => m.ParamName.ToLower() == "PF".ToLower());
                MeterParam fr = item.MeterParams.Find(m => m.ParamName.ToLower() == "Fr".ToLower());

                if (pf != null)
                    baseElec.PF = pf.ParamValue;

                if (fr != null)
                    baseElec.Fr = fr.ParamValue;

                if(pf != null || fr != null)
                    baseElecDataList.Add(baseElec);
            }

            return baseElecDataList;
        }


    }
}
