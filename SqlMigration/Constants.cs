namespace SqlMigration
{
    public static class TaskTypeConstants
    {
        public const string RunSqlFileTask = "/sql";
        public const string DeploymentTask = "/d";
        public const string MigrateDatabaseForwardTask = "/m";
    }

    public static class ArgumentConstants
    {
        public const string ScriptDirectoryArg = "/sd";
        public const string IncludeTestScriptsArg = "/t";
        public const string DateArg = "/date";
        public const string ConnectionStringArg = "/cs";
        public const string RunWithoutTransactionArg= "/nt";

    }
}
