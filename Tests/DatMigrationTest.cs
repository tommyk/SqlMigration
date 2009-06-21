//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using NUnit.Framework;
//using NUnit.Framework.SyntaxHelpers;
//using SqlMigration;


//namespace SqlMigration.Test
//{


//    /// <summary>
//    ///This is a test class for DatMigrationTest and is intended
//    ///to contain all DatMigrationTest Unit Tests
//    ///</summary>
//    [TestFixture]
//    public class DatMigrationTest
//    {

//        /// <summary>
//        ///A test for GetSqlCommand
//        ///</summary>
//        [Test]
//        public void make_sure_it_creates_the_sql_command_for_bulk_insert()
//        {
//            string fileName = @"c:\2008-04-04_10h44m-YourQuest.Account.Person.dat";


//            var datMigration = new DatMigration(fileName);
//            string sqlString = datMigration.GetSqlCommand();

//            //make sure it has what we want
//            Assert.IsTrue(Regex.IsMatch(sqlString, "BULK INSERT YourQuest.Account.Person"), "We should see the parsed filename");
//            string fromMatchPattern = string.Format("FROM '{0}'", fileName);
//            Assert.IsTrue(sqlString.Contains(fromMatchPattern), "This should be in the sql text");

//        }

//        [Test]
//        public void make_sure_it_creates_the_sql_command_for_bulk_insert_with_numbers_in_the_database_name()
//        {
//            string fileName = @"c:\2008-04-04_10h44m-YourQuestQ32008.Account.Person.dat";

//            var datMigration = new DatMigration(fileName);
//            IList<string> sqlCommands = datMigration.GetSqlCommands();
//            Assert.That(sqlCommands.Count, Is.EqualTo(1)); //there should only be one command ever for DatMigrations

//            string sqlString = sqlCommands[0];
//            //make sure it has what we want
//            Assert.IsTrue(Regex.IsMatch(sqlString, "BULK INSERT YourQuestQ32008.Account.Person"), "We should see the parsed filename");
//            string fromMatchPattern = string.Format("FROM '{0}'", fileName);
//            Assert.IsTrue(sqlString.Contains(fromMatchPattern), "This should be in the sql text");

//        }

//    }
//}
