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
    }
}
