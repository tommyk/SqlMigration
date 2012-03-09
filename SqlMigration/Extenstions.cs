using System;
using System.Data;

namespace SqlMigration
{
    /// <summary>
    /// Extenstion methods
    /// </summary>
    public static class Extenstions
    {
        /// <summary>
        /// This tries to get the ordinal postion from IDataRecord.
        /// </summary>
        /// <param name="dataRecord">IDataRecord to run GetOrdinal method on</param>
        /// <param name="columnName">column name your looking for</param>
        /// <param name="position">position found from GetOrdinal method</param>
        /// <returns>true if it found position, false if its not</returns>
        public static bool TryGetOrdinal(this IDataRecord dataRecord, string columnName, out int position)
        {
            bool found = false;
            try
            {
                position = dataRecord.GetOrdinal(columnName);
                found = true;
            }
            catch (IndexOutOfRangeException)
            {
                position = -1;
            }
            return found;
        }
    }
}
