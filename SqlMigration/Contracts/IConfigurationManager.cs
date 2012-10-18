using System.Collections.Specialized;

namespace SqlMigration.Contracts
{
    public interface IConfigurationManager
    {
        NameValueCollection AppSettings { get; }
    }
}
