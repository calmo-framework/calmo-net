using Calmo.Data;
using Calmo.Data.MySql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calmo.Tests
{
    public class MariaDbRepository : Repository
    {
        public IEnumerable<TestDataModel> GetData()
        {
            return this.Data.Db()
                            .UseConnection("mariadb")
                            .AsStatement()
                            .List<TestDataModel>("select id, name, description, created_at as CreatedAt from test_data");
        }
    }
}
