using System;
using System.Text.RegularExpressions;

namespace SqlMigration
{
    public class DateParser
    {
        private const string DATE_TIME_FILE_FORMAT = @"(?<year>\d\d\d\d)-(?<month>\d\d)-(?<day>\d\d)_(?<hour>\d\d)h(?<minute>\d\d)";

        /// <summary>
        /// Used to parse a file name or passed in date in the format we choose for migrations.
        /// 2008-04-06_16h04m would be 4/6/2008 at 4:04PM.
        /// </summary>
        /// <param name="dateToParse"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static DateTime TryPrase(string dateToParse, out bool success)
        {
            //default datetime value
            var migrationDate = new DateTime();

            //flag success as false unless we actually do get a successful match
            success = false;

            //parse filename / string that came in to get date
            var match = Regex.Match(dateToParse, DATE_TIME_FILE_FORMAT);
            if (match.Success)
            {
                //we got a match, extract the date
                int year = Convert.ToInt32(match.Result("${year}"));
                int month = Convert.ToInt32(match.Result("${month}"));
                int day = Convert.ToInt32(match.Result("${day}"));
                int hour = Convert.ToInt32(match.Result("${hour}"));
                int minute = Convert.ToInt32(match.Result("${minute}"));

                //create date time and assign it to the module variable keeping track of it
                migrationDate = new DateTime(year, month, day, hour, minute, 0);

                //mark success
                success = true;
            }

            return migrationDate;
        }
    }
}
