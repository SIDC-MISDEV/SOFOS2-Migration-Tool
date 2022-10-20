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
        Dictionary<string, string> paymentmode = new Dictionary<string, string>();
        public List<JournalVoucher> GetJournalVoucherHeader(string date, string transprefix)
        {
            try
            {
                var result = new List<JournalVoucher>();
                var refRemarks = new List<string>();
                string principalaccount = "112010000000001";
                string oldinterestaccount = "441200000000000";
                string newinterestaccount = "430400000000000";
                string duetointercompany = "212010000000000";
                string miscellaneousincome = "440400000000000";
                string growingincome = "410100000000000";
                string expensespayable = "214010000000000";

                var filter = new Dictionary<string, object>()
                {
                    { "@transdate", date },
                    { "@principalaccount", principalaccount },
                    { "@oldinterestaccount", oldinterestaccount },
                    { "@newinterestaccount", newinterestaccount },
                    { "@duetointercompany", duetointercompany },
                    { "@miscellaneousincome", miscellaneousincome },
                    { "@growingincome", growingincome },
                    { "@expensespayable", expensespayable },
                    { "@transprefix", transprefix }
                };


                using (var conn = new MySQLHelper(Global.DestinationDatabase, PaymentQuery.GetQuery(payment.JVRemarks)))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            refRemarks.Add(dr["remarks"].ToString());
                        }
                    }

                
                           
                }

        

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetQuery(payment.JVHeader), filter))

                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            if(!refRemarks.Contains(dr["remarks"].ToString()))
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
                g = new Global();
                var result = new List<JournalVoucher>();
                paymentmode = g.GetAllPaymentMode();

                string principalaccount = "112010000000001";
                string oldinterestaccount = "441200000000000";
                string newinterestaccount = "430400000000000";
                string duetointercompany = "212010000000000";
                string miscellaneousincome = "440400000000000";
                string growingincome = "410100000000000";
                string expensespayable = "214010000000000";

                var filter = new Dictionary<string, object>()
                {
                    { "@transdate", date },
                    { "@principalaccount", principalaccount },
                    { "@oldinterestaccount", oldinterestaccount },
                    { "@newinterestaccount", newinterestaccount },
                    { "@duetointercompany", duetointercompany },
                    { "@miscellaneousincome", miscellaneousincome },
                    { "@growingincome", growingincome },
                    { "@expensespayable", expensespayable },
                    { "@cash", paymentmode.FirstOrDefault(n => n.Value == "CASH").Key },
                    { "@check", paymentmode.FirstOrDefault(n => n.Value == "CHECK").Key },
                    { "@giftcheck", paymentmode.FirstOrDefault(n => n.Value == "Gift Check").Key },
                    { "@transprefix", transprefix }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetQuery(payment.JVDetail), filter))
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
                            result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.MemberName = g.GetFileName(dr["memberId"].ToString()); return c; }).ToList();
                            result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.AccountNumber = g.GetAccountNumber(dr["memberId"].ToString(), "CI"); return c; }).ToList();
                            if (dr["accountCode"].ToString() == principalaccount)
                            {
                                result.Where(c => c.AccountCode == principalaccount).Select(c => { c.AccountName = g.GetAccountName(dr["accountCode"].ToString()); return c; }).ToList();
                            }
                            else if (dr["accountCode"].ToString() == newinterestaccount)
                            {
                                result.Where(c => c.AccountCode == newinterestaccount).Select(c => { c.AccountName = g.GetAccountName(dr["accountCode"].ToString()); return c; }).ToList();
                            }
                            else if (dr["accountCode"].ToString() == oldinterestaccount)
                            {
                                result.Where(c => c.AccountCode == oldinterestaccount).Select(c => { c.AccountName = g.GetAccountName(dr["accountCode"].ToString()); return c; }).ToList();
                            }
                            else if (dr["accountCode"].ToString() == duetointercompany)
                            {
                                result.Where(c => c.AccountCode == duetointercompany).Select(c => { c.AccountName = g.GetAccountName(dr["accountCode"].ToString()); return c; }).ToList();
                            }
                            else if (dr["accountCode"].ToString() == miscellaneousincome)
                            {
                                result.Where(c => c.AccountCode == miscellaneousincome).Select(c => { c.AccountName = g.GetAccountName(dr["accountCode"].ToString()); return c; }).ToList();
                            }
                            else
                            {
                                result.Where(c => c.AccountCode == dr["accountCode"].ToString()).Select(c => { c.AccountName = g.GetAccountName(dr["accountCode"].ToString()); return c; }).ToList();
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

        public void InsertJV(List<JournalVoucher> _header, List<JournalVoucher> _detail)
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
