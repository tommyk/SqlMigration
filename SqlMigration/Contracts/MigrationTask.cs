namespace SqlMigration
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

        public abstract int RunTask();
    }
}
