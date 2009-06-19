using System;
using System.Collections.Generic;
using System.Data;
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
        private string _connectionString;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IDbCommand _command;

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
                _command = _connection.CreateCommand();
                _command.Connection = _connection;
                //hook into transaction
                if (runInsideTransaction)
                    _command.Transaction = _transaction;

                //loop on migrations
                foreach (Migration migration in migrations)
                {
                    //todo: log with a logger class, not just console
                    Console.WriteLine(string.Format("Running migration\t'{0}'", migration));
                    //grab sql and run it
                    _command.CommandText = migration.GetSqlCommand();
                    _command.ExecuteNonQuery();
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
    }
}
