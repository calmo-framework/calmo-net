using Calmo.Data;
using Calmo.Data.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Calmo.Tests
{
    [TestClass]
    public class PostgreTests
    {
        [TestMethod]
        public void TestPostgres()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var repo = new PostgreRepository();
            var result = repo.GetData();

            Assert.IsTrue(result.HasItems());
            foreach(var item in result)
            {
                Console.WriteLine($"id:{item.Id}|name:{item.Name}|desc:{item.Description}");
            }
        }

        [TestMethod]
        public void TestMariaDb()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var repo = new MariaDbRepository();
            var result = repo.GetData();

            Assert.IsTrue(result.HasItems());
            foreach (var item in result)
            {
                Console.WriteLine($"id:{item.Id}|name:{item.Name}|desc:{item.Description}");
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddCalmoDataService(configuration);
        }
    }

    public class PostgreRepository : Repository
    {
        public IEnumerable<TestDataModel> GetData()
        {
            return this.Data.Db()
                            .AsStatement()
                            .List<TestDataModel>("select id, name, description, created_at as CreatedAt from test_data");
        }
    }

    public class TestDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
