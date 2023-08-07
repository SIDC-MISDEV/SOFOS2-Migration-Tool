using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Migration
{
    /// <summary>
    /// Types of separator for CSV output
    /// </summary>
    public enum ColumnSeparator
    {
        /// <summary>
        /// (,)
        /// </summary>
        Comma,
        /// <summary>
        /// (|)
        /// </summary>
        BitwiseOR
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ExportResult
    {
        /// <summary>
        /// 
        /// </summary>
        Success,
        /// <summary>
        /// 
        /// </summary>
        Failed,
        /// <summary>
        /// 
        /// </summary>
        ZeroRows
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ExportTextType
    {
        /// <summary>
        /// With quote in every string ""
        /// </summary>
        WithQuote,
        /// <summary>
        /// 
        /// </summary>
        NoQuote
    }
}
