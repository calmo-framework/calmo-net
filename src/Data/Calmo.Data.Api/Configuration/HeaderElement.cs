using System.Configuration;

namespace Calmo.Data.Api.Configuration
{
    public class HeaderElement : ConfigurationElement
    {
        public const string SectionPath = "coreSettings/apiSettings/headers/add";

        [ConfigurationProperty("name", IsRequired = true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}