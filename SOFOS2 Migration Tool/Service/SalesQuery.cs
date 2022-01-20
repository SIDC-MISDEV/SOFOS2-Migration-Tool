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
                                    DATE_FORMAT(l.date, '%Y-%m-%d %H:%i:%s') as 'TransDate',
                                    left(l.reference,2) as 'TransType',
                                    l.reference as 'Reference',
                                    l.crossreference as 'Crossreference',
                                    0 as 'NoEffectOnInventory',
                                    if(f.type = 'SIDC','Non-Member',f.type) as 'CustomerType',
                                    l.idFile as 'MemberId',
                                    if(f.type in ('SIDC','MEMBER','AMEMBER'),l.idFile,'') as 'MemberId',
                                    if(f.type in ('SIDC','MEMBER','AMEMBER'),f.name,'') as 'MemberName',
                                    if(f.type = 'EMPLOYEE',l.idFile,'') as 'EmployeeID',
                                    if(f.type = 'EMPLOYEE',f.name,'') as 'EmployeeName',
                                    ' ' as 'YoungCoopID',
                                    ' ' as 'YoungCoopName',
                                    l.idaccount as 'AccountCode',
                                    c.account as 'AccountName',
                                    l.PaidToDate as 'PaidToDate',
                                    if(LEFT(l.reference, 2) IN ('CI','CO','AP','CT'), l.debit,l.credit) as 'Total',
                                    SUM(i.selling * i.quantity) as 'Total',
                                    l.amountReceived as 'AmountTendered',
                                    0 as 'InterestPaid',
                                    0 as 'InterestBalance',
                                    l.cancelled as 'Cancelled',
                                    (
                                        CASE
                                            WHEN LEFT(l.reference, 2) IN ('CI','CO','AP','CT') and l.PaidToDate = l.debit THEN 'CLOSED'
                                            WHEN LEFT(l.reference, 2) IN ('CI','CO','AP','CT') and l.PaidToDate != l.debit THEN 'OPEN'
                                            ELSE 'CLOSED'
                                        END
                                    ) AS 'Status',

                                    l.extracted as 'Extracted',
                                    ' ' as 'ColaReference',
                                    l.signatory as 'Signatory',
                                    'x' as 'Remarks',
                                    'x' as 'SystemDate',
                                    l.idUser as 'IdUser',
                                    l.lrBatch as 'LrBatch',
                                    l.lrType as 'LrType',
                                    'x' as 'SrDiscount',
                                    'x' as 'FeedsDiscount',
                                    'x' as 'Vat',
                                    'x' as 'VatExemptSales',
                                    l.timeStamp as 'SystemDate',
                                    ' ' as 'ColaReference'
                                    FROM ledger l
                                    INNER JOIN invoice i ON l.reference = i.reference
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
