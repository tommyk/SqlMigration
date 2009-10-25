using System.Collections.Generic;

namespace SqlMigration
{
    public interface IFileIO
    {
        bool Copy(string filePath, string locationToCopyTo);
        bool CreateFolder(string folderLocation);
        string ReadConentsOfFile(string fileLocation);
        List<string> ReadDirectoryFilenames(string directoryPath);
        void WriteFile(string path, string fileContents);
    }
}