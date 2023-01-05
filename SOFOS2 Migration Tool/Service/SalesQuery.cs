using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    public class SalesQuery
    {
        public static StringBuilder GetSalesQuery(SalesEnum process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case SalesEnum.SalesHeader:

                    sQuery.Append(@"SELECT
                                    DATE_FORMAT(l.date, '%Y-%m-%d %H:%i:%s') AS 'TransDate',
                                    left(l.reference,2) AS 'TransType',
                                    l.reference AS 'Reference',
                                    l.crossreference AS 'Crossreference',
                                    CASE WHEN LEFT(l.reference, 2) =  'GO' OR LEFT(l.reference, 2) =  'VS' OR LEFT(l.reference, 2) =  'RO' THEN 1 ELSE 0 END AS 'NoEffectOnInventory',
                                    CASE WHEN (f.type = 'NON-MEMBER' OR f.type = 'SIDC') AND left(l.reference, 2) NOT IN ('CO','CT') THEN 'Non-Member'
									     WHEN (f.type = 'MEMBER' OR f.type = 'AMEMBER') AND left(l.reference, 2) NOT IN ('CO','CT') THEN 'Member'
									     WHEN f.type = 'EMPLOYEE' THEN 'Employee'
										 ELSE 'Branch' END as CustomerType,
                                    IF(f.type in ('SIDC','MEMBER','AMEMBER', 'NON-MEMBER','WAREHOUSE'),l.idFile, e.idfile) AS 'MemberId',
                                    IF(f.type in ('SIDC','MEMBER','AMEMBER', 'WAREHOUSE'),f.name, '') AS 'MemberName',
                                    IF(f.type = 'EMPLOYEE',l.idFile,'') AS 'EmployeeID',
                                    IF(f.type = 'EMPLOYEE',f.name,'') AS 'EmployeeName',
                                    null AS 'YoungCoopID',
                                    null AS 'YoungCoopName',
                                    l.idaccount AS 'AccountCode',
                                    c.account AS 'AccountName',
                                    l.PaidToDate AS 'PaidToDate',
                                    IF(LEFT(l.reference, 2) IN ('CI','CO','AP','CT', 'SB', 'CG', 'PI', 'EC', 'CE'), l.debit - l.discount - l.kanegodiscount, l.credit - l.discount - l.kanegodiscount) AS 'Total',
                                    IF(LEFT(l.reference, 2) IN ('CI','CO','AP','CT', 'SB', 'CG', 'PI', 'EC', 'CE'), l.debit, l.credit) as 'GrossTotal',
                                    SUM(i.selling * i.quantity) AS 'TotalBasedOnDetails',
                                    l.amountReceived AS 'AmountTendered',
                                    0 AS 'InterestPaid',
                                    0 AS 'InterestBalance',
                                    l.cancelled AS 'Cancelled',

                                    /* start of Status*/
                                    (
                                        CASE
                                            WHEN LEFT(l.reference, 2) IN ('CI','CO','AP','CT') and l.PaidToDate = l.debit THEN 'CLOSED'
                                            WHEN LEFT(l.reference, 2) IN ('CI','CO','AP','CT') and l.PaidToDate != l.debit THEN 'OPEN'
                                            ELSE 'CLOSED'
                                        END
                                    ) AS 'Status',
                                    /* end of Status*/

                                    l.extracted AS 'Extracted',
                                    ' ' as 'ColaReference',
                                    l.signatory AS 'Signatory',
                                    null AS 'Remarks',
                                    l.idUser AS 'IdUser',
                                    l.lrBatch AS 'LrBatch',
                                    l.lrType AS 'LrType',
                                    0 AS 'SrDiscount',
                                    l.kanegodiscount AS 'Kanegodiscount',
                                    SUM(ifnull(i.discount,0)) AS 'Feedsdiscount',
                                    
                                    /* start of Vat*/
                                    SUM(IF(s.taxable = '1' and LEFT(l.idfile, 2) IN ('NM'), (i.selling * i.quantity/(1+(12/100)))*12/100,0)) AS 'Vat',
                                    /* end of Vat*/

                                    /* start of VatExemptSales*/
                                    (
                                        CASE
                                            WHEN LEFT(l.idfile, 2) IN ('NM') THEN SUM(IF(s.taxable = 0, i.selling * i.quantity,0))
                                            ELSE SUM(i.selling * i.quantity)
                                        END
                                    ) AS 'VatExemptSales',
                                    /* end of VatExemptSales*/

                                    /* start of VatAmount*/
                                    (
                                        CASE
                                            WHEN LEFT(l.idfile, 2) IN ('NM') THEN SUM(IF(s.taxable = 1, i.selling * i.quantity,0))
                                            ELSE SUM(i.selling * i.quantity)
                                        END
                                    ) AS 'VatAmount',
                                    /* end of VatAmount*/
                                    DATE_FORMAT(l.timeStamp, '%Y-%m-%d %H:%i:%s') AS 'SystemDate',
                                    null AS 'ColaReference',
                                    l.sow,
                                    LPAD(parity, 5, '0') as parity
                                    FROM ledger l
                                    INNER JOIN invoice i ON l.reference = i.reference
                                    INNER JOIN stocks s ON i.idstock = s.idstock
                                    LEFT JOIN files f ON l.idfile = f.idfile
                                    LEFT JOIN coa c ON l.idaccount = c.idaccount
                                    LEFT JOIN employees e ON l.idfile = e.idemployee
                                    where LEFT(l.reference, 2) IN ('SI','CI','CO','AP','CT','EC','FS','RT','CP','SB','PI','CB','BT','CS','RT','CL', 'CG', 'OL', 'CE')
                                    AND date(l.date) = @date
                                    GROUP BY l.reference
                                    ORDER BY l.date ASC;
                            ");
                    break;

                case SalesEnum.SalesDetail:

                    sQuery.Append(@"SELECT
                                    i.reference AS 'Reference',
                                    IF(length(trim(p.barcode)) > 0, p.barcode, CONCAT(i.idstock, '-',i.unit)) AS 'Barcode',
                                    i.idstock AS 'ItemCode',
                                    s.name AS 'ItemDescription',
                                    i.unit AS 'UomCode',
                                    i.unit AS 'UomDescription',
                                    SUM(i.quantity) AS 'Quantity',
                                    0 AS 'Cost',
                                    i.selling AS 'SellingPrice',
                                    SUM(ifnull(i.discount,0)) AS 'Feedsdiscount',
                                    SUM(i.amount) AS 'Total',
                                    i.unitQuantity AS 'Conversion',
                                    DATE_FORMAT(i.timeStamp, '%Y-%m-%d %H:%i:%s') AS 'SystemDate',
                                    i.idUser AS 'IdUser',
                                    0 AS 'Srdiscount',
                                    SUM(i.quantity) AS 'RunningQuantity',
                                    0 AS 'KanegoDiscount',
                                    0 AS 'AverageCost',
                                    0 AS 'RunningValue',
                                    0 AS 'RunningQty',
                                    SUM(i.amount) AS 'Linetotal',
                                    0 AS 'DedDiscount',
                                    SUM(IF(s.taxable = '1' and LEFT(i.idfile, 2) IN ('NM'), (i.selling * i.quantity/(1+(12/100)))*12/100,0)) AS 'Vat',

                                    /* start of Vatable*/
                                    (
                                        CASE
                                            WHEN LEFT(i.idfile, 2) IN ('NM') THEN SUM(IF(s.taxable = 1, i.selling * i.quantity,0))
                                            ELSE 0
                                        END
                                    ) AS 'Vatable',
                                    /* end of Vatable*/

                                    /* start of VatExemptSales*/
                                    (
                                        CASE
                                            WHEN LEFT(i.idfile, 2) IN ('NM') THEN SUM(IF(s.taxable = 0, i.selling * i.quantity,0))
                                            ELSE SUM(i.selling * i.quantity)
                                        END
                                    ) AS 'Vatexempt',
                                    /* end of VatExemptSales*/

                                    0 AS 'CancelledQty',
                                    CASE WHEN s.packaging = 25 THEN 1
										 WHEN s.packaging = 50 THEN 2
										 ELSE 0 END AS 'packaging',
                                    s.CatID
                                    FROM invoice i
                                    INNER JOIN ledger l ON i.reference = l.reference
                                    INNER JOIN stocks s ON i.idstock = s.idstock
                                    INNER JOIN pcosting p ON i.idstock = p.idstock AND i.unit = p.unit
                                    WHERE
                                    /*left(i.reference, 2) = @transType AND date(l.date) = @date*/
                                    LEFT(i.reference, 2) IN ('SI','CI','CO','AP','CT','EC','FS','RT','CP','SB','PI','CB','BT','CS','RT','CL', 'CG', 'OL', 'CE')
                                    AND date(l.date) = @date
                                    GROUP BY i.reference, i.idstock, i.unit
                                    ORDER BY l.reference ASC;
                                    ");
                    break;

                case SalesEnum.SalesPayment:
                    sQuery.Append(@"SELECT
                                        b.reference AS 'Reference',
                                        CASE WHEN b.idPaymentMethod = 'GC' THEN 'Gift Check' ELSE b.idpaymentmethod END AS 'PaymentCode',
                                        b.amount AS 'Amount',
                                        IF(b.idpaymentmethod = 'CASH', NULL, b.checkno) as 'CheckNumber',
                                        b.bank AS 'BankCode',
                                        IF(b.idpaymentmethod = 'CASH', NULL, DATE_FORMAT(b.checkDate, '%Y-%m-%d %H:%i:%s')) AS 'CheckDate',
                                        DATE_FORMAT(b.date, '%Y-%m-%d %H:%i:%s') AS 'SystemDate',
                                        b.idUser AS 'idUser',
                                        left(b.reference, 2) AS 'TransType',
                                        '' AS 'AccountCode',
                                        '' AS 'AccountName',
                                        ROUND(b.amount - a.credit, 2) as 'ChangeAmount',
                                        b.extracted AS 'Extracted',
                                        0 AS 'OrDetailNum'
                                    FROM transactionpayments b
                                    INNER JOIN ledger a ON a.reference = b.reference      
                                    WHERE LEFT(b.reference, 2) IN ('SI','CI','CO','AP','CT','EC','FS','RT','CP','SB','PI','CB','BT','CS','RT','CL', 'CG', 'OL', 'CE') AND date(b.date) = @date;");
                    break;
                case SalesEnum.SellingPrice:

                    sQuery.Append(@"SELECT
	                                    idstock, unit, selling
                                    FROM pcosting WHERE selling > 0
                                    order by idstock, unit ASC;");

                    break;
                default:
                    break;
            }

            return sQuery;
        }

        public static StringBuilder InsertSalesQuery(SalesEnum process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case SalesEnum.SalesHeader:
                    sQuery.Append(@"INSERT INTO SAPT0 (transNum, transDate, transType, reference, crossreference, NoEffectOnInventory, customerType, memberId, memberName, employeeID, employeeName, youngCoopID, youngCoopName, accountCode, accountName, paidToDate, Total, amountTendered, interestPaid, interestBalance, cancelled, status, extracted, colaReference, segmentCode, businessSegment, branchCode, signatory, remarks, systemDate, idUser, lrBatch, lrType, srDiscount, feedsDiscount, vat, vatExemptSales, vatAmount, warehouseCode, lrReference, kanegoDiscount, grossTotal, series, lastpaymentdate, allowNoEffectInventory, printed, TerminalNo, AccountNo, sow, parity) 
                            VALUES (@transNum, @transDate, @transType, @reference, @crossreference, @NoEffectOnInventory, @customerType, @memberId, @memberName, @employeeID, @employeeName, @youngCoopID, @youngCoopName, @accountCode, @accountName, @paidToDate, @Total, @amountTendered, @interestPaid, @interestBalance, @cancelled, @status, @extracted, @colaReference, @segmentCode, @businessSegment, @branchCode, @signatory, @remarks, @systemDate, @idUser, @lrBatch, @lrType, @srDiscount, @feedsDiscount, @vat, @vatExemptSales, @vatAmount, @warehouseCode, @lrReference, @kanegoDiscount, @grossTotal, @series, @lastpaymentdate, @allowNoEffectInventory, @printed, @TerminalNo, @AccountNo, @sow, @parity)");

                    break;
                case SalesEnum.SalesDetail:
                    sQuery.Append(@"INSERT INTO SAPT1 (transNum, barcode, itemCode, itemDescription, uomCode, uomDescription, quantity, cost, sellingPrice, feedsdiscount, Total, conversion, systemDate, idUser, srdiscount, runningQuantity, kanegoDiscount, averageCost, runningValue, runningQty, linetotal, dedDiscount, vat, vatable, vatexempt, cancelledQty) 
                            VALUES (@transNum, @barcode, @itemCode, @itemDescription, @uomCode, @uomDescription, @quantity, @cost, @sellingPrice, @feedsdiscount, @Total, @conversion, @systemDate, @idUser, @srdiscount, @runningQuantity, @kanegoDiscount, @averageCost, @runningValue, @runningQty, @linetotal, @dedDiscount, @vat, @vatable, @vatexempt, @cancelledQty)");

                    break;

                case SalesEnum.SalesPayment:
                    sQuery.Append(@"INSERT INTO FTP00 (transNum, paymentCode, amount, checkNumber, bankCode, checkDate, systemDate, idUser, transType, accountCode, accountName, changeAmount, extracted, orDetailNum) 
                            VALUES (@transNum, @paymentCode, @amount, @checkNumber, @bankCode, @checkDate, @systemDate, @idUser, @transType, @accountCode, @accountName, @changeAmount, @extracted, @orDetailNum)");

                    break;
                default:
                    break;
            }

            return sQuery;
        }

        public static StringBuilder GetKanegoItemCategory()
        {
            return new StringBuilder(@"SELECT icnum FROM ic000 WHERE icnum IN (SELECT catid FROM sdsc0) AND prefix <> 'RCE'");
        }

        public static StringBuilder GetKanegoRiceDiscount()
        {
            return new StringBuilder(@"SELECT id, NoBagsFrom, NoBagsTo, Discountper25kg FROM sdsb0;");
        }

        public static StringBuilder GetKanegoNonRiceDiscount()
        {
            return new StringBuilder(@"SELECT id, amountFrom, amountTo, percentage FROM sds00;");
        }

        public static StringBuilder UpdateAccountNumber()
        {
            return new StringBuilder(@"UPDATE sapt0 SET accountCode = @accountCode, accountName = @accountName WHERE transtype = @transtype AND date(transdate) = @date");
        }

        public static StringBuilder GetAccountNumber()
        {
            return new StringBuilder(@"SELECT t.transtype, t.accountCode, a.accountName
                    FROM sst00 t
                    INNER JOIN aca00 a ON t.accountCode = a.accountCode
                    where transtype IN ('CT', 'CO', 'CI');");
        }

        public static StringBuilder GetAccountNumberCreditLimit()
        {
            return new StringBuilder(@"SELECT memberid, transtype, accountNumber from acl00 WHERE transtype != 'CI' AND memberid IN (SELECT employeeid FROM sapt0 WHERE length(employeeid) > 0 AND date(transdate) = @date)
                UNION ALL
                SELECT memberid, transtype, accountNumber from acl00 WHERE transtype = 'CI' AND chargetype = 'NON-COLLATERAL' AND memberid IN(SELECT memberid FROM sapt0 WHERE memberid <> 'NM00000' AND date(transdate) = @date)");
        }

        public static StringBuilder UpdateAccountNumberCreditLimit(bool isEmployee)
        {
            string query = string.Empty;

            if (isEmployee)
                query = @"UPDATE sapt0 SET accountNo = @accountno WHERE transtype = @transtype AND employeeid = @memberid AND date(transdate) = @date";
            else
                query = @"UPDATE sapt0 SET accountNo = @accountno WHERE transtype = @transtype AND memberid = @memberid AND date(transdate) = @date";


            return new StringBuilder(query);
        }
    }


    public enum SalesEnum
    {
        SalesHeader, SalesDetail, SalesPayment, SellingPrice
    }
}
