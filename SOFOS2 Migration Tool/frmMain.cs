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
            //ReceiveFromVendorController test = new ReceiveFromVendorController();
            //PurchaseRequestController test2 = new PurchaseRequestController();
            //ReturnGoodsController test3 = new ReturnGoodsController();
            string date = string.Empty;

            date = "2022-01-17";

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


            //#region Sales Module

            //SalesController salesController = new SalesController();
            //var salesdata = salesController.GetSalesHeader(date);
            //var salesdetail = salesController.GetSalesItems(date);
            //var salespayment = salesController.GetSalesPayment(date);
            //salesController.InsertSales(salesdata, salesdetail, salespayment);

            //ReturnFromCustomerController returnFromCustomerController = new ReturnFromCustomerController();
            //var returnFromCustomerdata = returnFromCustomerController.GetReturnFromCustomerHeader(date);
            //var returnFromCustomerdetail = returnFromCustomerController.GetReturnFromCustomerItems(date);
            //returnFromCustomerController.InsertReturnFromCustomer(returnFromCustomerdata, returnFromCustomerdetail);

            //#endregion Sales Module

            #region Inventory Module

            //GoodsReceiptController goodsReceiptController = new GoodsReceiptController();
            //var goodsReceiptdata = goodsReceiptController.GetGoodsReceiptHeader(date);
            //var goodsReceiptdetail = goodsReceiptController.GetGoodsReceiptItems(date);
            //goodsReceiptController.InsertGoodsReceipt(goodsReceiptdata, goodsReceiptdetail);

            //GoodsIssuanceController goodsIssuanceController = new GoodsIssuanceController();
            //var goodsIssuancedata = goodsIssuanceController.GetGoodsIssuanceHeader(date);
            //var goodsIssuancedetail = goodsIssuanceController.GetGoodsIssuanceItems(date);
            //goodsIssuanceController.InsertGoodsIssuance(goodsIssuancedata, goodsIssuancedetail);

            AdjustmentController adjustmentController = new AdjustmentController();
            var adjustmentData = adjustmentController.GetAdjustmentHeader(date);
            var adjustmentDetail = adjustmentController.GetAdjustmentItems(date);
            adjustmentController.InsertAdjustment(adjustmentData, adjustmentDetail);
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
