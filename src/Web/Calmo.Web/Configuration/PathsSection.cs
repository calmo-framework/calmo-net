using System.Configuration;

namespace Calmo.Web.Configuration
{
    public class PathsSection : ConfigurationSection
    {
        internal const string SectionName = "portalSettings/pathSettings";

        [ConfigurationProperty("paths", IsDefaultCollection = true, IsRequired = true)]
        public PathCollection List
        {
            get { return (PathCollection)base["paths"]; }
        }
    }
}