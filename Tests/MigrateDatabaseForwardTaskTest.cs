using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SqlMigration;
using SqlMigration.Contracts;
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
            //migrations
            var migartions = new List<Migration>();
            //connection string
            var connectionString = "asdfasdf";
            //make arguments file
            var args = new Arguments(new string[] {ArgumentConstants.ConnectionStringArg, connectionString, ArgumentConstants.RunWithoutTransactionArg});

            var mock = new Rhino.Mocks.MockRepository();
            var fileIO = mock.DynamicMock<IFileIO>().OverloadFactory();
            var sqlRunner = mock.DynamicMock<ISqlRunner>().OverloadFactory();
            var migrationTaskFactory = mock.DynamicMock<IMigrationTaskFactory>().OverloadFactory();
            var migrationTask = mock.Stub<MigrationTask>(new[] {args});
            

            using (mock.Record())
            {
                //create sql script
                Expect.Call(migrationTaskFactory.GetMigrationTaskByTaskType(null))
                    .IgnoreArguments()
                    .Return(migrationTask);
                Expect.Call(migrationTask.Run()).Return(0);

                //get text for sql
                Expect.Call(fileIO.ReadConentsOfFile(null))
                    .IgnoreArguments()
                    .Return("test sql");

                //call sqlrunner
                Expect.Call(sqlRunner.ConnectionString).SetPropertyAndIgnoreArgument();
                Expect.Call(sqlRunner.RunSql("test sql", false))
                    .Return(0);
            }
            using (mock.Playback())
            {
                var runSqlTask = new MigrateDatabaseForwardTask(args);
                int success = runSqlTask.Run();

                Assert.AreEqual(0, success, "we should get 0 reuslting in a success");
            }

        }
    }
}