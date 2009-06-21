using System;
using System.Data;
using NUnit.Framework;
using Rhino.Mocks;
using SqlMigration;

using System.Collections.Generic;

namespace SqlMigration.Test
{
    /// <summary>
    ///This is a test class for SqlRunnerTest and is intended
    ///to contain all SqlRunnerTest Unit Tests
    ///</summary>
    [TestFixture]
    public class SqlRunnerTest
    {
        private MockRepository _mock;
        private IDbConnection _iConnection;
        private IDbCommand _iCommand;
        private IDbTransaction _iTransaction;


        //Use TestInitialize to run code before running each test
        [SetUp]
        public void MyTestInitialize()
        {
            //setup mocks
            _mock = new MockRepository();
            _iConnection = _mock.DynamicMock<IDbConnection>();
            _iCommand = _mock.DynamicMock<IDbCommand>();
            _iTransaction = _mock.DynamicMock<IDbTransaction>();
        }



        /// <summary>
        ///A test for StartMigrations
        ///</summary>
        [Test]
        public void mock_running_two_fake_sql_file_with_no_errors()
        {
            //setup fake migrations
            var migrations = new List<Migration>();
            migrations.Add(FileIOTest.CreateMigrationObject(DateTime.Parse("1/1/2000")));
            migrations.Add(FileIOTest.CreateMigrationObject(DateTime.Parse("1/1/2000")));

            using(_mock.Record())
            {
                //hand our mocked command in with the CreateCommand method
                Expect.Call(_iConnection.CreateCommand())
                    .Return(_iCommand);

                //hand in our mocked transaction
                Expect.Call(_iConnection.BeginTransaction())
                    .Return(_iTransaction);

                //make sure it hits the db with the command
                Expect.Call(_iCommand.ExecuteNonQuery())
                    .Repeat.Times(4) //because we are acting like there are two items in the command
                    .Return(0);
            }
            using(_mock.Playback())
            {
                var sqlRunner = new SqlRunner(_iConnection, _iTransaction);
                sqlRunner.ConnectionString = string.Empty;
                sqlRunner.StartMigrations(migrations, true);
            }
        }

        [Test]
        public void mock_two_migrations_NOT_running_under_a_transaction()
        {
            //setup fake migrations
            var migrations = new List<Migration>();
            migrations.Add(FileIOTest.CreateMigrationObject(DateTime.Parse("1/1/2000")));
            migrations.Add(FileIOTest.CreateMigrationObject(DateTime.Parse("1/1/2000")));

            using (_mock.Record())
            {
                //hand our mocked command in with the CreateCommand method
                Expect.Call(_iConnection.CreateCommand())
                    .Return(_iCommand);

                //make sure it hits the db with the command
                Expect.Call(_iCommand.ExecuteNonQuery())
                    .Repeat.Times(2)
                    .Return(0);
            }
            using (_mock.Playback())
            {
                var sqlRunner = new SqlRunner(_iConnection, _iTransaction);
                sqlRunner.ConnectionString = string.Empty;
                sqlRunner.StartMigrations(migrations, false);
            }
        }

    }
}
