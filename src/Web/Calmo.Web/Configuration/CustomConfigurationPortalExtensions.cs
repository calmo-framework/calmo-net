using System;
using System.Configuration;
using Calmo.Core;
using Calmo.Core.Configuration;
using Calmo.Web.Configuration;
using Calmo.Core.Properties;

namespace Calmo.Web
{
    public static class CustomConfigurationPortalExtensions
    {
        private static readonly object _lockObjectUrl = new object();
        private static UrlsSection _urlSection;

        public static UrlsSection Urls(this CustomConfiguration config)
        {
            lock (_lockObjectUrl)
            {
                if (_urlSection == null)
                {
                    _urlSection = ConfigurationManager.GetSection(UrlsSection.SectionName) as UrlsSection;

                    if (_urlSection == null)
                        throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.SectionNotFound, UrlsSection.SectionName));
                }
            }

            return _urlSection;
        }

        private static readonly object _lockObjectPath = new object();
        private static PathsSection _pathSection;

        public static PathsSection Paths(this CustomConfiguration config)
        {
            lock (_lockObjectPath)
            {
                if (_pathSection == null)
                {
                    _pathSection = ConfigurationManager.GetSection(PathsSection.SectionName) as PathsSection;

                    if (_pathSection == null)
                        throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.SectionNotFound, PathsSection.SectionName));
                }
            }

            return _pathSection;
        }

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

        private static readonly object _lockObjectGoogle = new object();
        private static GoogleSection _googleSection;

        public static GoogleSection Google(this CustomConfiguration config)
        {
            lock (_lockObjectGoogle)
            {
                if (_googleSection == null)
                {
                    _googleSection = ConfigurationManager.GetSection(GoogleSection.SectionName) as GoogleSection;

                    if (_googleSection == null)
                        throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.SectionNotFound, GoogleSection.SectionName));
                }
            }

            return _googleSection;
        }

        private static readonly object _lockObjectLucene = new object();
        private static LuceneSection _luceneSection;

        public static LuceneSection Lucene(this CustomConfiguration config)
        {
            lock (_lockObjectLucene)
            {
                if (_luceneSection == null)
                {
                    _luceneSection = ConfigurationManager.GetSection(LuceneSection.SectionName) as LuceneSection;

                    if (_luceneSection == null)
                        throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.SectionNotFound, LuceneSection.SectionName));
                }
            }

            return _luceneSection;
        }
    }
}