using System;
using System.Collections.Generic;

namespace SqlMigration
{
    public interface IMigrationHelper
    {
        IList<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts);
        IList<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts, DateTime filterDate);
        bool CreateFolder(string folderLocation);
        bool CopyFile(string fileLocation, string copyLocation);
        string ReadFileContents(string filePath);
        void WriteFile(string locationOfFile, string fileContents);
    }
}