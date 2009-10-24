using System;
using System.Data;
using System.Data.SqlClient;

namespace SqlMigration
{
    public class RunSqlFileTask : MigrationTask
    {
        private readonly IFileIO _fileIo;
        private readonly IDbConnection _connection;

        public RunSqlFileTask(Arguments arguments, IFileIO fileIo, IDbConnection dbConnection)
            : base(arguments)
        {
            _fileIo = fileIo;
            _connection = dbConnection;
        }

        public override int RunTask()
        {
            //get connection string
            string connectionString = base.Arguments.GetArgumentValue(ArgumentConstants.ConnectionStringArg);
            //grab the filepath
            string sqlFilePath = base.Arguments.GetArgumentValue(TaskTypeConstants.RunSqlFileTask);

            //get file contents
            string sqlCommand = _fileIo.ReadFileContents(sqlFilePath);

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
                Console.WriteLine("Starting to run sql");
                command.CommandText = sqlCommand;
                command.ExecuteNonQuery();
                Console.WriteLine("Ending running sql");

                //commit transaction if we are running under one
                if (runInsideTransaction)
                {
                    Console.WriteLine("Before Transaction Commit");
                    transaction.Commit();
                    Console.WriteLine("After Transaction Commit");
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
                        Console.WriteLine("Error, trying to rollback");
                        transaction.Rollback();
                        Console.WriteLine("rollback complete");
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Exception" + ex.GetType() + " encountered while rolling back transaction.");
                    Console.WriteLine(string.Format("Exception Message: {0}", ex.Message));
                }
                Console.WriteLine("Exception " + e.GetType() + " encountered while running sql files.");
                WriteOutAllExcpetionInformation(e);
            }
            finally
            {
                if (_connection != null)
                {
                    Console.WriteLine("Closing connection...");
                    _connection.Close();
                    Console.WriteLine("Done closing connection");
                }
            }

            return success;

        }

        private static void WriteOutAllExcpetionInformation(Exception e)
        {
            if (e != null)
            {
                Console.WriteLine(e.Message);
                if (e.InnerException != null)
                    WriteOutAllExcpetionInformation(e.InnerException);
            }
        }
    }
}