using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Accounting.Model
{
    public class AccountCreditLimits
    {
        public int TransNum { get; set; }
        public decimal ShareCapital { get; set; }
        public string AccountNum { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string AccountNumber { get; set; }
        public string TransType { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal ChargeAmount { get; set; }
        public string AccountCode { get; set; }
        public string Status { get; set; }
        public decimal CreditBalance { get; set; }
        public string BranchCode { get; set; }
        public string ChargeType { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Terms { get; set; }
        public string InterestAccount { get; set; }
    }
}
