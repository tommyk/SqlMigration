using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public class FileIOTest
    {

        /// <summary>
        ///A test for GetFileNameFromFullPath
        ///</summary>
        [Test]
        public void GetFileNameFromFullPathTest()
        {
            string filePath = "C:\\testfolder\\asdf asdf\\filename.sql";
            string expected = "filename.sql";
            string actual;
            actual = FileIO.GetFileNameFromFullPath(filePath);
            Assert.AreEqual(expected, actual, "It should find the file name");
        }

        [Test]
        public void WriteFileTest()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            int lastIndexOf = location.LastIndexOf(char.Parse("\\"));
            location = location.Substring(0, lastIndexOf);
            string filePath = location+ "\\Test.txt";
            try
            {
                new FileIO().WriteFile(filePath, "asdf");
                Assert.That("asdf", Is.EqualTo(File.ReadAllText(filePath)));
            }
            finally
            {
                File.Delete(filePath);
            }

        }
    }
}