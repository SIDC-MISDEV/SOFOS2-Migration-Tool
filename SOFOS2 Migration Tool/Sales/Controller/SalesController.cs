﻿using SOFOS2_Migration_Tool.Sales.Model;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Sales.Controller
{
    public class SalesController
    {
        string transType = "PR";

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
                                Total = Convert.ToDecimal(dr["Total"]),
                                InterestPaid = Convert.ToDecimal(dr["InterestPaid"]),
                                InterestBalance = Convert.ToDecimal(dr["InterestBalance"]),
                                Cancelled = Convert.ToBoolean(dr["cancelled"]),
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
                                Vat = Convert.ToDecimal(dr["Vat"]),
                                VatExemptSales = Convert.ToDecimal(dr["VatExemptSales"]),
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

        public void InsertSales(List<Sales.Model.Sales> _header, List<SalesItem> _detail)
        {
            try
            {
                Global g = new Global();
                int transNum = 0;
                long series = 0;

                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {


                    transNum = g.GetLatestTransNum("sapt0", "transNum");

                    foreach (var item in _header)
                    {
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
                            { "@status", item.Status },
                            { "@extracted", item.Extracted },
                            { "@colaReference", item.ColaReference },
                            { "@signatory", item.Signatory },
                            { "@remarks", item.Remarks },
                            { "@idUser", item.IdUser },
                            { "@lrBatch", item.LrBatch },
                            { "@lrType", item.LrType },
                            { "@srDiscount", item.SrDiscount },
                            { "@feedsDiscount", item.FeedsDiscount },
                            { "@vat", item.Vat },
                            { "@vatExemptSales", item.VatExemptSales },
                            { "@systemDate", item.SystemDate },
                            { "@segmentCode", Global.MainSegment },
                            { "@businessSegment", Global.BusinessSegment },
                            { "@branchCode", Global.BranchCode }
                        };

                        conn.ArgSQLCommand = PurchasingQuery.InsertPR(PR.PRHeader);
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
                        #endregion

                        transNum++;
                        series = Convert.ToInt32(item.Reference.Replace(transType, "")) + 1;
                    }

                    conn.ArgSQLCommand = Query.UpdateReferenceCount();
                    conn.ArgSQLParam = new Dictionary<string, object>() { { "@series", series - 1 }, { "@transtype", transType } };
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
