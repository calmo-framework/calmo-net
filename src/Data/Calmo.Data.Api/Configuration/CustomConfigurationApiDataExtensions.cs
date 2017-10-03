using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Calmo.Core;
using Calmo.Data.Api.Configuration;
using Calmo.Core.Configuration;
using Calmo.Core.Properties;

namespace Calmo.Data
{
    public static class CustomConfigurationApiDataExtensions
    {
        private static readonly object _lockObjectApiData = new object();
        private static ApiDataSection _apiDataSection;

        public static ApiDataSection Api(this CustomConfiguration config)
        {
            lock (_lockObjectApiData)
            {
                if (_apiDataSection == null)
                {
                    _apiDataSection = ConfigurationManager.GetSection(ApiDataSection.SectionName) as ApiDataSection;

                    if (_apiDataSection == null)
                        throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.SectionNotFound, ApiDataSection.SectionName));
                }
            }

            return _apiDataSection;
        }
    }
}