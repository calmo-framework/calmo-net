using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Calmo.Data.Properties;

namespace Calmo.Data.SQLServer
{
    public class SQLServerDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetDbConnection(string currentConnectionString)
        {
            var connectionStringData = ConfigurationManager.ConnectionStrings[currentConnectionString];

            if (!String.IsNullOrEmpty(connectionStringData.ProviderName))
            {
                if (!String.Equals(connectionStringData.ProviderName, "System.Data.SqlClient", StringComparison.InvariantCultureIgnoreCase))
                    throw new ConfigurationErrorsException(String.Format(Messages.IncorrectProvider, connectionStringData.ProviderName, currentConnectionString, "SQL Server"));
            }

            return new SqlConnection(connectionStringData.ConnectionString);
        }
    }
}
