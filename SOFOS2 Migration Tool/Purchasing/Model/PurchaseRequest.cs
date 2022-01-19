using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Purchasing.Model
{
    public class PurchaseRequest
    {
        public string Reference { get; set; }
        public string CrossReference { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public decimal Total { get; set; }
        public string TransType { get; set; }
        public string ToWarehouse { get; set; }
        public string FromWarehouse { get; set; }
        public string SegmentCode { get; set; }
        public string BusinessCode { get; set; }
        public string BranchCode { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public bool Cancelled { get; set; }
        public string Printed { get; set; }
        public bool Approved { get; set; }
        public string Iduser { get; set; }
        public bool ExtractedToDW { get; set; }
        public string Extracted { get; set; }
        public string TransactionDate { get; set; }
    }

    public class PurchaseRequestItem
    {
        public int PRId { get; set; }
        public string Reference { get; set; }
        public string Barcode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string UOMCode { get; set; }
        public string UOMDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public decimal Conversion { get; set; }
        public string AccountCode { get; set; }
        public string WarehouseCode { get; set; }
        public string IdUser { get; set; }
        public string TransDate { get; set; }
    }
}
