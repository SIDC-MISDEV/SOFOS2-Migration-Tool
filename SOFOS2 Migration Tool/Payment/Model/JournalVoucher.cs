using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Payment.Model
{
    public class JournalVoucher
    {
        //Header
        public int TransNum { get; set; }
        public string Reference { get; set; }
        public decimal Total { get; set; }
        public string TransDate { get; set; }
        public string IdUser { get; set; }
        public string Status { get; set; }
        public bool Cancelled { get; set; }
        public string Remarks { get; set; }

        //Details
        public string AccountCode { get; set; }
        public string CrossReference { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string AccountName { get; set; }
        public string DetRefTransType { get; set; }
        public string IntComputed { get; set; }
        public string PaidToDate { get; set; }
        public string LastPaymentDate { get; set; }
        public string AccountNumber { get; set; }
    }
}
