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
    public class MigrationController
    {
        private string preMigrationPath = Path.Combine(Application.StartupPath, "LOGS", "Pre-Migration");
        private string preMigrationCheckScripts = Path.Combine(Application.StartupPath, "Pre-Migration Scripts");
        private string postMigrationPath = Path.Combine(Application.StartupPath, "LOGS", "Post-Migration");
        private string postMigrationCheckScripts = Path.Combine(Application.StartupPath, "Post-Migration Scripts");

        private string migrationPath = string.Empty;
        private string migrationCheckScripts = string.Empty;
        private bool _isPremigration = false;

        private List<string> extension = new List<string>
        {
            "sql", "txt"
        };


        public MigrationController(bool isPreMigration)
        {
            migrationPath = isPreMigration ? preMigrationPath : postMigrationPath;
            migrationCheckScripts = isPreMigration ? preMigrationCheckScripts : postMigrationCheckScripts;
            _isPremigration = isPreMigration;
        }

        private DataTable GetData(string query, bool isPremigration, bool isPOS1)
        {
            try
            {
                string connString = isPremigration && isPOS1 ? Global.SourceDatabase : Global.DestinationDatabase;

                using (MySQLHelper db = new MySQLHelper(connString, new StringBuilder(query)))
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

                if (Directory.Exists(migrationPath))
                    Array.ForEach(Directory.GetFiles(migrationPath), File.Delete);

                if (!Directory.Exists(migrationPath))
                    Directory.CreateDirectory(migrationPath);              

                if (!Directory.Exists(migrationCheckScripts))
                    Directory.CreateDirectory(migrationCheckScripts);

                var scriptFiles = Directory.EnumerateFiles(migrationCheckScripts, "*.*", SearchOption.AllDirectories)
                    .Where(s => extension.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()));

                

                foreach (string scriptFile in scriptFiles)
                {
                    string script = File.ReadAllText(scriptFile);

                    dt = new DataTable();

                    Thread.Sleep(1000);

                    if(scriptFile.Split('\\').Last().Contains("POS1"))
                        dt = GetData(script, _isPremigration, true);
                    else
                        dt = GetData(script, _isPremigration, isPOS1: false);


                    DataTableToCSV.Export(dt, Path.Combine(migrationPath, $"{Path.GetFileNameWithoutExtension(scriptFile)}.csv"));

                    count++;
                }

                return Directory.GetFiles(migrationPath).Count();
            }
            catch
            {

                throw;
            }
        }
    }
}
