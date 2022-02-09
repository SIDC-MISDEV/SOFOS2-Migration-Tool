using SOFOS2_Migration_Tool.Helper;
using SOFOS2_Migration_Tool.Inventory.Model;
using SOFOS2_Migration_Tool.Sales.Model;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Sales.Controller
{
    public class ReComputeSalesCreditController
    {
        string dropSitePath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "LOGS");
        string folder = "Recompute/";

        public List<ReComputeSalesCreditModel> GetSalesAndReturnFromCustomerTransactions(string date)
        {
            try
            {
                var result = new List<ReComputeSalesCreditModel>();
                var prm = new Dictionary<string, object>() { { "@date", date } };

                using (var conn = new MySQLHelper(Global.DestinationDatabase, ReComputeSalesCreditQuery.GetSalesAndReturnFromCustomerTransactions(), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new ReComputeSalesCreditModel
                            {
                                TransactionType = dr["transtype"].ToString(),
                                MemberCode = dr["MemberId"].ToString(),
                                Amount = Convert.ToDecimal(dr["amount"])
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

        public void UpdateChargeAmount(List<ReComputeSalesCreditModel> _transactions)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>(),
                    itemParam = new Dictionary<string, object>();

                StringBuilder sQuery = new StringBuilder();

                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    conn.BeginTransaction();

                    foreach (var transaction in _transactions)
                    {

                        conn.ArgSQLCommand = ReComputeSalesCreditQuery.UpdateAccountCreditLimit();
                        conn.ArgSQLParam = new Dictionary<string, object>()
                        {
                            { "@memberId", transaction.MemberCode },
                            { "@transactionType", transaction.TransactionType },
                            { "@amount", transaction.Amount }
                        };
                        conn.ExecuteMySQL();


                    }

                    conn.CommitTransaction();
                }
            }
            catch
            {
                throw;
            }
        }

        public string UpdateChargeAmountLogs(List<ReComputeSalesCreditModel> _header, string date)
        {

            string fileName = string.Format("Recompute Sales Credit Limit-{0}-{1}.csv", date.Replace(" / ", ""), DateTime.Now.ToString("ddMMyyyyHHmmss"));
            dropSitePath = Path.Combine(dropSitePath, folder);

            if (!Directory.Exists(dropSitePath))
                Directory.CreateDirectory(dropSitePath);

            ObjectToCSV<ReComputeSalesCreditModel> receiveFromVendorObjectToCSV = new ObjectToCSV<ReComputeSalesCreditModel>();
            string filename = Path.Combine(dropSitePath, fileName);
            receiveFromVendorObjectToCSV.SaveToCSV(_header, filename);
            return folder;
        }
    }
}
