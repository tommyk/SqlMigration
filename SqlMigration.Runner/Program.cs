using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlMigration.Runner
{
    public class Program
    {
        public static int Main(string[] args)
        {
            int returnValue = -1;

            if (args.Length != 0)
            {
                //setup IoC
                IoC.Current.SetupWindsorContainer();

                //create the arguments
                Arguments arguments = new Arguments(args);

                //setup a task and run it
                MigrationTask task = MigrationTaskFactory.GetMigrationTaskByTaskType(arguments);

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
