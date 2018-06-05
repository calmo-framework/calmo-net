using System;
using System.Configuration;
using Calmo.Core;
using Calmo.Services.Configuration;
using Calmo.Core.Properties;

namespace Calmo.Services
{
    public static class CustomConfigurationServicesExtensions
    {
        private static readonly object _lockObjectServices = new object();
        private static ServicesSection _servicesSection;

        public static ServicesSection Services(this CustomConfiguration config)
        {
            lock (_lockObjectServices)
            {
                if (_servicesSection == null)
                {
                    _servicesSection = ConfigurationManager.GetSection(ServicesSection.SectionName) as ServicesSection;

                    if (_servicesSection == null)
                        _servicesSection = new ServicesSection();
                }
            }

            return _servicesSection;
        }
    }
}