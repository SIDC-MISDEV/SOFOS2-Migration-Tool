using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    public class ReComputeSalesCreditQuery
    {
        public static StringBuilder GetSalesAndReturnFromCustomerTransactions()
        {
            var sQuery = new StringBuilder();

            sQuery.Append(@"SELECT x.* FROM (
                                SELECT transdate,reference,transtype,memberId, total AS 'amount', AccountNo FROM sapt0
                                WHERE transtype IN ('CI','AP','WR') AND DATE(transdate)= @date
                                UNION ALL
                                SELECT transdate,REFERENCE,transtype,memberId,total *-1 AS 'amount', AccountNo FROM sapt0
                                WHERE left(crossreference,2) IN ('CI','AP','WR') AND DATE(transdate)= @date
                                UNION ALL
                                SELECT transdate,REFERENCE,transtype,memberId,total *-1 AS 'amount', AccountNo FROM sapr0
                                WHERE left(crossreference,2) IN ('CI','AP','WR') AND DATE(transdate)= @date
                                UNION ALL
                                SELECT transdate,reference,transtype,employeeId as 'memberId', total AS 'amount', AccountNo FROM sapt0
                                WHERE transtype IN ('CO','CT') AND DATE(transdate)= @date
                                UNION ALL
                                SELECT transdate,REFERENCE,transtype,employeeId as 'memberId',total *-1 AS 'amount', AccountNo FROM sapt0
                                WHERE left(crossreference,2) IN ('CO','CT') AND DATE(transdate)= @date
                                UNION ALL
                                SELECT transdate,REFERENCE,transtype,employeeId as 'memberId',total *-1 AS 'amount', AccountNo FROM sapr0
                                WHERE left(crossreference,2) IN ('CO','CT') AND DATE(transdate)= @date
                                UNION ALL 
                                SELECT transdate,reference,'CI' transtype,memberId, total AS 'amount', AccountNo FROM fjv00 a, fjv10 b
                                WHERE a.transNum=b.transNum AND debit > 0 AND accountCode = '112010000000001' AND  DATE(transdate)= @date
                        ) AS x order by x.transdate;");

            return sQuery;
        }
     
        public static StringBuilder UpdateAccountCreditLimit()
        {
            return new StringBuilder(@"UPDATE acl00 SET chargeAmount = chargeAmount + @amount  WHERE memberId = @memberId AND transType=@transactionType AND accountNumber=@accountNo;");
        }

    }
}
