using System.Configuration;

namespace Calmo.Data.Api.Configuration
{
    public class ApiDataSection : ConfigurationSection
    {
        internal const string SectionName = "core.Settings/api.Settings";

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)this["url"]; }
            set { this["url"] = value; }
        }

        [ConfigurationProperty("headers", IsDefaultCollection = true, IsRequired = true)]
        public HeaderCollection Headers
        {
            get { return (HeaderCollection)base["headers"]; }
        }
    }
}