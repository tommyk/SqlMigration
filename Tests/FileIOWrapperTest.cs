using NUnit.Framework;
using SqlMigration;

namespace Tests
{
    /// <summary>
    ///This is a test class for FileIOWrapperTest and is intended
    ///to contain all FileIOWrapperTest Unit Tests
    ///</summary>
    [TestFixture]
    public class FileIOWrapperTest
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
    }
}