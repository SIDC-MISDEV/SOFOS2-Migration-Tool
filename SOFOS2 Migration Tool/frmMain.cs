﻿using SOFOS2_Migration_Tool.Purchasing.Controller;
using SOFOS2_Migration_Tool.Sales.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOFOS2_Migration_Tool
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            /*
            PurchaseRequestController test = new PurchaseRequestController();

            var data = test.GetPRHeader("2022-01-17");
            var detail = test.GetPRItem("2022-01-17");

            test.InsertPR(data, detail);
            */

            SalesController salesController = new SalesController();

            var data = salesController.GetSalesHeader("2022-01-17");
            var detail = salesController.GetSalesItems("2022-01-17");
            var payment = salesController.GetSalesPayment("2022-01-17");

            salesController.InsertSales(data, detail, payment);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                Global g = new Global();
                g.InitializeBranch();
            }
            catch (Exception er)
            {
                MessageBox.Show(this, er.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new MethodInvoker(Close));
            }
        }
    }
}
