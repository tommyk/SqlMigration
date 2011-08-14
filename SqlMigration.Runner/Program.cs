using System;
using SqlMigration.Contracts;

namespace SqlMigration.Runner
{
    public class Program
    {
        public static int Main(string[] args)
        {
            //setup log4net
            log4net.Config.XmlConfigurator.Configure();

            int returnValue = -1;

            if (args.Length != 0)
            {
                //create the arguments
                Arguments arguments = new Arguments(args);

                //setup a task and run it
                MigrationTask task = Factory.Get<IMigrationTaskFactory>().GetMigrationTaskByTaskType(arguments);

                returnValue = task.RunTask();
            }
            else
            {
                //no args were passed in, lets display the help contents
                string helpFile = Resources.HelpInstructions;
                Console.Write(helpFile);
            }
            return returnValue;
        }

    }
}
