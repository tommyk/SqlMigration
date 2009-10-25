using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SqlMigration;


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

        public DeploymentTaskTest()
        {
            base.Setup += SetupTests;
        }

        void SetupTests(object sender, EventArgs e)
        {
            this._iMigrationHelper = Mock.StrictMock<IMigrationHelper>();
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
            migrations.Add(FileIOTest.CreateMigrationObject(secondDate));
            migrations.Add(FileIOTest.CreateMigrationObject(firstDate));

            string fileContents =
@"BEGIN TRY
BEGIN TRANSACTION SqlMigrationTransaction
DECLARE @debug varchar(max);
set @debug = 'Starting Migrations' + CHAR(13);
IF (SELECT COUNT(NAME) FROM SqlMigration WHERE Name = '2008-01-01_01h11m-test.sql') = 0
BEGIN
set @debug = @debug + CHAR(13) + 'Starting 2008-01-01_01h11m-test.sql'
exec ('command1')
exec ('command2')
set @debug = @debug + CHAR(13) + 'Ending 2008-01-01_01h11m-test.sql'
INSERT INTO SqlMigration VALUES ('2008-01-01_01h11m-test.sql')
END
IF (SELECT COUNT(NAME) FROM SqlMigration WHERE Name = '2008-01-01_01h12m-test.sql') = 0
BEGIN
set @debug = @debug + CHAR(13) + 'Starting 2008-01-01_01h12m-test.sql'
exec ('command1')
exec ('command2')
set @debug = @debug + CHAR(13) + 'Ending 2008-01-01_01h12m-test.sql'
INSERT INTO SqlMigration VALUES ('2008-01-01_01h12m-test.sql')
END
COMMIT TRANSACTION SqlMigrationTransaction
SELECT @debug as Tracing
END TRY
BEGIN CATCH
SELECT ERROR_NUMBER() as ErrorNumber, ERROR_MESSAGE() as ErrorMessage, @debug as Tracing;
ROLLBACK TRANSACTION SqlMigrationTransaction
END CATCH";


            using (Mock.Record())
            {
                //get the migrations (all of them in this case)
                Expect.Call(_iMigrationHelper.GetMigrationsInOrder(scriptDirectory, true))
                    .Return(migrations);

                //write out deployment script
                Expect.Call(() => _iMigrationHelper.WriteFile(locationToDeploy, fileContents));

            }
            using (Mock.Playback())
            {
                //act like we are pulling and pushing from a:\test, just easier to test against one location
                var args = new Arguments(new[] { TaskTypeConstants.DeploymentTask, locationToDeploy, ArgumentConstants.ScriptDirectoryArg, scriptDirectory, ArgumentConstants.IncludeTestScriptsArg });
                var deploymentTask = new DeploymentTask(args, _iMigrationHelper);

                //try to run task
                int returnVal = deploymentTask.RunTask();

                Assert.AreEqual(0, returnVal, "We should get a clean run from this");
            }
        }


    }
}