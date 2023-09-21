using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    public class RecomputeQuery
    {
        public static StringBuilder GetAllTransactions()
        {
            var sQuery = new StringBuilder();

            sQuery.Append(@"SELECT * 
                        FROM
                        (
                        -- POS 
                        select h.reference, d.itemcode, d.uomCode, d.conversion, IF(h.transtype = 'CD',d.quantity * d.conversion,  d.quantity * d.conversion * -1) as 'quantity', d.cost, IF(h.transtype = 'CD',d.total,  d.total * -1) as 'total', 'Sales' as 'TransactionType', h.transDate, h.AllowNoEffectInventory
                        from sapt0 h 
                        INNER JOIN sapt1 d ON h.transNum = d.transNum
                        WHERE date(h.transDate) = @date
                        -- END POS 

                        UNION ALL

                        -- RC 
                        select h.reference, d.itemcode, d.uomCode, d.conversion, IF(h.transType = 'CD',d.quantity * d.conversion * - 1, d.quantity * d.conversion) as 'quantity', d.cost, IF(h.transtype = 'CD', d.total * -1, d.total) as 'total','ReturnFromCustomer' as 'TransactionType', h.transDate, 0 as `AllowNoEffectInventory`
                        from sapr0 h 
                        INNER JOIN sapr1 d ON h.transNum = d.transNum
                        WHERE date(h.transDate) = @date
                        -- END RC 

                        UNION ALL

                        -- IA 
                        select h.reference, d.itemcode, d.uomCode, d.conversion, (d.variance * d.conversion) as 'quantity', ROUND(d.price/d.conversion, 2), d.total,'Adjustment' as 'TransactionType', h.transDate, 0 as `AllowNoEffectInventory`
                        from iia00 h 
                        INNER JOIN iia10 d ON h.transNum = d.transNum
                        #INNER JOIN iiuom u ON d.itemcode = u.itemcode AND d.uomCode = u.uomCode
                        WHERE date(h.transDate) = @date
                        -- END IA 

                        UNION ALL

                        -- ISSUANCE 
                        select h.reference, d.itemcode, d.uomCode, d.conversion, d.quantity * d.conversion * -1 as 'quantity', ROUND(d.price/d.conversion, 2) as 'cost', d.total * -1 as 'total','Issuance' as 'TransactionType', h.transDate, 0 as `AllowNoEffectInventory`
                        from iii00 h 
                        INNER JOIN iii10 d ON h.transNum = d.transNum
                        WHERE date(h.transDate) = @date
                        -- END ISSUANCE 

                        UNION ALL

                        -- RG 
                        select h.reference, d.itemcode, d.uomCode, d.conversion, IF(h.transtype = 'CD',d.quantity * d.conversion,  d.quantity * d.conversion * -1) as 'quantity', ROUND(d.price/d.conversion, 2) as 'cost', IF(h.transtype = 'CD',d.total,  d.total * -1) as 'total','ReturnGoods' as 'TransactionType', h.transDate, 0 as `AllowNoEffectInventory`
                        from prg00 h 
                        INNER JOIN prg10 d ON h.transNum = d.transNum
                        WHERE date(h.transDate) = @date
                        -- END RG 

                        UNION ALL

                        -- RR 
                        select h.reference, d.itemcode, d.uomCode, d.conversion, d.quantity * d.conversion, ROUND(d.price/d.conversion, 2) as 'cost', d.total,'Receiving' as 'TransactionType', h.transDate, 0 as `AllowNoEffectInventory`
                        from iir00 h 
                        INNER JOIN iir10 d ON h.transNum = d.transNum
                        WHERE date(h.transDate) = @date
                        -- END RR


                        UNION ALL

                        -- RV
                        select h.reference, d.itemcode, d.uomCode, d.conversion, d.quantity * d.conversion, ROUND(d.price/d.conversion, 2) as 'cost', d.total,'ReceiveFromVendor' as 'TransactionType', h.transDate, 0 as `AllowNoEffectInventory`
                        from prv00 h 
                        INNER JOIN prv10 d ON h.transNum = d.transNum
                        WHERE date(h.transDate) = @date
                        -- END RV

                        ) as t
                        ORDER BY t.transDate asc;");

            return sQuery;
        }

        public static StringBuilder GetItemRunningQuantityValue()
        {
            return new StringBuilder(@"SELECT i.itemCode, d.uomCode, d.Conversion, d.cost, i.runningQuantity, i.runningValue 
                                        FROM ii000 i 
                                        INNER JOIN iiuom d ON i.itemcode = d.itemcode 
                                        WHERE i.itemCode = @itemCode AND d.conversion = 1 AND d.isbaseuom=1;");
        }

        public static StringBuilder GetItemUom()
        {
            return new StringBuilder(@"SELECT itemCode, uomCode FROM iiuom WHERE itemcode = @itemCode AND uomCode = @uomCode;");
        }

        public static StringBuilder UpdateRunningQuantityValue(Process process)
        {
            var sQuery = new StringBuilder();

            switch (process)
            {
                case Process.Sales:

                    sQuery.Append(@"UPDATE sapt0 h 
                            INNER JOIN sapt1 d ON h.transNum = d.transNum
                            SET
                                d.runningQty = @runningQuantity,
                                d.cost = @cost * d.conversion,
                                d.transValue = @transvalue,
                                d.runningValue = @runningValue
                            WHERE h.reference = @reference
                            AND d.itemcode = @itemCode AND uomCode = @uomCode; ");

                    break;
                case Process.Adjustment:

                    sQuery.Append(@"UPDATE iia00 h 
                            INNER JOIN iia10 d ON h.transNum = d.transNum
                            SET
                                d.runningQuantity = @runningQuantity,
                                d.transValue = @transvalue,
                                d.runningValue = @runningValue,
                                d.price = @cost
                            WHERE h.reference = @reference
                            AND d.itemcode = @itemCode AND uomCode = @uomCode; ");

                    break;
                case Process.Issuance:

                    sQuery.Append(@"UPDATE iii00 h 
                            INNER JOIN iii10 d ON h.transNum = d.transNum
                            SET
                                d.runningQty = @runningQuantity,
                                d.transValue = @transvalue,
                                d.runningValue = @runningValue,
                                d.averageCost = @cost
                            WHERE h.reference = @reference
                            AND d.itemcode = @itemCode AND uomCode = @uomCode; ");

                    break;
                case Process.ReturnGoods:

                    sQuery.Append(@"UPDATE prg00 h 
                            INNER JOIN prg10 d ON h.transNum = d.transNum
                            SET
                                d.runningQty = @runningQuantity,
                                d.transValue = @transvalue,
                                d.runningValue = @runningValue,
                                d.averageCost = @cost
                            WHERE h.reference = @reference
                            AND d.itemcode = @itemCode AND uomCode = @uomCode; ");

                    break;
                case Process.Receiving:

                    sQuery.Append(@"UPDATE iir00 h 
                            INNER JOIN iir10 d ON h.transNum = d.transNum
                            SET
                                d.runningQty = @runningQuantity,
                                d.transValue = @transvalue,
                                d.runningValue = @runningValue,
                                d.averageCost = @cost
                            WHERE h.reference = @reference
                            AND d.itemcode = @itemCode AND uomCode = @uomCode; ");

                    break;
                case Process.ReceiveFromVendor:

                    sQuery.Append(@"UPDATE prv00 h 
                            INNER JOIN prv10 d ON h.transNum = d.transNum
                            SET
                                d.runningQty = @runningQuantity,
                                d.transValue = @transvalue,
                                d.runningValue = @runningValue,
                                d.averageCost = @cost
                            WHERE h.reference = @reference
                            AND d.itemcode = @itemCode AND uomCode = @uomCode; ");

                    break;

                case Process.ReturnFromCustomer:

                    sQuery.Append(@"UPDATE sapr0 h
                            INNER JOIN sapr1 d ON h.transNum = d.transNum
                            SET
                                d.runningQty = @runningQuantity,
                                d.transValue = @transvalue,
                                d.runningValue = @runningValue,
                                d.averageCost = @cost,
                                d.cost = @cost
                            WHERE h.reference = @reference
                            AND d.itemcode = @itemCode AND uomCode = @uomCode; ");
                    break;

                default:
                    break;
            }

            return sQuery;
        }

        public static StringBuilder UpdateItemRunningQuantityValue()
        {
            return new StringBuilder(@"UPDATE ii000 SET runningQuantity = @runningQuantity, runningValue = @runningValue WHERE itemcode = @itemCode;");
        }

        public static StringBuilder UpdateItemCost()
        {
            return new StringBuilder(@"UPDATE iiuom SET cost = @cost * conversion WHERE itemCode = @itemCode;");
        }

        public static StringBuilder UpdateSellingPrice()
        {
            //return new StringBuilder(@"UPDATE iiuom set sellingPrice = IF(cost > 0 AND markup > 0, ROUND((cost * (markup/100)) + cost, 2) , 0) where itemCode=@itemCode;");
            return new StringBuilder(@"UPDATE iiuom set sellingPrice = ROUND((@cost * (markup/100)) + @cost, 2) where itemCode=@itemCode;");
        }

        public static StringBuilder GetMultipleItemInOneTransaction()
        {
            return new StringBuilder(@"SELECT
	                                        x.module, x.transnum, x.itemcode, x.count
                                        FROM(
                                        SELECT 'Sales' as module, transnum, itemcode, count(itemcode) 'count', a.transdate FROM sapt1 b
										INNER JOIN sapt0 a USING(transnum)
                                        GROUP BY transnum, itemcode
                                        HAVING COUNT(itemcode) > 1
                                        UNION ALL
                                        SELECT 'ReturnFromCustomer' as module, transnum, itemcode, count(itemcode) 'count', a.transdate FROM sapr1 b
										INNER JOIN sapr0 a USING(transnum)
                                        GROUP BY transnum, itemcode
                                        HAVING COUNT(itemcode) > 1
                                        UNION ALL
                                        SELECT 'ReceiveFromVendor' as module, transnum, itemcode, count(itemcode) 'count', a.transdate FROM prv10 b
										INNER JOIN prv00 a USING(transnum)
                                        GROUP BY transnum, itemcode
                                        HAVING COUNT(itemcode) > 1
                                        UNION ALL
                                        SELECT 'ReturnGoods' as module, transnum, itemcode, count(itemcode) 'count', a.transdate FROM prg10 b
										INNER JOIN prg00 a USING(transnum)
                                        GROUP BY transnum, itemcode
                                        HAVING COUNT(itemcode) > 1
                                        UNION ALL
                                        SELECT 'Issuance' as module,transnum, itemcode, count(itemcode) 'count', a.transdate FROM iii10 b
										INNER JOIN iii00 a USING(transnum)
                                        GROUP BY transnum, itemcode
                                        HAVING COUNT(itemcode) > 1
                                        UNION ALL
                                        SELECT 'Receiving' as module,transnum, itemcode, count(itemcode) 'count', a.transdate FROM iir10 b
										INNER JOIN iir00 a USING(transnum)
                                        GROUP BY transnum, itemcode
                                        HAVING COUNT(itemcode) > 1
                                        UNION ALL
                                        SELECT 'InventoryAdjustment' as module,transnum, itemcode, count(itemcode) 'count', a.transdate FROM iia10 b
										INNER JOIN iia00 a USING(transnum)
                                        GROUP BY transnum, itemcode
                                        HAVING COUNT(itemcode) > 1
                                        ) as x WHERE DATE(x.transdate) = @date;
                                        ");
        }

        public static StringBuilder GetItemMultiple(Process p)
        {
            var sb = new StringBuilder();
            string table = string.Empty;

            switch (p)
            {
                case Process.Sales:
                    table = "sapt1";
                    break;
                case Process.ReturnFromCustomer:
                    table = "sapr1";
                    break;
                case Process.ReceiveFromVendor:
                    table = "prv10";
                    break;
                case Process.ReturnGoods:
                    table = "prg10";
                    break;
                case Process.Issuance:
                    table = "iii10";
                    break;
                case Process.Receiving:
                    table = "iir10";
                    break;
                case Process.Adjustment:
                    table = "iia10";
                    break;
                default:
                    break;
            }

            sb.Append($@"SELECT detailnum, itemcode, uomcode, transValue FROM {table} where transnum = @transnum and itemcode = @itemcode ORDER BY detailnum asc");

            return sb;
        }

        public static StringBuilder UpdateMultipleUomTransValue(Process p)
        {
            var sb = new StringBuilder();
            string table = string.Empty,
                table2 = string.Empty;

            switch (p)
            {
                case Process.Sales:
                    table = "sapt1";
                    break;
                case Process.ReturnFromCustomer:
                    table = "sapr1";
                    break;
                case Process.ReceiveFromVendor:
                    table = "prv10";
                    break;
                case Process.ReturnGoods:
                    table = "prg10";
                    break;
                case Process.Issuance:
                    table = "iii10";
                    break;
                case Process.Receiving:
                    table = "iir10";
                    break;
                case Process.Adjustment:
                    table = "iia10";
                    break;
                default:
                    break;
            }

            sb.Append($@"UPDATE {table} SET transvalue = @transvalue where detailnum = @detailnum and itemcode = @itemcode;");

            return sb;
        }
    }

    public enum Process
    {
        Sales, Adjustment, Issuance, ReturnGoods, Receiving, ReceiveFromVendor, ReturnFromCustomer
    }
}
