using System.Reflection;
using SqlMigration;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SqlMigration.Test
{
    
    
    /// <summary>
    ///This is a test class for ArgumentsTest and is intended
    ///to contain all ArgumentsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ArgumentsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        ///A test for TaskType
        ///</summary>
        [TestMethod()]
        public void make_sure_it_finds_deployment_task()
        {
            string[] arguments = new[] {"/d", "C:\\test"}; // TODO: Initialize to an appropriate value
            Arguments target = new Arguments(arguments); // TODO: Initialize to an appropriate value
            var task = TaskType.CreateDeploymentFolder;
            var actual = target.TaskType;
            
            //make sure they are equal
            Assert.AreEqual(task, actual, "The task type should match up");
        }

        [TestMethod()]
        public void make_sure_it_finds_migration_task()
        {
            string[] arguments = new[] { "/m" }; // TODO: Initialize to an appropriate value
            Arguments target = new Arguments(arguments); // TODO: Initialize to an appropriate value
            var task = TaskType.RunSQL;
            var actual = target.TaskType;

            //make sure they are equal
            Assert.AreEqual(task, actual, "The task type should match up");
        }


        [TestMethod()]
        public void find_argument_value()
        {
            string loc = @"C:\test";
            string[] arguments = new[] { "/sd", loc, "asdf", "asdf"}; // TODO: Initialize to an appropriate value
            Arguments target = new Arguments(arguments); // TODO: Initialize to an appropriate value

            //try to get the value
            string argumentValue = target.GetArgumentValue("/sd");

            //make sure they are equal
            Assert.IsNotNull(argumentValue, "arg value came back null");
            Assert.AreEqual(loc, argumentValue, "They should be the same");
        }

        /// <summary>
        ///A test for DoesArgumentExist
        ///</summary>
        [TestMethod()]
        public void DoesArgumentExist_yes_it_does()
        {
            string[] arguments = new[]{"test", "asdf", "/nt"}; 
            Arguments target = new Arguments(arguments);

            bool actual= target.DoesArgumentExist("/nt");
            
            //make sure it does
            Assert.IsTrue(actual, "We should find that argument");
        }

        [TestMethod()]
        public void no_scriptDirectory_passed_in_so_we_default_to_working_directory()
        {
            string[] arguments = new[] { "test", "asdf", "/nt" };
            Arguments target = new Arguments(arguments);

            string argumentValue = target.GetArgumentValue("/sd");

            //make sure its the applications working directory
            string appLocation = Environment.CurrentDirectory;//Assembly.GetExecutingAssembly().CodeBase;

            Assert.AreEqual(appLocation, argumentValue, string.Format("they should both be '{0}", appLocation));
            
        }

        /// <summary>
        ///A test for GetArgumentValue
        ///</summary>
        [TestMethod()]
        public void make_sure_we_dont_go_over_the_size_of_the_colletion_of_arguements()
        {
            string[] arguments = new[]{"flag1"}; 
            Arguments target = new Arguments(arguments);

            string argumentValue = target.GetArgumentValue("flag1");

            //make sure it comes back empty or null since it didn't exist in the collection of arguments
            Assert.IsNull(argumentValue, "Value should be null or empty");
        }
    }
}
