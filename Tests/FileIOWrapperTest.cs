﻿using URQuest.Tools.DBMigrator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace URQuest.Tools.DBMigrator.Test
{
    
    
    /// <summary>
    ///This is a test class for FileIOWrapperTest and is intended
    ///to contain all FileIOWrapperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileIOWrapperTest
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetFileNameFromFullPath
        ///</summary>
        [TestMethod()]
        [DeploymentItem("URQuest.Tools.DBMigrator.exe")]
        public void GetFileNameFromFullPathTest()
        {
            string filePath = "C:\\testfolder\\asdf asdf\\filename.sql";
            string expected = "filename.sql";
            string actual;
            actual = FileIOWrapper_Accessor.GetFileNameFromFullPath(filePath);
            Assert.AreEqual(expected, actual, "It should find the file name");
        }
    }
}
