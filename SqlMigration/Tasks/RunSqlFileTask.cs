using System;
using System.Data;
using System.Data.SqlClient;
using Castle.Core.Logging;

namespace SqlMigration
{
    public class RunSqlFileTask : MigrationTask
    {
        private readonly IMigrationHelper _migrationHelper;
        private readonly IDbConnection _connection;
        private readonly IFileIO _fileIo;
        private ILogger _logger = NullLogger.Instance;


        public RunSqlFileTask(Arguments arguments, IMigrationHelper migrationHelper, IDbConnection dbConnection, IFileIO fileIo)
            : base(arguments)
        {
            _migrationHelper = migrationHelper;
            _connection = dbConnection;
            _fileIo = fileIo;
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

            //setup command to be reused 
            IDbCommand command = null;
            IDbTransaction transaction = null;

            int success = -1;
            try
            {
                //create a connection to database
                _connection.ConnectionString = connectionString;
                _connection.Open();

                //START TRANSACTION
                if (runInsideTransaction)
                    transaction = _connection.BeginTransaction();

                //create sql command object
                command = _connection.CreateCommand();
                command.Connection = _connection;

                //hook into transaction
                if (runInsideTransaction)
                    command.Transaction = transaction;

                //run each sql command by itself
                Logger.Debug("Starting to run sql");
                command.CommandText = sqlCommand;
                command.ExecuteNonQuery();
                Logger.Debug("Ending running sql");

                //commit transaction if we are running under one
                if (runInsideTransaction)
                {
                    Logger.Debug("Before Transaction Commit");
                    transaction.Commit();
                    Logger.Debug("After Transaction Commit");
                }

                //mark success
                success = 0;
            }
            catch (Exception e)
            {
                try
                {
                    if (transaction != null)
                    {
                        Logger.Error("Error, trying to rollback");
                        transaction.Rollback();
                        Logger.Error("rollback complete");
                    }
                }
                catch (SqlException ex)
                {
                    Logger.Error("Exception" + ex.GetType() + " encountered while rolling back transaction.");
                    Logger.Error(string.Format("Exception Message: {0}", ex.Message));
                }
                Logger.Error("Exception " + e.GetType() + " encountered while running sql files.");
                WriteOutAllExcpetionInformation(e, Logger);
            }
            finally
            {
                if (_connection != null)
                {
                    Logger.Debug("Closing connection...");
                    _connection.Close();
                    Logger.Debug("Done closing connection");
                }
            }

            return success;

        }

        private static void WriteOutAllExcpetionInformation(Exception e, ILogger logger)
        {
            if (e != null)
            {
                logger.Error(e.Message);
                if (e.InnerException != null)
                    WriteOutAllExcpetionInformation(e.InnerException, logger);
            }
        }
    }
}