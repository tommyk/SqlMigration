using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SqlMigration;
using System.Collections.Generic;
using SqlMigration.Contracts;
using Rhino.Mocks;

namespace Tests
{
    /// <summary>
    ///This is a test class for MigrationHelperTest and is intended
    ///to contain all MigrationHelperTest Unit Tests
    ///</summary>
    [TestFixture]
    public class MigrationHelperTest : BaseTestClass
    {
        private MockRepository _mock;
        private IFileIO _fileWrapper;

        [SetUp]
        public void Setup()
        {
            Factory.Overrides.Clear();

            _mock = new MockRepository();
            _fileWrapper = _mock.DynamicMock<IFileIO>();
            Factory.Overrides.Add(typeof(IFileIO).FullName, _fileWrapper);
        }

        [TearDown]
        public void TearDown()
        {
            Factory.Overrides.Clear();
        }
        //[TestFixtureSetUp]
        //public void TestSetup()
        //{
        //    IoC.Current.SetupWindsorContainer();
        //}

        /// <summary>
        ///A test for GetMigrationsInOrder
        ///</summary>
        [Test]
        public void mock_files_and_make_sure_they_come_back_in_order()
        {
            //fake dates to test against
            var firstDate = DateTime.Parse("1/1/2008 1:11:00");
            var secondDate = DateTime.Parse("1/1/2008 1:12:00");
            var thirdDate = DateTime.Parse("1/2/2008 1:11:00");
            var fourthDate = DateTime.Parse("1/3/2008 1:11:00");
            var fifthDate = DateTime.Parse("1/20/2008 1:11:00");

            //random order
            var dates = new List<DateTime> { firstDate, fourthDate, fifthDate, secondDate, thirdDate };

            //create fake list to pass back
            var migrations = new List<Migration>();
            var fileNames = new List<string>();
            foreach (var date in dates)
            {
                //add migration
                migrations.Add(CreateMigrationObject(date));

                //add files
                fileNames.Add("a:\\test\\" + MakeMigrationFileNameFromDate(date, "does_not_matter"));
            }

            using (_mock.Record())
            {
                //pass back fake migration files
                Expect.Call(_fileWrapper.ReadDirectoryFilenames("a:\\test"))
                    .Return(fileNames);
            }
            using (_mock.Playback())
            {
                var fileIO = new MigrationHelper();
                IList<Migration> migrationsInOrder = fileIO.GetMigrationsInOrder("a:\\test", false);

                Assert.IsNotNull(migrationsInOrder, "Migration list came back null");
                Assert.AreEqual(dates.Count, migrationsInOrder.Count, "Should be equal in count");

                //make sure they are in order
                Migration previousMigration = null;
                for (int i = 0; i < migrationsInOrder.Count; i++)
                {
                    Migration migration = migrationsInOrder[i];
                    if (i == 0)
                        previousMigration = migration;

                    //they should be greater then or equal to in datetime
                    Assert.That(migration.MigrationDate, Is.GreaterThanOrEqualTo(previousMigration.MigrationDate));
                }
            }
        }

        [Test]
        public void GetMigrationsInOrderTest()
        {
            //fake dates to test against
            var firstDate = DateTime.Parse("1/1/2008 1:11:00");
            var secondDate = DateTime.Parse("1/1/2008 1:12:00");
            var thirdDate = DateTime.Parse("1/2/2008 1:11:00");
            var fourthDate = DateTime.Parse("1/3/2008 1:11:00");
            var fifthDate = DateTime.Parse("1/20/2008 1:11:00");

            //random order
            var dates = new List<DateTime> { firstDate, fourthDate, fifthDate, secondDate, thirdDate };

            //create fake list to pass back
            var migrations = new List<Migration>();
            var fileNames = new List<string>();
            foreach (var date in dates)
            {
                //add migration
                migrations.Add(CreateMigrationObject(date));

                //add files
                fileNames.Add("a:\\test\\" + MakeMigrationFileNameFromDate(date, "does_not_matter"));
            }

            using (_mock.Record())
            {
                //pass back fake migration files
                Expect.Call(_fileWrapper.ReadDirectoryFilenames("a:\\test"))
                    .Return(fileNames);
            }
            using (_mock.Playback())
            {
                var migrationHelper = new MigrationHelper();
                IList<Migration> migrationsInOrder = migrationHelper.GetMigrationsInOrder("a:\\test", false, secondDate);

                Assert.IsNotNull(migrationsInOrder, "Migration list came back null");
                Assert.AreEqual(4, migrationsInOrder.Count, "Should be 4 since we filtered one out");
                Assert.AreEqual(secondDate, migrationsInOrder[0].MigrationDate, "date should come back in order");
                Assert.AreEqual(thirdDate, migrationsInOrder[1].MigrationDate, "date should come back in order");
                Assert.AreEqual(fourthDate, migrationsInOrder[2].MigrationDate, "date should come back in order");
                Assert.AreEqual(fifthDate, migrationsInOrder[3].MigrationDate, "date should come back in order");
            }
        }

        #region Helper methods for testing, should be moved to its own class
        internal static MigrationTest CreateMigrationObject(DateTime date)
        {
            string filePath = string.Format("C:\\testDir\\{0}", MakeMigrationFileNameFromDate(date, "test"));

            return new MigrationTest(filePath);
        }

        /// <summary>
        /// Used to create a legit fileName for testing based on a date
        /// </summary>
        /// <param name="date"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        internal static string MakeMigrationFileNameFromDate(DateTime date, string desc)
        {
            string fileName = "{0}-{1}-{2}_{3}h{4}m-{5}.sql";
            string year = date.Year.ToString();
            string month = date.Month < 10 ? "0" + date.Month : date.Month.ToString();
            string day = date.Day < 10 ? "0" + date.Day : date.Day.ToString();
            string hour = date.Hour < 10 ? "0" + date.Hour : date.Hour.ToString();
            string minute = date.Minute < 10 ? "0" + date.Minute : date.Minute.ToString();

            return string.Format(fileName, year, month, day, hour, minute, desc);
        }

        internal class MigrationTest : Migration
        {
            public MigrationTest(string filePath)
                : base(filePath)
            {
            }

            public override IList<string> GetSqlCommands()
            {
                //return two sql commands
                return new[] { "command1", "command2" };
            }
        }
        #endregion
    }
}