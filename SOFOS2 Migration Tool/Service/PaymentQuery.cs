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
                                    credit as total,
                                    date as transDate,
                                    l.idUser,
                                    l.idfile as memberId,
                                    '' as memberName,
                                    'CLOSED' as status,
                                    cancelled,
                                    'Migration Tool' as remarks, 
                                    1 as type,
                                    idaccount as accountCode, 
                                    signatory as paidBy,
                                    '' as branchCode,
                                    extracted,
                                    'Collection Receipt' as transType,
                                    '' as series,
                                    'CI' as refTransType
                                    FROM ledger l INNER JOIN files f ON l.idfile = f.idfile
                                    WHERE LEFT(reference,2)='OR' AND idaccount='112010000000001' AND date(date)='2021-10-15'");

                    break;

                case payment.CRDetail:

                    sQuery.Append(@"SELECT
	                        i.reference,
	                        p.barcode,
	                        i.idstock 'ItemCode',
                            s.name 'Description',
	                        i.unit 'UOM',
	                        i.unit 'UOMDescription',
	                        SUM(i.quantity) 'Quantity',
	                        i.quantity + i.variance 'Remaining',
	                        i.cost 'Price',
	                        SUM(i.cost * i.quantity) 'Total',
	                        i.unitQuantity 'Conversion',
	                        '' as 'AccountCode'
                        FROM invoice i
                        INNER JOIN ledger l ON i.reference = l.reference
                        INNER JOIN stocks s ON i.idstock = s.idstock
                        INNER JOIN pcosting p ON i.idstock = p.idstock AND i.unit = p.unit
                        WHERE 
                        left(i.reference, 2) = @transType AND date(l.date) = @date
                        GROUP BY i.reference, i.idstock, i.unit
                        ORDER BY l.reference ASC;");

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
        CRHeader, CRDetail, ORHeader, ORDetail
    }

}
