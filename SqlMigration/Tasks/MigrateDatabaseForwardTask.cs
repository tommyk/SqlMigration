using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SqlMigration.Contracts;
using SqlMigration.Factories;

namespace SqlMigration.Tasks
{
    /// <summary>
    /// This class depends on <see cref="DeploymentTask"/> and the <see cref="RunSqlFileTask"/> 
    /// to first create the sql script to run, and then to run the generated sql.
    /// </summary>
    public class MigrateDatabaseForwardTask : MigrationTask
    {
        private ISqlRunner SqlRunner
        {
            get { return Factory.Get<ISqlRunner>(); }
        }

        public IMigrationTaskFactory MigrationTaskFactory
        {
            get { return Factory.Get<IMigrationTaskFactory>(); }
        }

        public IFileIO FileIO
        {
            get { return Factory.Get<IFileIO>(); }
        }

        public MigrateDatabaseForwardTask(Arguments arguments)
            : base(arguments)
        {
        }

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

            SqlRunner.ConnectionString = base.Arguments.GetArgumentValue(ArgumentConstants.ConnectionStringArg);
            //return SqlRunner.RunSql(File.ReadAllText(fileName), false);
            return SqlRunner.RunSql(FileIO.ReadConentsOfFile(fileName), false);
        }
    }
}
