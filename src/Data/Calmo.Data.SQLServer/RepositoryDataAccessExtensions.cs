namespace Calmo.Data.SQLServer
{
    public static class RepositoryDataAccessExtensions
    {
        public static RepositoryDbAccess Db(this RepositoryDataAccess data)
        {
            return new RepositoryDbAccess
            {
                DbConnectionFactory = new SQLServerDbConnectionFactory()
            };
        }
    }
}
