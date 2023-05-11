using SOFOS2_Migration_Tool.Helper;
using SOFOS2_Migration_Tool.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOFOS2_Migration_Tool.Migration.Controller
{
    public class PreMigrationController
    {
        private string preMigrationPath = Path.Combine(Application.StartupPath, "LOGS", "Pre-Migration");
        private string preMigrationCheckScripts = Path.Combine(Application.StartupPath, "Pre-Migration Scripts");
        private List<string> extension = new List<string>
        {
            "sql", "txt"
        };


        public PreMigrationController()
        {

        }

        private DataTable GetData(string query)
        {
            try
            {
                using (MySQLHelper db = new MySQLHelper(Global.DestinationDatabase, new StringBuilder(query)))
                {
                   return  db.GetMySQLDataTableAsync();
                }
            }
            catch
            {

                throw;
            }
        }

        public int CheckForErrorData()
        {
            try
            {
                DataTable dt = null;
                int count = 1;

                if (Directory.Exists(preMigrationPath))
                    Array.ForEach(Directory.GetFiles(preMigrationPath), File.Delete);

                if (!Directory.Exists(preMigrationPath))
                    Directory.CreateDirectory(preMigrationPath);              

                if (!Directory.Exists(preMigrationCheckScripts))
                    Directory.CreateDirectory(preMigrationCheckScripts);

                var scriptFiles = Directory.EnumerateFiles(preMigrationCheckScripts, "*.*", SearchOption.AllDirectories)
                    .Where(s => extension.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()));

                

                foreach (string scriptFile in scriptFiles)
                {
                    string script = File.ReadAllText(scriptFile);

                    dt = new DataTable();

                    Thread.Sleep(1000);

                    dt = GetData(script);

                    DataTableToCSV.Export(dt, Path.Combine(preMigrationPath, $"{Path.GetFileNameWithoutExtension(scriptFile)}.csv"));

                    count++;
                }

                return Directory.GetFiles(preMigrationPath).Count();
            }
            catch
            {

                throw;
            }
        }
    }
}
