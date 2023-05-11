using SOFOS2_Migration_Tool.Migration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOFOS2_Migration_Tool.Helper
{
    class DataTableToCSV
    {
        public static ExportResult Export(DataTable dataTable, string outputPath, ExportTextType exportTextType = ExportTextType.WithQuote, ColumnSeparator columnSeparator = ColumnSeparator.Comma, bool withHeader = true)
        {
            try
            {
                var sb = new StringBuilder();
                string separator = columnSeparator.Equals(ColumnSeparator.Comma) ? "," : "|";

                if (dataTable.Rows.Count > 0)
                {
                    if (exportTextType == ExportTextType.WithQuote)
                    {
                        if (withHeader)
                            sb.AppendLine(string.Join(separator, dataTable.Columns.Cast<DataColumn>().Select(col => $"\"{col.ColumnName}\"").ToArray()));

                        foreach (var item in dataTable.Rows.Cast<DataRow>().Select(row => row.ItemArray.Select(val => $"\"{val}\"").ToArray()))
                        {
                            sb.AppendLine(string.Join(separator, item));
                        }
                    }
                    else
                    {
                        if (withHeader)
                            sb.AppendLine(string.Join(separator, dataTable.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray()));

                        foreach (var item in dataTable.Rows.Cast<DataRow>().Select(row => row.ItemArray.Select(val => val).ToArray()))
                        {
                            sb.AppendLine(string.Join(separator, item));
                        }
                    }

                    File.WriteAllText(outputPath, sb.ToString());

                    return ExportResult.Success;
                }
                else
                {
                    return ExportResult.ZeroRows;
                }
            }
            catch
            {
                return ExportResult.Failed;
            }
        }
    }


}
