using SOFOS2_Migration_Tool.Helper;
using SOFOS2_Migration_Tool.Inventory.Model;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOFOS2_Migration_Tool.Inventory.Controller
{
    public class RecomputeController
    {
        string dropSitePath = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "LOGS");
        string folder = "Recompute/";

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

        private Item GetItem(MySQLHelper conn, string itemCode)
        {
            try
            {
                var result = new Item();
                var prm = new Dictionary<string, object>() { { "@itemcode", itemCode } };

                conn.ArgSQLCommand = RecomputeQuery.GetItemRunningQuantityValue();
                conn.ArgSQLParam = prm;
                using (var dr = conn.MySQLExecuteReaderBeginTransaction())
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
                List<ItemProblem> errorItem = new List<ItemProblem>();
                Item item = null;
                StringBuilder sQuery = new StringBuilder();
                decimal averageCost = 0, tranRunQty = 0, tranRunVal = 0;


                using (var conn = new MySQLHelper(Global.DestinationDatabase))
                {
                    conn.BeginTransaction();

                    foreach (var tran in _transactions)
                    {
                        item = new Item();
                        sQuery = new StringBuilder();
                        //Get running value and running quantity of an item.
                        item = GetItem(conn, tran.ItemCode);

                        //Initialize running qty,  running value and average cost
                        averageCost = 0;
                        tranRunQty = 0;
                        tranRunVal = 0;


                        if (!string.IsNullOrEmpty(item.ItemCode))
                        {


                            Process process;

                            Enum.TryParse(tran.TransactionType, out process);


                            //tranRunQty = Math.Round((tran.Quantity * tran.Conversion) + item.RunningQuantity, 2, MidpointRounding.AwayFromZero);
                            tranRunQty = Math.Round(tran.Quantity + item.RunningQuantity, 2, MidpointRounding.AwayFromZero);

                            if (tranRunQty == 0)
                            {
                                tranRunVal = 0;
                                tranRunQty = 0;
                            }
                            else if (tranRunQty < 0)
                            {
                                errorItem.Add(new ItemProblem
                                {
                                    ItemCode = item.ItemCode,
                                    CurrentRunningQuantity = item.RunningQuantity,
                                    CurrentRunningValue = item.RunningValue,
                                    TransactionRunningQuantity = tranRunQty,
                                    TransactionValue = tranRunVal
                                });

                                continue;
                            }
                            else
                            {

                                //if(process == Process.Sales)
                                //    tranRunVal = Math.Round((item.Cost * (tran.Quantity * tran.Conversion)) + item.RunningValue, 2, MidpointRounding.AwayFromZero);
                                //else
                                //    tranRunVal = Math.Round(tran.TransactionValue + item.RunningValue, 2, MidpointRounding.AwayFromZero);

                                if (process == Process.Sales)
                                    tranRunVal = Math.Round((item.Cost * tran.Quantity) + item.RunningValue, 2, MidpointRounding.AwayFromZero);
                                else
                                    tranRunVal = Math.Round(tran.TransactionValue + item.RunningValue, 2, MidpointRounding.AwayFromZero);


                                averageCost = Math.Round(tranRunVal / tranRunQty, 2, MidpointRounding.AwayFromZero);
                            }

                            

                            switch (process)
                            {
                                case Process.Sales:
                                case Process.Adjustment:

                                    param = new Dictionary<string, object>()
                                    {
                                        { "@itemCode", tran.ItemCode },
                                        { "@runningQuantity", tranRunQty },
                                        { "@runningValue", tranRunVal },
                                        { "@uomCode", tran.UomCode },
                                        { "@reference", tran.Reference },
                                        { "@cost", item.Cost }
                                    };

                                    sQuery = RecomputeQuery.UpdateRunningQuantityValue(process);

                                    break;

                                case Process.Issuance:
                                case Process.ReturnGoods:
                                case Process.Receiving:
                                case Process.ReceiveFromVendor:

                                    param = new Dictionary<string, object>()
                                    {
                                        { "@itemCode", tran.ItemCode },
                                        { "@runningQuantity", tranRunQty },
                                        { "@runningValue", tranRunVal},
                                        { "@uomCode", tran.UomCode },
                                        { "@reference", tran.Reference },
                                        { "@cost",  averageCost }
                                    };

                                    sQuery = RecomputeQuery.UpdateRunningQuantityValue(process);

                                    break;
                                default:
                                    break;
                            }

                            #region Update transaction running quantity and value and cost if any

                            conn.ArgSQLCommand = sQuery;
                            conn.ArgSQLParam = param;
                            conn.ExecuteMySQL();

                            #endregion

                            #region Update running quantity and running value of master data


                            itemParam = new Dictionary<string, object>()
                                {
                                     { "@itemCode", tran.ItemCode },
                                     { "@runningQuantity", tranRunQty },
                                     { "@runningValue", tranRunVal }
                                };

                            //update running quantity and value of master data
                            conn.ArgSQLCommand = RecomputeQuery.UpdateItemRunningQuantityValue();
                            conn.ArgSQLParam = itemParam;
                            conn.ExecuteMySQL();

                            #endregion

                            #region Update cost of an item

                            if (process.Equals(Process.Receiving) || process.Equals(Process.ReceiveFromVendor))
                            {
                                conn.ArgSQLCommand = RecomputeQuery.UpdateItemCost();
                                conn.ArgSQLParam = new Dictionary<string, object>()
                                {
                                    { "@cost", averageCost },
                                    { "@itemCode", tran.ItemCode }
                                };

                                conn.ExecuteMySQL();
                            }

                            #endregion
                        }

                    }

                    if (errorItem.Count < 1)
                        conn.CommitTransaction();
                    else
                    {
                        var dialogResult = MessageBox.Show("Detected negative qty items, do you still want to continue?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if(dialogResult == DialogResult.Yes)
                        {
                            conn.CommitTransaction();
                            NegativeRunningQuantityItemLogs(errorItem);
                        }
                        else
                        {
                            conn.RollbackTransaction();
                            NegativeRunningQuantityItemLogs(errorItem);

                            throw new Exception($@"Negative running quantity of items detected. Please check error log file.");
                        }
                    }

                }
            }
            catch
            {
                throw;
            }
        }

        public string UpdateRunningQuantityValueCostLogs(List<Transactions> _header, string date)
        {

            string fileName = string.Format("Recompute Inventory-{0}-{1}.csv", date.Replace(" / ", ""), DateTime.Now.ToString("ddMMyyyyHHmmss"));
            dropSitePath = Path.Combine(dropSitePath, folder);

            if (!Directory.Exists(dropSitePath))
                Directory.CreateDirectory(dropSitePath);

            ObjectToCSV<Transactions> receiveFromVendorObjectToCSV = new ObjectToCSV<Transactions>();
            string filename = Path.Combine(dropSitePath, fileName);
            receiveFromVendorObjectToCSV.SaveToCSV(_header, filename);
            return folder;
        }

        private void NegativeRunningQuantityItemLogs(List<ItemProblem> items)
        {
            string fileName = string.Format($"Negative Running Quantity Items-{DateTime.Now.ToString("ddMMyyyyHHmmss")}.csv");
            dropSitePath = Path.Combine(dropSitePath, folder);

            if (!Directory.Exists(dropSitePath))
                Directory.CreateDirectory(dropSitePath);

            ObjectToCSV<ItemProblem> receiveFromVendorObjectToCSV = new ObjectToCSV<ItemProblem>();
            string filename = Path.Combine(dropSitePath, fileName);
            receiveFromVendorObjectToCSV.SaveToCSV(items, filename);
        }
    }
}
