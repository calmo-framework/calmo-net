using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calmo.Data.MySql
{
    public static class MySqlDbAccessConfigExtensions
    {
        public static MySqlDbConnectionFactory OnMySql(this RepositoryDbAccess config)
        {
            return new MySqlDbConnectionFactory();
        }
    }
}
