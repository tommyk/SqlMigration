using System;
using System.Collections.Generic;
using System.Text;
using SqlMigration.Contracts;

namespace SqlMigration
{
    public class TSqlMigration : Migration
    {
        //private readonly IMigrationHelper _migrationHelper;
        //private readonly IFileIO _fileIo;

        //public TSqlMigration(string filePath)//, IMigrationHelper migrationHelper, IFileIO fileIo)
        //    : base(filePath)
        //{
        //    _migrationHelper = migrationHelper;
        //    _fileIo = fileIo;
        //}


        public TSqlMigration(string filePath) : base(filePath) { }

        public override IList<string> GetSqlCommands()
        {
            var commands = new List<string>(32);
            //grab file contents
            string fileContents = this.GetFileContents();


            //just a simple string to keep the command until the next GO statement
            //string oneCommand = string.Empty;
            StringBuilder oneCommand = new StringBuilder();

            //loop over each line
            //todo: get rid of the \r\n and use something from .NET for enivronment newline if you can
            string[] strings = fileContents.Split(new[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 0; i < strings.Length; i++)
            {
                if (oneCommand == null)
                    oneCommand = new StringBuilder();

                //get the current line
                string line = strings[i];

                //does it contain any go statements in the line?
                if (line.Equals("go", StringComparison.OrdinalIgnoreCase)/*it does*/)
                {
                    ////add it too the commands if its not blank
                    if (oneCommand.Length != 0)
                        commands.Add(oneCommand.ToString());
                    oneCommand = null;

                }
                else
                {
                    //just add it and keep going line by line
                    if (line != string.Empty)
                    {
                        ////make sure its not blank before adding a line break
                        oneCommand.AppendLine(line);
                    }
                }

            }

            //incase there was no GO statement we need to add the command that was running through
            if (oneCommand != null)
                if (oneCommand.Length != 0)
                    commands.Add(oneCommand.ToString());


            return commands;
        }

        private string GetFileContents()
        {
            return Factory.Get<IFileIO>().ReadConentsOfFile(base.FilePath);
        }
    }
}
