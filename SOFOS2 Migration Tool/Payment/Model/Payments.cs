using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Payment.Model
{
    public class Payments
    {
        public string TransDate { get; set; }
        public string TransType { get; set; }
        public string Reference { get; set; }
        public string MemberId { get; set; }
        public string AccountCode { get; set; }
        public decimal PaidToDate { get; set; }
        public decimal Amount { get; set; }
        public bool Cancelled { get; set; }
        public string Status { get; set; }
        public int IntComputed { get; set; }
        public string LastPaymentDate { get; set; }
        public string AccountNumber { get; set; }
    }
}
