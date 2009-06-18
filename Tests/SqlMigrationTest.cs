using NUnit.Framework;
using Rhino.Mocks;
using SqlMigration;


namespace SqlMigration.Test
{


    /// <summary>
    ///This is a test class for SqlMigrationTest and is intended
    ///to contain all SqlMigrationTest Unit Tests
    ///</summary>
    [TestFixture]
    public class SqlMigrationTest
    {

        /// <summary>
        ///A test for GetSqlCommand
        ///</summary>
        [Test]
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
            using (mock.Playback())
            {
                var sqlMigration = new TSqlMigration(filePath, fileIO);
                string sqlCommand = sqlMigration.GetSqlCommand();

                Assert.AreEqual(fileContents, sqlCommand, "This should be the same contents");
            }
        }

        /// <summary>
        ///A test for GetSqlCommand
        ///</summary>
        [Test]
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
