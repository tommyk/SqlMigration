namespace SqlMigration
{
    public class TSqlMigration : Migration
    {
        private readonly IFileIO _fileIO;

        public TSqlMigration(string filePath) 
            : this(filePath, new FileIO(new FileIOWrapper())) //todo: use DI to get rid of this!
        {
        }

        public TSqlMigration(string filePath, IFileIO fileIO)
            : base(filePath)
        {
            _fileIO = fileIO;
        }

        public override string GetSqlCommand()
        {
            string fileContents = _fileIO.ReadFileContents(base.FilePath);
            //test, strip out and 'GO' commands
            fileContents = fileContents.Replace("GO\r\n", string.Empty);
            fileContents = fileContents.Replace("\r\nGO", string.Empty);
            fileContents = fileContents.Replace("go\r\n", string.Empty);
            fileContents = fileContents.Replace("\r\ngo", string.Empty);
            return fileContents;
        }
    }
}
