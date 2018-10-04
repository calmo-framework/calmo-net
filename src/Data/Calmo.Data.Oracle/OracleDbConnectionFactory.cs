using System.Data;
using Oracle.DataAccess.Client;

namespace Calmo.Data.Oracle
{
    public class OracleDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetDbConnection(string currentConnectionString, RepositoryDbAccess dbAccess)
        {
#if NETCOREAPP
            return new SqlConnection(dbAccess.GetConnectionString(currentConnectionString));
#else
            return new OracleConnection(dbAccess.GetConnectionString(currentConnectionString, "Oracle.DataAccess.Client", "Oracle"));
#endif
        }
    }
}
