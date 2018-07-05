using System.Configuration;

namespace Calmo.Data.ActiveDirectory.Configuration
{
    public class ActiveDirectoryDataSection : ConfigurationSection
    {
        internal const string SectionName = "core.Settings/activeDirectory.data.Settings";

        [ConfigurationProperty("connectionStringName", IsRequired = true)]
        public string ConnectionStringName
        {
            get => (string)this["connectionStringName"];
            set => this["connectionStringName"] = value;
        }

        [ConfigurationProperty("userName", IsRequired = false)]
        public string UserName
        {
            get => (string)this["userName"];
            set => this["userName"] = value;
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get => (string)this["password"];
            set => this["password"] = value;
        }
    }
}