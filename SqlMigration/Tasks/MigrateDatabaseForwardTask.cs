using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SqlMigration.Tasks
{
    /// <summary>
    /// This class depends on <see cref="DeploymentTask"/> and the <see cref="RunSqlFileTask"/> 
    /// to first create the sql script to run, and then to run the generated sql.
    /// </summary>
    public class MigrateDatabaseForwardTask : MigrationTask
    {
        private readonly IMigrationHelper _migrationHelper;
        private readonly ISqlRunner _sqlRunner;

        #region Constructors

        public MigrateDatabaseForwardTask(Arguments arguments, IMigrationHelper migrationHelper, ISqlRunner sqlRunner)
            : base(arguments)
        {
            _migrationHelper = migrationHelper;
            _sqlRunner = sqlRunner;
        }

        #endregion

        public override int RunTask()
        {
            //create temp file location
            string fileName = Path.GetTempPath() + Guid.NewGuid() + ".sql";
            //todo: hack, please fix this to something better
            Arguments tempArguments = new Arguments(Arguments.CommandArguments.ToArray());
            tempArguments.CommandArguments.Insert(0,TaskTypeConstants.DeploymentTask);
            tempArguments.CommandArguments.Insert(1, fileName);

            //create SQL script
            MigrationTaskFactory.GetMigrationTaskByTaskType(tempArguments).RunTask();

            Logger.Info(string.Format("About to run SQL file located at {0}", fileName));

            _sqlRunner.ConnectionString = base.Arguments.GetArgumentValue(ArgumentConstants.ConnectionStringArg);
            return _sqlRunner.RunSql(File.ReadAllText(fileName), false);
            //int success = -1;

            ////grab migrations
            //string scriptDirectory = Arguments.GetArgumentValue(ArgumentConstants.ScriptDirectoryArg);
            ////make it look for a flag to include test data
            //bool includeTestData = Arguments.DoesArgumentExist(ArgumentConstants.IncludeTestScriptsArg);
            //IList<Migration> migartions = _migrationHelper.GetMigrationsInOrder(scriptDirectory, includeTestData); 


            ////grab connection string
            //string connectionString = base.Arguments.GetArgumentValue(ArgumentConstants.ConnectionStringArg);
            //_sqlRunner.ConnectionString = connectionString;

            ////find if we want to run inside transaction...
            //bool runInsideTransaction = !base.Arguments.DoesArgumentExist(ArgumentConstants.RunWithoutTransactionArg);

            ////run the migrations
            //int successRunningSql = _sqlRunner.StartMigrations(migartions, runInsideTransaction, true); 
            //success = successRunningSql;

            //return success;
        }
    }
}
