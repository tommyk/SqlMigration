using Rhino.Mocks;
using SqlMigration;

using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace SqlMigration.Test
{
    
    
    /// <summary>
    ///This is a test class for SqlMigrationTest and is intended
    ///to contain all SqlMigrationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SqlMigrationTest
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
        public void make_sure_we_attempt_to_read_the_contents_of_the_sql_file()
        {
            var mock = new MockRepository();
            var fileIO = mock.DynamicMock<IFileIO>();


            string filePath = "C:\\test";
            string fileContents = "test contents";
            

            using (mock.Record())
            {
                //try to read the file
                Expect.Call(fileIO.ReadFileContents(filePath))
                    .Return(fileContents);
            }
            using(mock.Playback())
            {
                var sqlMigration = new TSqlMigration(filePath, fileIO);
                string sqlCommand = sqlMigration.GetSqlCommand();

                Assert.AreEqual(fileContents, sqlCommand, "This should be the same contents");
            }
        }

        /// <summary>
        ///A test for GetSqlCommand
        ///</summary>
        [TestMethod()]
        public void parse_only_go_statements_not_any_word_containing_go()
        {
            var mock = new MockRepository();
            var fileIO = mock.DynamicMock<IFileIO>();


            string filePath = "C:\\test";
            string fileContents = @"test contents category
go
Test
GO
TEST
GO";
            string expectedResults = @"test contents category
Test
TEST";

            using (mock.Record())
            {
                //try to read the file
                Expect.Call(fileIO.ReadFileContents(filePath))
                    .Return(fileContents);
            }
            using (mock.Playback())
            {
                var sqlMigration = new TSqlMigration(filePath, fileIO);
                string sqlCommand = sqlMigration.GetSqlCommand();

                Assert.AreEqual(expectedResults, sqlCommand, "This should contain no GO or go statements");
            }
        }
    }
}
