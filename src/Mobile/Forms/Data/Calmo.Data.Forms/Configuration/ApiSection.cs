using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PCLAppConfig;
using PCLAppConfig.Infrastructure;

namespace Calmo.Data.Forms.Configuration
{
    public class ApiSection
    {
        internal const string ApiUrlKey = "calmo.data.apiUrl";
        internal const string ApiHeaderKey = "calmo.data.header";
        internal ApiSection()
        {
            if (ConfigurationManager.AppSettings == null)
                throw new FileNotFoundException("App.config not found or Calmo.Data.Forms.Data.Init() method not called on AppDelegate/MainActivity file.");

            var urlValue = ConfigurationManager.AppSettings[ApiSection.ApiUrlKey];
            if (!String.IsNullOrWhiteSpace(urlValue))
                this.Url = urlValue;

            var headers = new List<Setting>();
            var headerValues = ConfigurationManager.AppSettings.GetValues(ApiSection.ApiHeaderKey);
            foreach (var headerValue in headerValues)
            {
                if (!String.IsNullOrWhiteSpace(headerValue))
                {
                    var headerValueParts = headerValue.Split(';');
                    if (headerValueParts.Length > 1)
                        headers.Add(new Setting {Key = headerValueParts[0], Value = headerValueParts[1]});
                }
            }

            this.Headers = headers;
        }

        public string Url { get; }

        public IEnumerable<Setting> Headers { get; }
    }
}