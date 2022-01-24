using SOFOS2_Migration_Tool.Inventory.Model;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Inventory.Controller
{
    public class RecomputeController
    {
        public List<Transactions> GetTransactions(string date)
        {
            try
            {
                var result = new List<Transactions>();
                var prm = new Dictionary<string, object>() { { "@date", date } };

                using (var conn = new MySQLHelper(Global.DestinationDatabase, RecomputeQuery.GetAllTransactions(), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new Transactions
                            {
                                Reference = dr["reference"].ToString(),
                                ItemCode = dr["ItemCode"].ToString(),
                                UomCode = dr["UomCode"].ToString(),
                                Conversion = Convert.ToDecimal(dr["conversion"]),
                                Quantity = Convert.ToDecimal(dr["Quantity"]),
                                TransactionValue = Convert.ToDecimal(dr["Total"]),
                                TransactionType = dr["transactiontype"].ToString(),
                                TransDate = dr["transdate"].ToString()
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

        private Item GetItem(string itemCode)
        {
            try
            {
                var result = new Item();
                var prm = new Dictionary<string, object>() { { "@itemcode", itemCode } };

                using (var conn = new MySQLHelper(Global.DestinationDatabase, RecomputeQuery.GetItemRunningQuantityValue(), prm))
                {
                    using (var dr = conn.MySQLReader())
                    {
                        while (dr.Read())
                        {
                            result.ItemCode = dr["itemcode"].ToString();
                            result.RunningQuantity = Convert.ToDecimal(dr["runningQuantity"]);
                            result.RunningValue = Convert.ToDecimal(dr["runningValue"]);
                            result.Cost = Convert.ToDecimal(dr["cost"]);
                            result.UomCode = dr["uomCode"].ToString();
                            result.Conversion = Convert.ToDecimal(dr["Conversion"]);
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

        public void UpdateRunningQuantityValueCost(List<Transactions> _transactions)
        {
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>(),
                    itemParam = new Dictionary<string, object>();

                List<Transactions> trans = new List<Transactions>();
                Item item = null;
                StringBuilder sQuery = new StringBuilder();


                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    conn.BeginTransaction();

                    foreach (var tran in _transactions)
                    {
                        item = new Item();
                        sQuery = new StringBuilder();

                        item = GetItem(tran.ItemCode);

                        if(!string.IsNullOrEmpty(item.ItemCode))
                        {
                            param = new Dictionary<string, object>()
                                    {
                                        { "@itemCode", tran.ItemCode },
                                        { "@runningQuantity", tran.Quantity + item.RunningQuantity },
                                        { "@runningValue", tran.TransactionValue + item.RunningValue },
                                        { "@uomCode", tran.UomCode },
                                        { "@reference", tran.Reference }
                                    };

                            Process process;

                            Enum.TryParse(tran.TransactionType, out process);


                            switch (process)
                            {
                                case Process.Sales:

                                    

                                    sQuery = RecomputeQuery.UpdateRunningQuantityValue(process);

                                    break;
                                case Process.Adjustment:
                                    break;
                                case Process.Issuance:
                                    break;
                                case Process.ReturnGoods:
                                    break;
                                case Process.Receiving:
                                    break;
                                case Process.ReceiveFromVendor:
                                    break;
                                default:
                                    break;
                            }

                            if(sQuery.Length > 0)
                            {
                                decimal qty = 0,
                                    runVal = 0;

                                //Update transaction running quantity and value
                                conn.ArgSQLCommand = sQuery;
                                conn.ArgSQLParam = param;
                                conn.ExecuteMySQL();

                                qty = tran.Quantity + item.RunningQuantity > 0 ? tran.Quantity + item.RunningQuantity : 0;
                                runVal = tran.TransactionValue + item.RunningValue > 0 ? tran.TransactionValue + item.RunningValue : 0;

                                itemParam = new Dictionary<string, object>()
                                {
                                     { "@itemCode", tran.ItemCode },
                                     { "@runningQuantity", qty },
                                     { "@runningValue", runVal }
                                };

                                //update running quantity and value of master data
                                conn.ArgSQLCommand = RecomputeQuery.UpdateItemRunningQuantityValue();
                                conn.ArgSQLParam = itemParam;
                                conn.ExecuteMySQL();

                            }

                            
                        }

                    }


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
