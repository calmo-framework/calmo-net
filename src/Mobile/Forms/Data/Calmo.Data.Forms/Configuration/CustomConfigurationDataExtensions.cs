using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calmo.Data.Forms.Configuration;
using PCLAppConfig;
using Calmo.Core;

namespace Calmo.Data.Forms
{
    public static class CustomConfigurationDataExtensions
    {
        private static readonly object _lockObjectApi = new object();
        private static ApiSection _apiSection;

        public static ApiSection Api(this CustomConfiguration config)
        {
            lock (_lockObjectApi)
            {
                if (_apiSection == null)
                    _apiSection = new ApiSection(ConfigurationManager.AppSettings);
            }

            return _apiSection;
        }
    }
}