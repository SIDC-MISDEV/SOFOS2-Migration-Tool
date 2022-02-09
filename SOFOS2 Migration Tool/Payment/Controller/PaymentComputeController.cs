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
        decimal balance=0;
        int _count = 0;
        string _detailnum = string.Empty;
        bool isAllocated;

        public void ComputePayment(List<Payments> paymentlist, string date)
        {
            using (var conn = new MySQLHelper(Global.DestinationDatabase))
            {
                try
                {
                    conn.BeginTransaction();
                    foreach (var pl in paymentlist)
                    {
                        if (pl.AccountCode == "112010000000001")
                        {
                            var invoicelist = GetInvoice(conn, pl.MemberId, pl.AccountNumber, pl.Amount, pl.TransDate);

                            foreach (var il in invoicelist)
                            {
                                
                                var interestlist = ComputeInterest(conn, date, il.Reference, il.MemberId, il.AccountNumber, pl.TransDate, il.MemberName, pl.IdUser);
                                InsertInt(conn, interestlist);
                                if (invoicelist.Count > 0)
                                    UpdateInvoice(conn, il.TransNum, il.Reference, il.PaidToDate, il.IntComputed, il.LastPaymentDate, il.Status, il.AccountCode);
                                UpdatePayment(conn, pl.TransNum, pl.DetailNum, pl.MemberId, pl.AccountNumber, il.Reference, il.PaidToDate,il.PaidAmount, il.Total, pl.Amount,pl.IdUser,pl.AccountCode,invoicelist.Count);                                
                            }

                        }
                    }
                    conn.CommitTransaction();
                    
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }
            
        }

        public List<Invoice> GetInvoice(MySQLHelper conn, string memberid, string accountno, decimal amount, string date)
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

                conn.ArgSQLCommand = PaymentQuery.GetQuery(payment.Invoice);
                conn.ArgSQLParam = filter;

                using (var dr = conn.MySQLExecuteReaderBeginTransaction())
                {
                    while (dr.Read())
                    {
                        while (amount > 0)
                        {
                            decimal _paidtodate = 0;
                            var _asd = (Convert.ToDecimal(dr["total"])) ;
                            _paidtodate = amount > (Convert.ToDecimal(dr["total"])) - Convert.ToDecimal(dr["paidToDate"]) ? (Convert.ToDecimal(dr["total"])) - Convert.ToDecimal(dr["paidToDate"]) : amount;
                            var test = Convert.ToDecimal(dr["total"]);
                            result.Add(new Invoice
                            {
                                TransNum = dr["transNum"].ToString(),
                                TransDate = dr["transDate"].ToString(),
                                Reference = dr["reference"].ToString(),
                                MemberId = dr["memberId"].ToString(),
                                MemberName = dr["memberName"].ToString(),
                                AccountCode = dr["accountCode"].ToString(),
                                PaidToDate = Convert.ToDecimal(dr["paidToDate"]) + _paidtodate,
                                PaidAmount = _paidtodate,
                                Total = Convert.ToDecimal(dr["total"]),
                                Cancelled = Convert.ToBoolean(dr["cancelled"]),
                                Status = Convert.ToDecimal(dr["total"]) == Convert.ToDecimal(dr["paidToDate"]) + _paidtodate ? "CLOSED" : "OPEN",
                                IntComputed = 1,
                                LastPaymentDate = date,
                                AccountNumber = dr["AccountNo"].ToString()
                            });
                            amount = amount - _paidtodate;
                            break;
                        }
                        if (amount == 0)
                            break;


                        //g = new Global();
                        //result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.MemberName = g.GetMemberName(dr["memberId"].ToString()); return c; }).ToList();
                        //result.Where(c => c.MemberId == dr["memberId"].ToString()).Select(c => { c.AccountNumber = g.GetAccountNumber(dr["memberId"].ToString(), dr["refTransType"].ToString()); return c; }).ToList();
                    }
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Interest> ComputeInterest(MySQLHelper conn, string transdate, string reference, string memberid, string accountnumber, string paymenttransdate, string membername, string iduser)
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

                conn.ArgSQLCommand = PaymentQuery.GetQuery(payment.Interest);
                conn.ArgSQLParam = filter;

                using (var dr = conn.MySQLExecuteReaderBeginTransaction())
                {
                    while (dr.Read())
                    {
                        result.Add(new Interest
                        {

                            InterestAmount = Convert.ToDecimal(dr["intAmount"]),
                            TransDate = paymenttransdate,
                            CrossReference = reference,
                            AccountNumber = accountnumber,
                            TransType = "IN",
                            RefTransType = reference.Substring(0, 2),
                            MemberId = memberid,
                            MemberName = membername,
                            IdUser = iduser,
                            AccountCode = "441200000000000",

                        });
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

                using (var conn = new MySQLHelper(Global.DestinationDatabase, PaymentQuery.GetQuery(payment.TransactionPayments), filter))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new Payments
                            {
                                TransNum = dr["transnum"].ToString(),
                                DetailNum = dr["detailNum"].ToString(),
                                TransDate = dr["transDate"].ToString(),
                                Reference = dr["reference"].ToString(),
                                MemberId = dr["memberId"].ToString(),
                                Amount = Convert.ToDecimal(dr["amount"]),
                                AccountNumber = dr["AccountNo"].ToString(),
                                AccountCode = dr["AccountCode"].ToString(),
                                Balance = Convert.ToDecimal(dr["balance"]),
                                IdUser = dr["idUser"].ToString()
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

        public string getBalance(MySQLHelper conn, string transnum, string principalaccount, string crossreference)
        {
            string result = string.Empty;
            try
            {
                var filter = new Dictionary<string, object>()
                {
                    { "@transnum", transnum },
                    { "@crossreference", crossreference },
                    { "@principalaccount", principalaccount }
                };
                conn.ArgSQLCommand = PaymentQuery.GetQuery(payment.Balance);
                conn.ArgSQLParam = filter;

                using (var dr = conn.MySQLExecuteReaderBeginTransaction())
                {
                    result = dr["balance"].ToString();
                }
                
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void UpdateInvoice(MySQLHelper conn, string transnum, string reference, decimal paidtodate, int intcomputed, string lastpaymentdate, string status, string accountcode)
        {
            try
            {
                Global g = new Global();
                int transNum = 0;
                long series = 0;

                #region Update CI
                if (reference.Substring(0, 2) == "CI")
                {
                    var param = new Dictionary<string, object>()
                        {
                            { "@reference", reference },
                            { "@paidtodate", paidtodate },
                            { "@intcomputed", intcomputed },
                            { "@lastpaymentdate", lastpaymentdate },
                            { "@status", status }
                        };

                    conn.ArgSQLCommand = PaymentQuery.UpdateQuery(payment.Invoice);
                    conn.ArgSQLParam = param;

                    //Execute insert header
                    conn.ExecuteMySQL();
                }
                #endregion
                #region Update JV
                else if (reference.Substring(0, 2) == "JV")
                {
                    var param = new Dictionary<string, object>()
                        {
                            { "@transnum", transnum },
                            { "@accountcode", accountcode },
                            { "@paidtodate", paidtodate },
                            { "@intcomputed", intcomputed },
                            { "@lastpaymentdate", lastpaymentdate },
                            { "@status", status }
                        };

                    conn.ArgSQLCommand = PaymentQuery.UpdateQuery(payment.JVInvoice);
                    conn.ArgSQLParam = param;

                    //Execute insert header
                    conn.ExecuteMySQL();
                }
                    #endregion

            }
            catch
            {

                throw;
            }
        }

        private void InsertNewCrDetail(MySQLHelper conn)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateAccountCreditLimit(MySQLHelper conn, string memberid, string accountnumber, decimal amount)
        {
            try
            {
                Global g = new Global();
                var param = new Dictionary<string, object>()
                        {
                            { "@memberid", memberid },
                            { "@accountno", accountnumber },
                            { "@amount", amount },
                        };

                conn.ArgSQLCommand = PaymentQuery.UpdateQuery(payment.CreditLimit);
                conn.ArgSQLParam = param;

                //Execute insert header
                conn.ExecuteMySQL();
                //conn.CommitTransaction();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdatePayment(MySQLHelper conn, string transnum, string detailnum, string memberid, string accountnumber, string crossreference, decimal paidtodate, decimal paidamount, decimal total, decimal paymentamount, string iduser, string accountcode, int hasallocation)
        {
            try
            {
                Global g = new Global();

                #region Update Payment
                
                if (total==paidtodate && hasallocation>1 && isAllocated==false)
                {
                    _count = hasallocation-1;
                    var param = new Dictionary<string, object>()
                        {
                            { "@transnum", transnum },
                            { "@detailnum", detailnum },
                            { "@memberid", memberid },
                            { "@accountno", accountnumber },
                            { "@amount", paidamount },
                            { "@crossreference", crossreference },
                            { "@accountcode", accountcode },
                            { "@balance", 0 }
                        };

                    conn.ArgSQLCommand = PaymentQuery.UpdateQuery(payment.TransactionPayments);
                    conn.ArgSQLParam = param;

                    //Execute insert header
                    conn.ExecuteMySQL();

                    UpdateAccountCreditLimit(conn, memberid, accountnumber, paidamount);
                    isAllocated = true;
                    _count = _count - 1;
                    
                }

                else if (total == paidtodate && hasallocation == 1 && isAllocated == false)
                {
                    var param = new Dictionary<string, object>()
                        {
                            { "@transnum", transnum },
                            { "@detailnum", detailnum },
                            { "@memberid", memberid },
                            { "@accountno", accountnumber },
                            { "@amount", paidamount },
                            { "@crossreference", crossreference },
                            { "@accountcode", accountcode },
                            { "@balance", 0 }
                        };

                    conn.ArgSQLCommand = PaymentQuery.UpdateQuery(payment.TransactionPayments);
                    conn.ArgSQLParam = param;

                    //Execute insert header
                    conn.ExecuteMySQL();

                    UpdateAccountCreditLimit(conn, memberid, accountnumber, paidamount);
                    
                }
                else if (total != paidtodate && hasallocation == 1 && isAllocated == false)
                {
                   
                    var param = new Dictionary<string, object>()
                        {
                            { "@transnum", transnum },
                            { "@detailnum", detailnum },
                            { "@memberid", memberid },
                            { "@accountno", accountnumber },
                            { "@amount", paidamount },
                            { "@crossreference", crossreference },
                            { "@accountcode", accountcode },
                            { "@balance", 0 }
                        };

                    conn.ArgSQLCommand = PaymentQuery.UpdateQuery(payment.TransactionPayments);
                    conn.ArgSQLParam = param;

                    //Execute insert header
                    conn.ExecuteMySQL();

                    UpdateAccountCreditLimit(conn, memberid, accountnumber, paidamount);

                }
                else if (total != paidtodate && _count == 0 && isAllocated == false)
                {
                    string accountname = g.GetAccountName(accountcode);
                    var param = new Dictionary<string, object>()
                            {
                                { "@transNum", transnum },
                                { "@amount", paidamount },
                                { "@idUser", iduser },
                                { "@accountcode", accountcode },
                                { "@pType", "P" },
                                { "@balance", 0 },
                                { "@crossreference", crossreference },
                                { "@accountName", accountname }

                            };

                    conn.ArgSQLCommand = PaymentQuery.UpdateQuery(payment.TransactionPayments);
                    conn.ArgSQLParam = param;

                    //Execute insert header
                    conn.ExecuteMySQL();
                    UpdateAccountCreditLimit(conn, memberid, accountnumber, paidamount);

                }
                else if (total == paidtodate && _count != 0 && isAllocated == true)
                {
                    //pending process
                    balance = balance - paymentamount;
                    string accountname = g.GetAccountName(accountcode);
                    var param = new Dictionary<string, object>()
                            {
                                { "@transNum", transnum },
                                { "@amount", paidamount },
                                { "@idUser", iduser },
                                { "@accountcode", accountcode },
                                { "@pType", "P" },
                                { "@balance", 0 },
                                { "@crossreference", crossreference },
                                { "@accountName", accountname }

                            };

                    conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.NewCRDetail);
                    conn.ArgSQLParam = param;

                    //Execute insert header
                    conn.ExecuteMySQL();

                    UpdateAccountCreditLimit(conn, memberid, accountnumber, paidamount);
                    isAllocated = true;
                    _count = _count - 1;
                    //balance = 0;
                }
                else if (total != paidtodate && _count == 0 && isAllocated == true)
                {
                    string accountname = g.GetAccountName(accountcode);
                    var param = new Dictionary<string, object>()
                            {
                                { "@transNum", transnum },
                                { "@amount", paidamount },
                                { "@idUser", iduser },
                                { "@accountcode", accountcode },
                                { "@pType", "P" },
                                { "@balance", 0 },
                                { "@crossreference", crossreference },
                                { "@accountName", accountname }

                            };

                    conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.NewCRDetail);
                    conn.ArgSQLParam = param;

                    //Execute insert header
                    conn.ExecuteMySQL();
                    UpdateAccountCreditLimit(conn, memberid, accountnumber, paidamount);
                    isAllocated = false;
                    balance = 0;

                }
                else if (total == paidtodate && _count == 0 && isAllocated == true)
                {
                    string accountname = g.GetAccountName(accountcode);
                    var param = new Dictionary<string, object>()
                            {
                                { "@transNum", transnum },
                                { "@amount", paidamount },
                                { "@idUser", iduser },
                                { "@accountcode", accountcode },
                                { "@pType", "P" },
                                { "@balance", 0 },
                                { "@crossreference", crossreference },
                                { "@accountName", accountname }

                            };

                    conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.NewCRDetail);
                    conn.ArgSQLParam = param;

                    //Execute insert header
                    conn.ExecuteMySQL();
                    UpdateAccountCreditLimit(conn, memberid, accountnumber, paidamount);
                    isAllocated = false;
                    balance = 0;
                }
                else
                {
                }
                
                #endregion

            }
            catch
            {

                throw;
            }
        }

        public void InsertInt(MySQLHelper conn, List<Interest> _interest)
        {
            try
            {
                Global g = new Global();
                int transNum = 0;
                long series = 0;

                transNum = g.GetLatestTransNum("fint0", "transNum");

                foreach (var item in _interest)
                {
                    if (item.InterestAmount != 0)
                    {
                        var reference = g.GetCRReference("sst00", "series", "IN");
                        var param = new Dictionary<string, object>()
                        {
                            { "@transNum", transNum },
                            { "@transDate", item.TransDate },
                            { "@transtype", item.TransType },
                            { "@reftranstype", item.RefTransType },
                            { "@reference", reference },
                            { "@amount", item.InterestAmount },
                            { "@memberId", item.MemberId },
                            { "@memberName", item.MemberName },
                            { "@accountCode", item.AccountCode },
                            { "@idUser", item.IdUser },
                            { "@AccountNo", item.AccountNumber },
                            { "@crossreference", item.CrossReference },
                        };

                        conn.ArgSQLCommand = PaymentQuery.InsertQuery(payment.Interest);
                        conn.ArgSQLParam = param;

                        //Execute insert header
                        conn.ExecuteMySQL();

                        transNum++;
                        series = Convert.ToInt32(reference.Replace("IN", "")) + 1;
                    }
                    
                }

                conn.ArgSQLCommand = Query.UpdateReferenceCount();
                conn.ArgSQLParam = new Dictionary<string, object>() { { "@series", series - 1 }, { "@transtype", "IN" } };
                conn.ExecuteMySQL();


            }
            catch
            {

                throw;
            }
        }

        public void ArrangeDetailNum(MySQLHelper conn, string table, string field)
        {
            try
            {
                var result = new List<Payments>();
                conn.ArgSQLCommand = Query.ArrangeDetailNum(table, "detailNum, transNum");
                using (var dr = conn.MySQLExecuteReaderBeginTransaction())
                {
                    while (dr.Read())
                    {
                        result.Add(new Payments
                        {
                            DetailNum = dr["detailNum"].ToString(),
                            TransNum = dr["transNum"].ToString(),
                        });
                    }
                }

                var _countrecord = CountRecord(conn, table, field);
                int ctr = 1;
                DropPrimaryKey(conn, table, field);



                foreach (var item in result)
                {
                    conn.ArgSQLCommand = Query.UpdateDetailNum(table, field);
                    conn.ArgSQLParam = new Dictionary<string, object>() { { "@value", ctr }, { "@detailNum", item.DetailNum }, { "@transNum", item.TransNum } };
                    conn.ExecuteMySQL();

                    ctr++;
                }
                //conn.CommitTransaction();
                AlterPrimaryKey(conn, table, field);

            }
            catch (Exception)
            {

                throw;
            }
            


        }
        public void DropPrimaryKey(MySQLHelper conn, string table, string field)
        {
            conn.ArgSQLCommand = Query.DropPrimaryKey(table, field);
            conn.ExecuteMySQL();
            //conn.CommitTransaction();
        }

        public void AlterPrimaryKey(MySQLHelper conn, string table, string field)
        {
            conn.ArgSQLCommand = Query.AlterPrimaryKey(table, field);
            conn.ExecuteMySQL();
            //conn.CommitTransaction();
        }

        private string CountRecord(MySQLHelper conn, string table, string field)
        {
            try
            {
                var result = string.Empty;
                conn.ArgSQLCommand = Query.CountRecord(table, field);

                using (var dr = conn.MySQLExecuteReaderBeginTransaction())
                {
                    while (dr.Read())
                    {
                        result = dr["count"].ToString();
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
