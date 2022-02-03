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
                                    0 AS 'NoEffectOnInventory',
                                    IF(f.type = 'SIDC','Non-Member',f.type) AS 'CustomerType',
                                    IF(f.type in ('SIDC','MEMBER','AMEMBER'),l.idFile,'') AS 'MemberId',
                                    IF(f.type in ('SIDC','MEMBER','AMEMBER'),f.name,'') AS 'MemberName',
                                    IF(f.type = 'EMPLOYEE',l.idFile,'') AS 'EmployeeID',
                                    IF(f.type = 'EMPLOYEE',f.name,'') AS 'EmployeeName',
                                    null AS 'YoungCoopID',
                                    null AS 'YoungCoopName',
                                    l.idaccount AS 'AccountCode',
                                    c.account AS 'AccountName',
                                    l.PaidToDate AS 'PaidToDate',
                                    IF(LEFT(l.reference, 2) IN ('CI','CO','AP','CT'), l.debit,l.credit) AS 'Total',
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
                                    null AS 'ColaReference'
                                    FROM ledger l
                                    INNER JOIN invoice i ON l.reference = i.reference
                                    INNER JOIN stocks s ON i.idstock = s.idstock
                                    LEFT JOIN files f ON l.idfile = f.idfile
                                    LEFT JOIN coa c ON l.idaccount = c.idaccount
                                    where LEFT(l.reference, 2) IN ('SI','CI','CO','AP','CT')
                                    AND date(l.date) = @date
                                    GROUP BY l.reference
                                    ORDER BY l.date ASC;
                            ");
                    break;

                case SalesEnum.SalesDetail:

                    sQuery.Append(@"SELECT
                                    i.reference AS 'Reference',
                                    p.barcode AS 'Barcode',
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

                                    0 AS 'CancelledQty'
                                    FROM invoice i
                                    INNER JOIN ledger l ON i.reference = l.reference
                                    INNER JOIN stocks s ON i.idstock = s.idstock
                                    INNER JOIN pcosting p ON i.idstock = p.idstock AND i.unit = p.unit
                                    WHERE
                                    /*left(i.reference, 2) = @transType AND date(l.date) = @date*/
                                    LEFT(i.reference, 2) IN ('SI','CI','CO','AP','CT')
                                    AND date(l.date) = @date
                                    GROUP BY i.reference, i.idstock, i.unit
                                    ORDER BY l.reference ASC;
                                    ");
                    break;

                case SalesEnum.SalesPayment:
                    sQuery.Append(@"SELECT
                                    reference AS 'Reference',
                                    idPaymentMethod AS 'PaymentCode',
                                    amount AS 'Amount',
                                    checkNo as 'CheckNumber',
                                    bank AS 'BankCode',
                                    DATE_FORMAT(checkDate, '%Y-%m-%d %H:%i:%s') AS 'CheckDate',
                                    DATE_FORMAT(date, '%Y-%m-%d %H:%i:%s') AS 'SystemDate',
                                    idUser AS 'idUser',
                                    left(reference, 2) AS 'TransType',
                                    '' AS 'AccountCode',
                                    '' AS 'AccountName',
                                    0 as 'ChangeAmount',
                                    extracted AS 'Extracted',
                                    0 AS 'OrDetailNum'
                                     FROM transactionpayments
                                    WHERE LEFT(reference, 2) IN('SI', 'CI', 'CO', 'AP', 'CT') AND date(date) = @date;
                                     ");
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
                    sQuery.Append(@"INSERT INTO SAPT0 (transNum, transDate, transType, reference, crossreference, NoEffectOnInventory, customerType, memberId, memberName, employeeID, employeeName, youngCoopID, youngCoopName, accountCode, accountName, paidToDate, Total, amountTendered, interestPaid, interestBalance, cancelled, status, extracted, colaReference, segmentCode, businessSegment, branchCode, signatory, remarks, systemDate, idUser, lrBatch, lrType, srDiscount, feedsDiscount, vat, vatExemptSales, vatAmount, warehouseCode, lrReference, kanegoDiscount, grossTotal, series, lastpaymentdate, allowNoEffectInventory, printed, TerminalNo, AccountNo) 
                            VALUES (@transNum, @transDate, @transType, @reference, @crossreference, @NoEffectOnInventory, @customerType, @memberId, @memberName, @employeeID, @employeeName, @youngCoopID, @youngCoopName, @accountCode, @accountName, @paidToDate, @Total, @amountTendered, @interestPaid, @interestBalance, @cancelled, @status, @extracted, @colaReference, @segmentCode, @businessSegment, @branchCode, @signatory, @remarks, @systemDate, @idUser, @lrBatch, @lrType, @srDiscount, @feedsDiscount, @vat, @vatExemptSales, @vatAmount, @warehouseCode, @lrReference, @kanegoDiscount, @grossTotal, @series, @lastpaymentdate, @allowNoEffectInventory, @printed, @TerminalNo, @AccountNo)");

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
    }


    public enum SalesEnum
    {
        SalesHeader, SalesDetail, SalesPayment
    }
}
