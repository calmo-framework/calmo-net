using System.Configuration;

namespace Calmo.Data.ActiveDirectory.Configuration
{
    public class ActiveDirectoryDataSection : ConfigurationSection
    {
        internal const string SectionName = "coreSettings/activeDirectoryDataSettings";

        [ConfigurationProperty("connectionStringName", IsRequired = true)]
        public string ConnectionStringName
        {
            get { return (string)this["connectionStringName"]; }
            set { this["connectionStringName"] = value; }
        }

        [ConfigurationProperty("userName", IsRequired = false)]
        public string UserName
        {
            get { return (string)this["userName"]; }
            set { this["userName"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }
    }
}