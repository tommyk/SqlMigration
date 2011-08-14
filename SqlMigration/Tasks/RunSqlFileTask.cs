using Castle.Core.Logging;
using SqlMigration.Contracts;

namespace SqlMigration.Tasks
{
    public class RunSqlFileTask : MigrationTask
    {
        private readonly IFileIO _fileIo;
        private readonly ISqlRunner _sqlRunner;
        private ILogger _logger = NullLogger.Instance;


        public RunSqlFileTask(Arguments arguments, IFileIO fileIo, ISqlRunner sqlRunner)
            : base(arguments)
        {
            _fileIo = fileIo;
            _sqlRunner = sqlRunner;
        }

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public override int RunTask()
        {
            //get connection string
            string connectionString = base.Arguments.GetArgumentValue(ArgumentConstants.ConnectionStringArg);
            //grab the filepath
            string sqlFilePath = base.Arguments.GetArgumentValue(TaskTypeConstants.RunSqlFileTask);

            //get file contents
            string sqlCommand = _fileIo.ReadConentsOfFile(sqlFilePath);

            //run inside transaction?
            bool runInsideTransaction = !Arguments.DoesArgumentExist(ArgumentConstants.RunWithoutTransactionArg);

            _sqlRunner.ConnectionString = connectionString;
            return _sqlRunner.RunSql(sqlCommand, runInsideTransaction);
        }
    }
}