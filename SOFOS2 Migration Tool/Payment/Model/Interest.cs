using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Payment.Model
{
    public class Interest
    {
        public string TransDate { get; set; }
        public string TransType { get; set; }
        public string RefTransType { get; set; }
        public string Reference { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal Amount { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string AccountCode { get; set; }
        public string CrossReference { get; set; }
        public string AccountNumber { get; set; }
        public string IdUser { get; set; }
    }
}
