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
    }
}
