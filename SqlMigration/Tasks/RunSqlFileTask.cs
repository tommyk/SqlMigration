using SqlMigration.Contracts;

namespace SqlMigration.Tasks
{
    public class RunSqlFileTask : MigrationTask
    {
        //private readonly IFileIO _fileIo;
        //private readonly ISqlRunner _sqlRunner;


        //public RunSqlFileTask(Arguments arguments, IFileIO fileIo, ISqlRunner sqlRunner)
        //    : base(arguments)
        //{
        //    _fileIo = fileIo;
        //    _sqlRunner = sqlRunner;
        //}

        public RunSqlFileTask(Arguments arguments) : base(arguments) { }

        protected override int RunTask()
        {
            //get connection string
            string connectionString = base.Arguments.GetArgumentValue(ArgumentConstants.ConnectionStringArg);
            //grab the filepath
            string sqlFilePath = base.Arguments.GetArgumentValue(TaskTypeConstants.RunSqlFileTask);

            //get file contents
            string sqlCommand = Factory.Get<IFileIO>().ReadConentsOfFile(sqlFilePath);

            //run inside transaction?
            bool runInsideTransaction = !Arguments.DoesArgumentExist(ArgumentConstants.RunWithoutTransactionArg);

            ISqlRunner sqlRunner = Factory.Get<ISqlRunner>();
            sqlRunner.ConnectionString = connectionString;
            return sqlRunner.RunSql(sqlCommand, runInsideTransaction);
        }
    }
}