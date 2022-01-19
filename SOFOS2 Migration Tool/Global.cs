using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool
{
    public class Global
    {
        private static string server = Properties.Settings.Default.HOST;
        private static string sourceDB = Properties.Settings.Default.SOURCE_DB;
        private static string destinationDB = Properties.Settings.Default.DESTINATION_DB;
        private static string userName = Properties.Settings.Default.USERNAME;
        private static string password = Properties.Settings.Default.PASSWORD;

        public static string SourceDatabase = $"Server={server};Database={sourceDB};Username={userName};Password={password};";
        public static string DestinationDatabase = $"Server={server};Database={destinationDB};Username={userName};Password={password};";

        public static string MainSegment = string.Empty;
        public static string BusinessSegment = string.Empty;
        public static string BranchCode = string.Empty;
        public static string WarehouseCode = string.Empty;
        public static string BranchName = string.Empty;


        /// <summary>
        /// Get latest transaction number of header table in sofos2
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int GetLatestTransNum(string tableName, string fieldName)
        {
            try
            {
                int result = 0;
                string query = string.Empty;

                query = $@"SELECT {fieldName} FROM {tableName} ORDER BY {fieldName} DESC LIMIT 1;";


                using (var conn = new MySQLHelper(DestinationDatabase, new StringBuilder(query)))
                {
                    var data = conn.GetMySQLScalar();

                    result = data == null ? 1 : Convert.ToInt32(data);
                }

                return result;
            }
            catch
            {

                throw;
            }
        }

        public void InitializeBranch()
        {
            try
            {
                int result = 0;
                string query = string.Empty;

                query = $@"SELECT main_segment, business_segment, branchCode, branchName, whse FROM business_segments;";


                using (var conn = new MySQLHelper(SourceDatabase, new StringBuilder(query)))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            MainSegment = dr["main_segment"].ToString();
                            BusinessSegment = dr["business_segment"].ToString();
                            BranchCode = dr["branchCode"].ToString();
                            BranchName = dr["branchName"].ToString();
                            WarehouseCode = dr["whse"].ToString();
                        }
                    }
                }
            }
            catch
            {

                throw;
            }
        }
    }
}
