using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    public class PurchasingQuery
    {
        public static StringBuilder GetQuery(PR process)
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
                              l.extracted,
                              l.idUser
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

                case PR.RVHeader:

                    sQuery.Append(@"SELECT
	                        DATE_FORMAT(l.date, '%Y-%m-%d %H:%i:%s') as date,
	                        l.reference,
	                        l.crossReference,
	                        l.idFile 'VendorCode',
	                        f.name 'VendorName',
                            f.address as 'VendorAddress',
	                        l.credit 'Total',
	                        left(l.reference, 2) 'TransType',
	                        l.cancelled,
	                        l.extracted,
                            l.idUser
                        FROM ledger l
                        INNER JOIN files f ON l.idfile = f.idfile
                        where left(reference, 2) = @transType AND date(l.date) = @date;");

                    break;

                case PR.RGHeader:

                    sQuery.Append(@"SELECT
	                        DATE_FORMAT(l.date, '%Y-%m-%d %H:%i:%s') as date,
	                        l.reference,
	                        l.crossReference,
	                        l.idFile 'VendorCode',
	                        f.name 'VendorName',
                            f.address as 'VendorAddress',
	                        l.debit 'Total',
	                        left(l.reference, 2) 'TransType',
	                        l.cancelled,
	                        l.extracted,
                            l.idUser
                        FROM ledger l
                        INNER JOIN files f ON l.idfile = f.idfile
                        where left(reference, 2) = @transType AND date(l.date) = @date;");

                    break;

                case PR.RVDetail:
                case PR.RGDetail:

                    sQuery.Append(@"SELECT
                            i.reference,
	                        p.barcode,
	                        i.idstock 'ItemCode',
	                        s.name,
	                        i.unit 'UOMCode',
	                        i.unit 'UOMDescription',
	                        i.quantity,
	                        i.quantity + i.variance 'Remaining',
	                        i.cost,
	                        i.quantity * i.cost 'Total',
	                        i.unitQuantity 'Conversion'
                        FROM invoice i
                        INNER JOIN stocks s ON i.idstock = s.idstock
                        INNER JOIN ledger l ON i.reference = l.reference
                        INNER JOIN pcosting p ON s.idstock = p.idstock AND i.unit = p.unit
                        WHERE left(i.reference ,2 ) = @transType AND date(l.date) = @date;");

                    break;

                default:
                    break;
            }

            return sQuery;
        }

        public static StringBuilder InsertTransaction(PR process)
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

                case PR.RVHeader:

                    sQuery.Append(@"INSERT INTO PRV00 (vendorCode,vendorName,transNum,reference,crossreference,prReference,Total,transType,toWarehouse,fromWarehouse,segmentCode,businessSegment,branchCode,remarks,cancelled,transDate,idUser,status, extracted) 
                            VALUES (@vendorCode,@vendorName,@transNum,@reference,@crossreference,@inventoryRequest,@Total,@transType,@toWarehouse,@fromWarehouse,@segmentCode,@businessSegment,@branchCode,@remarks,@cancelled,@transDate,@idUser,@status, @extracted)");

                    break;
                case PR.RVDetail:

                    sQuery.Append(@"INSERT INTO PRV10 (barcode,transNum,itemCode,itemDescription,uomCode,uomDescription,quantity,remaining,price,Total,conversion,accountCode,transDate,idUser)
                            VALUES (@barcode,@transNum,@itemCode,@itemDescription,@uomCode,@uomDescription,@quantity,@remaining,@price,@Total,@conversion,@accountCode,@transDate,@idUser)");

                    break;

                case PR.RGHeader:

                    sQuery.Append(@"INSERT INTO PRG00 (vendorCode,vendorName,transNum,reference,crossreference,Total,transType,toWarehouse,fromWarehouse,segmentCode,businessSegment,branchCode,remarks,cancelled,transDate,idUser,status,extracted) 
                        VALUES (@vendorCode,@vendorName,@transNum,@reference,@crossreference,@Total,@transType,@toWarehouse,@fromWarehouse,@segmentCode,@businessSegment,@branchCode,@remarks,@cancelled,@transDate,@idUser, @status, @extracted)");

                    break;

                case PR.RGDetail:

                    sQuery.Append(@"INSERT INTO PRG10 (barcode,transNum,itemCode,itemDescription,uomCode,uomDescription,quantity,price,Total,conversion,accountCode,transDate,idUser)
                        VALUES (@barcode,@transNum,@itemCode,@itemDescription,@uomCode,@uomDescription,@quantity,@price,@Total,@conversion,@accountCode,@transDate,@idUser)");

                    break;
                
                default:
                    break;
            }

            return sQuery;
        }

        public static StringBuilder UpdateLastPurchasePrice()
        {
            return new StringBuilder(@"UPDATE iiuom set lastPurchasePrice = @cost where itemCode=@itemCode and uomcode=@uomCode;");
        }

        
    }

    public enum PR
    {
        PRHeader, PRDetail, RVHeader, RVDetail, RGHeader, RGDetail
    }
}
