using SOFOS2_Migration_Tool.Helper;
using SOFOS2_Migration_Tool.Sales.Model;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Sales.Controller
{
    public class ReturnFromCustomerController
    {
        string transType = "RC";
        string dropSitePath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "LOGS");
        string folder = "Sales/";

        #region Public Methods

        #region GET

        public List<ReturnFromCustomer> GetReturnFromCustomerHeader(string date)
        {
            try
            {
                var result = new List<ReturnFromCustomer>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transtype", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, ReturnFromCustomerQuery.GetReturnFromCustomerQuery(ReturnFromCustomerEnum.ReturnFromCustomerHeader), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new ReturnFromCustomer
                            {
                                TransDate = dr["TransDate"].ToString(),
                                TransType = dr["TransType"].ToString(),
                                Reference = dr["Reference"].ToString(),
                                Crossreference = dr["Crossreference"].ToString(),
                                NoEffectOnInventory = Convert.ToBoolean(dr["NoEffectOnInventory"]),
                                CustomerType = dr["CustomerType"].ToString(),
                                MemberId = dr["MemberId"].ToString(),
                                MemberName = dr["MemberName"].ToString(),
                                EmployeeID = dr["EmployeeID"].ToString(),
                                EmployeeName = dr["EmployeeName"].ToString(),
                                YoungCoopID = dr["YoungCoopID"].ToString(),
                                YoungCoopName = dr["YoungCoopName"].ToString(),
                                AccountCode = dr["AccountCode"].ToString(),
                                AccountName = dr["AccountName"].ToString(),
                                PaidToDate = Convert.ToDecimal(dr["PaidToDate"]),
                                Total = Convert.ToDecimal(dr["Total"]),// for check
                                AmountTendered = Convert.ToDecimal(dr["AmountTendered"]),
                                Cancelled = Convert.ToBoolean(dr["Cancelled"]),
                                Status = dr["Status"].ToString(),
                                Extracted = dr["Extracted"].ToString(),
                                ColaReference = dr["ColaReference"].ToString(),
                                Signatory = dr["Signatory"].ToString(),
                                Remarks = dr["Remarks"].ToString(),
                                IdUser = dr["IdUser"].ToString(),
                                LrBatch = dr["LrBatch"].ToString(),
                                LrType = dr["LrType"].ToString(),
                                SrDiscount = Convert.ToDecimal(dr["SrDiscount"]),
                                FeedsDiscount = Convert.ToDecimal(dr["FeedsDiscount"]),
                                Vat = Convert.ToDecimal(dr["Vat"]),// for check
                                VatExemptSales = Convert.ToDecimal(dr["VatExemptSales"]),// for check
                                VatAmount = Convert.ToDecimal(dr["VatAmount"]),// for check

                                SystemDate = dr["SystemDate"].ToString(),

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

        public List<ReturnFromCustomerItems> GetReturnFromCustomerItems(string date)
        {
            try
            {
                var result = new List<ReturnFromCustomerItems>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transType", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, ReturnFromCustomerQuery.GetReturnFromCustomerQuery(ReturnFromCustomerEnum.ReturnFromCustomerDetail), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new ReturnFromCustomerItems
                            {
                                Reference = dr["Reference"].ToString(),
                                Barcode = dr["Barcode"] == null ? "" : dr["Barcode"].ToString(),
                                ItemCode = dr["ItemCode"].ToString(),
                                ItemDescription = dr["ItemDescription"].ToString(),
                                UomCode = dr["UomCode"].ToString(),
                                UomDescription = dr["UomDescription"].ToString(),
                                Quantity = Convert.ToDecimal(dr["Quantity"]),
                                Cost = Convert.ToDecimal(dr["Cost"]),
                                SellingPrice = Convert.ToDecimal(dr["SellingPrice"]),
                                Discount = Convert.ToDecimal(dr["Discount"]),
                                Total = Convert.ToDecimal(dr["Total"]),
                                Conversion = Convert.ToDecimal(dr["Conversion"]),
                                SystemDate = dr["SystemDate"].ToString(),
                                IdUser = dr["IdUser"].ToString(),
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

        public void InsertReturnFromCustomer(List<ReturnFromCustomer> _header, List<ReturnFromCustomerItems> _detail)
        {
            string currentReference = "";
            transType = "";
            try
            {
                Global global = new Global();
                int transNum = 0;
                long series = 0;

                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    conn.BeginTransaction();

                    transNum = global.GetLatestTransNum("sapr0", "transNum");

                    foreach (var item in _header)
                    {
                        currentReference = item.Reference;
                        transType = item.TransType;

                        #region Creation of Document

                        CreateReturnFromCustomerHeaderDocument(conn, item, transNum, global);

                        var details = _detail.Where(n => n.Reference == item.Reference).ToList();
                        CreateReturnFromCustomerDetailDocument(conn, details, transNum);

                        #endregion Creation of Document

                        #region Creation of Cancellation document for cancelled document

                        if (item.Cancelled)
                        {
                            transNum++;

                            ReturnFromCustomer cancelledDocument = (ReturnFromCustomer)item.Clone();
                            cancelledDocument.Crossreference = item.Reference;
                            cancelledDocument.Reference = global.GetLatestTransactionReference(conn, "POS", "CD");
                            cancelledDocument.TransType = "CD";

                            CreateReturnFromCustomerHeaderDocument(conn, cancelledDocument, transNum, global);
                            CreateReturnFromCustomerDetailDocument(conn, details, transNum);

                            var cancelledDocumentseries = Convert.ToInt32(cancelledDocument.Reference.Replace(cancelledDocument.TransType, "")) + 1;
                            UpdateLastReference(conn, cancelledDocumentseries, cancelledDocument.TransType);
                        }

                        #endregion Creation of Cancellation document for cancelled document

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

        public string InsertReturnFromCustomerLogs(List<ReturnFromCustomer> _header, string date)
        {

            string fileName = string.Format("ReturnFromCustomer-{0}-{1}.csv", date.Replace(" / ", ""), DateTime.Now.ToString("ddMMyyyyHHmmss"));
            dropSitePath = Path.Combine(dropSitePath, folder);

            if (!Directory.Exists(dropSitePath))
                Directory.CreateDirectory(dropSitePath);

            ObjectToCSV<ReturnFromCustomer> receiveFromVendorObjectToCSV = new ObjectToCSV<ReturnFromCustomer>();
            string filename = Path.Combine(dropSitePath, fileName);
            receiveFromVendorObjectToCSV.SaveToCSV(_header, filename);
            return folder;
        }

        #endregion INSERT

        #endregion Public Methods

        #region Private Methods

        private void CreateReturnFromCustomerDetailDocument(MySQLHelper conn, List<ReturnFromCustomerItems> details, int transNum)
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
                                    {"@cost", detail.Cost },
                                    {"@sellingPrice", detail.SellingPrice },
                                    {"@discount", detail.Discount },
                                    {"@total", detail.Total },
                                    {"@conversion", detail.Conversion },
                                    {"@systemDate", detail.SystemDate },
                                    {"@idUser", detail.IdUser },
                                    {"@averageCost", detail.AverageCost },
                                    {"@runningValue", detail.RunningValue },
                                    {"@runningQty", detail.RunningQty }
                                };

                conn.ArgSQLCommand = ReturnFromCustomerQuery.InsertReturnFromCustomerQuery(ReturnFromCustomerEnum.ReturnFromCustomerDetail);
                conn.ArgSQLParam = detailParam;

                //execute insert detail
                var cmdDetail = conn.ExecuteMySQL();
            }
        }
        
        private void CreateReturnFromCustomerHeaderDocument(MySQLHelper conn, ReturnFromCustomer item, int transNum, Global global)
        {

            if (!item.NoEffectOnInventory)
            {
                item.Series = global.GetBIRSeries(conn, "Sales Invoice");
                global.UpdateBIRSeries(conn, "Sales Invoice");
                //UpdateBIRSeries(conn, "Sales Invoice");
            }

            var param = new Dictionary<string, object>()
                        {
                            { "@transDate", item.TransDate },
                            { "@transType", item.TransType },
                            { "@transNum", transNum },
                            { "@reference", item.Reference },
                            { "@crossreference", item.Crossreference },
                            { "@noEffectOnInventory", item.NoEffectOnInventory },
                            { "@customerType", item.CustomerType },
                            { "@memberId", item.MemberId },
                            { "@memberName", item.MemberName },
                            { "@employeeID", item.EmployeeID },
                            { "@employeeName", item.EmployeeName },
                            { "@youngCoopID", item.YoungCoopID },
                            { "@youngCoopName", item.YoungCoopName },
                            { "@accountCode", item.AccountCode },
                            { "@accountName", item.AccountName },
                            { "@paidToDate", item.PaidToDate },
                            { "@total", item.Total },
                            { "@amountTendered", item.AmountTendered },
                            { "@cancelled", item.Cancelled },
                            { "@status", item.Status },
                            { "@extracted", item.Extracted },
                            { "@colaReference", item.ColaReference },
                            { "@signatory", item.Signatory },
                            { "@remarks", String.Format("{0}; Migrated from Sofos1 - {1};",item.Remarks, DateTime.Now) },
                            { "@idUser", item.IdUser },
                            { "@lrBatch", item.LrBatch },
                            { "@lrType", item.LrType },
                            { "@srDiscount", item.SrDiscount },
                            { "@feedsDiscount", item.FeedsDiscount },
                            { "@vat", item.Vat },
                            { "@vatExemptSales", item.VatExemptSales },
                            { "@vatAmount", item.VatAmount },
                            { "@systemDate", item.SystemDate },
                            { "@segmentCode", Global.MainSegment },
                            { "@businessSegment", Global.BusinessSegment },
                            { "@branchCode", Global.BranchCode },
                            { "@warehouseCode", Global.WarehouseCode },

                            { "@series", item.Series },
                            { "@lastpaymentdate", null }, // for check
                            { "@allowNoEffectInventory", false }, // for check
                            { "@printed", "0" }, // for check
                            { "@TerminalNo", "TerminalNo" }, // for check
                            { "@AccountNo", null } // for check

                        };

            conn.ArgSQLCommand = ReturnFromCustomerQuery.InsertReturnFromCustomerQuery(ReturnFromCustomerEnum.ReturnFromCustomerHeader);
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
