using System;
using System.Collections.Generic;

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
            string fileContents = this.GetFileContents();
            //test, strip out and 'GO' commands
            fileContents = fileContents.Replace("GO\r\n", string.Empty);
            fileContents = fileContents.Replace("\r\nGO", string.Empty);
            fileContents = fileContents.Replace("go\r\n", string.Empty);
            fileContents = fileContents.Replace("\r\ngo", string.Empty);
            return fileContents;
        }

        public override IList<string> GetSqlCommands()
        {
            var commands = new List<string>(32);
            //grab file contents
            string fileContents = this.GetFileContents();


            //just a simple string to keep the command until the next GO statement
            string oneCommand = string.Empty;
            //loop over each line
            string[] strings = fileContents.Split(new[]{"\r\n"}, StringSplitOptions.None);
            for (int i = 0; i < strings.Length; i++)
            {
                //get the current line
                string line = strings[i];

                //does it contain any go statements in the line?
                if (line.Equals("go", StringComparison.OrdinalIgnoreCase)/*it does*/)
                {
                    //add it too the commands if its not blank
                    if(oneCommand != string.Empty)
                        commands.Add(oneCommand);
                    //reset the running command
                    oneCommand = string.Empty;
                }
                else
                {
                    //just add it and keep going line by line
                    if (line != string.Empty)
                    {
                        //make sure its not blank before adding a line break
                        if (oneCommand != string.Empty)
                            oneCommand += "\r\n";

                        //add command
                        oneCommand += line;
                    }
                }

            }


            return commands;
        }

/*
        private string RemoveGoStatement(string line)
        {
            //line = line.Replace("GO\r\n", string.Empty);
            //line = line.Replace("\r\nGO", string.Empty);
            //line = line.Replace("go\r\n", string.Empty);
            //line = line.Replace("\r\ngo", string.Empty);

            return line.Replace("GO\r\n", string.Empty)
                .Replace("\r\nGO", string.Empty)
                .Replace("go\r\n", string.Empty)
                .Replace("\r\ngo", string.Empty)
                .Replace("GO", string.Empty)
                .Replace("go", string.Empty);
        }
*/

        private string GetFileContents()
        {
            return _fileIO.ReadFileContents(base.FilePath);
        }
    }
}
