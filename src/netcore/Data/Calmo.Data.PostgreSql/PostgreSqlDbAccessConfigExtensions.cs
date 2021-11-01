using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calmo.Data.PostgreSql
{
    public static class MySqlDbAccessConfigExtensions
    {
        public static PostgreSqlDbConnectionFactory OnPostgreSql(this RepositoryDbAccess config)
        {
            return new PostgreSqlDbConnectionFactory();
        }
    }
}
