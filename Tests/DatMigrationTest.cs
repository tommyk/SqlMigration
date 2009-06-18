using System.Text.RegularExpressions;
using SqlMigration;

using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace SqlMigration.Test
{


    /// <summary>
    ///This is a test class for DatMigrationTest and is intended
    ///to contain all DatMigrationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DatMigrationTest
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetSqlCommand
        ///</summary>
        [TestMethod()]
        public void make_sure_it_creates_the_sql_command_for_bulk_insert()
        {
            string fileName = @"c:\2008-04-04_10h44m-YourQuest.Account.Person.dat";


            var datMigration = new DatMigration(fileName);
            string sqlString = datMigration.GetSqlCommand();

            //make sure it has what we want
            Assert.IsTrue(Regex.IsMatch(sqlString, "BULK INSERT YourQuest.Account.Person"), "We should see the parsed filename");
            string fromMatchPattern = string.Format("FROM '{0}'", fileName);
            Assert.IsTrue(sqlString.Contains(fromMatchPattern), "This should be in the sql text");

        }

        [TestMethod()]
        public void make_sure_it_creates_the_sql_command_for_bulk_insert_with_numbers_in_the_database_name()
        {
            string fileName = @"c:\2008-04-04_10h44m-YourQuestQ32008.Account.Person.dat";

            var datMigration = new DatMigration(fileName);
            string sqlString = datMigration.GetSqlCommand();

            //make sure it has what we want
            Assert.IsTrue(Regex.IsMatch(sqlString, "BULK INSERT YourQuestQ32008.Account.Person"), "We should see the parsed filename");
            string fromMatchPattern = string.Format("FROM '{0}'", fileName);
            Assert.IsTrue(sqlString.Contains(fromMatchPattern), "This should be in the sql text");

        }
    }
}
