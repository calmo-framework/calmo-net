using System.Configuration;

namespace Calmo.Data.Configuration
{
    public class DataSection : ConfigurationSection
    {
        internal const string SectionName = "core.Settings/data.Settings";

        [ConfigurationProperty("defaultConnectionString", IsRequired = true)]
        public string DefaultConnectionString
        {
            get { return (string)this["defaultConnectionString"]; }
            set { this["defaultConnectionString"] = value; }
        }

        [ConfigurationProperty("pageSize")]
        public int PageSize
        {
            get { return (int)this["pageSize"]; }
            set { this["pageSize"] = value; }
        }
    }
}