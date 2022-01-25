using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Inventory.Model
{
   public class GoodsIssuance
    {
        public int TransNum { get; set; }
        public string Reference { get; set; }
        public string Crossreference { get; set; }
        public decimal Total { get; set; }
        public string TransType { get; set; }
        public string ToWarehouse { get; set; }
        public string FromWarehouse { get; set; }
        public string SegmentCode { get; set; }
        public string BusinessSegment { get; set; }
        public string BranchCode { get; set; }
        public string Remarks { get; set; }
        public bool Cancelled { get; set; }
        public string Status { get; set; }
        public string TransDate { get; set; }
        public string SystemDate { get; set; }
        public string IdUser { get; set; }
        public string Extracted { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public bool IsDummy { get; set; }
        public bool IsManual { get; set; }
        public string AccountNo { get; set; }
        public string TerminalNo { get; set; }

        public bool Synced { get; set; }
        public string ToBranchCode { get; set; } // for check
        public string EmpName { get; set; }
        public string Department { get; set; }
        public decimal Paidtodate { get; set; }
         
        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    public class GoodsIssuanceItems
    {
        public int DetailNum { get; set; }
        public string Reference { get; set; }
        public int TransNum { get; set; }
        public string Barcode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string UomCode { get; set; }
        public string UomDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public decimal Conversion { get; set; }
        public string IdUser { get; set; }
        public string AccountCode { get; set; }
        public string WarehouseCode { get; set; }
        public string TransDate { get; set; }
        public decimal RunningQty { get; set; }
        public decimal AverageCost { get; set; }
        public decimal RunningValue { get; set; }
        public string SystemDate { get; set; }
    }
}
