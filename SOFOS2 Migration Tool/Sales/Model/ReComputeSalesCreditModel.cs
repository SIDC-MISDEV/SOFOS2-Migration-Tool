using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Sales.Model
{
    public class ReComputeSalesCreditModel
    {
        public string TransactionType { get; set; }
        public string MemberCode { get; set; }
        public decimal Amount { get; set; }
    }
}
