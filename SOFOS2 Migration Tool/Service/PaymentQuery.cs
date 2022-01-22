using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    class PaymentQuery
    {
        public static StringBuilder GetCRQuery(payment process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case payment.CRHeader:

                    sQuery.Append(@"SELECT 
                                    '' as transNum,
                                    reference, 
                                    sum(credit) as total,
                                    date as transDate,
                                    l.idUser,
                                    l.idfile as memberId,
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
                                    FROM ledger l INNER JOIN files f ON l.idfile = f.idfile
                                    WHERE LEFT(reference,2)=@transprefix AND idaccount in (@principalaccount,@oldinterestaccount,@newinterestaccount) AND date(date)=@transdate GROUP BY memberid");
                    break;

                case payment.CRDetail:

                    sQuery.Append(@"SELECT 
                                    '' as detailNum, 
                                    '' transNum, 
                                    '' crossReference, 
                                    credit as amount, 
                                    idUser, 
                                    '' balance,
                                    idaccount as accountCode, 
                                    if(idaccount='112010000000001','P','I') as pType, 
                                    '' as accountName,
                                    if(idaccount='441200000000000' or idaccount='430400000000000','CI','') as refTransType 
                                    FROM ledger
                                    WHERE LEFT(reference,2)=@transprefix and idaccount in (@principalaccount,@oldinterestaccount,@newinterestaccount) and date(date)=@transdate");
                    break;

                case payment.Invoice:

                    sQuery.Append(@"SELECT 
                                    '' as detailNum, 
                                    '' transNum, 
                                    '' crossReference, 
                                    credit as amount, 
                                    idUser, 
                                    '' balance,
                                    idaccount as accountCode, 
                                    if(idaccount='112010000000001','P','I') as pType, 
                                    '' as accountName,
                                    if(idaccount='441200000000000','CI','') as refTransType 
                                    FROM ledger
                                    WHERE LEFT(reference,2)='OR' and idaccount in('112010000000001','441200000000000') and date(date)='2021-10-15'");
                    break;

                case payment.CreditLimit:

                    sQuery.Append(@"SELECT 
                                    '' as detailNum, 
                                    '' transNum, 
                                    '' crossReference, 
                                    credit as amount, 
                                    idUser, 
                                    '' balance,
                                    idaccount as accountCode, 
                                    if(idaccount='112010000000001','P','I') as pType, 
                                    '' as accountName,
                                    if(idaccount='441200000000000','CI','') as refTransType 
                                    FROM ledger
                                    WHERE LEFT(reference,2)='OR' and idaccount in('112010000000001','441200000000000') and date(date)='2021-10-15'");
                    break;
                default:
                    break;
            }

            return sQuery;
        }

        public static StringBuilder InsertCR(payment process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case payment.CRHeader:

                    sQuery.Append(@"INSERT INTO PPR00 (vendorCode,vendorName,transNum,reference,crossreference,Total,transType,toWarehouse,fromWarehouse,segmentCode,businessSegment,branchCode,remarks,cancelled,transDate,idUser,printed, extracted) 
                            VALUES (@vendorCode,@vendorName,@transNum,@reference,@crossreference,@Total,@transType,@toWarehouse,@fromWarehouse,@segmentCode,@businessSegment,@branchCode,@remarks,@cancelled,@transDate,@idUser,@printed, @extracted)");

                    break;
                case payment.CRDetail:

                    sQuery.Append(@"INSERT INTO PPR10 (barcode,transNum,itemCode,itemDescription,uomCode,uomDescription,quantity,remaining,price,Total,conversion,accountCode,transDate,idUser) 
                            VALUES (@barcode,@transNum,@itemCode,@itemDescription,@uomCode,@uomDescription,@quantity,@remaining,@price,@Total,@conversion,@accountCode,@transDate,@idUser)");

                    break;
                default:
                    break;
            }

            return sQuery;
        }


    }

    public enum payment
    {
        CRHeader, CRDetail, ORHeader, ORDetail, Invoice, CreditLimit
    }

}
