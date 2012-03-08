using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SqlMigration;
using SqlMigration.Contracts;
using SqlMigration.Factories;


namespace Tests
{
    /// <summary>
    ///This is a test class for DeploymentTaskTest and is intended
    ///to contain all DeploymentTaskTest Unit Tests
    ///</summary>
    [TestFixture]
    public class MigrationFactoryTest : BaseTestClass
    {
        /// <summary>
        ///A test for RunTask
        ///</summary>
        [Test]
        public void make_sure_we_return_null_if_nothing_is_found()
        {
            Assert.That(MigrationFactory.GetMigrationByFileExtenstion(".fffff"),
                        Is.Null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void fail_if_fileExtenstion_is_null_or_empty()
        {
            MigrationFactory.GetMigrationByFileExtenstion(null);
        }

        [Test]
        public void sql_extenstion_should_come_back_with_TSqlMigration_type()
        {
            Migration migration = MigrationFactory.GetMigrationByFileExtenstion("c:\asdf.sql");
            Assert.That(migration, Is.Not.Null);
            Assert.That(migration.GetType(), Is.EqualTo(typeof(TSqlMigration)));
        }
    }
}