using System;
using SqlMigration.Contracts;

namespace SqlMigration.Factories
{
    public class MigrationTaskFactory : IMigrationTaskFactory
    {
        public MigrationTask GetMigrationTaskByTaskType(Arguments args)
        {
            MigrationTask task = null;

            //resolve using windsor
            Type migrationType = TaskTypeFactory.GetTaskType(args.TaskType);

            //task = (MigrationTask)IoC.Current.Resolve(migrationType, new { arguments = args });
            task = CreateMigrationTask(migrationType, args);

            return task;
        }

        protected MigrationTask CreateMigrationTask(Type migrationTaskType, Arguments arguments)
        {
            return (MigrationTask) Activator.CreateInstance(migrationTaskType, arguments);
        }
    }
}