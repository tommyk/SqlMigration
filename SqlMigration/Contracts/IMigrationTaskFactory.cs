namespace SqlMigration.Contracts
{
    public interface IMigrationTaskFactory
    {
        MigrationTask GetMigrationTaskByTaskType(Arguments args);
    }
}