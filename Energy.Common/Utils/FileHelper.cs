using Energy.Common.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Utils
{
    public class FileHelper
    {
        /// <summary>
        /// 当前存储文件的实时目录
        /// </summary>
        public static string RealTimeDataPath = $@"{AppDomain.CurrentDomain.BaseDirectory}\RealTime";
        public static string HistoryDataPath = $@"{AppDomain.CurrentDomain.BaseDirectory}\History";

        public static void CheckDirExistAndCreate(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }

        public static List<string> GetFileOrder()
        {
            string[] files = Directory.GetFiles(RealTimeDataPath);


            var list = files.Select(s => s.Replace(RealTimeDataPath+"\\", "")).OrderBy(file => ParseTimestamp(file)).ToList();

            return list.Take(1000).ToList();
        }

        public static void DeleteDataFile(string fileName)
        {
            try
            {
                File.Delete(RealTimeDataPath+"\\" +fileName);
            }
            catch
            { 
            }
        }

        public static MeterList AnalysiaMeterListFromFile(string fileName)
        {
            try
            {
                string fullPath = RealTimeDataPath + "\\" + fileName;

                string jsonText = File.ReadAllText(fullPath);

                MeterList meterList = JsonConvert.DeserializeObject<MeterList>(jsonText);

                string dateStr = meterList.CollectTime.ToString("yyyyMMdd");

                SaveFile2HistoryDir(fileName, dateStr, jsonText);
                return meterList;
            }
            catch { }

            return null;
            
        }

        private static DateTime ParseTimestamp(string fileName)
        {
            string[] parts = fileName.Split('_');

            if (parts.Length > 3)
            {
                try
                {
                    return DateTime.ParseExact(parts[2], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch
                {
                    return new DateTime(1900, 1, 1);
                }
            }
            else
            {
                return new DateTime(1900, 1, 1);
            }
        }

        /// <summary>
        /// 存储数据到文件中
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static bool SaveJsonData2File(string fileName, string source, out string message)
        {
            string fullPath = RealTimeDataPath + "\\" + fileName;

            try
            {
                File.WriteAllText(fullPath, source);

                message = $"{fileName} write success,";

                return true;
            }
            catch (Exception er)
            {
                message = $"{fileName} write text failed. {er.Message}.";
            }

            return false;
        }

        public static bool SaveFile2HistoryDir(string fileName, string date, string text)
        {
            string dir = HistoryDataPath + "\\" + date;
            
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            try
            {
                File.WriteAllText(dir + "\\" + fileName, text);
                return true;
            }
            catch
            { 

            }

            return false;
        }

        
    }
}
