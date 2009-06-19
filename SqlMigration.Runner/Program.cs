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
                Arguments arguments = new Arguments(args);

                //setup a task and run it
                MigrationTask task;

                //decide what task we want, and create it
                switch (arguments.TaskType)
                {
                    case TaskType.CreateDeploymentFolder:
                        task = new DeploymentTask(arguments);
                        break;
                    case TaskType.RunSQL:
                        task = new RunSQLTask(arguments);
                        break;
                    default:
                        throw new ArgumentException("No tasktype found");
                }

                //run the task
                try
                {
                    //try to run the task
                    returnValue = task.RunTask();
                }
                catch (Exception ex)
                {
                    //todo: Log error possibly?
                    Console.WriteLine(ex.Message);
                    returnValue = -1;
                } 
            }
            else
            {
                //no args were passed in, lets display the help contents
                string helpFile = Resources.HelpInstructions;
                Console.Write(helpFile);
            }

            Console.Read();

            return returnValue;
        }

    }
}
