using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using Castle.Core.Logging;


namespace SqlMigration
{
    public class SqlRunner : ISqlRunner
    {
        private const string SqlmigrationTableName = "SqlMigration";
        private readonly IDbConnection _connection;
        private ILogger _logger = NullLogger.Instance;


        public SqlRunner(IDbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("Connection must not be null");
            _connection = connection;
        }

        public string ConnectionString
        {
            get { return _connection.ConnectionString; }
            set { _connection.ConnectionString = value; }
        }
       
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        /// <summary>
        /// Just used to run a peice of SQL.
        /// </summary>
        /// <param name="sqlToRun"></param>
        /// <param name="runInsideTransaction"></param>
        /// <returns></returns>
        public int RunSql(string sqlToRun, bool runInsideTransaction)
        {
            //setup command to be reused 
            IDbCommand command = null;
            IDbTransaction transaction = null;

            int success = -1;
            try
            {
                //create a connection to database
                _connection.Open();

                //create sql command object
                command = _connection.CreateCommand();
                command.CommandTimeout = 60; //todo: remove hard coded value
                //START TRANSACTION
                if (runInsideTransaction)
                {
                    transaction = _connection.BeginTransaction();
                    command.Transaction = transaction;
                }


                command.CommandText = sqlToRun;
                IDataReader executeReader = command.ExecuteReader(CommandBehavior.SingleResult);
                //expecting back a single row with 'ErrorNumber', 'ErrorMessage', and 'Tracing'
                executeReader.Read();
                int errorNumber = 0;
                if (!executeReader.IsDBNull(executeReader.GetOrdinal("ErrorNumber")))
                {
                    executeReader.GetInt32(executeReader.GetOrdinal("ErrorNumber"));
                }
                if(errorNumber != 0)
                {
                    var errorMessage = executeReader.GetString(executeReader.GetOrdinal("ErrorMessage"));
                    var tracing = executeReader.GetString(executeReader.GetOrdinal("Tracing"));
                    Logger.Fatal(string.Format("ErrorNumber: {0}", errorNumber));
                    Logger.Fatal(string.Format("Message: {0}", errorMessage));
                    Logger.Fatal(string.Format("Tracing: {0}", tracing));
                    return -1;
                }


                //commit transaction if we are running under one
                if (runInsideTransaction)
                {
                    Logger.Debug("Before Commit");
                    transaction.Commit();
                    Logger.Debug("After Commit");
                }

                //mark success
                success = 0;
            }
            catch (Exception e)
            {
                WriteOutAllExcpetionInformation(e, Logger);
            }
            finally
            {
                if (_connection != null)
                {
                    Logger.Debug("Closing connection...");
                    _connection.Close();
                    _connection.Dispose();
                    Logger.Debug("Done closing connection");
                }
            }

            return success;
        }
        [Obsolete]
        public int StartMigrations(IList<Migration> migrations, bool runInsideTransaction, bool trackMigrations)
        {
            //setup command to be reused 
            IDbCommand command = null;
            IDbTransaction transaction = null;

            int success = -1;
            try
            {
                //create a connection to database
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

                //check to see which migration we should should run that have not been run yet
                IList<string> migrationsThatHaveAlreadyBeenRun = trackMigrations
                    ? SetupAndCheckWhatMigrationsShouldBeRun(command)
                    : new List<string>(1);

                //loop on migrations
                foreach (Migration migration in migrations.Where(m => !migrationsThatHaveAlreadyBeenRun.Contains(m.ToString())))
                {
                    Logger.Debug(string.Format("Running migration : {0}", migration.ToString()));

                    //run each sql command by itself
                    foreach (string sqlCommand in migration.GetSqlCommands())
                    {
                        Logger.Debug("Starting to run sql");
                        command.CommandText = sqlCommand;
                        command.ExecuteNonQuery();
                        Logger.Debug("Ending running sql");
                    }

                    //insert into table the name of the migration that was run
                    if (trackMigrations)
                    {
                        command.CommandText = string.Format("INSERT INTO {0} VALUES ('{1}');", SqlmigrationTableName,
                                                            migration.ToString());
                        command.ExecuteNonQuery();
                    }
                }

                //commit transaction if we are running under one
                if (runInsideTransaction)
                {
                    Logger.Debug("Before Commit");
                    transaction.Commit();
                    Logger.Debug("After Commit");
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
                WriteOutAllExcpetionInformation(e,Logger);
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
            if(e != null)
            {
                logger.Error(e.Message);
                if (e.InnerException != null) 
                    WriteOutAllExcpetionInformation(e.InnerException,logger);
            }
        }

        private IList<string> SetupAndCheckWhatMigrationsShouldBeRun(IDbCommand command)
        {
            //first check for the table
            command.CommandText = string.Format("SELECT case when object_id('{0}')is not null then 1 else 0 end", SqlmigrationTableName);

            bool isTableSetup = (int)command.ExecuteScalar() > 0;
            //if its not there, create it
            if (!isTableSetup)
            {
                command.CommandText = "CREATE TABLE SqlMigration (	[Name]  varchar(128) not null )";
                command.ExecuteNonQuery();
            }

            //now see what migration names are in the table
            var migrationNames = new List<string>();

            command.CommandText = "SELECT [Name] FROM SqlMigration";
            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
                migrationNames.Add(reader.GetString(0));
            //close the reader
            reader.Close();

            return migrationNames;
        }
    }
}
