using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Payment.Model
{
    public class Invoice
    {
        public string Reference { get; set; }
        public string LastPaymentDate { get; set; }
        public decimal PaidToDate { get; set; }
    }
}
