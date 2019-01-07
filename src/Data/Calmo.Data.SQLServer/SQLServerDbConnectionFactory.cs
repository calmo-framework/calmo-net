using System.Data;
using System.Data.SqlClient;

namespace Calmo.Data.SQLServer
{
    public class SQLServerDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetDbConnection(string currentConnectionString, RepositoryDbAccess dbAccess)
        {
#if NETCOREAPP
            return new SqlConnection(dbAccess.GetConnectionString(currentConnectionString));
#else
            return new SqlConnection(dbAccess.GetConnectionString(currentConnectionString, "System.Data.SqlClient", "SQL Server"));
#endif
        }
    }
}
