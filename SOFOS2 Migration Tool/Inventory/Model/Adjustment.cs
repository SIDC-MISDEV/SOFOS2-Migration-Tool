using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Inventory.Model
{
    public class Adjustment
    {
        public int TransNum { get; set; }
        public string Reference { get; set; }
        public string Crossreference { get; set; }
        public decimal Total { get; set; }
        public string TransType { get; set; }
        public string WarehouseCode { get; set; }
        public string SegmentCode { get; set; }
        public string BusinessSegment { get; set; }
        public string BranchCode { get; set; }
        public string Remarks { get; set; }
        public string TransDate { get; set; }
        public string SystemDate { get; set; }
        public string IdUser { get; set; }
        public bool Cancelled { get; set; }
        public string Status { get; set; }
        public string TerminalNo { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    public class AdjustmentItems
    {
        public int DetailNum { get; set; }
        public string Reference { get; set; }
        public int TransNum { get; set; }
        public string Barcode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Category { get; set; }
        public decimal RunningQuantity { get; set; }
        public decimal Total { get; set; }
        public decimal ActualCount { get; set; }
        public decimal Variance { get; set; }
        public string SystemDate { get; set; }
        public string TransDate { get; set; }
        public string IdUser { get; set; }
        public decimal Price { get; set; }
        public string UomCode { get; set; }
        public decimal RunningValue { get; set; }
    }
}
