using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Inventory.Model
{
    public class Item
    {
        public string ItemCode { get; set; }
        public string UomCode { get; set; }
        public decimal Cost { get; set; }
        public decimal AverageCost { get; set; }
        public decimal RunningValue { get; set; }
        public decimal RunningQuantity { get; set; }
        public decimal Conversion { get; set; }
        public decimal CurrentRunVal { get; set; }
        public decimal CurrentRunQty { get; set; }
    }

    public class Transactions
    {
        public string Reference { get; set; }
        public string ItemCode { get; set; }
        public string UomCode { get; set; }
        public decimal Conversion { get; set; }
        public decimal Quantity { get; set; }
        public decimal Cost { get; set; }
        public decimal TransactionValue { get; set; }
        public string TransactionType { get; set; }
        public string TransDate { get; set; }
    }

    public class ItemProblem
    {
        public string ItemCode { get; set; }
        public decimal CurrentRunningQuantity { get; set; }
        public decimal CurrentRunningValue { get; set; }
        public decimal TransactionRunningQuantity { get; set; }
        public decimal TransactionValue { get; set; }
    }
    
}
