using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOFOS2_Migration_Tool.Payment.Model;
using SOFOS2_Migration_Tool.Service;

namespace SOFOS2_Migration_Tool.Payment.Controller
{
    class PaymentComputeController
    {
        Global g = null;

        public List<Invoice> GetInvoice(string memberid, string accountno, decimal amount, string date)
        {
            try
            {
                var result = new List<Invoice>();
                string principalaccount = "112010000000001";

                var filter = new Dictionary<string, object>()
                {
                    { "@principalaccount", principalaccount },
                    { "@memberid", memberid },
                    { "@accountno", accountno }
                };

                using (var conn = new MySQLHelper(Global.DestinationDatabase, PaymentQuery.GetPaymentQuery(payment.Invoice), filter))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            while (amount > 0)
                            {
                                decimal _paidtodate = 0;
                                if (amount > (Convert.ToDecimal(dr["total"])))
                                {
                                    _paidtodate = (Convert.ToDecimal(dr["total"])) - Convert.ToDecimal(dr["paidToDate"]);
                                }
                                else
                                {
                                    _paidtodate = amount;
                                }

                                result.Add(new Invoice
                                {
                                    TransDate = dr["transDate"].ToString(),
                                    Reference = dr["reference"].ToString(),
                                    MemberId = dr["memberId"].ToString(),
                                    AccountCode = dr["accountCode"].ToString(),
                                    PaidToDate = Convert.ToDecimal(dr["paidToDate"]) + _paidtodate,
                                    Total = Convert.ToDecimal(dr["total"]),
                                    Cancelled = Convert.ToBoolean(dr["cancelled"]),
                                    Status = Convert.ToDecimal(dr["total"]) == Convert.ToDecimal(dr["paidToDate"]) + _paidtodate ? "CLOSED":"OPEN",
                                    IntComputed = 1,
                                    LastPaymentDate = date,
                                    AccountNumber = dr["AccountNo"].ToString(),
                                    isSelected = true
                                });
                                amount = amount - _paidtodate;
                                break;
                            }
                            if(amount==0)
                                break;

                            //g = new Global();
                            //result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.MemberName = g.GetMemberName(dr["memberId"].ToString()); return c; }).ToList();
                            //result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.AccountNumber = g.GetAccountNumber(dr["memberId"].ToString(), dr["refTransType"].ToString()); return c; }).ToList();
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

        public List<Interest> ComputeInterest(string transdate, string reference, string memberid, string accountnumber)
        {
            
            try
            {
                var result = new List<Interest>();
                string principalaccount = "112010000000001";
                string intafter = "30";
                string intrate = "14";

                var filter = new Dictionary<string, object>()
                {
                    { "@transdate", transdate },
                    { "@principalaccount", principalaccount },
                    { "@intrate", intrate },
                    { "@intafter", intafter },
                    { "@memberid", memberid },
                    { "@reference", reference },
                    { "@accountno", accountnumber }
                };

                using (var conn = new MySQLHelper(Global.DestinationDatabase, PaymentQuery.GetPaymentQuery(payment.GetInterest), filter))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new Interest
                            {
                                InterestAmount = Convert.ToDecimal(dr["intAmount"]),
                                Reference = dr["reference"].ToString()
                                
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

        public List<Payments> GetAllTransactionPayments(string transdate)
        {
            try
            {
                var result = new List<Payments>();
                string principalaccount = "112010000000001";
                string oldinterestaccount = "441200000000000";
                string newinterestaccount = "430400000000000";

                var filter = new Dictionary<string, object>()
                {
                    { "@transDate", transdate },
                    { "@principalaccount", principalaccount },
                    { "@oldinterestaccount", oldinterestaccount },
                    { "@newinterestaccount", newinterestaccount }
                };

                using (var conn = new MySQLHelper(Global.DestinationDatabase, PaymentQuery.GetPaymentQuery(payment.GetAllTransactionPayments), filter))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new Payments
                            {
                                TransDate = dr["transDate"].ToString(),
                                Reference = dr["reference"].ToString(),
                                MemberId = dr["memberId"].ToString(),
                                Amount = Convert.ToDecimal(dr["amount"]),
                                AccountNumber = dr["AccountNo"].ToString(),
                                AccountCode = dr["AccountCode"].ToString()
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

        public void UpdateInvoice(List<Invoice> cilist)
        {
            try
            {
                Global g = new Global();
                int transNum = 0;
                long series = 0;

                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    
                    foreach (var item in cilist)
                    {
                        #region Update CI
                        var param = new Dictionary<string, object>()
                        {
                            { "@reference", item.Reference },
                            { "@paidtodate", item.PaidToDate },
                            { "@intcomputed", item.IntComputed },
                            { "@lastpaymentdate", item.LastPaymentDate },
                            { "@status", item.Status }
                        };

                        conn.ArgSQLCommand = PaymentQuery.UpdateInvoice(payment.UpInvoice);
                        conn.ArgSQLParam = param;

                        //Execute insert header
                        conn.ExecuteMySQL();
                        #endregion

                        #region Update JV
                        //var details = _detail.Where(n => n.Reference == item.Reference).ToList();

                        //foreach (var detail in details)
                        //{
                        //    var detailParam = new Dictionary<string, object>()
                        //        {
                        //            {"@transNum", transNum },
                        //            {"@crossReference", "" },
                        //            {"@amount", detail.Amount },
                        //            {"@idUser", detail.IdUser },
                        //            {"@balance", detail.Balance },
                        //            {"@accountCode", detail.AccountCode },
                        //            {"@pType", detail.pType },
                        //            {"@accountName", detail.AccountName },
                        //            {"@refTransType", detail.DetRefTransType }
                        //        };

                        //    conn.ArgSQLCommand = PaymentQuery.InsertCR(payment.CRDetail);
                        //    conn.ArgSQLParam = detailParam;

                        //    //execute insert detail
                        //    var cmdDetail = conn.ExecuteMySQL();
                        //}
                        #endregion
                    
                    }

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
