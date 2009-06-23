using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using SqlMigration;


namespace Tests
{
    /// <summary>
    ///This is a test class for RunSqlFileTaskTest and is intended
    ///to contain all RunSqlFileTaskTest Unit Tests
    ///</summary>
    [TestFixture]
    public class RunSqlFileTaskTest
    {

        /// <summary>
        ///A test for RunTask
        ///</summary>
        [Test]
        public void run_task_wihtout_running_inside_transaction()
        {
            var mock = new Rhino.Mocks.MockRepository();
            var fileIO = mock.DynamicMock<IFileIO>();
            var sqlRunner = mock.DynamicMock<ISqlRunner>();

            //connection string
            var connectionString = "asdfasdf";
            string filePath = @"A:\test.sql";
            string sqlFileContents = "create table asdf()";


            var args = new Arguments(new string[] {"/sql", filePath, "/nt", "/cs", connectionString});

            using (mock.Record())
            {
                //expect call to sql runner and return 0 for success
                Expect.Call(sqlRunner.StartMigrations(null, false, true))
                    .IgnoreArguments()
                    .Constraints(Property.Value("Count", 1), Is.Equal(false), Is.Equal(false))
                    .Return(0);
            }
            using (mock.Playback())
            {
                var runSqlTask = new RunSqlFileTask(args, fileIO, sqlRunner);
                int success = runSqlTask.RunTask();

                Assert.AreEqual(0, success, "we should get 0 reuslting in a success");
            }

        }
    }
}