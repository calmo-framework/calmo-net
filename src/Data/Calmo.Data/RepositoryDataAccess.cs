using System;

namespace Calmo.Data
{
    public class RepositoryDataAccess
    {
        
    }

    public static class RepositoryDataAccessExtensions
    {
        public static RepositoryDbAccess Db(this RepositoryDataAccess data, Func<RepositoryDbAccess, IDbConnectionFactory> dbFactory)
        {
            var dbAccess = new RepositoryDbAccess();
            var factory = dbFactory.Invoke(dbAccess);
            dbAccess.DbConnectionFactory = factory;

            return dbAccess;
        }
    }
}