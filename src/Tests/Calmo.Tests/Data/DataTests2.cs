using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Calmo.Data;
using Calmo.Data.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calmo.Data.SQLServer;

namespace Calmo.Tests.Data
{
    [TestClass]
    public class DataTests2
    {
        [TestMethod]
        public void ValidateModelList()
        {
            var repository = new ModelRepository2();
            var x = repository.ListSQLServer();
        }
    }

    public class ModelRepository2 : Repository
    {
        public IEnumerable<Model2> ListSQLServer()
        {
            return this.Data.Db().UseConnection("conciliadorAlfaSeg")
                .AsProcedure()
                .WithParameters(new {
                cdarquivo = 3
            })
                .List<Model2>("arq_pr_ConsultaArquivoPorID");
        }
    }

    public class Model2
    {
        public int cdarquivo { get; set; }
        public string descricao { get; set; }
        public byte[] arquivo { get; set; }
    }
}
