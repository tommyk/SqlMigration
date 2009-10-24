using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
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
            var connectionString = "asdfasdf";
            string filePath = @"A:\test.sql";
            string sqlFileContents = "create table asdf()";
            var fileIO = MockRepository.GenerateMock<IFileIO>();
            var dbConnection = MockRepository.GenerateMock<IDbConnection>();
            var transaction = MockRepository.GenerateMock<IDbTransaction>();
            var command = MockRepository.GenerateMock<IDbCommand>();
            
            
            var args = new Arguments(new[] {TaskTypeConstants.RunSqlFileTask, filePath, ArgumentConstants.ConnectionStringArg, connectionString});

            //Arrange

            //get file contents...
            fileIO.Stub(io => io.ReadFileContents(filePath)).Return(sqlFileContents);

            //transaction
            dbConnection.Stub(connection => connection.BeginTransaction()).Return(transaction);

            //create command
            dbConnection.Stub(connection => connection.CreateCommand()).Return(command);


            //Act
            var runSqlFileTask = new RunSqlFileTask(args, fileIO, dbConnection);
            int success = runSqlFileTask.RunTask();

            //Assert
            command.AssertWasCalled(dbCommand => dbCommand.ExecuteNonQuery());
            transaction.AssertWasCalled(dbTransaction => dbTransaction.Commit());
            dbConnection.AssertWasCalled(connection => connection.ConnectionString = connectionString);
            Assert.That(success, Is.EqualTo(0));
        }
    }
}