using Calmo.Data;
using Calmo.Data.ActiveDirectory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Calmo.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var repo = new ADRepository();
            var result = repo.GetUserByMail("some.mail@domain.com");
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

    public class ADRepository : Repository
    {
        public object GetUserByMail(string mail)
        {
            return this.Data.ActiveDirectory()
                            .Users()
                            .WithParameters(new { mail })
                            .Get();
        }
    }
}
