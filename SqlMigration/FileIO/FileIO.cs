using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SqlMigration
{
    public interface IFileIO
    {
        IList<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts);
        IList<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts, DateTime filterDate);
        bool CreateFolder(string folderLocation);
        bool CopyFile(string fileLocation, string copyLocation);
        string ReadFileContents(string filePath);
        void WriteFile(string locationOfFile, string fileContents);
    }

    public class FileIO : IFileIO
    {
        private readonly IFileIOWrapper _fileOperationsWrapper;

        //todo: take out the file wrapper, its unnecessary
        public FileIO(IFileIOWrapper fileWrapper)
        {
            _fileOperationsWrapper = fileWrapper;
        }


        public IList<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts)
        {
            //push down a default datetime object (1/1/0001) since all migrations will be before that.
            return GetMigrationsInOrder(directoryOfScripts, includeTestScripts, new DateTime());
        }


        //todo: do we really need the filter date?
        public IList<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts, DateTime filterDate)
        {
            var fileNames = new List<string>();

            //grab files
            fileNames.AddRange(_fileOperationsWrapper.ReadDirectoryFilenames(directoryOfScripts));
            //grab test files?
            if (includeTestScripts)
                fileNames.AddRange(_fileOperationsWrapper.ReadDirectoryFilenames(directoryOfScripts + "\\test"));

            //add to migrations
            var migrations = new List<Migration>();
            foreach (string fileName in fileNames)
            {
                //todo: make this not be hard coded for only sql files.  Do some magic and allow people to 
                //overload and add their own style of migrations in the future
                //add migration
                if (fileName.EndsWith(".sql"))
                    migrations.Add(new TSqlMigration(fileName));
                //else if (fileName.EndsWith(".dat"))
                //    migrations.Add(new DatMigration(fileName));
            }

            return migrations
                .Where(migration => migration.MigrationDate >= filterDate) //filter date
                .OrderBy(migration => migration.MigrationDate) //order by date
                .ToList();
        }

        public bool CreateFolder(string folderLocation)
        {
            return _fileOperationsWrapper.CreateFolder(folderLocation);
        }

        public bool CopyFile(string fileLocation, string copyLocation)
        {
            return _fileOperationsWrapper.Copy(fileLocation, copyLocation);
        }

        public string ReadFileContents(string filePath)
        {
            return File.ReadAllText(filePath);
            //return _fileOperationsWrapper.ReadConentsOfFile(filePath);
        }

        public void WriteFile(string locationOfFile, string contents)
        {
            File.WriteAllText(locationOfFile, contents);
        }
    }
}
