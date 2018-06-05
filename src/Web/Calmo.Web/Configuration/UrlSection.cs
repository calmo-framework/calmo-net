using System.Configuration;

namespace Calmo.Web.Configuration
{
    public class UrlsSection : ConfigurationSection
    {
        internal const string SectionName = "portalSettings/urlSettings";

        [ConfigurationProperty("urls", IsDefaultCollection = true, IsRequired = true)]
        public UrlCollection List
        {
            get { return (UrlCollection)base["urls"]; }
        }
    }
}