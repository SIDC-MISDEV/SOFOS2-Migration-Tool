﻿using MySql.Data.MySqlClient;
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

                    result = data == null ? 0 : Convert.ToInt32(data);
                }

                return result;
            }
            catch
            {

                throw;
            }
        }
        public string GetLatestTransactionReference(MySQLHelper conn, string module, string transactionType)
        {
            try
            {
                string result = "";
                conn.ArgSQLCommand = Query.GetLatestTransactionReference();
                conn.ArgSQLParam = new Dictionary<string, object>() { { "@transactionType", transactionType }, { "@module", module } };
                using (var dr = conn.MySQLExecuteReaderBeginTransaction())
                {
                    while (dr.Read())
                    {
                        result =  dr["reference"].ToString();
                    }
                }
                return result;
            }
            catch
            {

                throw;
            }
        }

        public string GetBIRSeries(MySQLHelper conn, string docuType)
        {
            try
            {
                string val = string.Empty,
                    prefix = string.Empty,
                    series = string.Empty;

                int prefixVal = 0;
                ulong seriesVal = 0;

                string query = string.Empty;

                conn.ArgSQLCommand = Query.GetBIRSeries();
                conn.ArgSQLParam = new Dictionary<string, object>() { { "@transtype", docuType } };
                using (var dr = conn.MySQLExecuteReaderBeginTransaction())
                {
                    while (dr.Read())
                    {
                        prefix = dr["prefix"] == DBNull.Value ? "001" : dr["prefix"].ToString();
                        series = dr["series"] == DBNull.Value ? "0000000001" : dr["series"].ToString();
                    }

                    if (series == "9999999999")
                    {
                        prefixVal = Convert.ToInt32(prefix) + 1;
                        seriesVal = 1;
                    }
                    else
                    {
                        prefixVal = Convert.ToInt32(prefix);
                        seriesVal = Convert.ToUInt64(series) + 1;
                    }

                    val = $"{prefixVal.ToString().PadLeft(3, '0')}-{seriesVal.ToString().PadLeft(10, '0')}";
                }

                return val;

            }
            catch
            {

                throw;
            }

        }

        public void UpdateBIRSeries(MySQLHelper conn, string transtype)
        {
            try
            {
                conn.ArgSQLCommand = Query.UpdateBIRSeries();
                conn.ArgSQLParam = new Dictionary<string, object>() { { "@transtype", transtype } };
                conn.ExecuteMySQL();
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
