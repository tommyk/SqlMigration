using System.Collections.Specialized;
using System.Configuration;
using SqlMigration.Contracts;

namespace SqlMigration
{
    public class ConfigurationManagerWrapper : IConfigurationManager
    {
        public NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }
    }
}
