using System;
using System.Configuration;
using System.Data;
using Calmo.Data.Properties;
using Oracle.DataAccess.Client;

namespace Calmo.Data.Oracle
{
    public class OracleDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetDbConnection(string currentConnectionString)
        {
            var connectionStringData = ConfigurationManager.ConnectionStrings[currentConnectionString];

            if (!String.IsNullOrEmpty(connectionStringData.ProviderName))
            {
                if (!String.Equals(connectionStringData.ProviderName, "Oracle.DataAccess.Client", StringComparison.InvariantCultureIgnoreCase))
                    throw new ConfigurationErrorsException(String.Format(Messages.IncorrectProvider, connectionStringData.ProviderName, currentConnectionString, "Oracle"));
            }

            return new OracleConnection(connectionStringData.ConnectionString);
        }
    }
}
