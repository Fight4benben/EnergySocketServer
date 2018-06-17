using Energy.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Energy.Common.DAL;
using System.Data.SQLite;

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
            
            List<string> insertSqls = Energy.Common.DAL.MySQLHelper.GetInsertSqls(GetEnergyData(meters),GetVoltageData(meters),GetCurrentData(meters));

            int count = 0;
            MySqlConnection connection = new MySqlConnection(Runtime.MySqlConnectString);
            SQLiteConnection sqliteConnection = new SQLiteConnection("Data Source=TempData;Version=3;");
            SQLiteCommand sQLiteCommand = new SQLiteCommand(SQLiteHelper.GetUpdateStatusSQL(header),sqliteConnection);

            connection.Open();
            sqliteConnection.Open();

            SQLiteTransaction sqliteTrans = sqliteConnection.BeginTransaction();
            MySqlTransaction mySqlTrans = connection.BeginTransaction();
            try
            {
                foreach (var sql in insertSqls)
                {
                    MySqlCommand mySqlCommand = new MySqlCommand(sql, connection, mySqlTrans);

                    count += mySqlCommand.ExecuteNonQuery();
                }
                count += sQLiteCommand.ExecuteNonQuery();

                sqliteTrans.Commit();
                mySqlTrans.Commit();

                Console.WriteLine("事务执行成功，共影响{0}条数据。",count);
            }
            catch (Exception e)
            {
                Console.WriteLine("事务执行失败，正在回滚操作，失败内容为：{0}。",e.Message);
                sqliteTrans.Rollback();
                mySqlTrans.Rollback();
                Console.WriteLine("事务回滚成功！");
            }
            finally
            { 
                sQLiteCommand.Connection.Close();
                connection.Close();
                Console.WriteLine("已关闭数据库连接！");
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
                MeterParam param = item.MeterParams.Find(meterParam => meterParam.ParamName.ToLower() == "EPI".ToLower() || meterParam.ParamName == "LJLL".ToLower()
                                                || meterParam.ParamName.ToLower() == "LLHeat".ToLower());

                if (param == null)
                    continue;

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

                MeterParam i = item.MeterParams.Find(m => m.ParamName.ToLower() == "U".ToLower());
                MeterParam ia = item.MeterParams.Find(m => m.ParamName.ToLower() == "Ua".ToLower());
                MeterParam ib = item.MeterParams.Find(m => m.ParamName.ToLower() == "Ub".ToLower());
                MeterParam ic = item.MeterParams.Find(m => m.ParamName.ToLower() == "Uc".ToLower());

                if (i != null)
                    current.I = i.ParamValue;

                if (ia != null)
                    current.Ia = ia.ParamValue;

                if (ib != null)
                    current.Ib = ib.ParamValue;

                if (ic != null)
                    current.Ic = ic.ParamValue;

                currentDataList.Add(current);
            }

            return currentDataList;
        }


    }
}
