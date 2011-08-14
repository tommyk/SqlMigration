using System;
using System.Collections.Generic;

namespace SqlMigration.Contracts
{
    public interface IMigrationHelper
    {
        IList<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts);
        IList<Migration> GetMigrationsInOrder(string directoryOfScripts, bool includeTestScripts, DateTime filterDate);
    }
}