using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Calmo.Data;
using Calmo.Data.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calmo.Data.Api;
using Calmo.Data.MySql;
using Calmo.Data.Oracle;
using Calmo.Data.Sharepoint;
using Calmo.Data.SQLServer;

namespace Calmo.Tests.Data
{
    [TestClass]
    public class DataTests
    {
        [TestMethod]
        public void ValidateModelList()
        {
            var repository = new ModelRepository();
            
        }
    }

    public class ModelRepository : Repository
    {
        public IEnumerable<Model> ListSQLServer()
        {
            return this.Data.Db(db => db.OnSQLServer())
                .AsProcedure()
                .WithParameters(null)
                .List<Model>("Lalala");
        }

        public IEnumerable<Model> ListOracle()
        {
            return this.Data.Db(db => db.OnOracle())
                .AsProcedure()
                .WithParameters(null)
                .List<Model>("Lalala");
        }

        public IEnumerable<Model> ListMySql()
        {
            return this.Data.Db(db => db.OnMySql())
                .AsProcedure()
                .WithParameters(null)
                .List<Model>("Lalala");
        }

        public async Task<IEnumerable<Model>> ListApi()
        {
            return await this.Data.Api()
                .WithParameters(null)
                .UseBearer("asdadsadfsdlnfkasldfnsfd")
                .List<Model>("Lalala");
        }

        public IEnumerable<ActiveDirectoryUserInfo> ListAD()
        {
            return this.Data.ActiveDirectory()
                .Users()
                .WithParameters(null)
                .List();
        }

        public bool ValidateAD(string login, out bool isLockedOut)
        {
            return this.Data.ActiveDirectory()
                .User()
                .Validate(login, out isLockedOut);
        }

        public bool AuthenticateAD(string login, string password)
        {
            return this.Data.ActiveDirectory()
                .User()
                .Authenticate(login, password);
        }

        public IEnumerable<Model> ListSharepoint()
        {
            return this.Data.Sharepoint()
                .UseMapping(new {})
                .List<Model>("lalala")
                .Retrieve();
        }
    }

    public class Model
    {
        public string Name { get; set; }
        public string CPF { get; set; }
    }
}
