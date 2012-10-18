using System;
using System.Data;
using log4net;
using SqlMigration.Contracts;


namespace SqlMigration
{
    public class SqlRunner : ISqlRunner
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SqlRunner));

        private IDbConnection Connection
        {
            get { return Factory.Get<IDbConnection>(); }
        }

        private IConfigurationManager ConfigurationManager
        {
            get { return Factory.Get<IConfigurationManager>(); }
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

                //note that if there is no 'commandTimeout' in the appsettings it will return 0
                //which is a never ending limit to how long a command will go.
                //http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlcommand.commandtimeout.aspx
                command.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["commandTimeout"]);

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
                            Logger.Fatal(string.Format("Tracing: {0}", tracing));
                            Logger.Fatal(string.Format("ErrorNumber: {0}", errorNumber));
                            Logger.Fatal(string.Format("Message: {0}", errorMessage));
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

        private static void WriteOutAllExcpetionInformation(Exception e, ILog logger)
        {
            if (e != null)
            {
                logger.Error(e.Message, e);
                if (e.InnerException != null)
                    WriteOutAllExcpetionInformation(e.InnerException, logger);
            }
        }
    }
}
