using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Reflection;

namespace SqlMigration
{
    /// <summary>
    /// Response for creating factories and services that
    /// SqlMigration use. Everything is created once and cached as
    /// as Singleton.
    /// </summary>
    public static class Factory
    {
        //todo: not public, get rid of stupid string...
        public static readonly Dictionary<string, object> Overrides = new Dictionary<string, object>();

        public static TService Get<TService>() where TService : class
        {
            string fullName = typeof(TService).FullName;
            //check Overrides first
            if (Overrides.ContainsKey(fullName))
                return (TService) Overrides[fullName];

            string concreteTypeName = ConfigurationManager.AppSettings[fullName];
            if (string.IsNullOrEmpty(concreteTypeName))
                throw new Exception(string.Format("Could not find key '{0}' in AppSettings", fullName));

            var instance = (TService)Activator.CreateInstance(Type.GetType(concreteTypeName), BindingFlags.CreateInstance, null, null,
                                                              CultureInfo.CurrentCulture);
            Overrides.Add(fullName, instance);
            return instance;
        }
    }
}
