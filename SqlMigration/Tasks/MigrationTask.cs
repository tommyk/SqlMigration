using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace URQuest.Tools.DBMigrator
{
    public abstract class MigrationTask
    {
        private readonly Arguments _arguments;

        public Arguments Arguments
        {
            get { return _arguments; }
        }

        public TaskType TaskType { get; set; }

        protected MigrationTask(Arguments arguments)
        {
            _arguments = arguments;
        }

        public abstract int RunTask();
    }
}
