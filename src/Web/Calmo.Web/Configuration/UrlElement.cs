using System.Configuration;

namespace Calmo.Web.Configuration
{
    public class UrlElement : ConfigurationElement
    {
        public const string SectionPath = "portalSettings/urlSettings/urls/add";

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }
    }
}