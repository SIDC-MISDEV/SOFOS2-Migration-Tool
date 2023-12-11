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
using SOFOS2_Migration_Tool.Customer.Controller;
using SOFOS2_Migration_Tool.Migration.Controller;

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

            StartProcess(true);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                Global g = new Global();
                g.InitializeBranch();

                txtBranchCode.Text = Global.BranchCode;
                txtWarehouseCode.Text = Global.WarehouseCode;
                lblBranchName.Text = Global.BranchName;

                if(string.IsNullOrEmpty(txtBranchCode.Text) || string.IsNullOrEmpty(txtWarehouseCode.Text))
                {
                    var confirm = MessageBox.Show(this, "Branch code and warehouse code is missing. Do you still want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirm.Equals(DialogResult.No))
                        this.Close();
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(this, er.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.BeginInvoke(new MethodInvoker(Close));
            }
        }

        private void StartProcess(bool isPremigration)
        {
            try
            {
                int errorCount = 0;
                string migrationDb = string.Empty;
                var controller = new MigrationController(isPremigration);

                errorCount = controller.CheckForErrorData();
                migrationDb = isPremigration ? "Pre-migration" : "Post-migration";

                if (errorCount > 0)
                    throw new Exception($"System detected {errorCount} files generated from {migrationDb}  error checking.");

                if(!isPremigration && errorCount < 1)
                    MessageBox.Show(this, "No detected errors based on Post-migration scripts.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception er)
            {
                MessageBox.Show(this, er.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                if(isPremigration)
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

                string message = string.Format(@"No transactions found in SOFOS1 - Payment module (Collection Receipt and Official Receipt and Journal Voucher) dated : {0}.", date);
                if (crheader.Count + orheader.Count + jvheader.Count > 0)
                    message = string.Format(@" ({0}) Collection Receipt and ({1}) Official Receipt and ({2}) Journal Voucher transactions was transfered successfully.", crheader.Count, orheader.Count, jvheader.Count);

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
                    salesController.InsertSales(salesHeader, salesDetails, salesPayment, date);


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

                    if(moduleEnum == ModuleEnum.SellingPrice)
                        message = string.Format("Migrate Sofos1 {0} as of  {1}.", moduleEnum.ToString(), date);
                    else
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
                            message = string.Format("Compute sales (CI,JV,CT,APL) and update credit limit dated : {0}.", date);
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
                    recompute.UpdateRunningQuantityValueCost(trans, dtpDateParam.Value.ToString("yyyy-MM-dd"));
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
            pcbMembers.BackgroundImage = null;
            pcbSales.BackgroundImage = null;
            pbSellingPrice.BackgroundImage = null;
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

                string message = string.Format(@"No transactions in Sales module (CI,JV,CT,CO, AP and WR) found in SOFOS2 dated : {0}.", date);
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

        private void btnMembers_Click(object sender, EventArgs e)
        {
            try
            {
                int memberResult = 0, employeeResult = 0;
                string msg = string.Empty;

                if(UserConfirmation(ProcessEnum.Migrate, ModuleEnum.Customers)) 
                    return;

                #region Members and Employees
                CustomerController controller = new CustomerController();

                var employees = controller.GetEmployees();
                var members = controller.GetCustomer();
                

                if (members.Count > 0)
                    memberResult = controller.InsertCustomer(members);
                
                if (employees.Count > 0)
                    employeeResult = controller.InsertEmployee(employees);


                if (memberResult > 0)
                    msg = $"{memberResult.ToString("#,##")} member(s) was transferred successfully.";
                else
                    msg = $"No member was transferred.";

                if (employeeResult > 0)
                    msg += $"{Environment.NewLine}{memberResult} employee(s) was transferred successfully.";
                else
                    msg += $"{Environment.NewLine}No employee was transferred.";


                MessageBox.Show(this, msg, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pcbMembers.BackgroundImage = checkedImage;

                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSellingPrice_Click(object sender, EventArgs e)
        {
            try
            {
                int result = 0;
                string msg = string.Empty;
                MessageBoxIcon icon;

                if (UserConfirmation(ProcessEnum.Migrate, ModuleEnum.SellingPrice))
                    return;

                #region Update selling price
                SalesController controller = new SalesController();

                var items = controller.GetUpdatedSellingPrice();

                result = controller.UpdateSellingPrice(items);

                if(result > 0)
                {
                    msg = $"{result.ToString("N0")} Item Uom updated succesfully";
                    pbSellingPrice.BackgroundImage = checkedImage;
                    icon = MessageBoxIcon.Information;
                    btnSellingPrice.Enabled = false;
                }
                else
                {
                    msg = "No item with selling price detected in POS1.";
                    icon = MessageBoxIcon.Warning;
                }

                MessageBox.Show(this, msg, "Information", MessageBoxButtons.OK, icon);
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Error : {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPostMigrate_Click(object sender, EventArgs e)
        {
            try
            {
                StartProcess(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format(ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
