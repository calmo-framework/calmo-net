﻿namespace Calmo.Data.PostgreSql
{
    public static class RepositoryDataAccessExtensions
    {
        public static RepositoryDbAccess Db(this RepositoryDataAccess data)
        {
            return new RepositoryDbAccess
            {
                DbConnectionFactory = new PostgreSqlDbConnectionFactory()
            };
        }
    }
}
