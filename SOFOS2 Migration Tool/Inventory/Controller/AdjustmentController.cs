using SOFOS2_Migration_Tool.Helper;
using SOFOS2_Migration_Tool.Inventory.Model;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Inventory.Controller
{
    public class AdjustmentController
    {
        string transType = "IA";
        string dropSitePath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "LOGS");
        string folder = "Inventory/";

        #region Public Methods

        #region GET

        public List<Adjustment> GetAdjustmentHeader(string date)
        {
            try
            {
                var result = new List<Adjustment>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transtype", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, AdjustmentQuery.GetAdjustmentQuery(AdjustmentEnum.AdjustmentHeader), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new Adjustment
                            {
                                TransDate = dr["TransDate"].ToString(),
                                TransType = dr["TransType"].ToString(),
                                Reference = dr["Reference"].ToString(),
                                Crossreference = dr["Crossreference"].ToString(),
                                Total = Convert.ToDecimal(dr["Total"]),
                                WarehouseCode = dr["WarehouseCode"].ToString(),
                                Remarks = dr["Remarks"].ToString(),
                                Cancelled = Convert.ToBoolean(dr["Cancelled"]),
                                Status = dr["Status"].ToString(),
                                SystemDate = dr["SystemDate"].ToString(),
                                IdUser = dr["IdUser"].ToString(),
                                TerminalNo = dr["TerminalNo"].ToString(),

                                SegmentCode = Global.MainSegment,
                                BusinessSegment = Global.BusinessSegment,
                                BranchCode = Global.BranchCode
                            });
                        }
                    }
                }

                return result;
            }
            catch
            {

                throw;
            }
        }

        public List<AdjustmentItems> GetAdjustmentItems(string date)
        {
            try
            {
                var result = new List<AdjustmentItems>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transType", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, AdjustmentQuery.GetAdjustmentQuery(AdjustmentEnum.AdjustmentDetail), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new AdjustmentItems
                            {
                                TransDate = dr["TransDate"].ToString(),
                                Reference = dr["Reference"].ToString(),
                                Barcode = dr["Barcode"] == null ? "" : dr["Barcode"].ToString(),
                                ItemCode = dr["ItemCode"].ToString(),
                                ItemDescription = dr["ItemDescription"].ToString(),
                                Category = dr["Category"].ToString(),
                                UomCode = dr["UomCode"].ToString(),
                                RunningQuantity = Convert.ToDecimal(dr["RunningQuantity"]),
                                ActualCount = Convert.ToDecimal(dr["ActualCount"]),
                                Variance = Convert.ToDecimal(dr["Variance"]),
                                Price = Convert.ToDecimal(dr["Price"]),
                                Total = Convert.ToDecimal(dr["Total"]),
                                SystemDate = dr["SystemDate"].ToString(),
                                IdUser = dr["IdUser"].ToString(),
                                RunningValue = Convert.ToDecimal(dr["RunningValue"]),
                            });
                        }
                    }
                }

                return result;
            }
            catch
            {

                throw;
            }
        }


        #endregion GET
        #region INSERT

        public void InsertAdjustment(List<Adjustment> _header, List<AdjustmentItems> _detail)
        {
            transType = "";
            try
            {
                Global global = new Global();
                int transNum = 0;
                long series = 0;

                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    conn.BeginTransaction();

                    transNum = global.GetLatestTransNum("iia00", "transNum");

                    foreach (var item in _header)
                    {
                        transType = item.TransType;

                        #region Creation of Document

                        CreateAdjustmentHeaderDocument(conn, item, transNum, global);

                        var details = _detail.Where(n => n.Reference == item.Reference).ToList();
                        CreateAdjustmentDetailDocument(conn, details, transNum);

                        #endregion Creation of Document

                        transNum++;
                        series = Convert.ToInt32(item.Reference.Replace(transType, "")) + 1;

                        UpdateLastReference(conn, series, transType);

                    }

                    conn.CommitTransaction();
                }

            }
            catch
            {
                throw;
            }
        }

        public string InsertAdjustmentLogs(List<Adjustment> _header, string date)
        {

            string fileName = string.Format("Adjustment-{0}-{1}.csv", date.Replace(" / ", ""), DateTime.Now.ToString("ddMMyyyyHHmmss"));
            dropSitePath = Path.Combine(dropSitePath, folder);

            if (!Directory.Exists(dropSitePath))
                Directory.CreateDirectory(dropSitePath);

            ObjectToCSV<Adjustment> receiveFromVendorObjectToCSV = new ObjectToCSV<Adjustment>();
            string filename = Path.Combine(dropSitePath, fileName);
            receiveFromVendorObjectToCSV.SaveToCSV(_header, filename);
            return folder;
        }

        #endregion INSERT

        #endregion Public Methods

        #region Private Methods

        private void CreateAdjustmentDetailDocument(MySQLHelper conn, List<AdjustmentItems> details, int transNum)
        {
            foreach (var detail in details)
            {
                var detailParam = new Dictionary<string, object>()
                                {
                                    {"@barcode", detail.Barcode },
                                    {"@transNum", transNum },
                                    {"@itemCode", detail.ItemCode },
                                    {"@itemDescription", detail.ItemDescription },
                                    {"@uomCode", detail.UomCode },
                                    {"@category", detail.Category },
                                    {"@runningQuantity", detail.RunningQuantity },
                                    {"@actualCount", detail.ActualCount },
                                    {"@variance", detail.Variance },
                                    {"@price", detail.Price },
                                    {"@total", detail.Total },
                                    {"@warehouseCode", Global.WarehouseCode },
                                    {"@systemDate", detail.SystemDate },
                                    {"@idUser", detail.IdUser },
                                    {"@runningValue", detail.RunningValue },
                                    {"@transDate", detail.TransDate }
                                };
                conn.ArgSQLCommand = AdjustmentQuery.InsertAdjustmentQuery(AdjustmentEnum.AdjustmentDetail);
                conn.ArgSQLParam = detailParam;

                //execute insert detail
                var cmdDetail = conn.ExecuteMySQL();
            }
        }

        private void CreateAdjustmentHeaderDocument(MySQLHelper conn, Adjustment item, int transNum, Global global)
        {

            var param = new Dictionary<string, object>()
                        {
                            { "@transDate", item.TransDate },
                            { "@transType", item.TransType },
                            { "@transNum", transNum },
                            { "@reference", item.Reference },
                            { "@crossreference", item.Crossreference },
                            { "@warehouseCode", Global.WarehouseCode },

                            { "@segmentCode", Global.MainSegment },
                            { "@businessSegment", Global.BusinessSegment },
                            { "@branchCode", Global.BranchCode },
                            { "@remarks", String.Format("{0}; Migrated from Sofos1 - {1};",item.Remarks, DateTime.Now) },
                            { "@cancelled", item.Cancelled },
                            { "@status", item.Status },
                            { "@total", item.Total },
                            { "@idUser", item.IdUser },
                            { "@systemDate", item.SystemDate },
                            { "@TerminalNo", "TerminalNo" }, // for check
                            { "@AccountNo", null }, // for check

                        };

            conn.ArgSQLCommand = AdjustmentQuery.InsertAdjustmentQuery(AdjustmentEnum.AdjustmentHeader);
            conn.ArgSQLParam = param;

            //Execute insert header
            conn.ExecuteMySQL();


        }

        private void UpdateLastReference(MySQLHelper conn, long series, string transType)
        {
            conn.ArgSQLCommand = Query.UpdateReferenceCount();
            conn.ArgSQLParam = new Dictionary<string, object>() { { "@series", series - 1 }, { "@transtype", transType } };
            conn.ExecuteMySQL();
        }
        #endregion Private Methods
    }
}
