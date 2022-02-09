using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Service
{
    public class Query
    {
        public static StringBuilder UpdateReferenceCount()
        {
            return new StringBuilder(@"UPDATE sst00 SET series = @series WHERE transtype = @transtype;");
        }
        public static StringBuilder UpdateBIRSeries()
        {
            return new StringBuilder(@"UPDATE ssbir SET 
	                                    Prefix = CASE WHEN series = 9999999999 THEN Prefix + 1 ELSE prefix END,
	                                    Series = CASE WHEN series = 9999999999 THEN 1 ELSE Series + 1 END
                                    WHERE transtype = @transtype;");
        }

        public static StringBuilder GetBIRSeries()
        {
            return new StringBuilder(@"SELECT prefix, series FROM ssbir WHERE TransType = @transtype LIMIT 1;");
        }

        public static StringBuilder UpdateTagging(string field, string table)
        {
            return new StringBuilder($@"UPDATE {table} SET {field} = @value WHERE reference=@reference;");
        }

        public static StringBuilder GetLatestTransactionReference()
        {
            return new StringBuilder(@"SELECT CONCAT(transtype,LPAD(series+1, 10, '0')) as reference FROM SST00 WHERE transtype = @transactionType AND module = @module LIMIT 1;");
        }
        public static StringBuilder GetLatestCreditLimitAccountNumber()
        {
            return new StringBuilder(@"SELECT LPAD(IFNULL(MAX(accountNumber *1),0) +1,10,'0') AS 'AccountNumber' FROM ACL00 LIMIT 1;");
        }

        public static StringBuilder DropPrimaryKey(string table, string field)
        {
            return new StringBuilder($@"ALTER TABLE `{table}` 
                                        CHANGE COLUMN `{field}` `{field}` INT(10) UNSIGNED NOT NULL ,
                                        DROP PRIMARY KEY;");
        }

        public static StringBuilder AlterPrimaryKey(string table, string field)
        {
            return new StringBuilder($@"ALTER TABLE `{table}` 
                                        CHANGE COLUMN `{field}` `{field}` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT ,
                                        ADD PRIMARY KEY (`{field}`);");
        }

        public static StringBuilder CountRecord(string table, string field)
        {
            return new StringBuilder($@"SELECT COUNT({field}) as count FROM {table};");
        }

        public static StringBuilder ArrangeDetailNum(string table, string field)
        {
            return new StringBuilder($@"SELECT {field} FROM {table} ORDER BY transNum ASC;");
        }

        public static StringBuilder UpdateDetailNum(string table, string field)
        {
            return new StringBuilder($@"UPDATE {table} SET {field} = @value WHERE transNum=@transNum AND detailNum=@detailNum;");
        }
    }
}
