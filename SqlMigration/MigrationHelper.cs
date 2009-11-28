using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SqlMigration
{
    public class MigrationHelper : IMigrationHelper
    {
        private readonly IFileIO _fileOperationsWrapper;

        public MigrationHelper(IFileIO fileWrapper)
        {
            _fileOperationsWrapper = fileWrapper;
        }


        public IList<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts)
        {
            //push down a default datetime object (1/1/0001) since all migrations will be before that.
            return GetMigrationsInOrder(directoryOfScripts, includeTestScripts, new DateTime());
        }


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

                /* Lets use a factory that we can let users set a file extenstion to a type
                 * so that its very extensivble. Follow the same thing as TaskTypeFactory and MigrationTaskFactory
                 */
                
                //add migration
                Migration migration = MigrationFactory.GetMigrationByFileExtenstion(fileName);
                if(migration != null)
                    migrations.Add(migration);
                
            }

            return migrations
                .Where(migration => migration.MigrationDate >= filterDate) //filter date
                .OrderBy(migration => migration.MigrationDate) //order by date
                .ToList();
        }

    }
}
