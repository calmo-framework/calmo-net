using System.Configuration;

namespace Calmo.Web.Configuration
{
    public class LuceneSection : ConfigurationSection
    {
        internal const string SectionName = "portalSettings/luceneSettings";

        [ConfigurationProperty("indexPath", IsRequired = true)]
        public string IndexPath
        {
            get { return (string)this["indexPath"]; }
            set { this["indexPath"] = value; }
        }
    }
}
