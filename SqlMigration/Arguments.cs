using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace URQuest.Tools.DBMigrator
{
    /// Arugments:
    /// Task Types -- Always first argument!
    /// /d  -  creates a deployment directory at the specified location
    /// 
    /// Other Args -- Just other stuff
    /// /sd "location of scripts" - location of the scripts

    public class Arguments
    {
        private readonly string[] _arguments;
        private static readonly List<KeyValuePair<TaskType, string>> TaskTypeList;

        #region Constructors
        static Arguments()
        {
            TaskTypeList = new List<KeyValuePair<TaskType, string>>();

            //add to the list
            TaskTypeList.Add(new KeyValuePair<TaskType, string>(TaskType.CreateDeploymentFolder, "/d"));
            TaskTypeList.Add(new KeyValuePair<TaskType, string>(TaskType.RunSQL, "/m"));
        }


        public Arguments(string[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
                throw new ArgumentException("Must atleast provide a TaskType");

            _arguments = arguments;
        }
        #endregion


        public string[] CommandArguments
        {
            get { return _arguments; }
        }

        private TaskType? _taskType;
        public TaskType TaskType
        {
            get
            {
                if (!_taskType.HasValue)
                {
                    KeyValuePair<TaskType, string> valuePair = TaskTypeList.Find(pair => pair.Value == _arguments[0]);
                    _taskType = valuePair.Key;
                }

                return _taskType.Value;
            }
        }



        /// <summary>
        /// Idea is to take in '/sd' and then find the next value in the argument that goes with it.
        /// </summary>
        /// <param name="flagToFind"></param>
        /// <returns></returns>
        public string GetArgumentValue(string flagToFind)
        {
            //value to return
            string argValue = null;

            //try to find flag, then get next arg
            for (int i = 0; i < _arguments.Length; i++)
            {
                string args = _arguments[i];
                if (args == flagToFind)
                {
                    //make sure we don't go over length
                    if ((i + 1) != _arguments.Length)
                    {
                        argValue = _arguments[i + 1];
                        break;
                    }
                }
            }

            //lets check to see if its a flag that has a default value if its not passed in!
            if (argValue == null)
            {
                argValue = SeeIfThereIsADefaultValue(flagToFind);
            }

            return argValue;
        }

        /// <summary>
        /// Checks to see if the flag is one that has a default value we should pass back
        /// </summary>
        /// <param name="flagToFind"></param>
        /// <returns></returns>
        private static string SeeIfThereIsADefaultValue(string flagToFind)
        {
            string argValue = null;

            switch (flagToFind)
            {
                case "/sd":
                    argValue = Environment.CurrentDirectory;
                    break;
                default:
                    break;
            }

            return argValue;
        }

        /// <summary>
        /// Use to find flags such as /nt where they don't have any value associated with it, it just is there.
        /// </summary>
        /// <param name="flagToFind"></param>
        /// <returns></returns>
        public bool DoesArgumentExist(string flagToFind)
        {
            return _arguments.Contains(flagToFind);
        }
    }
}
