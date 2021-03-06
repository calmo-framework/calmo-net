﻿using Calmo.Data.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Calmo.Data
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCalmoDataService(this IServiceCollection services, IConfiguration configuration)
        {
            RepositoryDbAccess.Configuration = configuration;
        }
        public static void AddCalmoConfiguration(IConfiguration configuration)
        {
            RepositoryDbAccess.Configuration = configuration;
        }

        public static void AddCalmoConnectionString(string connection)
        {
            RepositoryDbAccess.ForcedConnectionString = connection;
        }
    }
}
