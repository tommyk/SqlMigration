using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using log4net;
using log4net.Core;
using SqlMigration.Contracts;


namespace SqlMigration
{
    public class SqlRunner : ISqlRunner
    {
        private const string SqlmigrationTableName = "SqlMigration";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlRunner));


        public IDbConnection Connection
        {
            get { return Factory.Get<IDbConnection>(); }
        }

        public string ConnectionString
        {
            get { return Connection.ConnectionString; }
            set { Connection.ConnectionString = value; }
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
                Connection.Open();

                //create sql command object
                command = Connection.CreateCommand();
                command.CommandTimeout = 60; //todo: remove hard coded value
                //START TRANSACTION
                if (runInsideTransaction)
                {
                    transaction = Connection.BeginTransaction();
                    command.Transaction = transaction;
                }

                command.CommandText = sqlToRun;
                IDataReader executeReader = command.ExecuteReader(CommandBehavior.Default);
                //expecting back a single row with 'ErrorNumber', 'ErrorMessage', and 'Tracing'
                do
                {
                    if (executeReader.Read())
                    {
                        int errorNumber = 0;
                        int errorPosition;
                        //if we can't find it we continue
                        if (!executeReader.TryGetOrdinal("ErrorNumber", out errorPosition))
                            continue;

                        if (!executeReader.IsDBNull(errorPosition))
                            errorNumber = executeReader.GetInt32(errorPosition);

                        if (errorNumber != 0)
                        {
                            var errorMessage = executeReader.GetString(executeReader.GetOrdinal("ErrorMessage"));
                            var tracing = executeReader.GetString(executeReader.GetOrdinal("Tracing"));
                            Logger.Fatal(string.Format("ErrorNumber: {0}", errorNumber));
                            Logger.Fatal(string.Format("Message: {0}", errorMessage));
                            Logger.Fatal(string.Format("Tracing: {0}", tracing));
                            return -1;
                        }
                    }
                } while (executeReader.NextResult());

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
                if (Connection != null)
                {
                    Logger.Debug("Closing connection...");
                    Connection.Close();
                    Connection.Dispose();
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
                Connection.Open();

                //START TRANSACTION
                if (runInsideTransaction)
                    transaction = Connection.BeginTransaction();

                //create sql command object
                command = Connection.CreateCommand();
                command.Connection = Connection;

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
                WriteOutAllExcpetionInformation(e, Logger);
            }
            finally
            {
                if (Connection != null)
                {
                    Logger.Debug("Closing connection...");
                    Connection.Close();
                    Logger.Debug("Done closing connection");
                }
            }

            return success;
        }

        private static void WriteOutAllExcpetionInformation(Exception e, ILog logger)
        {
            if (e != null)
            {
                logger.Error(e.Message, e);
                if (e.InnerException != null)
                    WriteOutAllExcpetionInformation(e.InnerException, logger);
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
