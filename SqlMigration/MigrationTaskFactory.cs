using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlMigration
{
    public class MigrationTaskFactory
    {
        public static MigrationTask GetMigrationTaskByTaskType(Arguments args)
        {
            MigrationTask task = null;

            //resolve using windsor
            Type migrationType = TaskTypeFactory.GetTaskType(args.TaskType);

            task = (MigrationTask)IoC.Current.Container.Resolve(migrationType,
                                                                new { arguments = args });

            return task;
        }
    }
}