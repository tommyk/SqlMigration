using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using SqlMigration;

using System.Collections.Generic;

namespace Tests
{
    /// <summary>
    ///This is a test class for FileIOTest and is intended
    ///to contain all FileIOTest Unit Tests
    ///</summary>
    [TestFixture]
    public class MigrationTaskFactoryTest
    {
        [TestFixtureSetUp]
        public void SetupIoC()
        {
            IoC.Current.SetupWindsorContainer();
        }

        [Test]
        public void make_sure_a_TaskType_will_always_return_an_object()
        {
            RunTestCode("/sql");
            RunTestCode("/m");
            RunTestCode("/d");
        }

        private void RunTestCode(string commandTask)
        {
            var args = new Arguments(new[] { commandTask });
            Assert.That(MigrationTaskFactory.GetMigrationTaskByTaskType(args), Is.Not.Null);
        }
    }
}