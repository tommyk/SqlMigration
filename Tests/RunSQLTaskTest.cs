using System.Collections.Generic;
using Rhino.Mocks;
using URQuest.Tools.DBMigrator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace URQuest.Tools.DBMigrator.Test
{


    /// <summary>
    ///This is a test class for RunSQLTaskTest and is intended
    ///to contain all RunSQLTaskTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RunSQLTaskTest
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
        ///A test for RunTask
        ///</summary>
        [TestMethod()]
        public void run_task_wihtout_running_inside_transaction()
        {
            var mock = new Rhino.Mocks.MockRepository();
            var fileIO = mock.DynamicMock<IFileIO>();
            var sqlRunner = mock.DynamicMock<ISqlRunner>();

            //migrations
            var migartions = new List<Migration>();
            //connection string
            var connectionString = "asdfasdf";
            //make arguments file
            var args = new Arguments(new string[] {"/cs", connectionString, "/nt"});



            using (mock.Record())
            {
                //expect to try and grab migrations
                Expect.Call(fileIO.GetMigrationsInOrder(null, true))
                    .IgnoreArguments()
                    .Return(migartions);

                //expect call to sql runner and return 0 for success
                Expect.Call(sqlRunner.StartMigrations(migartions, false))
                    .Return(0);
            }
            using (mock.Playback())
            {
                var runSqlTask = new RunSQLTask(args, fileIO, sqlRunner);
                int success = runSqlTask.RunTask();

                Assert.AreEqual(0, success, "we should get 0 reuslting in a success");
            }

        }
    }
}
