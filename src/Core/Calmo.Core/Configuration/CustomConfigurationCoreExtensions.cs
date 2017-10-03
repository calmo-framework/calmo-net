using System;
using System.Configuration;
using Calmo.Core.Properties;

namespace Calmo.Core.Configuration
{
    public static class CustomConfigurationCoreExtensions
    {
        private static readonly object _lockObjectProxy = new object();
        private static ProxySection _proxySection;

        public static ProxySection Proxy(this CustomConfiguration config)
        {
            lock (_lockObjectProxy)
            {
                if (_proxySection == null)
                {
                    _proxySection = ConfigurationManager.GetSection(ProxySection.SectionName) as ProxySection;

                    if (_proxySection == null)
                        throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.SectionNotFound, ProxySection.SectionName));
                }
            }

            return _proxySection;
        }
    }
}
