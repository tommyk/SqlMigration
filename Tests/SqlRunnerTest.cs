using System.Data;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using SqlMigration;
using SqlMigration.Contracts;

namespace Tests
{
    /// <summary>
    ///This is a test class for SqlRunnerTest and is intended
    ///to contain all SqlRunnerTest Unit Tests
    ///</summary>
    [TestFixture]
    public class SqlRunnerTest
    {
        private IDbConnection _iConnection;
        private IDbCommand _iCommand;
        private IDbTransaction _iTransaction;
        private IDataReader _dataReader;
        private IConfigurationManager _configurationManager;


        //Use TestInitialize to run code before running each test
        [SetUp]
        public void MyTestInitialize()
        {
            _configurationManager = MockRepository.GenerateMock<IConfigurationManager>().OverloadFactory();
            _iConnection = MockRepository.GenerateMock<IDbConnection>().OverloadFactory();
            _iCommand = MockRepository.GenerateMock<IDbCommand>();
            _iTransaction = MockRepository.GenerateMock<IDbTransaction>();
            _dataReader = MockRepository.GenerateMock<IDataReader>();

            //create command
            _iConnection.Stub(connection => connection.CreateCommand())
                .Return(_iCommand);

            _configurationManager.Stub(x => x.AppSettings["commandTimeout"])
                .Return(22.ToString());
        }

        [TearDown]
        public void TearDown()
        {
            Factory.Overrides.Clear();
        }

        [Test]
        public void use_transaction_get_no_error()
        {
            //setup transaction
            _iConnection.Stub(dbConnection => dbConnection.BeginTransaction())
                .Return(_iTransaction);

            //setup reader and pass back
            _iCommand.Stub(x => x.ExecuteReader(CommandBehavior.Default)).Return(_dataReader);
            //setup the reader
            SetupDataReader(0, false);

            var sqlRunner = new SqlRunner();
            sqlRunner.ConnectionString = string.Empty;
            int sucess = sqlRunner.RunSql("sql_string", true);

            Assert.That(sucess, Is.EqualTo(0));

            _iConnection.AssertWasCalled(connection1 => connection1.BeginTransaction());
            _iTransaction.AssertWasCalled(transaction => transaction.Commit());
            _iCommand.AssertWasCalled(x => x.CommandTimeout = 22);
        }

        [Test]
        public void no_transaction_get_no_error()
        {
            //setup reader and pass back
            _iCommand.Stub(x => x.ExecuteReader(CommandBehavior.Default)).Return(_dataReader);

            //setup reader
            SetupDataReader(0, false);

            var sqlRunner = new SqlRunner();
            sqlRunner.ConnectionString = string.Empty;
            int sucess = sqlRunner.RunSql("sql_string", false);

            Assert.That(sucess, Is.EqualTo(0));

            _iConnection.AssertWasNotCalled(connection1 => connection1.BeginTransaction());
            _iTransaction.AssertWasNotCalled(transaction => transaction.Commit());
        }

        [Test]
        public void error_returned_no_transaction()
        {
            //setup reader and pass back
            _iCommand.Stub(x => x.ExecuteReader(CommandBehavior.SingleResult)).Return(_dataReader);

            //setup reader
            SetupDataReader(110, false);

            var sqlRunner = new SqlRunner();
            sqlRunner.ConnectionString = string.Empty;
            int sucess = sqlRunner.RunSql("sql_string", false);

            Assert.That(sucess, Is.EqualTo(-1));

            _iConnection.AssertWasNotCalled(connection1 => connection1.BeginTransaction());
            _iTransaction.AssertWasNotCalled(transaction => transaction.Commit());
        }

        /// <summary>
        /// Helper method to setup the mocked data reader
        /// </summary>
        private void SetupDataReader(int errorNumberToReturn, bool isErrorNumberNull)
        {
            //reader to get name
            _dataReader.Stub(reader => reader.Read()).Return(true);
            _dataReader.Stub(dataReader => dataReader.GetOrdinal("ErrorNumber")).Return(2000);
            _dataReader.Stub(dataReader => dataReader.IsDBNull(2000)).Return(isErrorNumberNull);
            _dataReader.Stub(dataReader => dataReader.GetInt32(2000)).Return(errorNumberToReturn);
        }


    }
}