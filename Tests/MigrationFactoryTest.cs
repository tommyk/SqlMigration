using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using SqlMigration;


namespace Tests
{
    /// <summary>
    ///This is a test class for DeploymentTaskTest and is intended
    ///to contain all DeploymentTaskTest Unit Tests
    ///</summary>
    [TestFixture]
    public class MigrationFactoryTest : BaseTestClass
    {

        //public MigrationFactoryTest()
        //{
        //    base.Setup += Setup;
        //}

        //void Setup(object sender, EventArgs e)
        //{
        //    //setup the ioc
        //    //IoC.Current.SetupWindsorContainer();
        //}

        /// <summary>
        ///A test for RunTask
        ///</summary>
        [Test]
        public void make_sure_we_return_null_if_nothing_is_found()
        {
            Assert.That(MigrationFactory.GetMigrationByFileExtenstion(".fffff"),
                        Is.Null);
        }

        [Test]
        public void fail_if_fileExtenstion_is_null_or_empty()
        {
            try
            {
                MigrationFactory.GetMigrationByFileExtenstion(null);
                Assert.Fail("This should not be hit");
            }
            catch (ArgumentNullException ex)
            {
            }
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