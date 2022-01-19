using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    public class PurchasingQuery
    {
        public static StringBuilder GetPRQuery(PR process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case PR.PRHeader:

                    sQuery.Append(@"SELECT
                              DATE_FORMAT(l.date, '%Y-%m-%d %H:%i:%s') as date,
                              l.reference,
                              SUM(i.quantity) 'Quantity',
                              SUM(i.cost * i.quantity) as 'Total',
                              l.crossReference 'Remarks',
                              l.idFile as 'Vendor',
                              f.name as 'VendorName',
                              f.address as 'VendorAddress',
                              left(l.reference, 2) 'TransType',
                              l.cancelled,
                              l.extracted
                            FROM ledger l
                            INNER JOIN invoice i ON l.reference = i.reference
                            INNER JOIN files f ON l.idfile = f.idfile
                            where LEFT(l.reference, 2) = @transType AND date(l.date) = @date
                            GROUP BY l.reference
                            ORDER BY l.date ASC;
                            ");

                    break;

                case PR.PRDetail:

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

        public static StringBuilder InsertPR(PR process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case PR.PRHeader:

                    sQuery.Append(@"INSERT INTO PPR00 (vendorCode,vendorName,transNum,reference,crossreference,Total,transType,toWarehouse,fromWarehouse,segmentCode,businessSegment,branchCode,remarks,cancelled,transDate,idUser,printed, extracted) 
                            VALUES (@vendorCode,@vendorName,@transNum,@reference,@crossreference,@Total,@transType,@toWarehouse,@fromWarehouse,@segmentCode,@businessSegment,@branchCode,@remarks,@cancelled,@transDate,@idUser,@printed, @extracted)");

                    break;
                case PR.PRDetail:

                    sQuery.Append(@"INSERT INTO PPR10 (barcode,transNum,itemCode,itemDescription,uomCode,uomDescription,quantity,remaining,price,Total,conversion,accountCode,transDate,idUser) 
                            VALUES (@barcode,@transNum,@itemCode,@itemDescription,@uomCode,@uomDescription,@quantity,@remaining,@price,@Total,@conversion,@accountCode,@transDate,@idUser)");

                    break;
                default:
                    break;
            }

            return sQuery;
        }

        
    }

    public enum ItemMasterData
    {
        Item, ItemUOM, UOM, Category, SubCategory, SubSubCategory
    }

    public enum PR
    {
        PRHeader, PRDetail
    }
}
