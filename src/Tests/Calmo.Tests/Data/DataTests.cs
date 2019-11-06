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
using Calmo.Data.SAP;
using System;

namespace Calmo.Tests.Data
{
    [TestClass]
    public class DataTests
    {
        [TestMethod]
        public void ValidateModelList()
        {
            var repository = new ModelRepository();
            var items = repository.ListSAP();
            var items2 = repository.ListSAP2();
            var items3 = repository.ListSAP3();
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

        public IEnumerable<Model> ListSAP()
        {
            return this.Data.SAP()
                .Bapi("Z_RFC_BORLAND_CONS_FORNECEDOR")
                .WithParameters(new {
                    I_FORNECEDOR = "",
                    I_CPF = "",
                    I_CNPJ = "",
                    I_NOME = "renato"
                })
                .UseMapping<Model>("T_FORNECEDOR", m => m.Map(p => p.CPF, "STCD2")
                                                         .Map(p => p.CNPJ, "STCD1")
                                                         .Map(p => p.Name, "NAME1"))
                .List<Model>();
        }

        public Tuple<IEnumerable<Model>, IEnumerable<Model2>> ListSAP2()
        {
            return this.Data.SAP()
                .Bapi("Z_RFC_BORLAND_CONS_FORNECEDOR")
                .WithParameters(new
                {
                    I_FORNECEDOR = "",
                    I_CPF = "",
                    I_CNPJ = "",
                    I_NOME = "renato"
                })
                .UseMapping<Model>("T_FORNECEDOR", m => m.Map(p => p.CPF, "STCD2")
                                                         .Map(p => p.CNPJ, "STCD1")
                                                         .Map(p => p.Name, "NAME1"))
                .UseMapping<Model2>("BANCOS", m => m.Map(p => p.BankLine, "BANKL")
                                                    .Map(p => p.BANKN)
                                                    .Map(p => p.BANKA))
                .List<Model, Model2>();
        }

        public Tuple<IEnumerable<Model2>, IEnumerable<Model>> ListSAP3()
        {
            return this.Data.SAP()
                .Bapi("Z_RFC_BORLAND_CONS_FORNECEDOR")
                .WithParameters(new
                {
                    I_FORNECEDOR = "",
                    I_CPF = "",
                    I_CNPJ = "",
                    I_NOME = "renato"
                })
                .UseMapping<Model>("T_FORNECEDOR", m => m.Map(p => p.CPF, "STCD2")
                                                         .Map(p => p.CNPJ, "STCD1")
                                                         .Map(p => p.Name, "NAME1"))
                .UseMapping<Model2>("BANCOS", m => m.Map(p => p.BankLine, "BANKL")
                                                    .Map(p => p.BANKN)
                                                    .Map(p => p.BANKA))
                .List<Model2, Model>();
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
        public string CNPJ { get; set; }
    }

    public class Model2
    {
        public string BankLine { get; set; }
        public string BANKN { get; set; }
        public string BANKA { get; set; }
    }
}
