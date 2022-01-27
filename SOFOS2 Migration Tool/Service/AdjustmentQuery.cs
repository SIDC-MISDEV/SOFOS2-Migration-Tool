using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
     public class AdjustmentQuery
    {
        public static StringBuilder GetAdjustmentQuery(AdjustmentEnum process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case AdjustmentEnum.AdjustmentHeader:

                    sQuery.Append(@"SELECT
                                    DATE_FORMAT(l.date, '%Y-%m-%d %H:%i:%s') AS 'TransDate',
                                    left(l.reference,2) AS 'TransType',
                                    l.reference AS 'Reference',
                                    l.crossreference AS 'Crossreference',
                                    l.debit AS 'Total',
                                    l.idFile AS 'WarehouseCode',
                                    null AS 'Remarks',
                                    l.cancelled AS 'Cancelled',
                                    'CLOSED' AS 'Status',
                                    DATE_FORMAT(l.timeStamp, '%Y-%m-%d %H:%i:%s') AS 'SystemDate',
                                    l.idUser AS 'IdUser',
                                    '' AS 'TerminalNo'
                                    FROM ledger l
                                    INNER JOIN invoice i ON l.reference = i.reference
                                    LEFT JOIN coa c ON l.idaccount = c.idaccount
                                    where LEFT(l.reference, 2) IN ('IA')
                                    AND date(l.date) = @date
                                    GROUP BY l.reference
                                    ORDER BY l.date ASC;
                            ");
                    break;

                case AdjustmentEnum.AdjustmentDetail:

                    sQuery.Append(@"SELECT
                                    DATE_FORMAT(l.date, '%Y-%m-%d %H:%i:%s') AS 'TransDate',
                                    i.reference AS 'Reference',
                                    p.barcode AS 'Barcode',
                                    i.idstock AS 'ItemCode',
                                    s.name AS 'ItemDescription',
                                    i.unit AS 'UomCode',
                                    0 AS 'RunningQuantity',
                                    0 AS 'ActualCount',
                                    SUM(i.quantity) AS 'Variance',
                                    i.Cost AS 'Price',
                                    SUM(i.amount) AS 'Total',
                                    i.unitQuantity AS 'Conversion',
                                    DATE_FORMAT(i.timeStamp, '%Y-%m-%d %H:%i:%s') AS 'SystemDate',
                                    i.idUser AS 'IdUser',
                                    DATE_FORMAT(i.date, '%Y-%m-%d %H:%i:%s') AS 'SystemDate',
                                    0 AS 'RunningValue',
                                    '' AS 'Category'
                                    FROM invoice i
                                    INNER JOIN ledger l ON i.reference = l.reference
                                    INNER JOIN stocks s ON i.idstock = s.idstock
                                    INNER JOIN pcosting p ON i.idstock = p.idstock AND i.unit = p.unit
                                    WHERE
                                    LEFT(i.reference, 2) IN ('IA')
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

        public static StringBuilder InsertAdjustmentQuery(AdjustmentEnum process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case AdjustmentEnum.AdjustmentHeader:
                    sQuery.Append(@"INSERT INTO IIA00 (transNum, reference, transType, warehouseCode, segmentCode, businessSegment, branchCode, remarks, Total, transDate, systemDate, idUser, cancelled, status, crossreference, TerminalNo) 
                            VALUES (@transNum, @reference, @transType, @warehouseCode, @segmentCode, @businessSegment, @branchCode, @remarks, @Total, @transDate, @systemDate, @idUser, @cancelled, @status, @crossreference, @TerminalNo)");

                    break;
                case AdjustmentEnum.AdjustmentDetail:
                    sQuery.Append(@"INSERT INTO IIA10 (transNum, itemCode, itemDescription, category, runningQuantity, actualCount, variance, systemDate, transDate, idUser, price, uomCode, total, runningValue) 
                            VALUES (@transNum, @itemCode, @itemDescription, @category, @runningQuantity, @actualCount, @variance, @systemDate, @transDate, @idUser, @price, @uomCode, @total, @runningValue)");

                    break;

                default:
                    break;
            }

            return sQuery;
        }
    }


    public enum AdjustmentEnum
    {
        AdjustmentHeader, AdjustmentDetail
    }
}
