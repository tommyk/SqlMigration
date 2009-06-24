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
    public class TaskTypeFactoryTest
    {
        [Test]
        public void make_sure_we_map_default_TaskCommands()
        {
            Assert.That(TaskTypeFactory.GetTaskType("/sql"), Is.EqualTo(typeof (RunSqlFileTask)));
            Assert.That(TaskTypeFactory.GetTaskType("/d"), Is.EqualTo(typeof (DeploymentTask)));
            Assert.That(TaskTypeFactory.GetTaskType("/m"), Is.EqualTo(typeof (MigrateDatabaseForwardTask)));

        }
    }
}