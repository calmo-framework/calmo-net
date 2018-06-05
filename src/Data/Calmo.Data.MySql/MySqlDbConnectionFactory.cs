using System;
using System.Configuration;
using System.Data;
using Calmo.Data.Properties;
using MySql.Data.MySqlClient;

namespace Calmo.Data.MySql
{
    public class MySqlDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetDbConnection(string currentConnectionString)
        {
            var connectionStringData = ConfigurationManager.ConnectionStrings[currentConnectionString];

            if (!String.IsNullOrEmpty(connectionStringData.ProviderName))
            {
                if (!String.Equals(connectionStringData.ProviderName, "MySql.Data.MySqlClient", StringComparison.InvariantCultureIgnoreCase))
                    throw new ConfigurationErrorsException(String.Format(Messages.IncorrectProvider, connectionStringData.ProviderName, currentConnectionString, "MySql"));
            }

            return new MySqlConnection(connectionStringData.ConnectionString);
        }
    }
}
