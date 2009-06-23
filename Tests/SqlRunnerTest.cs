using System;
using System.Data;
using NUnit.Framework;
using Rhino.Mocks;
using SqlMigration;

using System.Collections.Generic;

namespace Tests
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
        private IDataReader _dataReader;


        //Use TestInitialize to run code before running each test
        [SetUp]
        public void MyTestInitialize()
        {
            //setup mocks
            _mock = new MockRepository();
            _iConnection = _mock.DynamicMock<IDbConnection>();
            _iCommand = _mock.StrictMock<IDbCommand>();
            _iTransaction = _mock.DynamicMock<IDbTransaction>();
            _dataReader = _mock.DynamicMock<IDataReader>();
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
            migrations.Add(FileIOTest.CreateMigrationObject(DateTime.Parse("1/2/2000")));

            using(_mock.Record())
            {
                //hand our mocked command in with the CreateCommand method
                Expect.Call(_iConnection.CreateCommand())
                    .Return(_iCommand);

                //hand in our mocked transaction
                Expect.Call(_iConnection.BeginTransaction())
                    .Return(_iTransaction);

                //make sure it hits the db with the command
                Expect.Call(_iCommand.Connection).SetPropertyAndIgnoreArgument();
                Expect.Call(_iCommand.Transaction).SetPropertyAndIgnoreArgument();
                Expect.Call(_iCommand.CommandText).SetPropertyAndIgnoreArgument().Repeat.Any();
                Expect.Call(_iCommand.ExecuteNonQuery())
                    .IgnoreArguments()
                    .Repeat.Times(3) //two for the one migration and once to create the table
                    .Return(0);

                //insert a record to the SqlMigration table
                Expect.Call(_iCommand.ExecuteNonQuery())
                    .IgnoreArguments()
                    .Repeat.Times(1) //two for the one migration and once to create the table
                    .Return(0);

                //mock table not existing so we try to make it
                Expect.Call(_iCommand.ExecuteScalar())
                    .Return(0);


                //now get the check table
                Expect.Call(_iCommand.ExecuteReader())
                    .Return(_dataReader);

                //mock one row from the datareader
                Expect.Call(_dataReader.Read())
                    .Return(true)
                    .Repeat.Once();

                Expect.Call(_dataReader.GetString(0))
                    .Return("2000-01-01_00h00m-test.sql");

            }
            using(_mock.Playback())
            {
                var sqlRunner = new SqlRunner(_iConnection);
                sqlRunner.ConnectionString = string.Empty;
                sqlRunner.StartMigrations(migrations, true, true);
            }
        }

        //[Test]
        //public void mock_two_migrations_NOT_running_under_a_transaction()
        //{
        //    //setup fake migrations
        //    var migrations = new List<Migration>();
        //    migrations.Add(FileIOTest.CreateMigrationObject(DateTime.Parse("1/1/2000")));
        //    migrations.Add(FileIOTest.CreateMigrationObject(DateTime.Parse("1/1/2000")));

        //    using (_mock.Record())
        //    {
        //        //hand our mocked command in with the CreateCommand method
        //        Expect.Call(_iConnection.CreateCommand())
        //            .Return(_iCommand);

        //        //make sure it hits the db with the command
        //        Expect.Call(_iCommand.ExecuteNonQuery())
        //            .Repeat.Times(2)
        //            .Return(0);
        //    }
        //    using (_mock.Playback())
        //    {
        //        var sqlRunner = new SqlRunner(_iConnection, _iTransaction);
        //        sqlRunner.ConnectionString = string.Empty;
        //        sqlRunner.StartMigrations(migrations, false);
        //    }
        //}

    }
}