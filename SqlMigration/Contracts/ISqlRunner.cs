namespace SqlMigration.Contracts
{
    public interface ISqlRunner
    {
        /// <summary>
        /// Setter for Connection String
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Used to just run SQL thats already generated.
        /// </summary>
        /// <param name="sqlToRun">Sql to run against database</param>
        /// <param name="runInsideTransaction">run inside transaction?</param>
        /// <returns></returns>
        int RunSql(string sqlToRun, bool runInsideTransaction);
    }
}