using System.Data;
using Npgsql;

namespace Calmo.Data.PostgreSql
{
    public class PostgreSqlDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection GetDbConnection(string currentConnectionString, RepositoryDbAccess dbAccess)
        {
#if NETSTANDARD
            return new NpgsqlConnection (dbAccess.GetConnectionString(currentConnectionString));
#else
            return new MySqlConnection(dbAccess.GetConnectionString(currentConnectionString, "MySql.Data.MySqlClient", "MySql"));
#endif
        }
    }
}
