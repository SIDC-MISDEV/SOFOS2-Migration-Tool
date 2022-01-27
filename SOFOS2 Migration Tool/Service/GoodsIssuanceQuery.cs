using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    public class GoodsIssuanceQuery
    {
        public static StringBuilder GetGoodsIssuanceQuery(GoodsIssuanceEnum process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case GoodsIssuanceEnum.GoodsIssuanceHeader:

                    sQuery.Append(@"SELECT
                                    DATE_FORMAT(l.date, '%Y-%m-%d %H:%i:%s') AS 'TransDate',
                                    left(l.reference,2) AS 'TransType',
                                    l.reference AS 'Reference',
                                    l.crossreference AS 'Crossreference',
                                    IF(LEFT(l.reference, 2) IN ('AU','SO','SP'), l.debit,l.credit) AS 'Total',
                                    l.idFile AS 'ToWarehouse',
                                    null AS 'Remarks',
                                    l.cancelled AS 'Cancelled',
                                    'CLOSED' AS 'Status',
                                    DATE_FORMAT(l.timeStamp, '%Y-%m-%d %H:%i:%s') AS 'SystemDate',
                                    l.idUser AS 'IdUser',
                                    l.extracted AS 'Extracted',
                                    '' AS 'VendorCode',
                                    '' AS 'FromBranchCode',
                                    l.idaccount AS 'AccountCode',
                                    c.account AS 'AccountName',
                                    false AS 'IsDummy',
                                    false AS 'IsManual',
                                    false AS 'Synced',
                                    '' AS 'AccountNo',
                                    '' AS 'TerminalNo',
                                    '' AS 'ToBranchCode',
                                    '' AS 'EmpName',
                                    '' AS 'Department',
                                    0 as 'Paidtodate'
                                    FROM ledger l
                                    INNER JOIN invoice i ON l.reference = i.reference
                                    LEFT JOIN coa c ON l.idaccount = c.idaccount
                                    where LEFT(l.reference, 2) IN ('ST', 'AU', 'SO','SP','SF')
                                    AND date(l.date) = @date
                                    GROUP BY l.reference
                                    ORDER BY l.date ASC;
                            ");
                    break;

                case GoodsIssuanceEnum.GoodsIssuanceDetail:

                    sQuery.Append(@"SELECT
                                    DATE_FORMAT(l.date, '%Y-%m-%d %H:%i:%s') AS 'TransDate',
                                    i.reference AS 'Reference',
                                    p.barcode AS 'Barcode',
                                    i.idstock AS 'ItemCode',
                                    s.name AS 'ItemDescription',
                                    i.unit AS 'UomCode',
                                    i.unit AS 'UomDescription',
                                    SUM(i.quantity) AS 'Quantity',
                                    i.Cost AS 'Price',
                                    SUM(i.amount) AS 'Total',
                                    i.unitQuantity AS 'Conversion',
                                    DATE_FORMAT(i.timeStamp, '%Y-%m-%d %H:%i:%s') AS 'SystemDate',
                                    i.idUser AS 'IdUser',
                                    null AS 'AccountCode',
                                    l.idfile AS 'WarehouseCode',
                                    DATE_FORMAT(i.date, '%Y-%m-%d %H:%i:%s') AS 'SystemDate',
                                    0 AS 'RunningValue',
                                    0 AS 'RunningQty',
                                    0 AS 'AverageCost'
                                    FROM invoice i
                                    INNER JOIN ledger l ON i.reference = l.reference
                                    INNER JOIN stocks s ON i.idstock = s.idstock
                                    INNER JOIN pcosting p ON i.idstock = p.idstock AND i.unit = p.unit
                                    WHERE
                                    LEFT(i.reference, 2) IN ('ST', 'AU', 'SO','SP','SF')
                                    AND date(l.date) = @date
                                    GROUP BY i.reference, i.idstock, i.unit,i.Cost
                                    ORDER BY l.reference ASC;
                                    ");
                    break;

                default:
                    break;
            }

            return sQuery;
        }

        public static StringBuilder InsertGoodsIssuanceQuery(GoodsIssuanceEnum process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case GoodsIssuanceEnum.GoodsIssuanceHeader:
                    sQuery.Append(@"INSERT INTO III00 (transNum, reference, crossreference, Total, transType, toWarehouse, fromWarehouse, segmentCode, businessSegment, branchCode, remarks, cancelled, status, synced, transDate, systemDate, idUser, extracted, toBranchCode, accountCode, accountName, IsDummy, EmpName, Department, paidtodate, AccountNo, TerminalNo) 
                            VALUES (@transNum, @reference, @crossreference, @Total, @transType, @toWarehouse, @fromWarehouse, @segmentCode, @businessSegment, @branchCode, @remarks, @cancelled, @status, @synced, @transDate, @systemDate, @idUser, @extracted, @toBranchCode, @accountCode, @accountName, @IsDummy, @EmpName, @Department, @paidtodate, @AccountNo, @TerminalNo)");

                    break;
                case GoodsIssuanceEnum.GoodsIssuanceDetail:
                    sQuery.Append(@"INSERT INTO III10 (transNum, barcode, itemCode, itemDescription, uomCode, uomDescription, quantity, price, Total, conversion, accountCode, warehouseCode, systemDate, transDate, idUser, runningQty, averageCost, runningValue) 
                            VALUES (@transNum, @barcode, @itemCode, @itemDescription, @uomCode, @uomDescription, @quantity, @price, @Total, @conversion, @accountCode, @warehouseCode, @systemDate, @transDate, @idUser, @runningQty, @averageCost, @runningValue)");

                    break;

                default:
                    break;
            }

            return sQuery;
        }
    }


    public enum GoodsIssuanceEnum
    {
        GoodsIssuanceHeader, GoodsIssuanceDetail
    }
}
