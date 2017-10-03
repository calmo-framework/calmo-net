using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
