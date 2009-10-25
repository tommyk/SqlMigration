using System.Collections.Generic;
using NUnit.Framework;
using SqlMigration;

using System;

namespace Tests
{
    /// <summary>
    ///This is a test class for MigrationTest and is intended
    ///to contain all MigrationTest Unit Tests
    ///</summary>
    [TestFixture]
    public class MigrationTest
    {

        /// <summary>
        ///A test for MigrationDate
        ///</summary>
        [Test]
        public void test_date_parser_with_working_date()
        {
            //test file path
            string filePath = "C:\\2008-04-20_11h22m-test.sql";

            var testMigration = new TestMigration(filePath);

            //make sure it is right
            var date = new DateTime(2008, 4, 20, 11, 22, 0);
            Assert.AreEqual(date, testMigration.MigrationDate, "The dates should match up");
        }

        #region Helper Class

        internal class TestMigration : Migration
        {
            public TestMigration(string filePath) : base(filePath)
            {
            }

            public override IList<string> GetSqlCommands()
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        
        /// <summary>
        ///A test for ToString
        ///</summary>
        [Test]
        public void ToStringTest()
        {
            var migration = new TestMigration("C:\\test\\this_is_the_file_name.txt");
            string result = migration.ToString();

            Assert.AreEqual("this_is_the_file_name.txt", result, "The result should just be the filename");
        }
    }
}