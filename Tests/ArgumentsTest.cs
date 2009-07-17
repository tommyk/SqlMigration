using System.Reflection;
using NUnit.Framework;
using SqlMigration;

using System;

namespace Tests
{
    /// <summary>
    ///This is a test class for ArgumentsTest and is intended
    ///to contain all ArgumentsTest Unit Tests
    ///</summary>
    //[TestFixture]
    [TestFixture]
    public class ArgumentsTest
    {


        /// <summary>
        ///A test for TaskType
        ///</summary>
        [Test]
        public void make_sure_it_uses_the_first_command_as_the_TaskType()
        {
            string command = TaskTypeConstants.RunSqlFileTask;
            string[] arguments = new[] { command, "/asdf" };
            Arguments target = new Arguments(arguments);
            var actual = target.TaskType;

            //make sure they are equal
            Assert.AreEqual(command, actual, "The task type should match up");
        }


        [Test]
        public void find_argument_value()
        {
            string loc = @"C:\test";
            string[] arguments = new[] { ArgumentConstants.ScriptDirectoryArg, loc, "asdf", "asdf" }; // TODO: Initialize to an appropriate value
            Arguments target = new Arguments(arguments); // TODO: Initialize to an appropriate value

            //try to get the value
            string argumentValue = target.GetArgumentValue(ArgumentConstants.ScriptDirectoryArg);

            //make sure they are equal
            Assert.IsNotNull(argumentValue, "arg value came back null");
            Assert.AreEqual(loc, argumentValue, "They should be the same");
        }

        /// <summary>
        ///A test for DoesArgumentExist
        ///</summary>
        [Test]
        public void DoesArgumentExist_yes_it_does()
        {
            string[] arguments = new[] { "test", "asdf", ArgumentConstants.RunWithoutTransactionArg };
            Arguments target = new Arguments(arguments);

            bool actual = target.DoesArgumentExist(ArgumentConstants.RunWithoutTransactionArg);

            //make sure it does
            Assert.IsTrue(actual, "We should find that argument");
        }

        [Test]
        public void no_scriptDirectory_passed_in_so_we_default_to_working_directory()
        {
            string[] arguments = new[] { "test", "asdf", ArgumentConstants.RunWithoutTransactionArg };
            Arguments target = new Arguments(arguments);

            string argumentValue = target.GetArgumentValue(ArgumentConstants.ScriptDirectoryArg);

            //make sure its the applications working directory
            string appLocation = Environment.CurrentDirectory;//Assembly.GetExecutingAssembly().CodeBase;

            Assert.AreEqual(appLocation, argumentValue, string.Format("they should both be '{0}", appLocation));

        }

        /// <summary>
        ///A test for GetArgumentValue
        ///</summary>
        [Test]
        public void make_sure_we_dont_go_over_the_size_of_the_colletion_of_arguements()
        {
            string[] arguments = new[] { "flag1" };
            Arguments target = new Arguments(arguments);

            string argumentValue = target.GetArgumentValue("flag1");

            //make sure it comes back empty or null since it didn't exist in the collection of arguments
            Assert.IsNull(argumentValue, "Value should be null or empty");
        }
    }
}