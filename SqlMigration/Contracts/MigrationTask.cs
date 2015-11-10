using System;
using log4net;

namespace SqlMigration.Contracts
{
    public abstract class MigrationTask
    {
        private readonly Arguments _arguments;

        protected MigrationTask(Arguments arguments)
        {
            _arguments = arguments;
        }

        public Arguments Arguments
        {
            get { return _arguments; }
        }

        public string TaskType { get; set; }

        public int Run()
        {
            try
            {
                return RunTask();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return -1;
            }
        }

        protected abstract int RunTask();

        public ILog Logger
        {
            get { return LogManager.GetLogger(this.GetType()); }
        }
    }
}
