using SOFOS2_Migration_Tool.Payment.Model;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Payment.Controller
{
    class OfficialReceiptController
    {
        string accountCode = Properties.Settings.Default.CASH_ACCOUNTCODE;
        Dictionary<string, string> paymentmode = new Dictionary<string, string>();
        Dictionary<string, string> accounts = new Dictionary<string, string>();
        Global g = null;

        public List<OfficialReceipt> GetOfficialReceiptHeader(string date, string transprefix)
        {
            try
            {
                var result = new List<OfficialReceipt>();
                var reference = new List<string>();

                var filter = new Dictionary<string, object>()
                {
                    { "@transdate", date },
                    { "@transprefix", transprefix }
                };

                using (var conn = new MySQLHelper(Global.DestinationDatabase, PaymentQuery.GetQuery(payment.ORReference)))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            reference.Add(dr["reference"].ToString());
                        }
                    }
                }

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetQuery(payment.ORHeader), filter))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            if (!reference.Contains(dr["reference"].ToString()))
                            {
                                result.Add(new OfficialReceipt
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
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<OfficialReceipt> GetOfficialReceiptDetail(string date, string transprefix)
        {
            try
            {
                var result = new List<OfficialReceipt>();
                g = new Global();
                paymentmode = g.GetAllPaymentMode();
                accounts = g.GetAllAccountCode();


                var filter = new Dictionary<string, object>()
                {
                    { "@transdate", date },
                    { "@transprefix", transprefix },
                    
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetQuery(payment.ORDetail), filter))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new OfficialReceipt
                            {
                                Amount = Convert.ToDecimal(dr["amount"]),
                                IdUser = dr["idUser"].ToString(),
                                AccountCode = dr["accountCode"].ToString(),
                                pType = dr["pType"].ToString(),
                                Reference = dr["reference"].ToString(),
                            });

                            g = new Global();
                            if (dr["accountCode"].ToString() != paymentmode.FirstOrDefault(n => n.Value == "CASH").Key && dr["accountCode"].ToString() != paymentmode.FirstOrDefault(n => n.Value == "CHECK").Key && dr["accountCode"].ToString() != paymentmode.FirstOrDefault(n => n.Value == "Gift Check").Key)
                            {
                                result.Where(c => c.AccountCode == dr["accountCode"].ToString()).Select(c => { c.AccountName = accounts[dr["accountCode"].ToString()]; return c; }).ToList();
                            }
                            else if (dr["accountCode"].ToString() == paymentmode.FirstOrDefault(n => n.Value == "CASH").Key)
                            {
                                result.Where(c => c.AccountCode == paymentmode.FirstOrDefault(n => n.Value == "CASH").Key).Select(c => { c.AccountName = accounts[dr["accountCode"].ToString()]; return c; }).ToList();
                            }
                            else if (dr["accountCode"].ToString() == paymentmode.FirstOrDefault(n => n.Value == "CHECK").Key)
                            {
                                result.Where(c => c.AccountCode == paymentmode.FirstOrDefault(n => n.Value == "CHECK").Key).Select(c => { c.AccountName = accounts[dr["accountCode"].ToString()]; return c; }).ToList();
                            }
                            else if (dr["accountCode"].ToString() == paymentmode.FirstOrDefault(n => n.Value == "Gift Check").Key)
                            {
                                result.Where(c => c.AccountCode == paymentmode.FirstOrDefault(n => n.Value == "Gift Check").Key).Select(c => { c.AccountName = accounts[dr["accountCode"].ToString()]; return c; }).ToList();
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

      

        public void InsertOR(List<OfficialReceipt> _header, List<OfficialReceipt> _detail)
        {
            try
            {
                Dictionary<string, string> accounts = new Dictionary<string, string>();
                Global g = new Global();
                int transNum = 0;
                long series = 0;
                int detailNum = 0;
                string memberName = string.Empty;
                int _transnum = 0;
                int _detailnum = 0;
                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    conn.BeginTransaction();

                    transNum = g.GetLatestTransNum("fp000", "transNum");
                    accounts = g.GetAllAccountCode();
                    detailNum = g.GetLatestDetailNum() - 1;
                    paymentmode = g.GetAllPaymentMode();

                    foreach (var item in _header)
                    {
                        item.Series = g.GetBIRSeries(conn, "OR");
                        memberName = g.GetFileName(item.MemberId);
                        g.UpdateBIRSeries(conn, "OR");
                        var param = new Dictionary<string, object>()
                        {
                            { "@transNum", transNum },
                            { "@reference", item.Reference},
                            { "@Total", item.Total },
                            { "@transDate", item.TransDate },
                            { "@idUser", item.IdUser },
                            { "@memberId", item.MemberId },
                            { "@memberName", memberName },
                            { "@status", item.Status },
                            { "@cancelled", item.Cancelled },
                            { "@remarks", item.Remarks },
                            { "@type", item.sType },
                            { "@accountCode", item.AccountCode },
                            { "@paidBy", item.PaidBy },
                            { "@branchCode", Global.BranchCode },
                            { "@extracted", item.Extracted },
                            { "@transType", item.TransType },
                            { "@series", item.Series },
                            { "@AccountNo", item.AccountNumber },
                            { "@refTransType", item.RefTransType }
                        };

                        conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.ORHeader);
                        conn.ArgSQLParam = param;

                        //Execute insert header
                        conn.ExecuteMySQL();


                        #region Insert Details
                        var details = _detail.Where(n => n.Reference == item.Reference).ToList();
                        
                        foreach (var detail in details)
                        {

                            detailNum++;
                            var test = paymentmode.FirstOrDefault(n => n.Value == "CASH").Key;
                            if (detail.AccountCode != paymentmode.FirstOrDefault(n => n.Value == "CASH").Key && detail.AccountCode != paymentmode.FirstOrDefault(n => n.Value == "CHECK").Key && detail.AccountCode != paymentmode.FirstOrDefault(n => n.Value == "Gift Check").Key)
                            {
                                
                                if (_transnum != transNum)
                                {
                                    _transnum = transNum;
                                    _detailnum = detailNum;
                                }


                                var detailParam = new Dictionary<string, object>()
                                {
                                    {"@transNum", transNum },
                                    {"@crossReference", "" },
                                    {"@amount", detail.Amount },
                                    {"@idUser", detail.IdUser },
                                    {"@balance", detail.Balance },
                                    {"@accountCode", detail.AccountCode },
                                    {"@pType", detail.pType },
                                    {"@accountName", detail.AccountName },
                                    {"@refTransType", detail.DetRefTransType }
                                };

                                conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.ORDetail);
                                conn.ArgSQLParam = detailParam;

                                //execute insert detail
                                var cmdDetail = conn.ExecuteMySQL();
                                
                            }

                            if (_transnum != transNum)
                            {
                                _transnum = transNum;
                                _detailnum = detailNum - 1;
                            }

                            if (paymentmode.ContainsKey(detail.AccountCode))
                            {
                                if (paymentmode[detail.AccountCode] == "CASH")
                                {
                                    var paymentOR = new Dictionary<string, object>()
                                    {
                                        { "@transNum", transNum },
                                        { "@paymentCode", paymentmode[detail.AccountCode] },
                                        { "@amount", item.Total },
                                        { "@idUser", item.IdUser },
                                        { "@transtype", "OR" },
                                        { "@accountcode", detail.AccountCode },
                                        { "@accountName", detail.AccountName },
                                        { "@orDetailNum", _detailnum }
                                    };

                                    conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.ORPayment);
                                    conn.ArgSQLParam = paymentOR;
                                    conn.ExecuteMySQL();
                                    _detailnum = detailNum;
                                }

                               else if (paymentmode[detail.AccountCode] == "CHECK")
                                {
                                    var paymentOR = new Dictionary<string, object>()
                                    {
                                        { "@transNum", transNum },
                                        { "@paymentCode", paymentmode[detail.AccountCode] },
                                        { "@amount", item.Total },
                                        { "@idUser", item.IdUser },
                                        { "@transtype", "OR" },
                                        { "@accountcode", detail.AccountCode },
                                        { "@accountName", detail.AccountName },
                                        { "@orDetailNum", _detailnum }
                                    };

                                    conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.ORPayment);
                                    conn.ArgSQLParam = paymentOR;
                                    conn.ExecuteMySQL();
                                    _detailnum = detailNum - 1;
                                }

                                else if (paymentmode[detail.AccountCode] == "Gift Check")
                                {
                                    var paymentOR = new Dictionary<string, object>()
                                    {
                                        { "@transNum", transNum },
                                        { "@paymentCode", paymentmode[detail.AccountCode] },
                                        { "@amount", item.Total },
                                        { "@idUser", item.IdUser },
                                        { "@transtype", "OR" },
                                        { "@accountcode", detail.AccountCode },
                                        { "@accountName", detail.AccountName },
                                        { "@orDetailNum", _detailnum }
                                    };

                                    conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.ORPayment);
                                    conn.ArgSQLParam = paymentOR;
                                    conn.ExecuteMySQL();
                                    _detailnum = detailNum - 1;
                                }
                            }

                           
                        }

                        //var paymentOR = new Dictionary<string, object>()
                        //{
                        //    { "@transNum", transNum },
                        //    { "@paymentCode", "CASH" },
                        //    { "@amount", item.Total },
                        //    { "@idUser", item.IdUser },
                        //    { "@transtype", "OR" },
                        //    { "@accountcode", accountCode },
                        //    { "@accountName", accounts[accountCode] },
                        //    { "@orDetailNum", detailNum }
                        //};

                        //conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.ORPayment);
                        //conn.ArgSQLParam = paymentOR;
                        //conn.ExecuteMySQL();
                        #endregion

                        transNum++;
                        
                        series = Convert.ToInt32(item.Reference.Replace("OR", "")) + 1;
                        conn.ArgSQLCommand = Query.UpdateORDetailNum();
                        conn.ArgSQLParam = new Dictionary<string, object>() { { "@transtype", "OR" } };
                        conn.ExecuteMySQL();

                    }

                    conn.ArgSQLCommand = Query.UpdateReferenceCount();
                    conn.ArgSQLParam = new Dictionary<string, object>() { { "@series", series - 1 }, { "@transtype", "OR" } };
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
