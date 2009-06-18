using System;
using System.Linq;
using System.Text;

namespace SqlMigration
{
    public abstract class Migration
    {
        private DateTime? _migrationDate = null;

        public DateTime MigrationDate
        {
            get
            {
                if (!_migrationDate.HasValue)
                {
                    //parse date
                    bool successfulMatch;
                    DateTime date = DateParser.TryPrase(this.FilePath, out successfulMatch);

                    if (successfulMatch)
                        _migrationDate = date;
                    else
                        throw new ArgumentException(string.Format("Error parsing date for {0} file", this.FilePath));
                }
                return _migrationDate.Value;
            }
        }

        public string FilePath { get; private set; }

        protected Migration(string filePath)
        {
            this.FilePath = filePath;
        }

        /// <summary>
        /// Used to get sql to run against db
        /// </summary>
        /// <returns></returns>
        public abstract string GetSqlCommand();

        /// <summary>
        /// Pushes out just the file name.  Used for logger pretty much...
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder(64);
            foreach (char c in FilePath.Reverse())
            {
                if (c == Char.Parse("\\"))
                    break;
                //since were not there, insert it!
                sb.Insert(0, c);
            }

            return sb.ToString();
        }
    }
}
