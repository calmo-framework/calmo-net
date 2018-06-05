using System.Configuration;

namespace Calmo.Web.Configuration
{
    public class GoogleSection : ConfigurationSection
    {
        internal const string SectionName = "portalSettings/googleSettings";

        [ConfigurationProperty("apiKey", IsRequired = true)]
        public string ApiKey
        {
            get { return (string)this["apiKey"]; }
            set { this["apiKey"] = value; }
        }

        [ConfigurationProperty("dailyLimit", IsRequired = false)]
        public int DailyLimit
        {
            get { return (int)this["dailyLimit"]; }
            set { this["dailyLimit"] = value; }
        }

        [ConfigurationProperty("analyticsAccount", IsRequired = false)]
        public string AnalyticsAccount
        {
            get { return (string)this["analyticsAccount"]; }
            set { this["analyticsAccount"] = value; }
        }

        [ConfigurationProperty("clientId", IsRequired = false)]
        public string ClientId
        {
            get { return (string)this["clientId"]; }
            set { this["clientId"] = value; }
        }

        [ConfigurationProperty("cryptoKey", IsRequired = false)]
        public string CryptoKey
        {
            get { return (string)this["cryptoKey"]; }
            set { this["cryptoKey"] = value; }
        }
    }
}
