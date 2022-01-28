﻿using System;
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

        public static StringBuilder GetLatestTransactionReference()
        {
            return new StringBuilder(@"SELECT CONCAT(transtype,LPAD(series+1, 10, '0')) as reference FROM SST00 WHERE transtype = @transactionType AND module = @module LIMIT 1;");
        }
        public static StringBuilder GetLatestCreditLimitAccountNumber()
        {
            return new StringBuilder(@"SELECT LPAD(IFNULL(MAX(accountNumber *1),0) +1,10,'0') AS 'AccountNumber' FROM ACL00 LIMIT 1;");
        }
        
    }
}
