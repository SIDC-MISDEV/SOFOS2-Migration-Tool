using SOFOS2_Migration_Tool.Payment.Controller;
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
using SOFOS2_Migration_Tool.Enums;
using SOFOS2_Migration_Tool.Accounting.Controller;

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
            date = dtpDateParam.Value.ToString("yyyy-MM-dd");
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
        



        private void btnPayment_Click(object sender, EventArgs e)
        {
            if (UserConfirmation(ProcessEnum.Migrate, ModuleEnum.Payment))
                return;

            try
            {
                CollectionReceiptController crc = new CollectionReceiptController();
                var crheader = crc.GetCollectionReceiptHeader(date, "OR");
                var crdetail = crc.GetCollectionReceiptDetail(date, "OR");
                if (crheader.Count > 0)
                    crc.InsertCR(crheader, crdetail);


                OfficialReceiptController orc = new OfficialReceiptController();
                var orheader = orc.GetOfficialReceiptHeader(date, "OR");
                var ordetail = orc.GetOfficialReceiptDetail(date, "OR");
                if (orheader.Count > 0)
                    orc.InsertOR(orheader, ordetail);

                JournalVoucherController jvc = new JournalVoucherController();
                var jvheader = jvc.GetJournalVoucherHeader(date, "JV");
                var jvdetail = jvc.GetJournalVoucherDetail(date, "JV");
                if (jvheader.Count > 0)
                    jvc.InsertJV(jvheader, jvdetail);



                string message = string.Format(@"No transactions found in SOFOS1 - Payment module (Collection Receipt) dated : {0}.", date);
                if (crheader.Count > 0)
                    message = string.Format(@" ({0}) Collection Receipt transactions was transfered successfully.", crheader.Count);

                MessageBox.Show(message);

                MessageBox.Show(this, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);


                pcbPayment.BackgroundImage = checkedImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPurchasing_Click(object sender, EventArgs e)
        {
           
            if (UserConfirmation(ProcessEnum.Migrate, ModuleEnum.Purchasing))
                return;

            try
            { 
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

                string message = string.Format(@"No transactions found in SOFOS1 - Purchasing module(Receive From Vendor  and Receive Goods) dated : {0}.", date);
                if (receiveFromVendorHeader.Count + returnGoodsHeader.Count > 0)
                    message = string.Format(@" ({0}) Receive From Vendor  and ({1}) Receive Goods transactions was transfered successfully.", receiveFromVendorHeader.Count, returnGoodsHeader.Count);

                MessageBox.Show(this, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pcbPurchasing.BackgroundImage = checkedImage;

                #region LOGS
                receiveFromVendorController.InsertRVLogs(receiveFromVendorHeader, date);
                returnGoodsController.InsertReturnGoodsLogs(returnGoodsHeader, date);
                #endregion LOGS
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            if (UserConfirmation(ProcessEnum.Migrate, ModuleEnum.Sales))
                return;

            try
            {
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

                string message = string.Format(@"No transactions found in SOFOS1 - Point of Sale module (Sales and Return from Customer) dated : {0}.", date);
                if (salesHeader.Count + returnFromCustomerHeader.Count > 0)
                    message = string.Format(@" ({0}) Sales and ({1}) Return from Customer transactions was transfered successfully.", salesHeader.Count, returnFromCustomerHeader.Count);

                MessageBox.Show(this, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pcbSales.BackgroundImage = checkedImage;

                #region LOGS
                salesController.InsertSalesLogs(salesHeader, date);
                returnFromCustomerController.InsertReturnFromCustomerLogs(returnFromCustomerHeader, date);
                #endregion LOGS

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
 
        private void btnInventory_Click(object sender, EventArgs e)
        {
            if (UserConfirmation(ProcessEnum.Migrate, ModuleEnum.Inventory))
                return;
            try
            {
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

                string message = string.Format(@"No transactions found in SOFOS1 - Inventory module (Goods Receipt, Goods Issuances  and Inventory Adjustments ) dated : {0}.", date);
                if (goodsReceiptdata.Count + goodsIssuancedata.Count + adjustmentData.Count > 0)
                    message = string.Format(@" ({0}) Goods Receipt, {1} Goods Issuances and ({2}) Inventory Adjustments transactions was transfered successfully.", goodsReceiptdata.Count, goodsIssuancedata.Count, adjustmentData.Count);

                MessageBox.Show(this, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pcbInventory.BackgroundImage = checkedImage;

                #region LOGS
                goodsReceiptController.InsertGoodsReceiptLogs(goodsReceiptdata, date);
                goodsIssuanceController.InsertGoodsIssuanceLogs(goodsIssuancedata, date);
                adjustmentController.InsertAdjustmentLogs(adjustmentData, date);
                #endregion LOGS


            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Image GetImageByResult(string salesModule)
        {
            return string.IsNullOrWhiteSpace(salesModule) ? checkedImage : crossImage;
        }

        private bool UserConfirmation(ProcessEnum processEnum, ModuleEnum moduleEnum)
        {
            string message = string.Empty;
            switch(processEnum){
                case ProcessEnum.Migrate:
                    message = string.Format("Migrate Sofos1 {0} transactions dated : {1}.", moduleEnum.ToString(), date);
                    break;
                case ProcessEnum.Recompute:
                    switch (moduleEnum)
                    {
                        case ModuleEnum.Inventory:
                            message = string.Format("Compute and update cost, running quantity and running value using transaction dated : {0}.", date);
                            break;
                        case ModuleEnum.Payment:
                            message = string.Format("Compute payment and generate interest payment : {0}.", date);
                            break;
                        case ModuleEnum.SalesCreditLimit:
                            message = string.Format("Compute sales (CI,CT,APL) and update credit limit dated : {0}.", date);
                            break;
                        default:
                                message = string.Format("Compute for {0} module is not available.", moduleEnum.ToString());
                            break;
                    }
                    break;
            }

            DialogResult dialogResult = MessageBox.Show(this, message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return dialogResult != DialogResult.Yes;
        }

        private void btnRecomputePayment_Click(object sender, EventArgs e)
        {
            if (UserConfirmation(ProcessEnum.Recompute, ModuleEnum.Payment))
                return;

            try
            {
                PaymentComputeController pcc = new PaymentComputeController();

                string message = string.Format(@"No transactions in Payment module found in SOFOS2 dated : {0}.", date);

                var paymentlist = pcc.GetAllTransactionPayments(date);
                if (paymentlist.Count > 0)
                    pcc.ComputePayment(paymentlist, date);

                message = string.Format(@" ({0}) invoice transactions with payment was recomputed successfully.", paymentlist.Count);
                MessageBox.Show(this, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pcbRecomputePayment.BackgroundImage = checkedImage;

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error : {0}", ex.Message));
            }
        }

        private void btnRecomputeInventory_Click(object sender, EventArgs e)
        {
            if (UserConfirmation(ProcessEnum.Recompute, ModuleEnum.Inventory))
                return;
            try
            { 
                #region Recompute Value and Quantity
                RecomputeController recompute = new RecomputeController();
                var trans = recompute.GetTransactions(date);
                if(trans.Count > 0)
                    recompute.UpdateRunningQuantityValueCost(trans);
                #endregion

                string message = string.Format(@"No transactions(Purchasing, Inventory or Sales module) found in SOFOS2 dated : {0}.", date);
                if (trans.Count > 0)
                    message = string.Format(@" ({0}) transactions in 'Purchasing, Inventory or Sales module' was recomputed successfully.", trans.Count);

                MessageBox.Show(this, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pcbRecomputeInventory.BackgroundImage = checkedImage;

                #region LOGS
                recompute.UpdateRunningQuantityValueCostLogs(trans, date);
                #endregion LOGS

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtpDateParam_ValueChanged(object sender, EventArgs e)
        {
            date = dtpDateParam.Value.ToString("yyyy-MM-dd");
            pcbPurchasing.BackgroundImage = null;
            pcbCreditLimit.BackgroundImage = null;
            pcbInventory.BackgroundImage = null;
            pcbPayment.BackgroundImage = null;
            pcbRecomputeInventory.BackgroundImage = null;
            pcbRecomputePayment.BackgroundImage = null;
            pcbRecomputeSalesCreditLimit.BackgroundImage = null;
            pcbSales.BackgroundImage = null;
        }

        private void btnRecomputeSalesCreditLimit_Click(object sender, EventArgs e)
        {
            if (UserConfirmation(ProcessEnum.Recompute, ModuleEnum.SalesCreditLimit))
                return;
            try
            { 
                #region Recompute creditlimit amount
                ReComputeSalesCreditController reComputeSalesCreditController = new ReComputeSalesCreditController();
                var reComputeSalesCredit = reComputeSalesCreditController.GetSalesAndReturnFromCustomerTransactions(date);
                if (reComputeSalesCredit.Count > 0)
                    reComputeSalesCreditController.UpdateChargeAmount(reComputeSalesCredit);
                #endregion

                string message = string.Format(@"No transactions in Sales module (CI,CT,CO and AP) found in SOFOS2 dated : {0}.", date);
                if (reComputeSalesCredit.Count > 0)
                    message = string.Format(@" ({0}) sales transactions with credit was recomputed successfully.", reComputeSalesCredit.Count);

                MessageBox.Show(this, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pcbRecomputeSalesCreditLimit.BackgroundImage = checkedImage;

                #region LOGS
                reComputeSalesCreditController.UpdateChargeAmountLogs(reComputeSalesCredit, date);
                #endregion LOGS
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreditLimit_Click(object sender, EventArgs e)
        {
            if (UserConfirmation(ProcessEnum.Migrate, ModuleEnum.SalesCreditLimit))
                return;
            int affectedSalesTransaction = 0;
            try
            { 
                #region Credit Limit

                AccountCreditLimitController accountCreditLimitController = new AccountCreditLimitController();
                var accountCreditLimitData = accountCreditLimitController.GetAccountCreditLimits(date);
                if (accountCreditLimitData.Count > 0)
                    accountCreditLimitController.InsertAccountCreditLimits(accountCreditLimitData);

                affectedSalesTransaction = accountCreditLimitController.UpdateTransactionAccountNumber();
                #endregion Credit Limit

                string message = string.Format(@"No data found in SOFOS1 - Accounting module (Credit Limit) dated : {0}.", date);
                if (accountCreditLimitData.Count + affectedSalesTransaction > 0)
                    message = string.Format(@" ({0}) Credit Limit transactions was transfered successfully and {1} sales transactions are updated with their assigned account number in credit limit.", accountCreditLimitData.Count, affectedSalesTransaction);

                MessageBox.Show(this, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pcbCreditLimit.BackgroundImage = checkedImage;

                #region LOGS
                accountCreditLimitController.InsertAccountCreditLimitsLogs(accountCreditLimitData, date);
                #endregion LOGS
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
