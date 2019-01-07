using System.Configuration;

namespace Calmo.Data.Configuration
{
#if !NETCOREAPP
    public class DataSection : ConfigurationSection
#else
    public class DataSection
#endif
    {
#if !NETCOREAPP
        internal const string SectionName = "core.Settings/data.Settings";
#else
        internal const string SectionName = "calmoData";
#endif

#if !NETCOREAPP
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
#else
        public string DefaultConnectionString { get; set; }
        public int PageSize { get; set; }
#endif
    }
}