using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Payment.Model
{
    public class TransactionPayment
    {
        public int DetailNum { get; set; }
        public int TransNum { get; set; }
        public string PaymentCode { get; set; }
        public decimal Amount { get; set; }
        public string CheckNumber { get; set; }
        public string BankCode { get; set; }
        public DateTime CheckDate { get; set; }
        public DateTime SystemDate { get; set; }
        public string IdUser { get; set; }
        public string TransType { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal ChangeAmount { get; set; }
        public bool Extracted { get; set; }
        public int OrDetailNum { get; set; }
    }
}
