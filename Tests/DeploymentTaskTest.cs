using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SqlMigration;
using SqlMigration.Contracts;
using SqlMigration.Tasks;


namespace Tests
{
    /// <summary>
    ///This is a test class for DeploymentTaskTest and is intended
    ///to contain all DeploymentTaskTest Unit Tests
    ///</summary>
    [TestFixture]
    public class DeploymentTaskTest : BaseTestClass
    {
        private IMigrationHelper _iMigrationHelper;
        private IFileIO _iFileIo;

        public DeploymentTaskTest()
        {
            base.Setup += SetupTests;
        }

        void SetupTests(object sender, EventArgs e)
        {
            _iMigrationHelper = Mock.StrictMock<IMigrationHelper>().OverloadFactory();
            _iFileIo = Mock.StrictMock<IFileIO>().OverloadFactory();
        }

        //todo: Get rid of the dup'd code for testing!!!!


        /// <summary>
        ///A test for RunTask
        ///</summary>
        [Test]
        public void create_deploy_script_on_all_migrations()
        {
            //argument stuff
            string scriptDirectory = "c:\test";
            string locationToDeploy = @"a:\test";

            //fake dates to test against
            var firstDate = DateTime.Parse("1-1-2008 1:11:00");
            var secondDate = DateTime.Parse("1-1-2008 1:12:00");

            //create fake list to pass back
            var migrations = new List<Migration>();
            migrations.Add(MigrationHelperTest.CreateMigrationObject(secondDate));
            migrations.Add(MigrationHelperTest.CreateMigrationObject(firstDate));

            string fileContents = SqlResources.deployment_sql;

            using (Mock.Record())
            {
                //get the migrations (all of them in this case)
                Expect.Call(_iMigrationHelper.GetMigrationsInOrder(scriptDirectory, true))
                    .Return(migrations);

                //write out deployment script
                Expect.Call(() => _iFileIo.WriteFile(locationToDeploy, fileContents));

            }
            using (Mock.Playback())
            {
                //act like we are pulling and pushing from a:\test, just easier to test against one location
                var args = new Arguments(new[] { TaskTypeConstants.DeploymentTask, locationToDeploy, ArgumentConstants.ScriptDirectoryArg, scriptDirectory, ArgumentConstants.IncludeTestScriptsArg });
                var deploymentTask = new DeploymentTask(args);//, _iMigrationHelper,_iFileIo);

                //try to run task
                int returnVal = deploymentTask.Run();

                Assert.AreEqual(0, returnVal, "We should get a clean run from this");
            }
        }


    }
}