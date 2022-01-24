using SOFOS2_Migration_Tool.Inventory.Controller;
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
            string date = string.Empty;

            date = "2022-01-19";
            #region PR-SALES

            ReceiveFromVendorController rv = new ReceiveFromVendorController();
            //PurchaseRequestController test2 = new PurchaseRequestController();
            ReturnGoodsController rg = new ReturnGoodsController();


            //test.InsertPR(data, detail);

            //var data2 = test2.GetPRHeader(date);
            //var detail2 = test2.GetPRItem(date);

            //test2.InsertPR(data2, detail2);

            var dataRV = rv.GetRVHeader(date);
            var detailRV = rv.GetRVItem(date);

            rv.InsertRV(dataRV, detailRV);

            var dataRG = rg.GetRGHeader(date);
            var detailRG = rg.GetRGItem(date);

            rg.InsertReturnGoods(dataRG, detailRG);


            #region Sales Module

            SalesController salesController = new SalesController();
            var data = salesController.GetSalesHeader(date);
            var detail = salesController.GetSalesItems(date);
            var payment = salesController.GetSalesPayment(date);
            salesController.InsertSales(data, detail, payment);

            ReturnFromCustomerController returnFromCustomerController = new ReturnFromCustomerController();
            var data2 = returnFromCustomerController.GetReturnFromCustomerHeader(date);
            var detail2 = returnFromCustomerController.GetReturnFromCustomerItems(date);
            returnFromCustomerController.InsertReturnFromCustomer(data2, detail2);

            #endregion Sales Module

            #endregion

            #region Recompute Value and Quantity
            RecomputeController recompute = new RecomputeController();

            var trans = recompute.GetTransactions(date);

            recompute.UpdateRunningQuantityValueCost(trans);
            #endregion
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
