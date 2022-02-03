using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    class PaymentQuery
    {
        public static StringBuilder GetQuery(payment process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case payment.CRHeader:

                    sQuery.Append(@"SELECT 
                                    '' as transNum,
                                    reference, 
                                    sum(credit) as total,
                                    DATE_FORMAT(date, '%Y-%m-%d %H:%i:%s') as transDate,
                                    idUser,
                                    idfile as memberId,
                                    '' as memberName,
                                    'CLOSED' as status,
                                    cancelled,
                                    'Migration Tool' as remarks, 
                                    1 as type,
                                    @principalaccount as accountCode, 
                                    signatory as paidBy,
                                    '' as branchCode,
                                    extracted,
                                    'Collection Receipt' as transType,
                                    '' as series,
                                    'CI' as refTransType
                                    FROM ledger 
                                    WHERE LEFT(reference,2)=@transprefix AND idaccount IN (@principalaccount,@oldinterestaccount,@newinterestaccount) AND credit > 0 AND date(date)=@transdate GROUP BY idfile");
                    break;

                case payment.CRDetail:

                    sQuery.Append(@"SELECT  
                                    '' transNum,
                                    reference, 
                                    '' crossReference, 
                                    credit as amount, 
                                    idUser, 
                                    '' balance,
                                    idaccount as accountCode, 
                                    if(idaccount=@principalaccount,'P','I') as pType, 
                                    '' as accountName,
                                    if(idaccount=@oldinterestaccount or idaccount=@newinterestaccount,'CI','') as refTransType 
                                    FROM ledger
                                    WHERE LEFT(reference,2)=@transprefix AND idaccount IN (@principalaccount,@oldinterestaccount,@newinterestaccount) AND credit > 0 AND date(date)=@transdate");
                    break;

                case payment.Balance:

                    sQuery.Append(@"SELECT balance-amount as balance FROM fp100 WHERE accountcode=@principalaccount AND crossreference=@crossreference AND transnum < @transnum limit 1;");
                    break;

                case payment.JVHeader:

                    sQuery.Append(@"SELECT 
                                    '' transNum, 
                                    '' as transNum,
                                    reference, 
                                    sum(credit) as total, 
                                    DATE_FORMAT(date, '%Y-%m-%d %H:%i:%s') as transDate,
                                    idUser,
                                    'CLOSED' as status,
                                    cancelled,
                                    'Migration Tool' remarks 
                                    FROM ledger 
                                    WHERE LEFT(reference,2)=@transprefix AND idaccount IN (@principalaccount,@oldinterestaccount,@newinterestaccount) AND credit > 0 AND date(date)=@transdate GROUP BY idfile");
                    break;

                case payment.JVDetail:

                    sQuery.Append(@"SELECT
                                    '' as detailNum, 
                                    '' transNum,
                                    reference, 
                                    idaccount as accountCode,
                                    '' as crossReference,
                                    idUser,
                                    if(debit is null,0.00,debit) as debit,
                                    if(credit is null,0.00,credit) as credit, 
                                    idfile as memberId,
                                    '' as memberName,
                                    '' as accountName,
                                    '' as refTransType,
                                    '' as intComputed,
                                    '' as paidToDate,
                                    'CLOSED' as status,
                                    '' as lastpaymentdate,
                                    '' as AccountNo
                                    FROM ledger 
                                    WHERE LEFT(reference,2)=@transprefix AND idaccount IN (@principalaccount,@oldinterestaccount,@newinterestaccount) AND credit > 0 AND date(date)=@transdate");
                    break;

                case payment.Invoice:

                    sQuery.Append(@"SELECT * FROM 
                                    (SELECT 
                                    DATE_FORMAT(transDate, '%Y-%m-%d %H:%i:%s') as transDate, 
                                    reference,
                                    memberId,
                                    memberName,
                                    accountCode,
                                    paidToDate,
                                    Total,
                                    cancelled,
                                    status,
                                    intComputed,
                                    lastpaymentdate,
                                    AccountNo
                                    FROM sapt0
                                    WHERE accountcode=@principalaccount AND status='OPEN' AND cancelled=0 AND memberId=@memberId AND accountNo=@accountno 

                                    UNION ALL 

                                    SELECT DATE_FORMAT(transDate, '%Y-%m-%d %H:%i:%s') as transDate, 
                                    reference,
                                    memberId,
                                    memberName,
                                    accountCode,
                                    paidToDate,
                                    Total,
                                    cancelled,
                                    b.status,
                                    intComputed,
                                    lastpaymentdate,
                                    AccountNo FROM fjv00 a INNER JOIN fjv10 b ON a.transNum=b.transNum WHERE debit>0 AND accountcode=@principalaccount AND b.status='OPEN' AND cancelled=0 AND memberId=@memberId AND accountNo=@accountno) as x 
                                    ORDER BY x.transDate ASC");
                    break;

                case payment.TransactionPayments:

                    sQuery.Append(@"SELECT transNum, transDate, reference, AccountCode, memberId, AccountNo, amount, balance, idUser FROM (
                                    #CR
                                    SELECT 
                                    a.transNum,
                                    DATE_FORMAT(transDate, '%Y-%m-%d %H:%i:%s') as transDate,
                                    memberId,
                                    AccountNo,
                                    reference,
                                    amount,
                                    b.AccountCode,
                                    balance,
                                    a.idUser
                                    FROM fp000 a INNER JOIN fp100 b ON a.transNum=b.transNum 
                                    WHERE b.accountCode IN (@principalaccount,@oldinterestaccount,@newinterestaccount) AND amount > 0 AND date(transDate)=@transdate
                                    
                                    UNION ALL
                                    
                                    #JV
                                    SELECT 
                                    a.transNum,
                                    DATE_FORMAT(transDate, '%Y-%m-%d %H:%i:%s') as transDate,
                                    memberId,
                                    AccountNo,
                                    reference,
                                    credit as amount,
                                    b.AccountCode,
                                    0 as balance,
                                    a.idUser 
                                    from fjv00 a INNER JOIN fjv10 b ON a.transNum=b.transNum
                                    WHERE credit>0 AND b.accountCode IN (@principalaccount,@oldinterestaccount,@newinterestaccount) AND credit > 0 AND date(transDate)=@transdate) as x order by transdate, reference ASC;");
                    break;

                case payment.Interest:

                    sQuery.Append(@"select sum(round(intamount,2)) 'intAmount', reference from(
                            select CASE
                                WHEN datediff(lastpaymentdate,transdate)>@intAfter and lastpaymentdate is not null THEN datediff(@transDate,lastpaymentdate) * (@intRate/100)/360 * sum(total-paidtodate)
                                WHEN datediff(lastpaymentdate,transdate)>@intAfter and datediff(@transDate,transdate)>@intAfter THEN (datediff(@transDate,transdate)-@intAfter) * (@intRate/100)/360 * sum(total-paidtodate)
                                WHEN datediff(lastpaymentdate,transdate)<=@intAfter and datediff(@transDate,transdate)>@intAfter THEN (datediff(@transDate,transdate)-@intAfter) * (@intRate/100)/360 * sum(total-paidtodate)
                                WHEN datediff(@transDate,transdate)>@intAfter and lastpaymentdate is null THEN (datediff(@transDate,transdate)-@intAfter) * (@intRate/100)/360 * sum(total-paidtodate)
                                ELSE 0
                            END 'intamount', reference
                            from fjv00 a, fjv10 b where a.transNum =b.transNum and accountcode = @principalAccount 
                             and a.cancelled=0 and memberId=@memberId and AccountNo=@accountno and reference=@reference group by b.transnum

                            union all

                            select 
                                CASE
                                    WHEN datediff(lastpaymentdate,transdate)>@intAfter and lastpaymentdate is not null THEN datediff(@transDate,lastpaymentdate) * (@intRate/100)/360 * sum(total-paidtodate)
                                    WHEN datediff(lastpaymentdate,transdate)>@intAfter and datediff(@transDate,transdate)>@intAfter THEN (datediff(@transDate,transdate)-@intAfter) * (@intRate/100)/360 * sum(total-paidtodate)
                                    WHEN datediff(lastpaymentdate,transdate)<=@intAfter and datediff(@transDate,transdate)>@intAfter THEN (datediff(@transDate,transdate)-@intAfter) * (@intRate/100)/360 * sum(total-paidtodate)
                                    WHEN datediff(@transDate,transdate)>@intAfter and lastpaymentdate is null THEN (datediff(@transDate,transdate)-@intAfter) * (@intRate/100)/360 * sum(total-paidtodate)
                                    ELSE 0
                                END 'intamount', reference
                            from sapt0 where accountcode = @principalAccount and cancelled=0 
                            and memberId=@memberId and AccountNo=@accountno and reference=@reference group by reference) as x group by reference");
                    break;

                default:
                    break;
            }

            return sQuery;
        }

        public static StringBuilder InsertQuery(payment process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case payment.CRHeader:

                    sQuery.Append(@"INSERT INTO fp000 (transNum,reference,Total,transDate,idUser,memberId,memberName,status,cancelled,remarks,type,accountCode,paidBy,branchCode,extracted,transType,refTransType,AccountNo) 
                            VALUES (@transNum,@reference,@Total,@transDate,@idUser,@memberId,@memberName,@status,@cancelled,@remarks,@type,@accountCode,@paidBy,@branchCode,@extracted,@transType,@refTransType,@AccountNo)");

                    break;
                case payment.CRDetail:

                    sQuery.Append(@"INSERT INTO fp100 (transNum,crossReference,amount,idUser,balance,accountCode,pType,accountName,refTransType) 
                            VALUES (@transNum,@crossReference,@amount,@idUser,@balance,@accountCode,@pType,@accountName,@refTransType)");

                    break;
                case payment.ORHeader:

                    sQuery.Append(@"INSERT INTO fp000 (transNum,reference,Total,transDate,idUser,memberId,memberName,status,cancelled,remarks,type,accountCode,paidBy,branchCode,extracted,transType,refTransType,AccountNo) 
                            VALUES (@transNum,@reference,@Total,@transDate,@idUser,@memberId,@memberName,@status,@cancelled,@remarks,@type,@accountCode,@paidBy,@branchCode,@extracted,@transType,@refTransType,@AccountNo)");

                    break;
                case payment.ORDetail:

                    sQuery.Append(@"INSERT INTO fp100 (transNum,crossReference,amount,idUser,balance,accountCode,pType,accountName,refTransType) 
                            VALUES (@transNum,@crossReference,@amount,@idUser,@balance,@accountCode,@pType,@accountName,@refTransType)");

                    break;
                case payment.Interest:

                    sQuery.Append(@"INSERT INTO fint0 (transNum, transDate, transType, reftransType, reference, amount, memberId, memberName, accountCode, idUser, crossReference, AccountNo)
                            VALUES (@transnum, @transdate, @transtype, @reftranstype, @reference, @amount, @memberid, @membername, @accountcode, @iduser, @crossreference, @Accountno)");

                    break;
                default:
                    break;
            }

            return sQuery;
        }

        public static StringBuilder UpdateQuery(payment process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case payment.Invoice:

                    sQuery.Append(@"UPDATE sapt0 SET paidToDate=@paidtodate, intComputed=@intcomputed, status=@status, lastpaymentdate=@lastpaymentdate WHERE reference=@reference");
                    break;
                case payment.TransactionPayments:

                    sQuery.Append(@"UPDATE fp000 a INNER JOIN fp100 b ON a.transNum=b.transNum SET crossReference=@crossreference, balance=@balance WHERE memberId=@memberid AND AccountNo=@accountno AND reference=@reference");
                    break;
                case payment.CreditLimit:

                    sQuery.Append(@"UPDATE acl00 SET chargeAmount=@chargeamount WHERE memberId=@memberid AND accountNumber=@accountno AND transType=@transtype");
                    break;

                default:
                    break;
            }

            return sQuery;
        }

    }

    public enum payment
    {
        CRHeader, CRDetail, JVHeader, JVDetail, ORHeader, ORDetail, Invoice, CreditLimit, TransactionPayments, Interest, Balance
    }

}
