using SOFOS2_Migration_Tool.Accounting.Model;
using SOFOS2_Migration_Tool.Helper;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Accounting.Controller
{
    public class AccountCreditLimitController
    {

        string dropSitePath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "LOGS");
        string folder = "CreditLimit/";


        #region Public Methods

        #region GET

        public List<AccountCreditLimits> GetAccountCreditLimits(string date)
        {
            try
            {
                var result = new List<AccountCreditLimits>();

                var prm = new Dictionary<string, object>()
                {
                    { "@date", date },
                };

                using (var conn = new MySQLHelper(Global.SourceDatabase, AccountCreditLimitQuery.GetAccountCreditLimitQuery(), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new AccountCreditLimits
                            {
                                MemberId = dr["MemberId"].ToString(),
                                MemberName = dr["MemberName"].ToString(),
                                AccountNumber = dr["AccountNumber"].ToString(),
                                TransType = dr["TransType"].ToString(),
                                ShareCapital = Convert.ToDecimal(dr["ShareCapital"]),
                                ChargeAmount = Convert.ToDecimal(dr["ChargeAmount"]),
                                CreditLimit = Convert.ToDecimal(dr["CreditLimit"]),
                                AccountCode = dr["AccountCode"].ToString(),
                                Status = dr["Status"].ToString(),
                                CreditBalance = Convert.ToDecimal(dr["CreditBalance"]),
                                ChargeType = dr["ChargeType"].ToString(),
                                InterestRate = Convert.ToDecimal(dr["InterestRate"]),
                                Terms = Convert.ToDecimal(dr["Terms"]),
                                InterestAccount = dr["InterestAccount"].ToString(),
                                BranchCode = Global.BranchCode
                            });
                        }
                    }
                }

                return result;
            }
            catch
            {

                throw;
            }
        }

        #endregion GET
        #region INSERT

        public void InsertAccountCreditLimits(List<AccountCreditLimits> _header)
        {
            try
            {
                Global global = new Global();
                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {

                    foreach (var item in _header)
                    {
                        int accountCreditLimitId = GetAccountCreditLimitIdByMemberId(conn, item.MemberId);

                        if (accountCreditLimitId > 0)
                            continue;

                        #region Creation of Credit Limit
                        item.AccountNumber = global.GetLatestCreditLimitAccountNumber(conn);
                        CreateAccountCreditLimits(conn, item, global);

                        UpdateMemberShareCapital(conn, item.MemberId, item.ShareCapital);
                        #endregion Creation of Credit Limit

                    }

                    conn.CommitTransaction();
                }

            }
            catch
            {
                throw;
            }
        }

        public string InsertAccountCreditLimitsLogs(List<AccountCreditLimits> _header, string date)
        {

            string fileName = string.Format("AccountCreditLimits-{0}-{1}.csv", date.Replace(" / ", ""), DateTime.Now.ToString("ddMMyyyyHHmmss"));
            dropSitePath = Path.Combine(dropSitePath, folder);

            if (!Directory.Exists(dropSitePath))
                Directory.CreateDirectory(dropSitePath);

            ObjectToCSV<AccountCreditLimits> receiveFromVendorObjectToCSV = new ObjectToCSV<AccountCreditLimits>();
            string filename = Path.Combine(dropSitePath, fileName);
            receiveFromVendorObjectToCSV.SaveToCSV(_header, filename);
            return folder;
        }
        #endregion INSERT

        #region UPDATE
        public int UpdateTransactionAccountNumber()
        {
            int rowsAffected = 0;
            try
            {
                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    rowsAffected = UpdateSalesInvoiceAccountNumber(conn);
                    conn.CommitTransaction();

                    return rowsAffected;
                }
            }
            catch
            {
                throw;
            }
            
        }
        #endregion UPDATE

        #endregion Public Methods

        #region Private Methods
        private void CreateAccountCreditLimits(MySQLHelper conn, AccountCreditLimits item, Global global)
        {

            var param = new Dictionary<string, object>()
                        {
                            { "@memberId", item.MemberId },
                            { "@memberName", item.MemberName },
                            { "@accountCode", item.AccountCode },
                            { "@accountNumber", item.AccountNumber },
                            { "@transType", item.TransType },
                            { "@shareCapital", item.ShareCapital },
                            { "@chargeAmount", item.ChargeAmount },
                            { "@status", item.Status },
                            { "@creditBalance", item.CreditBalance },
                            { "@creditLimit", item.CreditLimit },
                            { "@chargeType", item.ChargeType },
                            { "@interestRate", item.InterestRate },
                            { "@terms", item.Terms },
                            { "@interestAccount", item.InterestAccount },
                            { "@branchCode", Global.BranchCode },


                        };

            conn.ArgSQLCommand = AccountCreditLimitQuery.InsertAccountCreditLimitQuery();
            conn.ArgSQLParam = param;

            //Execute insert header
            conn.ExecuteMySQL();


        }

        private int GetAccountCreditLimitIdByMemberId(MySQLHelper conn, string memberId)
        {
            int result = 0;
            conn.ArgSQLCommand = AccountCreditLimitQuery.GetAccountCreditLimitByMemberId();
            conn.ArgSQLParam = new Dictionary<string, object>() { { "@memberId", memberId } };
            using (var dr = conn.MySQLExecuteReaderBeginTransaction())
            {
                while (dr.Read())
                {
                    result = Convert.ToInt16(dr["accountNum"]);
                }
            }
            return result;
        }

        private void UpdateMemberShareCapital(MySQLHelper conn, string memberId, decimal shareCapital)
        {
            conn.ArgSQLCommand = AccountCreditLimitQuery.UpdateMemberShareCapital();
            conn.ArgSQLParam = new Dictionary<string, object>() { { "@memberId", memberId }, { "@shareCapital", shareCapital } };
            conn.ExecuteMySQL();
        }

        private int UpdateSalesInvoiceAccountNumber(MySQLHelper conn)
        {
            int rowsAffected = 0;
            rowsAffected = UpdateSalesInvoiceAccountNumberForMembersTransaction(conn);
            rowsAffected += UpdateSalesInvoiceAccountNumberForEmployeeTransaction(conn);
            return rowsAffected;
        }

        private int UpdateSalesInvoiceAccountNumberForMembersTransaction(MySQLHelper conn)
        {
            conn.ArgSQLCommand = AccountCreditLimitQuery.UpdateSalesInvoiceAccountNumberForMembersTransaction();
            return conn.ExecuteMySQL();
        }
        private int UpdateSalesInvoiceAccountNumberForEmployeeTransaction(MySQLHelper conn)
        {
            conn.ArgSQLCommand = AccountCreditLimitQuery.UpdateSalesInvoiceAccountNumberForEmployeeTransaction();
            return conn.ExecuteMySQL();
        }
        #endregion Private Methods
    }
}
