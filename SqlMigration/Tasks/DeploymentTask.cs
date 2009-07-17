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

            //try to see if they want to filter the date
            string filterDateArg = Arguments.GetArgumentValue(ArgumentConstants.DateArg);
            if (!string.IsNullOrEmpty(filterDateArg))
            {
                //filter it.. bool successParsing = false;
                bool successParsing = false;
                DateTime filterDate = DateParser.TryPrase(filterDateArg, out successParsing);
                //did it parse?
                if (!successParsing)
                    throw new ArgumentException("Could not parse the passed in filter date.");

                //get migrations based on a filter date
                migrations = _fileIO.GetMigrationsInOrder(locationOfScripts, includeTestData, filterDate);
            }
            else
            {
                //no filter date
                migrations = _fileIO.GetMigrationsInOrder(locationOfScripts, includeTestData);
            }

            //build up the commands
            var sb = new StringBuilder(1024);
            foreach (Migration migration in migrations)
            {
                //todo: use a logger instead
                Console.WriteLine(string.Format("Writing out migration {0}", migration));

                var sqlCommands = migration.GetSqlCommands();

                Console.WriteLine(string.Format("There are {0} commands", sqlCommands.Count));
                for (int i = 0; i < sqlCommands.Count; i++)
                {
                    string sqlCommand = sqlCommands[i];
                    //add the command to the string builder
                    sb.Append(sqlCommand);

                    //add GO statement
                    sb.Append("\r\nGO\r\n");
                }
            }

            //write file out
            string locationToCreateScript = base.Arguments.GetArgumentValue(TaskTypeConstants.DeploymentTask);
            _fileIO.WriteFile(locationToCreateScript, sb.ToString());

            return 0;
        }
    }
}
