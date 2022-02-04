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
                                SELECT transdate,reference,transtype,memberId, total AS 'amount' FROM sapt0
                                WHERE transtype IN ('CI','AP') AND DATE(transdate)= @date
                                UNION ALL
                                SELECT transdate,REFERENCE,transtype,memberId,total *-1 AS 'amount' FROM sapt0
                                WHERE left(crossreference,2) IN ('CI','AP') AND DATE(transdate)= @date
                                UNION ALL
                                SELECT transdate,REFERENCE,transtype,memberId,total *-1 AS 'amount' FROM sapr0
                                WHERE left(crossreference,2) IN ('CI','AP') AND DATE(transdate)= @date
                                UNION ALL
                                SELECT transdate,reference,transtype,employeeId as 'memberId', total AS 'amount' FROM sapt0
                                WHERE transtype IN ('CO','CT') AND DATE(transdate)= @date
                                UNION ALL
                                SELECT transdate,REFERENCE,transtype,employeeId as 'memberId',total *-1 AS 'amount' FROM sapt0
                                WHERE left(crossreference,2) IN ('CO','CT') AND DATE(transdate)= @date
                                UNION ALL
                                SELECT transdate,REFERENCE,transtype,employeeId as 'memberId',total *-1 AS 'amount' FROM sapr0
                                WHERE left(crossreference,2) IN ('CO','CT') AND DATE(transdate)= @date
                        ) AS x order by x.transdate;");

            return sQuery;
        }
     
        public static StringBuilder UpdateAccountCreditLimit()
        {
            return new StringBuilder(@"UPDATE acl00 SET chargeAmount = chargeAmount + @amount  WHERE memberId = @memberId AND transType=@transactionType;");
        }

    }
}
