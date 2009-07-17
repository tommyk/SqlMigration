using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SqlMigration;

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
            Assert.That(TaskTypeFactory.GetTaskType(TaskTypeConstants.RunSqlFileTask), Is.EqualTo(typeof (RunSqlFileTask)));
            Assert.That(TaskTypeFactory.GetTaskType(TaskTypeConstants.DeploymentTask), Is.EqualTo(typeof (DeploymentTask)));
            Assert.That(TaskTypeFactory.GetTaskType(TaskTypeConstants.MigrateDatabaseForwardTask), Is.EqualTo(typeof (MigrateDatabaseForwardTask)));

        }
    }
}