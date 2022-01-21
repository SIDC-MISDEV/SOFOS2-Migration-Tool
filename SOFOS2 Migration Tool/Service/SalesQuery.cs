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
                                    l.kanegodiscount AS 'FeedsDiscount',
                                    SUM(IF(s.taxable = '1' and LEFT(l.idfile, 2) IN ('NM'), (i.selling * i.quantity/(1+(12/100)))*12/100,0)) AS 'Vat',
                                    
                                    /* start of VatExemptSales*/
                                    (
                                        CASE
                                            WHEN LEFT(l.idfile, 2) IN ('NM') THEN SUM(IF(s.taxable = 0, i.selling * i.quantity,0))
                                            ELSE SUM(i.selling * i.quantity)
                                        END
                                    ) AS 'VatExemptSales',
                                    /* end of VatExemptSales*/
                                    
                                    DATE_FORMAT(l.timeStamp, '%Y-%m-%d %H:%i:%s') AS 'SystemDate',
                                    null AS 'ColaReference'
                                    FROM ledger l
                                    INNER JOIN invoice i ON l.reference = i.reference
                                    INNER JOIN stocks s ON i.idstock = s.idstock
                                    LEFT JOIN files f ON l.idfile = f.idfile
                                    LEFT JOIN coa c ON l.idaccount = c.idaccount
                                    where LEFT(l.reference, 2) IN ('SI','CI','CO','AP','CT')
                                    /* AND date(l.date) = '2022-01-17' */
                                    AND year(l.date) = '2022'
                                    GROUP BY l.reference
                                    ORDER BY l.date ASC;
                            ");

                    break;

                case SalesEnum.SalesDetail:

                    sQuery.Append(@"");

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
