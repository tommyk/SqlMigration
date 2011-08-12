using Castle.Core.Logging;

namespace SqlMigration
{
    public abstract class MigrationTask
    {
        private ILogger _logger = NullLogger.Instance;
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

        public abstract int RunTask();

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }
    }
}
