using Energy.Common.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Utils
{
    public class LogDBHelper
    {
        public static void CreateLogDbTable()
        {
            CreateLogDB();
            CreateLogTable();
        }

        private static void CreateLogDB()
        {
            SQLiteHelper.CreateLocalDB("nlog.db");
        }

        private static void CreateLogTable()
        {
            string sql = @"create table log(
                               Timestamp text NULL,
                               Loglevel text NULL,
                               Logger text NULL,
                               Callsite text NULL,
                               Message text NULL
                        )";
            SQLiteHelper.CreateTableInDB("nlog.db", "Log",sql);
        }
    }
}
