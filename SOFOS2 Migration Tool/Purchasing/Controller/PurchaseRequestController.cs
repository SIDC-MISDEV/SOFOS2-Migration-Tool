using SOFOS2_Migration_Tool.Purchasing.Model;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Purchasing.Controller
{
    public class PurchaseRequestController
    {
        string transType = "PR";

        public List<PurchaseRequest> GetPRHeader(string date)
        {
            try
            {
                var result = new List<PurchaseRequest>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transtype", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, PurchasingQuery.GetQuery(PR.PRHeader), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new PurchaseRequest
                            {
                                TransactionDate = dr["date"].ToString(),
                                Reference = dr["reference"].ToString(),
                                Total = Convert.ToDecimal(dr["total"]),
                                Remarks = dr["remarks"].ToString(),
                                VendorCode = dr["Vendor"].ToString(),
                                VendorName = dr["vendorname"].ToString(),
                                VendorAddress = dr["vendoraddress"].ToString(),
                                TransType = dr["transtype"].ToString(),
                                Cancelled = Convert.ToBoolean(dr["cancelled"]),
                                SegmentCode = Global.MainSegment,
                                BusinessCode = Global.BusinessSegment,
                                BranchCode = Global.BranchCode,
                                Extracted = dr["extracted"].ToString(),
                                Iduser = dr["idUser"].ToString()
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

        public List<PurchaseRequestItem> GetPRItem(string date)
        {
            try
            {
                var result = new List<PurchaseRequestItem>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transType", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, PurchasingQuery.GetQuery(PR.PRDetail), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new PurchaseRequestItem
                            {
                                Reference = dr["reference"].ToString(),
                                Barcode = dr["barcode"].ToString(),
                                ItemCode = dr["ItemCode"].ToString(),
                                ItemDescription = dr["Description"].ToString(),
                                UOMCode = dr["UOM"].ToString(),
                                UOMDescription = dr["UOMDescription"].ToString(),
                                Quantity = Convert.ToDecimal(dr["Quantity"]),
                                RemainingQuantity = Convert.ToDecimal(dr["Remaining"]),
                                Price = Convert.ToDecimal(dr["Price"]),
                                Total = Convert.ToDecimal(dr["Total"]),
                                Conversion = Convert.ToDecimal(dr["Conversion"])
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


        public void InsertPR(List<PurchaseRequest> _header, List<PurchaseRequestItem> _detail)
        {
            try
            {
                Global g = new Global();
                int transNum = 0;
                long series = 0;

                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    conn.BeginTransaction();

                    transNum = g.GetLatestTransNum("ppr00", "transNum");

                    foreach (var item in _header)
                    {
                        transNum++;
                        series = Convert.ToInt32(item.Reference.Replace(transType, "")) + 1;

                        var param = new Dictionary<string, object>()
                        {
                            { "@vendorCode", item.VendorCode },
                            { "@vendorName", item.VendorName },
                            { "@transNum", transNum },
                            { "@reference", item.Reference },
                            { "@crossreference", item.CrossReference },
                            { "@Total", item.Total },
                            { "@transType", item.TransType },
                            { "@toWarehouse", Global.WarehouseCode },
                            { "@fromWarehouse", item.FromWarehouse },
                            { "@segmentCode", item.SegmentCode },
                            { "@businessSegment", item.BusinessCode },
                             { "@branchCode", item.BranchCode },
                            { "@remarks", "Inserted by Migration Tool" },
                            { "@cancelled", item.Cancelled },
                            { "@transDate", item.TransactionDate },
                            { "@idUser", item.Iduser },
                            { "@printed", "1" },
                            {"@extracted", item.Extracted }
                        };

                        conn.ArgSQLCommand = PurchasingQuery.InsertTransaction(PR.PRHeader);
                        conn.ArgSQLParam = param;

                        //Execute insert header
                        conn.ExecuteMySQL();

                        #region Insert Details
                        var details = _detail.Where(n => n.Reference == item.Reference).ToList();

                        foreach (var detail in details)
                        {
                            var detailParam = new Dictionary<string, object>()
                                {
                                    {"@barcode", detail.Barcode },
                                    {"@transNum", transNum },
                                    {"@itemCode", detail.ItemCode },
                                    {"@itemDescription", detail.ItemDescription },
                                    {"@uomCode", detail.UOMCode },
                                    {"@uomDescription", detail.UOMDescription },
                                    {"@quantity", detail.Quantity },
                                    {"@remaining", detail.RemainingQuantity },
                                    {"@price", detail.Price },
                                    {"@total", detail.Total },
                                    {"@conversion", detail.Conversion },
                                    {"@transDate", item.TransactionDate },
                                    {"@iduser", item.Iduser },
                                    {"@accountCode", detail.AccountCode }
                                };

                            conn.ArgSQLCommand = PurchasingQuery.InsertTransaction(PR.PRDetail);
                            conn.ArgSQLParam = detailParam;

                            //execute insert detail
                            var cmdDetail = conn.ExecuteMySQL();
                        }
                        #endregion

                        
                    }

                    conn.ArgSQLCommand = Query.UpdateReferenceCount();
                    conn.ArgSQLParam = new Dictionary<string, object>() { { "@series", series }, { "@transtype", transType } };
                    conn.ExecuteMySQL();


                    conn.CommitTransaction();
                }

            }
            catch
            {

                throw;
            }
        }

    }
}
