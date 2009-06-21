using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using SqlMigration;


namespace Tests
{
    /// <summary>
    ///This is a test class for TSqlMigrationTest and is intended
    ///to contain all TSqlMigrationTest Unit Tests
    ///</summary>
    [TestFixture]
    public class TSqlMigrationTest : BaseTestClass
    {
        private IFileIO _fileIo;

        public TSqlMigrationTest()
        {
            base.Setup += SetupCrap;
        }

        void SetupCrap(object sender, EventArgs e)
        {
            _fileIo = Mock.DynamicMock<IFileIO>();
        }

        /// <summary>
        ///A test for GetSqlCommand
        ///</summary>
        [Test]
        public void make_sure_we_attempt_to_read_the_contents_of_the_sql_file()
        {
            string filePath = "C:\\test";
            string fileContents = "test contents";


            using (Mock.Record())
            {
                //try to read the file
                Expect.Call(_fileIo.ReadFileContents(filePath))
                    .Return(fileContents);
            }
            using (Mock.Playback())
            {
                var sqlMigration = new TSqlMigration(filePath, _fileIo);
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

            using (Mock.Record())
            {
                //try to read the file
                Expect.Call(_fileIo.ReadFileContents(filePath))
                    .Return(fileContents);
            }
            using (Mock.Playback())
            {
                var sqlMigration = new TSqlMigration(filePath, _fileIo);
                string sqlCommand = sqlMigration.GetSqlCommand();

                Assert.AreEqual(expectedResults, sqlCommand, "This should contain no GO or go statements");
            }
        }

        /// <summary>
        ///A test for GetSqlCommand
        ///</summary>
        [Test]
        public void break_statements_inbetween_go_statements()
        {

            string filePath = "C:\\test";
            string fileContents = @"test contents category
go
Test
GO
TEST
GO

GO
test
multiple
lines
GO";
            using (Mock.Record())
            {
                //try to read the file
                Expect.Call(_fileIo.ReadFileContents(filePath))
                    .Return(fileContents);
            }
            using (Mock.Playback())
            {
                var sqlMigration = new TSqlMigration(filePath, _fileIo);
                IList<string> sqlCommand = sqlMigration.GetSqlCommands();

                //there should be 3 commands inside this fake sql file
                Assert.That(sqlCommand.Count, Is.EqualTo(4));

                Assert.That(sqlCommand[0], Is.EqualTo("test contents category"));
                Assert.That(sqlCommand[1], Is.EqualTo("Test"));
                Assert.That(sqlCommand[2], Is.EqualTo("TEST"));
                //make sure it did not clump each line into one big string
                Assert.That(sqlCommand[3], Is.Not.EqualTo("testmultiplelines"));
            }
        }
    }
}