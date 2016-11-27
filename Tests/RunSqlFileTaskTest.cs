using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using SqlMigration;
using SqlMigration.Contracts;
using SqlMigration.Tasks;


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
            var connectionString = "asdfasdf";
            string filePath = @"A:\test.sql";
            string sqlFileContents = "create table asdf()";
            
            var sqlRunner = MockRepository.GenerateMock<ISqlRunner>().OverloadFactory();
            var fileIo = MockRepository.GenerateMock<IFileIO>().OverloadFactory();

            var args = new Arguments(new[] { TaskTypeConstants.RunSqlFileTask, filePath, ArgumentConstants.ConnectionStringArg, connectionString });


            //get file contents...
            fileIo.Stub(io => io.ReadConentsOfFile(filePath)).Return(sqlFileContents);

            //Act
            var runSqlFileTask = new RunSqlFileTask(args);//, fileIo, sqlRunner);
            int success = runSqlFileTask.Run();

            //Assert
            sqlRunner.AssertWasCalled(x => x.ConnectionString = connectionString);
            sqlRunner.AssertWasCalled(x=>x.RunSql(sqlFileContents, true));
            Assert.That(success, Is.EqualTo(0));
        }
    }
}