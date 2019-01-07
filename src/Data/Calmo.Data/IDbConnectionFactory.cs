using System.Data;

namespace Calmo.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetDbConnection(string currentConnectionString, RepositoryDbAccess dbAccess);
    }
}
