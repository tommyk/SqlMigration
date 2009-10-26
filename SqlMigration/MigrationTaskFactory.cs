using System;

namespace SqlMigration
{
    public class MigrationTaskFactory
    {
        public static MigrationTask GetMigrationTaskByTaskType(Arguments args)
        {
            MigrationTask task = null;

            //resolve using windsor
            Type migrationType = TaskTypeFactory.GetTaskType(args.TaskType);

            task = (MigrationTask)IoC.Current.Resolve(migrationType,
                                                                new { arguments = args });

            return task;
        }
    }
}