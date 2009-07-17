﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlMigration
{
    public class MigrationFactory
    {
        /// <summary>
        /// Used to get a Migration base object by looking in a table to find out
        /// what Migration type to use with what file extenstion.
        /// </summary>
        /// <param name="filePath">filename</param>
        /// <returns>A Migration object if it finds something related to that 
        /// file extenstion.  If it doe not, it returns null.</returns>
        public static Migration GetMigrationByFileExtenstion(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("Must supply a valid file extenstion");

            Migration migration = null;
            Type migrationType = null;
            //try to get the type to create for this file extenstion
            try
            {
                migrationType = MigrationTypeFactory.GetMigrationTypeByFileExtenstion(filePath);
            }
            catch {}

            //if we found a type, lets try to create it!!
            if (migrationType != null)
                migration = (Migration) IoC.Current.Container.Resolve(migrationType, new{filePath = filePath});

            return migration;
        }
    }

    public static class MigrationTypeFactory
    {
        private static readonly Dictionary<string, Type> _migrationType =
            new Dictionary<string, Type>
                {
                    {".sql", typeof(TSqlMigration)}
                };

        public static Type GetMigrationTypeByFileExtenstion(string fileName)
        {
            return _migrationType.Where(pair => fileName.EndsWith(pair.Key)).Single().Value;
        }

    }
}