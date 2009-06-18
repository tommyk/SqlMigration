using System;
using System.Collections.Generic;
using System.Reflection;
using Rhino.Mocks;
using SqlMigration;

using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace SqlMigration.Test
{


    /// <summary>
    ///This is a test class for DeploymentTaskTest and is intended
    ///to contain all DeploymentTaskTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DeploymentTaskTest
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


        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }

        //todo: Get rid of the dup'd code for testing!!!!


        /// <summary>
        ///A test for RunTask
        ///</summary>
        [TestMethod()]
        public void run_deployment_on_all_migrations()
        {
            //argument stuff
            string locationToDeploy = @"a:\test";

            //fake dates to test against
            var firstDate = DateTime.Parse("1-1-2008 1:11:00Z");
            var secondDate = DateTime.Parse("1-1-2008 1:12:00Z");

            //create fake list to pass back
            var migrations = new List<Migration>();
            migrations.Add(FileIOTest.CreateMigrationObject(secondDate));
            migrations.Add(FileIOTest.CreateMigrationObject(firstDate));


            var mock = new MockRepository();
            var iFileIO = mock.CreateMock<IFileIO>();

            using(mock.Record())
            {
                //get the migrations (all of them in this case)
                Expect.Call(iFileIO.GetMigrationsInOrder(locationToDeploy, true))
                    .Return(migrations);

                //expect call to create working directory
                Expect.Call(iFileIO.CreateFolder(locationToDeploy))
                    .IgnoreArguments()
                    .Return(true);

                //expect call for each file to be copied
                foreach (var migration in migrations)
                {
                    Expect.Call(iFileIO.CopyFile(migration.FilePath, locationToDeploy))
                        .IgnoreArguments()
                        .Return(true);
                }

                //todo: add copy the tool itself
                Expect.Call(iFileIO.CopyFile(null, null))
                    .IgnoreArguments()
                    .Return(true);

            }
            using(mock.Playback())
            {
                //act like we are pulling and pushing from a:\test, just easier to test against one location
                var args = new Arguments(new[] { "/d", "/sd", locationToDeploy, "/cd", locationToDeploy, "/t" });
                var deploymentTask = new DeploymentTask(args, iFileIO);
              
                //try to run task
                int returnVal = deploymentTask.RunTask();

                Assert.AreEqual(0, returnVal, "We should get a clean run from this");

            }


        }

        [TestMethod()]
        public void run_deployment_on_filtered_date()
        {
            //argument stuff
            string locationToDeploy = @"a:\test";

            //fake dates to test against
            var firstDate = DateTime.Parse("1-1-2008 1:11:00Z");
            var secondDate = DateTime.Parse("1-1-2008 1:12:00Z");

            //create fake list to pass back
            var migrations = new List<Migration>();
            migrations.Add(FileIOTest.CreateMigrationObject(secondDate));
            migrations.Add(FileIOTest.CreateMigrationObject(firstDate));


            var mock = new MockRepository();
            var iFileIO = mock.CreateMock<IFileIO>();

            using (mock.Record())
            {
                //get the migrations (all of them in this case)
                Expect.Call(iFileIO.GetMigrationsInOrder(locationToDeploy, true, new DateTime(2008, 1, 1, 1, 1,0)))
                    .Return(migrations);

                //expect call to create working directory
                Expect.Call(iFileIO.CreateFolder(locationToDeploy))
                    .IgnoreArguments()
                    .Return(true);

                //expect call for each file to be copied
                foreach (var migration in migrations)
                {
                    Expect.Call(iFileIO.CopyFile(migration.FilePath, locationToDeploy))
                        .IgnoreArguments()
                        .Return(true);
                }

                //todo: add copy the tool itself
                Expect.Call(iFileIO.CopyFile(null, null))
                    .IgnoreArguments()
                    .Return(true);

            }
            using (mock.Playback())
            {
                //act like we are pulling and pushing from a:\test, just easier to test against one location
                var args = new Arguments(new[] { "/d", "/sd", locationToDeploy, "/cd", locationToDeploy, "/t", "/date", "2008-01-01_01h01m" });
                var deploymentTask = new DeploymentTask(args, iFileIO);

                //try to run task
                int returnVal = deploymentTask.RunTask();

                Assert.AreEqual(0, returnVal, "We should get a clean run from this");

            }


        }
    }
}
