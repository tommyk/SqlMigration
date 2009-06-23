using System.Collections.Generic;

namespace SqlMigration
{
    public interface ISqlRunner
    {
        /// <summary>
        /// Starts running the migrations
        /// </summary>
        /// <param name="migrations"></param>
        /// <param name="runInsideTransaction">Do you want the migrations to run inside a transaction so if
        /// <param name="trackMigrations">true if you want to keep track of the migrations that have been applied to the database</param>
        /// an error occurs it will roll back?</param>
        /// <returns>0 for success, -1 for fail</returns>
        int StartMigrations(IList<Migration> migrations, bool runInsideTransaction, bool trackMigrations);

        /// <summary>
        /// Setter for Connection String
        /// </summary>
        string ConnectionString { get; set; }
    }
}