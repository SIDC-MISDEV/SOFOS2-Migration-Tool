using SOFOS2_Migration_Tool.Payment.Controller;
﻿using SOFOS2_Migration_Tool.Inventory.Controller;
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
        string date = string.Empty;
        Image checkedImage = global::SOFOS2_Migration_Tool.Properties.Resources.check_icon;
        Image crossImage = global::SOFOS2_Migration_Tool.Properties.Resources.cross_icon;

        public frmMain()
        {
            InitializeComponent(); 
            date = "2022-01-21";
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
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (UserConfirmation())
                return;

            #region Recompute Value and Quantity
            RecomputeController recompute = new RecomputeController();
            var trans = recompute.GetTransactions(date);
            recompute.UpdateRunningQuantityValueCost(trans);
            #endregion

            pcbRecomputeInventory.BackgroundImage = checkedImage;

        }



        private void btnPayment_Click(object sender, EventArgs e)
        {
            if (UserConfirmation())
                return;

            PaymentComputeController pcc = new PaymentComputeController();
            var invoicelist = pcc.GetInvoice("CI"); 
            //CollectionReceiptController crc = new CollectionReceiptController();
            //var crheader = crc.GetCollectionReceiptHeader("2021-05-31", "OR");
            //var crdetail = crc.GetCollectionReceiptDetail("2021-05-31", "OR");
            //crc.InsertCR(crheader, crdetail);

            //JournalVoucherController jvc = new JournalVoucherController();
            //var jvheader = jvc.GetJournalVoucherHeader("2021-03-31","JV");
            //var jvdetail = jvc.GetJournalVoucherDetail("2021-03-31", "JV");

            pcbPayment.BackgroundImage = checkedImage;
        }

        private void btnPurchasing_Click(object sender, EventArgs e)
        {

            if (UserConfirmation())
                return;

            #region Purchasing Module
            //PurchaseRequestController test2 = new PurchaseRequestController();

            ReceiveFromVendorController receiveFromVendorController = new ReceiveFromVendorController();
            var receiveFromVendorHeader = receiveFromVendorController.GetRVHeader(date);
            var receiveFromVendorDetails = receiveFromVendorController.GetRVItem(date);
            if (receiveFromVendorHeader.Count > 0)
                receiveFromVendorController.InsertRV(receiveFromVendorHeader, receiveFromVendorDetails);

            ReturnGoodsController returnGoodsController = new ReturnGoodsController();
            var returnGoodsHeader = returnGoodsController.GetRGHeader(date);
            var returnGoodsDetails = returnGoodsController.GetRGItem(date);
            if (returnGoodsHeader.Count > 0)
                returnGoodsController.InsertReturnGoods(returnGoodsHeader, returnGoodsDetails);

            #endregion Purchasing Module

            pcbPurchasing.BackgroundImage = checkedImage;

        }

        

        private void btnSales_Click(object sender, EventArgs e)
        {
            if (UserConfirmation())
                return;
            
            #region Sales Module

            SalesController salesController = new SalesController();
            var salesHeader = salesController.GetSalesHeader(date);
            var salesDetails = salesController.GetSalesItems(date);
            var salesPayment = salesController.GetSalesPayment(date);
            if (salesHeader.Count > 0)
                salesController.InsertSales(salesHeader, salesDetails, salesPayment);


            ReturnFromCustomerController returnFromCustomerController = new ReturnFromCustomerController();
            var returnFromCustomerHeader = returnFromCustomerController.GetReturnFromCustomerHeader(date);
            var returnFromCustomerDetails = returnFromCustomerController.GetReturnFromCustomerItems(date);
            if (returnFromCustomerHeader.Count > 0)
                returnFromCustomerController.InsertReturnFromCustomer(returnFromCustomerHeader, returnFromCustomerDetails);
            
            #endregion Sales Module

            pcbSales.BackgroundImage = checkedImage;

        }

        
        private void btnInventory_Click(object sender, EventArgs e)
        {
            if (UserConfirmation())
                return;

            #region Inventory Module

            GoodsReceiptController goodsReceiptController = new GoodsReceiptController();
            var goodsReceiptdata = goodsReceiptController.GetGoodsReceiptHeader(date);
            var goodsReceiptdetail = goodsReceiptController.GetGoodsReceiptItems(date);
            if (goodsReceiptdata.Count > 0)
                goodsReceiptController.InsertGoodsReceipt(goodsReceiptdata, goodsReceiptdetail);

            GoodsIssuanceController goodsIssuanceController = new GoodsIssuanceController();
            var goodsIssuancedata = goodsIssuanceController.GetGoodsIssuanceHeader(date);
            var goodsIssuancedetail = goodsIssuanceController.GetGoodsIssuanceItems(date);
            if (goodsIssuancedata.Count > 0)
                goodsIssuanceController.InsertGoodsIssuance(goodsIssuancedata, goodsIssuancedetail);

            AdjustmentController adjustmentController = new AdjustmentController();
            var adjustmentData = adjustmentController.GetAdjustmentHeader(date);
            var adjustmentDetail = adjustmentController.GetAdjustmentItems(date);
            if (adjustmentData.Count > 0)
                adjustmentController.InsertAdjustment(adjustmentData, adjustmentDetail);

            #endregion Inventory Module

            pcbInventory.BackgroundImage = checkedImage;

        }

        private Image GetImageByResult(string salesModule)
        {
            return string.IsNullOrWhiteSpace(salesModule) ? checkedImage : crossImage;
        }
        private bool UserConfirmation()
        {
            DialogResult dialogResult = MessageBox.Show("Confirmation", "Some Title", MessageBoxButtons.YesNo);
            return dialogResult != DialogResult.Yes;
        }

        private void btnRecomputePayment_Click(object sender, EventArgs e)
        {
            if (UserConfirmation())
                return;
            pcbRecomputePayment.BackgroundImage = checkedImage;
        }

        private void btnRecomputeInventory_Click(object sender, EventArgs e)
        {
            if (UserConfirmation())
                return;

            #region Recompute Value and Quantity
            RecomputeController recompute = new RecomputeController();
            var trans = recompute.GetTransactions(date);
            recompute.UpdateRunningQuantityValueCost(trans);
            #endregion

            pcbRecomputeInventory.BackgroundImage = checkedImage;
        }
    }
}
