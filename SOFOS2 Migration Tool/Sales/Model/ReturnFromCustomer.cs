using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Sales.Model
{
    public class ReturnFromCustomer
    {
        public int TransNum { get; set; }
        public string TransDate { get; set; }
        public string TransType { get; set; }
        public string Reference { get; set; }
        public string Crossreference { get; set; }
        public bool NoEffectOnInventory { get; set; }
        public string CustomerType { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string YoungCoopID { get; set; }
        public string YoungCoopName { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal PaidToDate { get; set; }
        public decimal Total { get; set; }
        public decimal AmountTendered { get; set; }
        public bool Cancelled { get; set; }
        public string Status { get; set; }
        public string Extracted { get; set; }
        public string ColaReference { get; set; }
        public string SegmentCode { get; set; }
        public string BusinessSegment { get; set; }
        public string BranchCode { get; set; }
        public string Signatory { get; set; }
        public string Remarks { get; set; }
        public string SystemDate { get; set; }
        public string IdUser { get; set; }
        public string LrBatch { get; set; }
        public string LrType { get; set; }
        public string WarehouseCode { get; set; }
        public decimal SrDiscount { get; set; }
        public decimal FeedsDiscount { get; set; }
        public decimal Vat { get; set; }
        public decimal VatExemptSales { get; set; }
        public decimal VatAmount { get; set; }
        public string Series { get; set; }
        public string AccountNo { get; set; }
        public string TerminalNo { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    public class ReturnFromCustomerItems
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
        public decimal Cost { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal Conversion { get; set; }
        public string SystemDate { get; set; }
        public string IdUser { get; set; }

        public decimal RunningQty { get; set; }
        public decimal AverageCost { get; set; }
        public decimal RunningValue { get; set; }
    }
 }
