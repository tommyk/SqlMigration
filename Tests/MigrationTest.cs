using SqlMigration;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SqlMigration.Test
{
    
    
    /// <summary>
    ///This is a test class for MigrationTest and is intended
    ///to contain all MigrationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MigrationTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        /// <summary>
        ///A test for MigrationDate
        ///</summary>
        [TestMethod()]
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

            public override string GetSqlCommand()
            {
                throw new System.NotImplementedException();
            }
        }

        #endregion

        
        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            var migration = new TestMigration("C:\\test\\this_is_the_file_name.txt");
            string result = migration.ToString();

            Assert.AreEqual("this_is_the_file_name.txt", result, "The result should just be the filename");
        }
    }
}
