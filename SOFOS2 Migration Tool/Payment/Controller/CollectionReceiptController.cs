using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOFOS2_Migration_Tool.Payment.Model;
using SOFOS2_Migration_Tool.Service;

namespace SOFOS2_Migration_Tool.Payment.Controller
{
    public class CollectionReceiptController
    {

        Global g = null;

        public List<CollectionReceipt> GetCollectionReceiptHeader(string date, string transprefix)
        {
            try
            {
               
                var result = new List<CollectionReceipt>();
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

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetCRQuery(payment.CRHeader), filter))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new CollectionReceipt
                            {
                                Reference = dr["reference"].ToString(),
                                Total = Convert.ToDecimal(dr["total"]),
                                TransDate = dr["transDate"].ToString(),
                                IdUser = dr["idUser"].ToString(),
                                MemberId = dr["memberId"].ToString(),
                                MemberName = dr["memberName"].ToString(),
                                Status = dr["status"].ToString(),
                                Remarks = dr["remarks"].ToString(),
                                Cancelled = Convert.ToBoolean(dr["cancelled"]),
                                sType = dr["type"].ToString(),
                                AccountCode = dr["accountCode"].ToString(),
                                PaidBy = dr["paidBy"].ToString(),
                                BranchCode = Global.BranchCode,
                                Extracted = dr["extracted"].ToString(),
                                TransType = dr["transType"].ToString(),
                                RefTransType = dr["refTransType"].ToString()
                            });

                            g = new Global();
                            result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.MemberName = g.GetMemberName(dr["memberId"].ToString()); return c; }).ToList();
                            result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.AccountNumber = g.GetAccountNumber(dr["memberId"].ToString(), dr["refTransType"].ToString()); return c; }).ToList();
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

        public List<CollectionReceipt> GetCollectionReceiptDetail (string date, string transprefix)
        {
            try
            {
                var result = new List<CollectionReceipt>();

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

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetCRQuery(payment.CRDetail), filter))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new CollectionReceipt
                            {
                                Amount = Convert.ToDecimal(dr["amount"]),
                                IdUser = dr["idUser"].ToString(),
                                AccountCode = dr["accountCode"].ToString(),
                                pType = dr["pType"].ToString(),
                            });
                            g = new Global();
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
