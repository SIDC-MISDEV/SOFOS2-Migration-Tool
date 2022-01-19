using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Purchasing.Model
{
    public class ReceiveFromVendor
    {
        public string Reference { get; set; }
        public string CrossReference { get; set; }
        public string PRReference { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public decimal Total { get; set; }
        public string TransType { get; set; }
        public string ToWarehouse { get; set; }
        public string FromWarehouse { get; set; }
        public string SegmentCode { get; set; }
        public string BussinessSegment { get; set; }
        public string BranchCode { get; set; }
        public string Remarks { get; set; }
        public bool Cancelled { get; set; }
        public string Status { get; set; }

    }
}
