using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SqlMigration;
using SqlMigration.Factories;
using SqlMigration.Tasks;

namespace Tests
{
    /// <summary>
    ///This is a test class for MigrationHelperTest and is intended
    ///to contain all MigrationHelperTest Unit Tests
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