using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;


namespace SqlMigration
{
    public interface ISqlRunner
    {
        /// <summary>
        /// Starts running the migrations
        /// </summary>
        /// <param name="migrations"></param>
        /// <param name="runInsideTransaction">Do you want the migrations to run inside a transaction so if
        /// an error occurs it will roll back?</param>
        /// <returns>0 for success, -1 for fail</returns>
        int StartMigrations(IList<Migration> migrations, bool runInsideTransaction);

        string ConnectionString { get; set; }
    }

    public class SqlRunner : ISqlRunner
    {
        private const string SQLMIGRATION_TABLE_NAME = "SqlMigration";
        private string _connectionString;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        //private IDbCommand _command;

        public SqlRunner()
            : this(null, null)
        {
        }

        //todo: can't the transactoin be created by the connection object?????
        public SqlRunner(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }


        public int StartMigrations(IList<Migration> migrations, bool runInsideTransaction)
        {
            //setup command to be reused 
            IDbCommand command = null;
            int success = -1;
            try
            {
                //todo: FAIL IF CONNECTION IS NULL
                //create a connection to database
                if (_connection == null)
                    _connection = new SqlConnection(_connectionString);
                _connection.Open();

                //START TRANSACTION
                if (runInsideTransaction)
                    _transaction = _connection.BeginTransaction();

                //create sql command object
                command = _connection.CreateCommand();
                command.Connection = _connection;

                //hook into transaction
                if (runInsideTransaction)
                    command.Transaction = _transaction;

                //check to see which migration we should should run that have not been run yet
                IList<string> migrationsThatHaveAlreadyBeenRun = SetupAndCheckWhatMigrationsShouldBeRun(command);

                //loop on migrations
                foreach (Migration migration in migrations.Where(m=> !migrationsThatHaveAlreadyBeenRun.Contains(m.ToString())))
                {
                    //run each sql command by itself
                    foreach (string sqlCommand in migration.GetSqlCommands())
                    {
                        //todo:replace with logger implemetation
                        Console.WriteLine(string.Format("Running command = '{0}'", sqlCommand));
                        command.CommandText = sqlCommand;
                        command.ExecuteNonQuery();
                    }

                    //insert into table the name of the migration that was run
                    command.CommandText = string.Format("INSERT INTO {0} VALUES ('{1}');", SQLMIGRATION_TABLE_NAME,
                                                        migration.ToString());
                    command.ExecuteNonQuery();
                }

                //commit transaction if we are running under one
                if (runInsideTransaction)
                    _transaction.Commit();

                //mark success
                success = 0;
            }
            catch (Exception e)
            {
                try
                {
                    if (_transaction != null)
                        _transaction.Rollback();
                }
                catch (SqlException ex)
                {
                    if (_transaction.Connection != null)
                        Console.WriteLine("Exception" + ex.GetType() + " encountered while rolling back transaction.");
                }
                Console.WriteLine("Exception " + e.GetType() + " encountered while running sql files.");
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (_connection != null)
                {
                    _connection.Close();
                }
            }

            return success;
        }

        private IList<string> SetupAndCheckWhatMigrationsShouldBeRun(IDbCommand command)
        {
            //first check for the table
            command.CommandText = string.Format("SELECT case when object_id('{0}')is not null then 1 else 0 end", SQLMIGRATION_TABLE_NAME);

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
