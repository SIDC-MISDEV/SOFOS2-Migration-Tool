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
        Dictionary<string, string> accounts = new Dictionary<string, string>();
        Dictionary<string, string> paymentmode = new Dictionary<string, string>();

        public List<CollectionReceipt> GetCollectionReceiptHeader(string date, string transprefix)
        {
            try
            {
                var result = new List<CollectionReceipt>();
                string principalaccount = "112010000000001";
                string oldinterestaccount = "441200000000000";
                string newinterestaccount = "430400000000000";
                string duetointercompany = "212010000000000";
                

                var filter = new Dictionary<string, object>()
                {
                    { "@transdate", date },
                    { "@principalaccount", principalaccount },
                    { "@oldinterestaccount", oldinterestaccount },
                    { "@newinterestaccount", newinterestaccount },
                    { "@duetointercompany", duetointercompany },
                    { "@transprefix", transprefix }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetQuery(payment.CRHeader), filter))
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
                g = new Global();
                var result = new List<CollectionReceipt>();
                paymentmode = g.GetAllPaymentMode();
                accounts = g.GetAllAccountCode();

                string principalaccount = "112010000000001";
                string oldinterestaccount = "441200000000000";
                string newinterestaccount = "430400000000000";
                string duetointercompany = "212010000000000";


                var filter = new Dictionary<string, object>()
                {
                    { "@transdate", date },
                    { "@principalaccount", principalaccount },
                    { "@oldinterestaccount", oldinterestaccount },
                    { "@newinterestaccount", newinterestaccount },
                    { "@duetointercompany", duetointercompany },
                    { "@cash", paymentmode.FirstOrDefault(n => n.Value == "CASH").Key },
                    { "@check", paymentmode.FirstOrDefault(n => n.Value == "CHECK").Key },
                    { "@giftcheck", paymentmode.FirstOrDefault(n => n.Value == "Gift Check").Key },
                    { "@transprefix", transprefix }
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, PaymentQuery.GetQuery(payment.CRDetail), filter))
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
                                DetRefTransType = dr["refTransType"].ToString(),
                                Reference = dr["reference"].ToString(),
                            });
                            g = new Global();
                            if (dr["accountCode"].ToString() == principalaccount)
                            {
                                result.Where(c => c.AccountCode == principalaccount).Select(c => { c.AccountName = accounts[dr["accountCode"].ToString()]; return c; }).ToList();
                            }
                            else if (dr["accountCode"].ToString() == newinterestaccount)
                            {
                                result.Where(c => c.AccountCode == newinterestaccount).Select(c => { c.AccountName = accounts[dr["accountCode"].ToString()]; return c; }).ToList();
                            }
                            else if (dr["accountCode"].ToString() == oldinterestaccount)
                            {
                                result.Where(c => c.AccountCode == oldinterestaccount).Select(c => { c.AccountName = accounts[dr["accountCode"].ToString()]; return c; }).ToList();
                            }
                            else if (dr["accountCode"].ToString() == duetointercompany)
                            {
                                result.Where(c => c.AccountCode == duetointercompany).Select(c => { c.AccountName = accounts[dr["accountCode"].ToString()]; return c; }).ToList();
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


        public void InsertCR(List<CollectionReceipt> _header, List<CollectionReceipt> _detail)
        {
            try
            {
                
                Global g = new Global();
                int transNum = 0;
                long series = 0;
                int detailNum = 0;

                using (var conns = new MySQLHelper(Global.SourceDatabase))
                {
                    using (var conn = new MySQLHelper(Global.DestinationDatabase))
                    {
                        transNum = g.GetLatestTransNum("fp000", "transNum");
                        var reference = g.GetCRReference("sst00", "series");
                        accounts = g.GetAllAccountCode();
                        paymentmode = g.GetAllPaymentMode();
                        detailNum = g.GetLatestDetailNum();

                        foreach (var item in _header)
                        {
                            
                            var param = new Dictionary<string, object>()
                            {
                                { "@transNum", transNum },
                                { "@reference", reference },
                                { "@Total", item.Total },
                                { "@transDate", item.TransDate },
                                { "@idUser", item.IdUser },
                                { "@memberId", item.MemberId },
                                { "@memberName", item.MemberName },
                                { "@status", item.Status },
                                { "@cancelled", item.Cancelled },
                                { "@remarks", item.Remarks },
                                { "@type", item.sType },
                                { "@accountCode", item.AccountCode },
                                { "@paidBy", item.PaidBy },
                                { "@branchCode", Global.BranchCode },
                                { "@extracted", item.Extracted },
                                { "@transType", item.TransType },
                                { "@series", "" },
                                { "@AccountNo", item.AccountNumber },
                                { "@refTransType", item.RefTransType }
                            };

                            conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.CRHeader);
                            conn.ArgSQLParam = param;

                            //Execute insert header
                            conn.ExecuteMySQL();

                            #region Insert Details
                            var details = _detail.Where(n => n.Reference == item.Reference).ToList();

                            foreach (var detail in details)
                            {
                                if (detail.AccountCode != paymentmode.FirstOrDefault(n => n.Value == "CASH").Key && detail.AccountCode != paymentmode.FirstOrDefault(n => n.Value == "CHECK").Key && detail.AccountCode != paymentmode.FirstOrDefault(n => n.Value == "Gift Check").Key)
                                {
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

                                    conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.CRDetail);
                                    conn.ArgSQLParam = detailParam;

                                    //execute insert detail
                                    var cmdDetail = conn.ExecuteMySQL();
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
                                        { "@orDetailNum", detailNum }
                                    };

                                        conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.ORPayment);
                                        conn.ArgSQLParam = paymentOR;
                                        conn.ExecuteMySQL();
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
                                        { "@orDetailNum", detailNum }
                                    };

                                        conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.ORPayment);
                                        conn.ArgSQLParam = paymentOR;
                                        conn.ExecuteMySQL();
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
                                        { "@orDetailNum", detailNum }
                                    };

                                        conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.ORPayment);
                                        conn.ArgSQLParam = paymentOR;
                                        conn.ExecuteMySQL();
                                    }
                                }
                            }
                            
                            #endregion

                            transNum++;
                            detailNum++;
                            series = Convert.ToInt32(reference.Replace("CR", "")) + 1;
                            reference = g.NextReference(series.ToString());

                            conns.ArgSQLCommand = Query.UpdateTagging("mextracted", "ledger");
                            conns.ArgSQLParam = new Dictionary<string, object>() { { "@value", "1" }, { "@reference", item.Reference } };
                            conns.ExecuteMySQL();


                        }

                        conn.ArgSQLCommand = Query.UpdateReferenceCount();
                        conn.ArgSQLParam = new Dictionary<string, object>() { { "@series", series - 1 }, { "@transtype", "CR" } };
                        conn.ExecuteMySQL();
                        conn.CommitTransaction();
                    }
                    conns.CommitTransaction();
                }
            }
            catch
            {

                throw;
            }
        }
        //public void InsertCR(List<CollectionReceipt> _header, List<CollectionReceipt> _detail)
        //{
        //    try
        //    {
        //        Global g = new Global();
        //        int transNum = 0;
        //        long series = 0;

        //        using (var conn = new MySQLHelper(Global.DestinationDatabase))
        //        {


        //            transNum = g.GetLatestTransNum("fp000", "transNum");

        //            foreach (var item in _header)
        //            {
        //                var reference = g.GetCRReference("sst00", "series","CR");
        //                var param = new Dictionary<string, object>()
        //                {
        //                    { "@transNum", transNum },
        //                    { "@reference", reference },
        //                    { "@Total", item.Total },
        //                    { "@transDate", item.TransDate },
        //                    { "@idUser", item.IdUser },
        //                    { "@memberId", item.MemberId },
        //                    { "@memberName", item.MemberName },
        //                    { "@status", item.Status },
        //                    { "@cancelled", item.Cancelled },
        //                    { "@remarks", item.Remarks },
        //                    { "@type", item.sType },
        //                    { "@accountCode", item.AccountCode },
        //                    { "@paidBy", item.PaidBy },
        //                    { "@branchCode", Global.BranchCode },
        //                    { "@extracted", item.Extracted },
        //                    { "@transType", item.TransType },
        //                    { "@series", "" },
        //                    { "@AccountNo", item.AccountNumber },
        //                    { "@refTransType", item.RefTransType }
        //                };

        //                conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.CRHeader);
        //                conn.ArgSQLParam = param;

        //                //Execute insert header
        //                conn.ExecuteMySQL();

        //                #region Insert Details
        //                var details = _detail.Where(n => n.Reference == item.Reference).ToList();

        //                foreach (var detail in details)
        //                {
        //                    var detailParam = new Dictionary<string, object>()
        //                        {
        //                            {"@transNum", transNum },
        //                            {"@crossReference", "" },
        //                            {"@amount", detail.Amount },
        //                            {"@idUser", detail.IdUser },
        //                            {"@balance", detail.Balance },
        //                            {"@accountCode", detail.AccountCode },
        //                            {"@pType", detail.pType },
        //                            {"@accountName", detail.AccountName },
        //                            {"@refTransType", detail.DetRefTransType }
        //                        };

        //                    conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.CRDetail);
        //                    conn.ArgSQLParam = detailParam;

        //                    //execute insert detail
        //                    var cmdDetail = conn.ExecuteMySQL();
        //                }
        //                #endregion

        //                transNum++;
        //                series = Convert.ToInt32(reference.Replace("CR", "")) + 1;
        //            }

        //            conn.ArgSQLCommand = Query.UpdateReferenceCount();
        //            conn.ArgSQLParam = new Dictionary<string, object>() { { "@series", series - 1 }, { "@transtype", "CR" } };
        //            conn.ExecuteMySQL();


        //            conn.CommitTransaction();
        //        }

        //    }
        //    catch
        //    {

        //        throw;
        //    }
        //}
    }
}
