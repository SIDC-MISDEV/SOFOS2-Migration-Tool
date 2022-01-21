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
        public List<CollectionReceipt> GetCollectionReceiptHeader(string date, string accountcode, string transprefix)
        {
            try
            {
                var result = new List<CollectionReceipt>();
                
                var prm = new Dictionary<string, object>()
                {
                    { "@transdate", date },
                    { "@accountcode", accountcode },
                    { "@transprefix", transprefix }
                };
                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetCRQuery(payment.CRHeader), prm))
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
