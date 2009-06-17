using System;
using System.Collections.Generic;
using System.Linq;

namespace URQuest.Tools.DBMigrator
{
    public interface IFileIO
    {
        List<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts);
        List<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts, DateTime filterDate);
        bool CreateFolder(string folderLocation);
        bool CopyFile(string fileLocation, string copyLocation);
        string ReadFileContents(string filePath);
    }

    public class FileIO : IFileIO
    {
        private readonly IFileIOWrapper _fileOperationsWrapper;

        public FileIO(IFileIOWrapper fileWrapper)
        {
            _fileOperationsWrapper = fileWrapper;
        }


        public List<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts)
        {
            //push down a default datetime object (1/1/0001) since all migrations will be before that.
            return GetMigrationsInOrder(directoryOfScripts, includeTestScripts, new DateTime());
        }

        public List<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts, DateTime filterDate)
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
                //add migration
                if (fileName.EndsWith(".sql"))
                    migrations.Add(new SqlMigration(fileName));
                else if (fileName.EndsWith(".dat"))
                    migrations.Add(new DatMigration(fileName));
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
            return _fileOperationsWrapper.ReadConentsOfFile(filePath);
        }
    }
}
