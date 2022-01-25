using System;
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
        public List<JournalVoucher> GetJournalVoucherHeader(string date, string transprefix)
        {
            try
            {
                var result = new List<JournalVoucher>();
                string principalaccount = "112010000000001";
                string oldinterestaccount = "441200000000000";
                string newinterestaccount = "430400000000000";

                var filter = new Dictionary<string, object>()
                {
                    { "@transdate", date },
                    { "@principalaccount", principalaccount },
                    { "@oldinterestaccount", oldinterestaccount },
                    { "@newinterestaccount", newinterestaccount },
                    { "@transprefix", transprefix }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetPaymentQuery(payment.JVHeader), filter))
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
                                Status = dr["status"].ToString(),
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

        public List<JournalVoucher> GetJournalVoucherDetail(string date, string transprefix)
        {
            try
            {
                var result = new List<JournalVoucher>();

                string principalaccount = "112010000000001";
                string oldinterestaccount = "441200000000000";
                string newinterestaccount = "430400000000000";

                var filter = new Dictionary<string, object>()
                {
                    { "@transdate", date },
                    { "@principalaccount", principalaccount },
                    { "@oldinterestaccount", oldinterestaccount },
                    { "@newinterestaccount", newinterestaccount },
                    { "@transprefix", transprefix }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetPaymentQuery(payment.JVDetail), filter))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new JournalVoucher
                            {
                                Reference = dr["reference"].ToString(),
                                AccountCode = dr["accountCode"].ToString(),
                                CrossReference = dr["crossReference"].ToString(),
                                IdUser = dr["idUser"].ToString(),
                                Debit = Convert.ToDecimal(dr["debit"]),
                                Credit = Convert.ToDecimal(dr["credit"]),
                                MemberId = dr["memberId"].ToString(),
                                MemberName = dr["memberName"].ToString(),
                                AccountName = dr["accountName"].ToString(),
                                DetRefTransType = dr["refTransType"].ToString(),
                                IntComputed = dr["intComputed"].ToString(),
                                PaidToDate = dr["paidToDate"].ToString(),
                                LastPaymentDate = dr["lastPaymentDate"].ToString(),
                                AccountNumber = dr["AccountNo"].ToString(),
                                Status = dr["status"].ToString(),
                            });
                            g = new Global();
                            result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.MemberName = g.GetMemberName(dr["memberId"].ToString()); return c; }).ToList();
                            result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.AccountNumber = g.GetAccountNumber(dr["memberId"].ToString(), "CI"); return c; }).ToList();
                            if (dr["accountCode"].ToString() == principalaccount)
                            {
                                result.Where(c => c.AccountCode == principalaccount).Select(c => { c.AccountName = g.GetAccountName(dr["accountCode"].ToString()); return c; }).ToList();
                            }
                            else if (dr["accountCode"].ToString() == newinterestaccount)
                            {
                                result.Where(c => c.AccountCode == newinterestaccount).Select(c => { c.AccountName = g.GetAccountName(dr["accountCode"].ToString()); return c; }).ToList();
                            }
                            else
                            {
                                result.Where(c => c.AccountCode == oldinterestaccount).Select(c => { c.AccountName = g.GetAccountName(dr["accountCode"].ToString()); return c; }).ToList();
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
    }
}
