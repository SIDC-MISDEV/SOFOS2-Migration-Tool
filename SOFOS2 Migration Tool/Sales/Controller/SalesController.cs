using SOFOS2_Migration_Tool.Helper;
using SOFOS2_Migration_Tool.Payment.Model;
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
    public class SalesController
    {
        string transType = "";
        string dropSitePath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "LOGS");
        string folder = "Sales/";


        #region Public Methods

        #region GET

        public List<Sales.Model.Sales> GetSalesHeader(string date)
        {
            try
            {
                var result = new List<Sales.Model.Sales>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transtype", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, SalesQuery.GetSalesQuery(SalesEnum.SalesHeader), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new Sales.Model.Sales
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
                                InterestPaid = Convert.ToDecimal(dr["InterestPaid"]),
                                InterestBalance = Convert.ToDecimal(dr["InterestBalance"]),
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
                                LrReference = "",
                                GrossTotal = Convert.ToDecimal(dr["Total"]), // for check

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

        public List<SalesItem> GetSalesItems(string date)
        {
            try
            {
                var result = new List<SalesItem>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transType", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, SalesQuery.GetSalesQuery(SalesEnum.SalesDetail), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new SalesItem
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
                                Feedsdiscount = Convert.ToDecimal(dr["Feedsdiscount"]),
                                Total = Convert.ToDecimal(dr["Total"]),
                                Conversion = Convert.ToDecimal(dr["Conversion"]),
                                SystemDate = dr["SystemDate"].ToString(),
                                IdUser = dr["IdUser"].ToString(),
                                Srdiscount = Convert.ToDecimal(dr["Srdiscount"]),
                                RunningQuantity = Convert.ToDecimal(dr["RunningQuantity"]),
                                KanegoDiscount = Convert.ToDecimal(dr["KanegoDiscount"]),
                                AverageCost = Convert.ToDecimal(dr["AverageCost"]),
                                RunningValue = Convert.ToDecimal(dr["RunningValue"]),
                                RunningQty = Convert.ToDecimal(dr["RunningQty"]),
                                Linetotal = Convert.ToDecimal(dr["Linetotal"]),
                                DedDiscount = Convert.ToDecimal(dr["DedDiscount"]),
                                Vat = Convert.ToDecimal(dr["Vat"]),
                                Vatable = Convert.ToDecimal(dr["Vatable"]),
                                Vatexempt = Convert.ToDecimal(dr["Vatexempt"]),
                                CancelledQty = Convert.ToDecimal(dr["CancelledQty"])

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

        public List<TransactionPayment> GetSalesPayment(string date)
        {
            try
            {
                var result = new List<TransactionPayment>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                    { "@transType", transType }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, SalesQuery.GetSalesQuery(SalesEnum.SalesPayment), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new TransactionPayment
                            {
                                Reference = dr["Reference"].ToString(),
                                PaymentCode = dr["PaymentCode"].ToString(),
                                Amount = Convert.ToDecimal(dr["Amount"]),
                                CheckNumber = dr["CheckNumber"].ToString(),
                                BankCode = dr["BankCode"].ToString(),
                                CheckDate = dr["CheckDate"].ToString(),
                                SystemDate = dr["SystemDate"].ToString(),
                                IdUser = dr["IdUser"].ToString(),
                                TransType = dr["TransType"].ToString(),
                                AccountCode = dr["AccountCode"].ToString(),
                                AccountName = dr["AccountName"].ToString(),
                                ChangeAmount = Convert.ToDecimal(dr["ChangeAmount"]),
                                Extracted = dr["Extracted"].ToString(),
                                OrDetailNum = Convert.ToInt16(dr["OrDetailNum"])
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

        public void InsertSales(List<Sales.Model.Sales> _header, List<SalesItem> _detail, List<TransactionPayment> _payments)
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

                    transNum = global.GetLatestTransNum("sapt0", "transNum");

                    foreach (var item in _header)
                    {
                        currentReference = item.Reference;
                        transType = item.TransType;

                        #region Creation of Document

                        CreateSalesHeaderDocument(conn, item, transNum, global);

                        var details = _detail.Where(n => n.Reference == item.Reference).ToList();
                        CreateSalesDetailDocument(conn, details, transNum);
                        var payments = _payments.Where(n => n.Reference == item.Reference).ToList();
                        CreateSalesPayment(conn, payments, transNum);


                        #endregion Creation of Document

                        #region Creation of Cancellation document for cancelled document

                        if (item.Cancelled)
                        {
                            transNum++;

                            Sales.Model.Sales cancelledDocument = (Sales.Model.Sales)item.Clone();
                            cancelledDocument.Crossreference = item.Reference;
                            cancelledDocument.Reference = global.GetLatestTransactionReference(conn,"POS", "CD");
                            cancelledDocument.TransType = "CD"; 

                            CreateSalesHeaderDocument(conn, cancelledDocument, transNum, global);
                            CreateSalesDetailDocument(conn, details, transNum);

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

        public string InsertSalesLogs(List<Sales.Model.Sales> _header, string date)
        {

            string fileName = string.Format("Sales-{0}-{1}.csv", date.Replace(" / ", ""), DateTime.Now.ToString("ddMMyyyyHHmmss"));
            dropSitePath = Path.Combine(dropSitePath, folder);

            if (!Directory.Exists(dropSitePath))
                Directory.CreateDirectory(dropSitePath);

            ObjectToCSV<Sales.Model.Sales> receiveFromVendorObjectToCSV = new ObjectToCSV<Sales.Model.Sales>();
            string filename = Path.Combine(dropSitePath, fileName);
            receiveFromVendorObjectToCSV.SaveToCSV(_header, filename);
            return folder;
        }

        #endregion INSERT

        #endregion Public Methods

        #region Private Methods

        private void CreateSalesDetailDocument(MySQLHelper conn, List<SalesItem> details, int transNum)
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
                                    {"@feedsdiscount", detail.Feedsdiscount },
                                    {"@total", detail.Total },
                                    {"@conversion", detail.Conversion },
                                    {"@systemDate", detail.SystemDate },
                                    {"@idUser", detail.IdUser },
                                    {"@srdiscount", detail.Srdiscount },
                                    {"@runningQuantity", detail.RunningQuantity },
                                    {"@kanegoDiscount", detail.KanegoDiscount },
                                    {"@averageCost", detail.AverageCost },
                                    {"@runningValue", detail.RunningValue },
                                    {"@runningQty", detail.RunningQty },
                                    {"@linetotal", detail.Linetotal },
                                    {"@dedDiscount", detail.DedDiscount },
                                    {"@vat", detail.Vat },
                                    {"@vatable", detail.Vatable },
                                    {"@vatexempt", detail.Vatexempt },
                                    {"@cancelledQty", detail.CancelledQty }
                                };

                conn.ArgSQLCommand = SalesQuery.InsertSalesQuery(SalesEnum.SalesDetail);
                conn.ArgSQLParam = detailParam;

                //execute insert detail
                var cmdDetail = conn.ExecuteMySQL();
            }
        }

        private void CreateSalesPayment(MySQLHelper conn, List<TransactionPayment> payments, int transNum)
        {
            foreach (var detail in payments)
            {
                var detailParam = new Dictionary<string, object>()
                                {
                                    {"@transNum", transNum },
                                    {"@paymentCode", detail.PaymentCode },
                                    {"@amount", detail.Amount },
                                    {"@checkNumber", detail.CheckNumber },
                                    {"@bankCode", detail.BankCode },
                                    {"@checkDate", string.IsNullOrWhiteSpace(detail.CheckDate) ? null : detail.CheckDate },
                                    {"@systemDate", detail.SystemDate },
                                    {"@idUser", detail.IdUser },
                                    {"@transType", detail.TransType },
                                    {"@accountCode", detail.AccountCode },
                                    {"@accountName", detail.AccountName },
                                    {"@changeAmount", detail.ChangeAmount },
                                    {"@extracted", detail.Extracted },
                                    {"@orDetailNum", detail.OrDetailNum }
                                };

                conn.ArgSQLCommand = SalesQuery.InsertSalesQuery(SalesEnum.SalesPayment);
                conn.ArgSQLParam = detailParam;
                var cmdDetail = conn.ExecuteMySQL();
            }
        }

        private void CreateSalesHeaderDocument(MySQLHelper conn, Model.Sales item, int transNum, Global global)
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
                            { "@interestPaid", item.InterestPaid },
                            { "@interestBalance", item.InterestBalance },
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
                            { "@kanegoDiscount", item.KanegoDiscount },
                            { "@vat", item.Vat },
                            { "@vatExemptSales", item.VatExemptSales },
                            { "@vatAmount", item.VatAmount },
                            { "@lrReference", item.LrReference },
                            { "@grossTotal", item.GrossTotal },
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

            conn.ArgSQLCommand = SalesQuery.InsertSalesQuery(SalesEnum.SalesHeader);
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
