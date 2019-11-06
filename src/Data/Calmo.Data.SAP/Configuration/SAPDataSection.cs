using System.Configuration;

namespace Calmo.Data.SAP.Configuration
{
    public class SAPDataSection : ConfigurationSection
    {
        internal const string SectionName = "core.Settings/sap.data.Settings";

        [ConfigurationProperty("applicationServer", IsRequired = true)]
        public string ApplicationServer
        {
            get => (string)this["applicationServer"];
            set => this["applicationServer"] = value;
        }

        [ConfigurationProperty("systemId", IsRequired = true)]
        public string SystemId
        {
            get => (string)this["systemId"];
            set => this["systemId"] = value;
        }

        [ConfigurationProperty("client", IsRequired = true)]
        public string Client
        {
            get => (string)this["client"];
            set => this["client"] = value;
        }

        [ConfigurationProperty("user", IsRequired = true)]
        public string User
        {
            get => (string)this["user"];
            set => this["user"] = value;
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get => (string)this["password"];
            set => this["password"] = value;
        }

        [ConfigurationProperty("language", IsRequired = true)]
        public string Language
        {
            get => (string)this["language"];
            set => this["language"] = value;
        }

        [ConfigurationProperty("traceLevel", IsRequired = true)]
        public string TraceLevel
        {
            get => (string)this["traceLevel"];
            set => this["traceLevel"] = value;
        }

        [ConfigurationProperty("systemNumber", IsRequired = true)]
        public string SystemNumber
        {
            get => (string)this["systemNumber"];
            set => this["systemNumber"] = value;
        }
    }
}