using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;


namespace SqlMigration
{
    public class SqlRunner : ISqlRunner
    {
        private const string SQLMIGRATION_TABLE_NAME = "SqlMigration";
        private readonly IDbConnection _connection;

        //public SqlRunner()
        //    : this(null)
        //{
        //}

        public SqlRunner(IDbConnection connection)
        {
            if(connection == null) throw new ArgumentNullException("Connection must not be null");
            _connection = connection;
        }

        public string ConnectionString
        {
            get { return _connection.ConnectionString; }
            set { _connection.ConnectionString= value; }
        }


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
                    if (trackMigrations)
                    {
                        command.CommandText = string.Format("INSERT INTO {0} VALUES ('{1}');", SQLMIGRATION_TABLE_NAME,
                                                            migration.ToString());
                        command.ExecuteNonQuery();
                    }
                }

                //commit transaction if we are running under one
                if (runInsideTransaction)
                    transaction.Commit();

                //mark success
                success = 0;
            }
            catch (Exception e)
            {
                try
                {
                    if (transaction != null)
                        transaction.Rollback();
                }
                catch (SqlException ex)
                {
                    if (transaction.Connection != null)
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
