namespace Calmo.Data.SQLServer
{
    public static class SQLServerDbAccessConfigExtensions
    {
        public static SQLServerDbConnectionFactory OnSQLServer(this RepositoryDbAccess config)
        {
            return new SQLServerDbConnectionFactory();
        }
    }
}
