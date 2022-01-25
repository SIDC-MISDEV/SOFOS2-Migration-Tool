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

        public List<Invoice> GetInvoice(string transprefix)
        {
            try
            {
                var result = new List<Invoice>();
                string principalaccount = "112010000000001";
                string oldinterestaccount = "441200000000000";
                string newinterestaccount = "430400000000000";

                var filter = new Dictionary<string, object>()
                {
                    { "@transprefix", transprefix }
                };

                using (var conn = new MySQLHelper(Global.DestinationDatabase, PaymentQuery.GetPaymentQuery(payment.Invoice), filter))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new Invoice
                            {
                                TransDate = dr["transDate"].ToString(),
                                TransType = dr["transType"].ToString(),
                                Reference = dr["reference"].ToString(),
                                MemberId = dr["memberId"].ToString(),
                                AccountCode = dr["accountCode"].ToString(),
                                PaidToDate = Convert.ToDecimal(dr["paidToDate"]),
                                Total = Convert.ToDecimal(dr["total"]),
                                Cancelled = Convert.ToBoolean(dr["cancelled"]),
                                Status = dr["status"].ToString(),
                                IntComputed = Convert.ToInt32(dr["intComputed"]),
                                LastPaymentDate = dr["lastpaymentdate"].ToString(),
                                AccountNumber = dr["AccountNo"].ToString(),
                            });

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
    }
}
