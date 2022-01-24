using SOFOS2_Migration_Tool.Purchasing.Controller;
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
            //ReceiveFromVendorController test = new ReceiveFromVendorController();
            //PurchaseRequestController test2 = new PurchaseRequestController();
            //ReturnGoodsController test3 = new ReturnGoodsController();
            //string date = string.Empty;

            //date = "2022-01-17";

            //test.InsertPR(data, detail);
            
            //var data2 = test2.GetPRHeader(date);
            //var detail2 = test2.GetPRItem(date);

            //test2.InsertPR(data2, detail2);

            //var data = test.GetRVHeader(date);
            //var detail = test.GetRVItem(date);

            //test.InsertRV(data, detail);

            //var data3 = test3.GetRGHeader(date);
            //var detail3 = test3.GetRGItem(date);

            //test3.InsertReturnGoods(data3, detail3);
            
            
            #region Sales Module
            
            SalesController salesController = new SalesController();
            var salesdata = salesController.GetSalesHeader("2022-01-17");
            var salesdetail = salesController.GetSalesItems("2022-01-17");
            var salespayment = salesController.GetSalesPayment("2022-01-17");
            salesController.InsertSales(salesdata, salesdetail, salespayment);

            ReturnFromCustomerController returnFromCustomerController = new ReturnFromCustomerController();
            var returnFromCustomerdata = returnFromCustomerController.GetReturnFromCustomerHeader("2022-01-17");
            var returnFromCustomerdetail = returnFromCustomerController.GetReturnFromCustomerItems("2022-01-17");
            returnFromCustomerController.InsertReturnFromCustomer(returnFromCustomerdata, returnFromCustomerdetail);
            
            #endregion Sales Module

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
