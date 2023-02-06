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
                        select h.reference, d.itemcode, d.uomCode, u.conversion, (d.variance * u.conversion) as 'quantity', d.price, d.total,'Adjustment' as 'TransactionType', h.transDate, 0 as `AllowNoEffectInventory`
                        from iia00 h 
                        INNER JOIN iia10 d ON h.transNum = d.transNum
                        INNER JOIN iiuom u ON d.itemcode = u.itemcode AND d.uomCode = u.uomCode
                        WHERE date(h.transDate) = @date
                        -- END IA 

                        UNION ALL

                        -- ISSUANCE 
                        select h.reference, d.itemcode, d.uomCode, d.conversion, d.quantity * d.conversion * -1 as 'quantity', d.price as 'cost', d.total * -1 as 'total','Issuance' as 'TransactionType', h.transDate, 0 as `AllowNoEffectInventory`
                        from iii00 h 
                        INNER JOIN iii10 d ON h.transNum = d.transNum
                        WHERE date(h.transDate) = @date
                        -- END ISSUANCE 

                        UNION ALL

                        -- RG 
                        select h.reference, d.itemcode, d.uomCode, d.conversion, IF(h.transtype = 'CD',d.quantity * d.conversion,  d.quantity * d.conversion * -1) as 'quantity', d.price as 'cost', IF(h.transtype = 'CD',d.total,  d.total * -1) as 'total','ReturnGoods' as 'TransactionType', h.transDate, 0 as `AllowNoEffectInventory`
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
                        select h.reference, d.itemcode, d.uomCode, d.conversion, d.quantity * d.conversion, d.price as 'cost', d.total,'ReceiveFromVendor' as 'TransactionType', h.transDate, 0 as `AllowNoEffectInventory`
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
                                d.runningValue = @runningValue
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
    }

    public enum Process
    {
        Sales, Adjustment, Issuance, ReturnGoods, Receiving, ReceiveFromVendor, ReturnFromCustomer
    }
}
