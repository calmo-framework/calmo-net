using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calmo.Data.Oracle
{
    public static class OracleDbAccessConfigExtensions
    {
        public static OracleDbConnectionFactory OnOracle(this RepositoryDbAccess config)
        {
            return new OracleDbConnectionFactory();
        }
    }
}
