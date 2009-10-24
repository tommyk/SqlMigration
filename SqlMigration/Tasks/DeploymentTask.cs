using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SqlMigration
{
    public class DeploymentTask : MigrationTask
    {
        private readonly IFileIO _fileIO;


        #region Constructors


        public DeploymentTask(Arguments arguments, IFileIO fileIO)
            : base(arguments)
        {
            _fileIO = fileIO;
        }

        #endregion

        public override int RunTask()
        {
            /// Steps to create deploy directory:
            /// 1. Get migrations neccessary to use (may need to filter on date)
            /// 2. loop over each one and each one of its commands and build up a file


            IList<Migration> migrations;
            string locationOfScripts = base.Arguments.GetArgumentValue(ArgumentConstants.ScriptDirectoryArg);

            //parse for flag that says to include them
            bool includeTestData = base.Arguments.DoesArgumentExist(ArgumentConstants.IncludeTestScriptsArg);

            //get migrations
            migrations = _fileIO.GetMigrationsInOrder(locationOfScripts, includeTestData);


            //build up the commands
            var sb = new StringBuilder(2048);
            //begin try and transaction 
            sb.AppendLine("BEGIN TRY");
            sb.AppendLine("BEGIN TRANSACTION SqlMigrationTransaction");
            sb.AppendLine("DECLARE @debug varchar(max);");
            sb.AppendLine("set @debug = 'Starting Migrations' + CHAR(13);");

            foreach (Migration migration in migrations.OrderBy(migration => migration.MigrationDate))
            {
                //todo: use a logger instead
                Console.WriteLine(string.Format("Writing out migration {0}", migration));

                //see if we need to run this migration
                sb.AppendLine(string.Format("IF (SELECT COUNT(NAME) FROM SqlMigration WHERE Name = '{0}') = 0", migration));
                sb.AppendLine("BEGIN");
                sb.AppendLine("set @debug = @debug + CHAR(13) + 'Starting " + migration + "'");

                var sqlCommands = migration.GetSqlCommands();

                for (int i = 0; i < sqlCommands.Count; i++)
                {
                    string sqlCommand = sqlCommands[i];
                    //add the command to the string builder
                    sb.AppendLine("exec ('" + sqlCommand.Replace("'", "''") + "')");
                }
                //add debug statement
                sb.AppendLine("set @debug = @debug + CHAR(13) + 'Ending " + migration + "'");

                //insert the migratin name
                sb.AppendLine(string.Format("INSERT INTO SqlMigration VALUES ('{0}')", migration));
                sb.AppendLine("END");
            }
            //attempt to commit the transaction at the end of the script
            sb.AppendLine("COMMIT TRANSACTION SqlMigrationTransaction");
            sb.AppendLine("SELECT @debug as Tracing");
            sb.AppendLine("END TRY");
            sb.AppendLine("BEGIN CATCH");
            sb.AppendLine("SELECT ERROR_NUMBER() as ErrorNumber, ERROR_MESSAGE() as ErrorMessage, @debug as Tracing;");
            sb.AppendLine("ROLLBACK TRANSACTION SqlMigrationTransaction");
            sb.Append("END CATCH");

            //write file out
            string locationToCreateScript = base.Arguments.GetArgumentValue(TaskTypeConstants.DeploymentTask);
            _fileIO.WriteFile(locationToCreateScript, sb.ToString());

            return 0;
        }
    }
}
