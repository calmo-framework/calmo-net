using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calmo.Data.Forms
{
    public class RepositoryDataAccess
    {

    }

    public static class RepositoryDataAccessExtensions
    {
        public static RepositoryDbAccess Db(this RepositoryDataAccess data)
        {
            return new RepositoryDbAccess();
        }

        public static RepositoryApiAccess Api(this RepositoryDataAccess data)
        {
            return new RepositoryApiAccess();
        }
    }
}
