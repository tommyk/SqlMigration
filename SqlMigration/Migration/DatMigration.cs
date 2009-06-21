//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace SqlMigration
//{
//    public class DatMigration : Migration
//    {
//        private const string BULK_INSERT_FORMAT =
//            @"BULK INSERT {0}
//FROM '{1}'
//WITH (DATAFILETYPE='native', TABLOCK, KEEPIDENTITY, KEEPNULLS);";

//        public DatMigration(string filePath) : base(filePath)
//        {
//        }

//        public override string GetSqlCommand()
//        {
//            string tableName = string.Empty;
//            string bulkSqlStatement = string.Empty;

//            //parse table name out of file
//            //todo: Just use the ToString method from the base class?
//            string pattern = @"[-](?<tableName>([\w\.]+[.dat]))";
//            if (Regex.IsMatch(base.FilePath, pattern))
//            {
//                //pull the value out
//                Match match = Regex.Match(base.FilePath, pattern);
//                tableName= match.Result("${tableName}").Replace(".dat", string.Empty);
//            }

//            if(!string.IsNullOrEmpty(tableName))
//            {
//                //we need to setup the bulk insert and return it...
//                bulkSqlStatement = string.Format(BULK_INSERT_FORMAT, tableName, base.FilePath);
//            }
//            else
//            {
//                throw new ArgumentException(string.Format("Error trying to find the table name for the bulk insert from file {0}", base.FilePath));
//            }

//            return bulkSqlStatement;
//        }

//        public override IList<string> GetSqlCommands()
//        {
//            //just return a single command, reuse that code from above
//            return new[] {this.GetSqlCommand()};
//        }
//    }
//}
