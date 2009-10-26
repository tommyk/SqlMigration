using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SqlMigration;

namespace Tests
{
    /// <summary>
    ///This is a test class for MigrationHelperTest and is intended
    ///to contain all MigrationHelperTest Unit Tests
    ///</summary>
    [TestFixture]
    public class MigrationTaskFactoryTest
    {
        [TestFixtureSetUp]
        public void SetupIoC()
        {
            //IoC.Current.SetupWindsorContainer();
        }

        [Test]
        public void make_sure_a_TaskType_will_always_return_an_object()
        {
            RunTestCode(TaskTypeConstants.RunSqlFileTask);
            RunTestCode(TaskTypeConstants.MigrateDatabaseForwardTask);
            RunTestCode(TaskTypeConstants.DeploymentTask);
        }

        private void RunTestCode(string commandTask)
        {
            var args = new Arguments(new[] { commandTask });
            Assert.That(MigrationTaskFactory.GetMigrationTaskByTaskType(args), Is.Not.Null);
        }
    }
}