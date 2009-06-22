using System;
using System.IO;
using NUnit.Framework;
using SqlMigration.Runner;
using Tests;

namespace SqlMigration.Test.integration_test
{
    /// <summary>
    /// Summary description for RunRealMigrationsTest
    /// </summary>
    [TestFixture]
    public class RunRealMigrationsTest
    {
        private DateTime? last_date_used;
        private const string CONNECTION_STRING = "Server=localhost;Trusted_Connection=True;";


        /// This file will need to run in a certain order to work.  Please
        /// be careful changing anything with the tests.

        [Test]
        public void run_real_migrations_that_pass_and_create_a_database_and_one_table_named_names()
        {
            //setup the files and get directory
            string migrationDirectory = WriteSqlFilesOut();
            
            //pass it in and make sure it runs
            var args = new[] {"/m", "/sd", migrationDirectory, "/nt", "/cs", CONNECTION_STRING};

            int returnVal = Program.Main(args);

            Assert.AreEqual(0, returnVal, "We shouldn't get any errors");
            //todo: hit db and test for table existence?
        }

        [Test]
        public void failing_transaction_that_should_leave_NO_rows_in_the_names_table()
        {
            string migrationDirectory = Environment.CurrentDirectory + "\\fail_transaction";
            
            //kill folder if its already there
            if(Directory.Exists(migrationDirectory))
                Directory.Delete(migrationDirectory, true);

            //create the directory
            Directory.CreateDirectory(migrationDirectory);

            //write out a working sql file, then a non working sql file
            File.WriteAllText(migrationDirectory + "\\" + CreateNewFileName("insert_names"), Sql_Files.Insert_names);
            File.WriteAllText(migrationDirectory + "\\" + CreateNewFileName("failing_sql"), "SELECT * FROM *s;");

            var args = new[] { "/m", "/sd", migrationDirectory, "/cs", CONNECTION_STRING };

            int main = Program.Main(args);

            Assert.IsTrue(main < 0, "We should see an error here");
            //todo: check db for no rows...?
        }

        [Test]
        public void passing_transaction_that_should_leave_3_rows_in_the_names_table()
        {
            string migrationDirectory = Environment.CurrentDirectory + "\\passing_transaction";

            //kill folder if its already there
            if (Directory.Exists(migrationDirectory))
                Directory.Delete(migrationDirectory, true);

            //create the directory
            Directory.CreateDirectory(migrationDirectory);

            //write out a working sql file, then a non working sql file
            run_real_migrations_that_pass_and_create_a_database_and_one_table_named_names(); //make sure db exists

            File.WriteAllText(migrationDirectory + "\\" + CreateNewFileName("insert_names"), Sql_Files.Insert_names);

            var args = new[] { "/m", "/sd", migrationDirectory, "/cs", CONNECTION_STRING };

            int main = Program.Main(args);

            Assert.IsTrue(main == 0, "We should see no error here");
            //todo: check db for no rows...?
        }

        private string WriteSqlFilesOut()
        {
            //get folder path
            string migrationDir = Environment.CurrentDirectory + "\\migrations";
            //create that folder
            if(Directory.Exists(migrationDir))
                Directory.Delete(migrationDir, true);

            Directory.CreateDirectory(migrationDir);

            //write out files to directory
            File.WriteAllText(migrationDir+ "\\" + CreateNewFileName("drop_database"), Sql_Files.Drop_database); //drop db
            File.WriteAllText(migrationDir+ "\\" + CreateNewFileName("create_database"), Sql_Files.Create_database); //create db
            File.WriteAllText(migrationDir+ "\\" + CreateNewFileName("create_table_names"), Sql_Files.Create_names_table); //create names table

            return migrationDir;
        }

        private string CreateNewFileName(string desc)
        {
            //doesn't have a value, so we start one
            last_date_used = !last_date_used.HasValue ? DateTime.Today : last_date_used.Value.AddDays(1);

            return FileIOTest.MakeMigrationFileNameFromDate(last_date_used.Value, desc);
        }
    }
}
