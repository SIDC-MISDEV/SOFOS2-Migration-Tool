using SOFOS2_Migration_Tool.Helper;
using SOFOS2_Migration_Tool.Purchasing.Model;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Purchasing.Controller
{
    public class ReceiveFromVendorController
    {
        string transType = "RV";
        string dropSitePath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),"LOGS");
        string folder = "Purchasing/";

        public List<ReceiveFromVendor> GetRVHeader(string date)
        {
            try
            {
                var result = new List<ReceiveFromVendor>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transtype", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, PurchasingQuery.GetQuery(PR.RVHeader), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new ReceiveFromVendor
                            {
                                BranchCode = Global.BranchCode,
                                BussinessSegment = Global.BusinessSegment,
                                SegmentCode = Global.MainSegment,
                                Cancelled = Convert.ToBoolean(dr["cancelled"]),
                                CrossReference = dr["crossReference"].ToString(),
                                ToWarehouse = Global.WarehouseCode,
                                Reference = dr["reference"].ToString(),
                                VendorCode = dr["VendorCode"].ToString(),
                                VendorName = dr["VendorName"].ToString(),
                                VendorAddress = dr["VendorAddress"].ToString(),
                                Total = Convert.ToDecimal(dr["total"]),
                                TransType = dr["transType"].ToString(),
                                Status = "CLOSED",
                                TransDate = dr["date"].ToString(),
                                IdUser = dr["idUser"].ToString(),
                                Extracted = dr["extracted"].ToString()
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

        public List<ReceiveFromVendorItem> GetRVItem(string date)
        {
            try
            {
                var result = new List<ReceiveFromVendorItem>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transType", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, PurchasingQuery.GetQuery(PR.RVDetail), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new ReceiveFromVendorItem
                            {
                                Barcode = dr["Barcode"].ToString(),
                                Reference = dr["reference"].ToString(),
                                ItemCode = dr["itemCode"].ToString(),
                                ItemDescription = dr["name"].ToString(),
                                UOMCode = dr["uomcode"].ToString(),
                                UOMDescription = dr["UOMCode"].ToString(),
                                Quantity = Convert.ToDecimal(dr["Quantity"]),
                                Price = Convert.ToDecimal(dr["cost"]),
                                Total = Convert.ToDecimal(dr["total"]),
                                Conversion = Convert.ToDecimal(dr["conversion"])
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

        public void InsertRV(List<ReceiveFromVendor> _header, List<ReceiveFromVendorItem> _detail)
        {
            try
            {
                Global g = new Global();
                int transNum = 0;
                long series = 0;

                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    conn.BeginTransaction();

                    transNum = g.GetLatestTransNum("prv00", "transNum");

                    foreach (var item in _header)
                    {
                        series = Convert.ToInt32(item.Reference.Replace(transType, "")) + 1;

                        var param = new Dictionary<string, object>()
                        {
                            { "@vendorCode", item.VendorCode },
                            { "@vendorName", item.VendorName },
                            { "@transNum", transNum },
                            { "@reference", item.Reference },
                            { "@crossreference", item.CrossReference },
                            { "@inventoryRequest", item.PRReference },
                            { "@Total", item.Total },
                            { "@transType", item.TransType },
                            { "@toWarehouse", Global.WarehouseCode },
                            { "@fromWarehouse", item.FromWarehouse },
                            { "@segmentCode", item.SegmentCode },
                            { "@businessSegment", item.BussinessSegment },
                             { "@branchCode", item.BranchCode },
                            { "@remarks", "Inserted by Migration Tool" },
                            { "@cancelled", item.Cancelled },
                            { "@transDate", item.TransDate },
                            { "@idUser", item.IdUser },
                            { "@status", item.Status },
                            {"@extracted", item.Extracted }
                        };

                        //Saving transaction header
                        conn.ArgSQLCommand = PurchasingQuery.InsertTransaction(PR.RVHeader);
                        conn.ArgSQLParam = param;
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
                                    {"@transDate", item.TransDate },
                                    {"@iduser", item.IdUser },
                                    {"@accountCode", detail.AccountCode }
                                };

                            var lppParam = new Dictionary<string, object>()
                            {
                                { "@itemCode", detail.ItemCode },
                                { "@uomCode", detail.UOMCode },
                                { "@cost", detail.Price }
                            };

                            //Saving transaction details
                            conn.ArgSQLCommand = PurchasingQuery.InsertTransaction(PR.RVDetail);
                            conn.ArgSQLParam = detailParam;
                            conn.ExecuteMySQL();

                            //Updating last purchase price
                            conn.ArgSQLCommand = PurchasingQuery.UpdateLastPurchasePrice();
                            conn.ArgSQLParam = lppParam;
                            conn.ExecuteMySQL();

                        }
                        #endregion

                        transNum++;


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

        public string InsertRVLogs(List<ReceiveFromVendor> _header, string date)
        {

            string fileName = string.Format("ReceiveFromVendor-{0}-{1}.csv" , date.Replace(" / ", ""), DateTime.Now.ToString("ddMMyyyyHHmmss"));
            dropSitePath = Path.Combine(dropSitePath, folder);

            if (!Directory.Exists(dropSitePath))
                Directory.CreateDirectory(dropSitePath);

            ObjectToCSV<ReceiveFromVendor> receiveFromVendorObjectToCSV = new ObjectToCSV<ReceiveFromVendor>();
            string filename = Path.Combine(dropSitePath, fileName);
            receiveFromVendorObjectToCSV.SaveToCSV(_header, filename);
            return folder;
        }
    }
}
