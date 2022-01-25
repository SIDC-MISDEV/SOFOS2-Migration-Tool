using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    public class ReturnFromCustomerQuery
    {
        public static StringBuilder GetReturnFromCustomerQuery(ReturnFromCustomerEnum process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case ReturnFromCustomerEnum.ReturnFromCustomerHeader:

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
                                    l.credit AS 'Total',
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
                                    l.reference as 'ColaReference',
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
                                    
                                    DATE_FORMAT(l.timeStamp, '%Y-%m-%d %H:%i:%s') AS 'SystemDate'
                                    
                                    FROM ledger l
                                    INNER JOIN invoice i ON l.reference = i.reference
                                    INNER JOIN stocks s ON i.idstock = s.idstock
                                    LEFT JOIN files f ON l.idfile = f.idfile
                                    LEFT JOIN coa c ON l.idaccount = c.idaccount
                                    where LEFT(l.reference, 2) = 'RC'
                                    AND date(l.date) = @date
                                    GROUP BY l.reference
                                    ORDER BY l.date ASC;
                            ");
                    break;

                case ReturnFromCustomerEnum.ReturnFromCustomerDetail:

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
                                    SUM(ifnull(i.discount,0)) AS 'Discount',
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
                                    LEFT(i.reference, 2) = 'RC'
                                    AND date(l.date) = @date
                                    GROUP BY i.reference, i.idstock, i.unit
                                    ORDER BY l.reference ASC;
                                    ");
                    break;
                default:
                    break;
            }

            return sQuery;
        }

        public static StringBuilder InsertReturnFromCustomerQuery(ReturnFromCustomerEnum process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case ReturnFromCustomerEnum.ReturnFromCustomerHeader:
                    sQuery.Append(@"INSERT INTO SAPR0 
                                    (transNum, transDate, transType, reference, NoEffectOnInventory, crossReference, customerType, memberId, memberName, employeeID, employeeName, youngCoopID, youngCoopName, accountCode, accountName, paidToDate, Total, amountTendered, cancelled, status, extracted, colaReference, segmentCode, businessSegment, branchCode, signatory, remarks, systemDate, idUser, lrBatch, lrType, warehouseCode, srDiscount, feedsDiscount, vatExemptSales, vatAmount, vat, series, AccountNo, TerminalNo) 
                                    VALUES 
                                    (@transNum, @transDate, @transType, @reference, @NoEffectOnInventory, @crossReference, @customerType, @memberId, @memberName, @employeeID, @employeeName, @youngCoopID, @youngCoopName, @accountCode, @accountName, @paidToDate, @Total, @amountTendered, @cancelled, @status, @extracted, @colaReference, @segmentCode, @businessSegment, @branchCode, @signatory, @remarks, @systemDate, @idUser, @lrBatch, @lrType, @warehouseCode, @srDiscount, @feedsDiscount, @vatExemptSales, @vatAmount, @vat, @series, @AccountNo, @TerminalNo)");

                    break;
                case ReturnFromCustomerEnum.ReturnFromCustomerDetail:
                    sQuery.Append(@"INSERT INTO SAPR1 
                            (transNum, barcode, itemCode, itemDescription, uomCode, uomDescription, quantity, cost, sellingPrice, discount, Total, conversion, systemDate, idUser, runningQty, averageCost, runningValue) 
                            VALUES
                            (@transNum, @barcode, @itemCode, @itemDescription, @uomCode, @uomDescription, @quantity, @cost, @sellingPrice, @discount, @Total, @conversion, @systemDate, @idUser, @runningQty, @averageCost, @runningValue)");
                    break;
                default:
                    break;
            }

            return sQuery;
        }
    }

    public enum ReturnFromCustomerEnum
    {
        ReturnFromCustomerHeader, ReturnFromCustomerDetail
    }

}
