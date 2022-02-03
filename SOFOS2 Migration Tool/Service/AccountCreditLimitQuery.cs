using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    public class AccountCreditLimitQuery
    {
        public static StringBuilder GetAccountCreditLimitQuery()
        {
            var sQuery = new StringBuilder();
            sQuery.Append(@"SELECT
                            f.idfile AS 'MemberId',
                            f.name AS 'MemberName',
                            null AS 'AccountNumber',
                            'CI' AS 'TransType',
                            c.credit AS 'ShareCapital',
                            0 AS 'ChargeAmount',
                            0 AS 'CreditBalance',
                            c.idAccount AS 'AccountCode',
                            1 AS 'Status',
                            (c.credit *0.75) AS 'CreditLimit',
                            'NON-COLLATERAL' AS 'ChargeType',
                            14 AS 'InterestRate',
                            30 AS 'Terms',
                            '430400000000000' AS 'InterestAccount'
                             FROM coabalances c
                            INNER JOIN files f on f.idFile=c.idFile
                             WHERE c.idAccount='311010000000000' AND DATE(c.dateForwarded)= @date;
                            ");
            return sQuery;
        }

        public static StringBuilder InsertAccountCreditLimitQuery()
        {
            var sQuery = new StringBuilder();

            sQuery.Append(@"INSERT INTO ACL00 (memberId, accountNumber, transType, creditLimit, chargeAmount, accountCode, status, creditBalance, branchCode, ChargeType, InterestRate, Terms, interestAccount) 
                            VALUES (@memberId, @accountNumber, @transType, @creditLimit, @chargeAmount, @accountCode, @status, @creditBalance, @branchCode, @ChargeType, @InterestRate, @Terms, @interestAccount)");

            return sQuery;
        }

        public static StringBuilder GetAccountCreditLimitByMemberId()
        {
            var sQuery = new StringBuilder();

            sQuery.Append(@"SELECT accountNum FROM ACL00 WHERE memberid = @memberId LIMIT 1;");

            return sQuery;
        }
        public static StringBuilder UpdateMemberShareCapital()
        {
            var sQuery = new StringBuilder();

            sQuery.Append(@"UPDATE cci00 SET shareCapital=@shareCapital WHERE memberId=@memberId;");

            return sQuery;
        }
        public static StringBuilder UpdateSalesInvoiceAccountNumberForMembersTransaction()
        {
            var sQuery = new StringBuilder();

            sQuery.Append(@"UPDATE SAPT0 S
                            SET AccountNo =(SELECT a.accountNumber from ACL00 a WHERE a.memberId=s.memberid AND transType=s.transtype LIMIT 1 )
                            WHERE AccountNo IS NULL AND transType NOT IN ('SI', 'CT','CO');");
            return sQuery;
        }
        public static StringBuilder UpdateSalesInvoiceAccountNumberForEmployeeTransaction()
        {
            var sQuery = new StringBuilder();

            sQuery.Append(@"UPDATE SAPT0 S
                            SET AccountNo =(SELECT a.accountNumber from ACL00 a WHERE a.memberId=s.EmployeeId AND transType=s.transtype LIMIT 1 )
                            WHERE AccountNo IS NULL AND transType NOT IN ('SI','CI','AP');");
            return sQuery;
        }

    }
    
}
