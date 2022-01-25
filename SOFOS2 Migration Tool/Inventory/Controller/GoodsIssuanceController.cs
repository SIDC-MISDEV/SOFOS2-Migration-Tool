using SOFOS2_Migration_Tool.Inventory.Model;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Inventory.Controller
{
     public class GoodsIssuanceController
    {
        string transType = "ST";

        #region Public Methods

        #region GET

        public List<GoodsIssuance> GetGoodsIssuanceHeader(string date)
        {
            try
            {
                var result = new List<GoodsIssuance>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transtype", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, GoodsIssuanceQuery.GetGoodsIssuanceQuery(GoodsIssuanceEnum.GoodsIssuanceHeader), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new GoodsIssuance
                            {
                                TransDate = dr["TransDate"].ToString(),
                                TransType = dr["TransType"].ToString(),
                                Reference = dr["Reference"].ToString(),
                                Crossreference = dr["Crossreference"].ToString(),
                                Total = Convert.ToDecimal(dr["Total"]),
                                ToWarehouse = dr["ToWarehouse"].ToString(),
                                Remarks = dr["Remarks"].ToString(),
                                Cancelled = Convert.ToBoolean(dr["Cancelled"]),
                                Status = dr["Status"].ToString(),
                                SystemDate = dr["SystemDate"].ToString(),
                                IdUser = dr["IdUser"].ToString(),
                                Extracted = dr["Extracted"].ToString(),
                                ToBranchCode = dr["ToBranchCode"].ToString(), // for check
                                AccountCode = dr["AccountCode"].ToString(),
                                AccountName = dr["AccountName"].ToString(),
                                IsDummy = Convert.ToBoolean(dr["IsDummy"]),
                                IsManual = Convert.ToBoolean(dr["IsManual"]),
                                AccountNo = dr["AccountNo"].ToString(),
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

        public List<GoodsIssuanceItems> GetGoodsIssuanceItems(string date)
        {
            try
            {
                var result = new List<GoodsIssuanceItems>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transType", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, GoodsIssuanceQuery.GetGoodsIssuanceQuery(GoodsIssuanceEnum.GoodsIssuanceDetail), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new GoodsIssuanceItems
                            {
                                TransDate = dr["TransDate"].ToString(),
                                Reference = dr["Reference"].ToString(),
                                Barcode = dr["Barcode"] == null ? "" : dr["Barcode"].ToString(),
                                ItemCode = dr["ItemCode"].ToString(),
                                ItemDescription = dr["ItemDescription"].ToString(),
                                UomCode = dr["UomCode"].ToString(),
                                UomDescription = dr["UomDescription"].ToString(),
                                Quantity = Convert.ToDecimal(dr["Quantity"]),
                                Price = Convert.ToDecimal(dr["Price"]),
                                Total = Convert.ToDecimal(dr["Total"]),
                                Conversion = Convert.ToDecimal(dr["Conversion"]),
                                SystemDate = dr["SystemDate"].ToString(),
                                IdUser = dr["IdUser"].ToString(),
                                AccountCode = dr["AccountCode"].ToString(),
                                WarehouseCode = dr["WarehouseCode"].ToString(),
                                AverageCost = Convert.ToDecimal(dr["AverageCost"]),
                                RunningValue = Convert.ToDecimal(dr["RunningValue"]),
                                RunningQty = Convert.ToDecimal(dr["RunningQty"]),
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

        public void InsertGoodsIssuance(List<GoodsIssuance> _header, List<GoodsIssuanceItems> _detail)
        {
            transType = "";
            try
            {
                Global global = new Global();
                int transNum = 0;
                long series = 0;

                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {


                    transNum = global.GetLatestTransNum("iii00", "transNum");

                    foreach (var item in _header)
                    {
                        transType = item.TransType;

                        #region Creation of Document

                        CreateGoodsIssuanceHeaderDocument(conn, item, transNum, global);

                        var details = _detail.Where(n => n.Reference == item.Reference).ToList();
                        CreateGoodsIssuanceDetailDocument(conn, details, transNum);

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


        #endregion INSERT

        #endregion Public Methods

        #region Private Methods

        private void CreateGoodsIssuanceDetailDocument(MySQLHelper conn, List<GoodsIssuanceItems> details, int transNum)
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
                                    {"@uomDescription", detail.UomDescription },
                                    {"@quantity", detail.Quantity },
                                    {"@price", detail.Price },
                                    {"@total", detail.Total },
                                    {"@conversion", detail.Conversion },
                                    {"@accountCode", detail.AccountCode },
                                    {"@warehouseCode", Global.WarehouseCode },
                                    {"@systemDate", detail.SystemDate },
                                    {"@idUser", detail.IdUser },
                                    {"@averageCost", detail.AverageCost },
                                    {"@runningValue", detail.RunningValue },
                                    {"@runningQty", detail.RunningQty },
                                    {"@transDate", detail.TransDate }
                                };
                conn.ArgSQLCommand = GoodsIssuanceQuery.InsertGoodsIssuanceQuery(GoodsIssuanceEnum.GoodsIssuanceDetail);
                conn.ArgSQLParam = detailParam;

                //execute insert detail
                var cmdDetail = conn.ExecuteMySQL();
            }
        }

        private void CreateGoodsIssuanceHeaderDocument(MySQLHelper conn, GoodsIssuance item, int transNum, Global global)
        {
            var param = new Dictionary<string, object>()
                        {
                            { "@transDate", item.TransDate },
                            { "@transType", item.TransType },
                            { "@transNum", transNum },
                            { "@reference", item.Reference },
                            { "@crossreference", item.Crossreference },
                            { "@toWarehouse", item.ToWarehouse },
                            { "@fromWarehouse", Global.WarehouseCode },

                            { "@segmentCode", Global.MainSegment },
                            { "@businessSegment", Global.BusinessSegment },
                            { "@branchCode", Global.BranchCode },
                            { "@remarks", String.Format("{0}; Migrated from Sofos1 - {1};",item.Remarks, DateTime.Now) },
                            { "@cancelled", item.Cancelled },
                            { "@status", item.Status },
                            { "@toBranchCode", item.ToBranchCode },
                            { "@IsDummy", item.IsDummy },
                            { "@IsManual", item.IsManual },
                            { "@synced", item.Synced },

                            { "@accountCode", item.AccountCode },
                            { "@accountName", item.AccountName },
                            { "@total", item.Total },
                            { "@extracted", item.Extracted },
                            { "@idUser", item.IdUser },
                            { "@systemDate", item.SystemDate },
                            { "@TerminalNo", "TerminalNo" }, // for check
                            { "@AccountNo", null }, // for check

                            { "@empName", item.EmpName },
                            { "@department", item.Department },
                            { "@paidtodate", item.Paidtodate }


                        };

            conn.ArgSQLCommand = GoodsIssuanceQuery.InsertGoodsIssuanceQuery(GoodsIssuanceEnum.GoodsIssuanceHeader);
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
