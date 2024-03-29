﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOFOS2_Migration_Tool.Payment.Model;
using SOFOS2_Migration_Tool.Service;

namespace SOFOS2_Migration_Tool.Payment.Controller
{
    public class JournalVoucherController
    {
        Global g = null;
        Dictionary<string, string> paymentmode = new Dictionary<string, string>();
        public List<JournalVoucher> GetJournalVoucherHeader(string date, string transprefix)
        {
            try
            {
                var result = new List<JournalVoucher>();
                //var refRemarks = new List<string>();
                //string principalaccount = "112010000000001";
                //string oldinterestaccount = "441200000000000";
                //string newinterestaccount = "430400000000000";
                //string duetointercompany = "212010000000000";
                //string miscellaneousincome = "440400000000000";
                //string growingincome = "410100000000000";
                //string expensespayable = "214010000000000";

                var filter = new Dictionary<string, object>()
                {
                    { "@transdate", date },
                    { "@transprefix", transprefix }
                };

                //using (var conn = new MySQLHelper(Global.DestinationDatabase, PaymentQuery.GetQuery(payment.JVRemarks)))
                //{
                //    using (var dr = conn.MySQLReader())
                //    {
                //        while (dr.Read())
                //        {
                //            refRemarks.Add(dr["remarks"].ToString());
                //        }
                //    }  
                //}

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetQuery(payment.JVHeader), filter))

                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new JournalVoucher
                            {
                                Reference = dr["reference"].ToString(),
                                Total = Convert.ToDecimal(dr["total"]),
                                TransDate = dr["transDate"].ToString(),
                                IdUser = dr["idUser"].ToString(),
                                Status = "CLOSED",
                                Cancelled = Convert.ToBoolean(dr["cancelled"]),
                                Remarks = dr["remarks"].ToString(),
                            });

                        }
                    }
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<JournalVoucherDetail> GetJournalVoucherDetail(string date, string transprefix)
        {
            try
            {
                g = new Global();
                var result = new List<JournalVoucherDetail>();
                string status = string.Empty;
                string _ref = string.Empty;
                bool isAr = false;

                paymentmode = g.GetAllPaymentMode();

                var filter = new Dictionary<string, object>()
                {
                    { "@transdate", date },
                    { "@transprefix", transprefix }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetQuery(payment.JVDetail), filter))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            
                            result.Add(new JournalVoucherDetail
                            {
                                Reference = dr["reference"].ToString(),
                                AccountCode = dr["accountCode"].ToString(),
                                IdUser = dr["idUser"].ToString(),
                                Debit = Convert.ToDecimal(dr["debit"]),
                                Credit = Convert.ToDecimal(dr["credit"]),
                                MemberId = dr["memberId"].ToString(),
                                MemberName = dr["memberName"].ToString(),
                                AccountName = dr["accountName"].ToString(),
                                DetRefTransType = dr["accountCode"].ToString() == "430400000000000" || dr["accountCode"].ToString() == "441200000000000" ? "CI" : "",
                                Status = "CLOSED",
                                IntComputed = "0",
                                PaidToDate = "0.00"
                            });
                            g = new Global();
                            result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.MemberName = g.GetFileName(dr["memberId"].ToString()); return c; }).ToList();
                            result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.AccountNumber = g.GetAccountNumber(dr["memberId"].ToString(), "CI"); return c; }).ToList();

                            result.Where(c => c.AccountCode == dr["accountCode"].ToString() && c.Reference == dr["reference"].ToString()).Select(c => { c.AccountName = g.GetAccountName(dr["accountCode"].ToString()); return c; }).ToList();
                            //result.Where(c => c.AccountCode == dr["accountCode"].ToString() && c.Reference == dr["reference"].ToString()).Select(c => { c.AccountName = g.GetAccountName(dr["accountCode"].ToString()); return c; }).ToList();

                            if (dr["accountCode"].ToString() == "112010000000001" && Convert.ToDecimal(dr["debit"]) != 0)
                            {
                                
                                result.Where(c => c.Debit != 0 && c.Reference == dr["reference"].ToString()).Select(c => { c.Status = "OPEN"; return c; }).ToList();
                                result.Where(c => c.Credit != 0 && c.Reference == dr["reference"].ToString()).Select(c => { c.Status = "OPEN"; return c; }).ToList();
                                isAr = true;
                            }
                            if (dr["accountCode"].ToString() == "212010000000000" && isAr == true)
                            {
                                result.Where(c => c.Debit != 0 && c.Reference == dr["reference"].ToString()).Select(c => { c.Status = "OPEN"; return c; }).ToList();
                                result.Where(c => c.Credit != 0 && c.Reference == dr["reference"].ToString()).Select(c => { c.Status = "OPEN"; return c; }).ToList();
                                isAr = false;
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void InsertJV(List<JournalVoucher> _header, List<JournalVoucherDetail> _detail)
        {
            try
            {
                Global g = new Global();
                int transNum = 0;
                long series = 0;

                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    conn.BeginTransaction();

                    transNum = g.GetLatestTransNum("fjv00", "transNum");

                    foreach (var item in _header)
                    {
                        var reference = g.GetJVReference("sst00", "series", series);
                        var param = new Dictionary<string, object>()
                        {
                            { "@transNum", transNum },
                            { "@reference", reference },
                            { "@Total", item.Total },
                            { "@transDate", item.TransDate },
                            { "@idUser", item.IdUser },
                            { "@status", item.Status },
                            { "@cancelled", item.Cancelled },
                            { "@remarks", item.Remarks }

                        };

                        conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.JVHeader);
                        conn.ArgSQLParam = param;

                        //Execute insert header
                        conn.ExecuteMySQL();

                        #region Insert Details
                        var details = _detail.Where(n => n.Reference == item.Reference).ToList();

                        foreach (var detail in details)
                        {
                            g = new Global();
                            if (detail.Debit > 0 && detail.AccountCode == "112010000000001")
                            {
                                g.UpdatePaymentStatuss(conn, transNum.ToString(), "fjv00", "OPEN");
                            }
                            var detailParam = new Dictionary<string, object>()
                                {
                                    {"@transNum", transNum },
                                    {"@accountCode", detail.AccountCode },
                                    {"@crossReference", "" },
                                    {"@idUser", detail.IdUser },
                                    {"@debit", detail.Debit },
                                    {"@credit", detail.Credit },
                                    {"@memberId", detail.MemberId },
                                    {"@memberName", detail.MemberName },
                                    {"@accountName", detail.AccountName },
                                    {"@refTransType", detail.DetRefTransType },
                                    {"@intComputed", detail.IntComputed },
                                    {"@paidToDate", detail.PaidToDate },
                                    {"@status", detail.Status },
                                    {"@AccountNo", detail.AccountNumber }
                                };

                            conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.JVDetail);
                            conn.ArgSQLParam = detailParam;

                            //execute insert detail
                            var cmdDetail = conn.ExecuteMySQL();
                        }
                        #endregion

                        transNum++;
                        series = Convert.ToInt32(reference.Replace("JV", "")) + 1;
                    }

                    conn.ArgSQLCommand = Query.UpdateReferenceCount();
                    conn.ArgSQLParam = new Dictionary<string, object>() { { "@series", series - 1 }, { "@transtype", "JV" } };
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
