using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlMigration
{
    public static class MigrationTypeFactory
    {
        private static readonly Dictionary<string, Type> _migrationType =
            new Dictionary<string, Type>
                {
                    {".sql", typeof(TSqlMigration)}
                };

        public static Type GetMigrationTypeByFileExtenstion(string fileName)
        {
            return _migrationType.Where(pair => fileName.EndsWith(pair.Key)).Single().Value;
        }

    }
}