﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Payment.Model
{
    public class CollectionReceipt
    {
        public int TransNum { get; set; }
        public string Reference { get; set; }
        public decimal Total { get; set; }
        public string VendorCode { get; set; }
        public string TransDate { get; set; }
        public string IdUser { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public bool Status { get; set; }
        public bool Cancelled { get; set; }
        public string Remarks { get; set; }
        public string Type { get; set; }
        public string AccountCode { get; set; }
        public string PaidBy { get; set; }
        public string BranchCode { get; set; }
        public bool Extracted { get; set; }
        public string TransType { get; set; }
        public string Series { get; set; }
        public string RefTransType { get; set; }

    }
}
