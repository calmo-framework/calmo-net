using System.Data;
using System.Data.SqlClient;

namespace Calmo.Data.SQLServer
{
    public class SQLServerDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetDbConnection(string currentConnectionString, RepositoryDbAccess dbAccess)
        {
#if NETSTANDARD
            return new SqlConnection(dbAccess.GetConnectionString(currentConnectionString));
#else
            return new SqlConnection(dbAccess.GetConnectionString(currentConnectionString, "System.Data.SqlClient", "SQL Server"));
#endif
        }
    }
}
