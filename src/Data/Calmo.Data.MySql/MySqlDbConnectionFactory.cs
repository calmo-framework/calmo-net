using System.Data;
using MySql.Data.MySqlClient;

namespace Calmo.Data.MySql
{
    public class MySqlDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetDbConnection(string currentConnectionString, RepositoryDbAccess dbAccess)
        {
#if NETCOREAPP
            return new MySqlConnection(dbAccess.GetConnectionString(currentConnectionString));
#else
            return new MySqlConnection(dbAccess.GetConnectionString(currentConnectionString, "MySql.Data.MySqlClient", "MySql"));
#endif
        }
    }
}
