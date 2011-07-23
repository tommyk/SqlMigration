using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SqlMigration;
using SqlMigration.Tasks;


namespace Tests
{
    /// <summary>
    ///This is a test class for MigrateDatabaseForwardTaskTest and is intended
    ///to contain all MigrateDatabaseForwardTaskTest Unit Tests
    ///</summary>
    [TestFixture]
    public class MigrateDatabaseForwardTaskTest
    {


        /// <summary>
        ///A test for RunTask
        ///</summary>
        [Test]
        public void run_task_wihtout_running_inside_transaction()
        {
            var mock = new Rhino.Mocks.MockRepository();
            var fileIO = mock.DynamicMock<IMigrationHelper>();
            var sqlRunner = mock.DynamicMock<ISqlRunner>();

            //migrations
            var migartions = new List<Migration>();
            //connection string
            var connectionString = "asdfasdf";
            //make arguments file
            var args = new Arguments(new string[] {ArgumentConstants.ConnectionStringArg, connectionString, ArgumentConstants.RunWithoutTransactionArg});



            using (mock.Record())
            {
                //expect to try and grab migrations
                Expect.Call(fileIO.GetMigrationsInOrder(null, true))
                    .IgnoreArguments()
                    .Return(migartions);

                //expect call to sql runner and return 0 for success
                Expect.Call(sqlRunner.StartMigrations(migartions, false, true))
                    .Return(0);
            }
            using (mock.Playback())
            {
                var runSqlTask = new MigrateDatabaseForwardTask(args, fileIO, sqlRunner);
                int success = runSqlTask.RunTask();

                Assert.AreEqual(0, success, "we should get 0 reuslting in a success");
            }

        }
    }
}