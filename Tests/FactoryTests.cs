using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using SqlMigration;
using SqlMigration.Contracts;

namespace Tests
{
    [TestFixture]
    public class FactoryTests
    {
        [SetUp]
        public void Setup()
        {
            Factory.Overrides.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            Factory.Overrides.Clear();
        }


        [Test]
        public void get_service_from_factory()
        {
            //try and get sql runner
            var sqlRunner = Factory.Get<ISqlRunner>();

            Assert.That(sqlRunner, Is.Not.Null);
            Assert.That(sqlRunner, Is.TypeOf(typeof(SqlRunner)));
        }

        [Test]
        public void Overrides_work_correctly()
        {
            var fakeSqlRunner = MockRepository.GenerateMock<ISqlRunner>();
            Factory.Overrides.Add(typeof(ISqlRunner).FullName, fakeSqlRunner);

            var sqlRunner = Factory.Get<ISqlRunner>();
            Assert.That(sqlRunner, Is.SameAs(fakeSqlRunner), "objects should be the same");
        }
    }
}
