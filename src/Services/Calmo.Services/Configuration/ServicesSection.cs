using System.Configuration;

namespace Calmo.Services.Configuration
{
    public class ServicesSection : ConfigurationSection
    {
        internal const string SectionName = "core.Settings/services.Settings";

        [ConfigurationProperty("impersonateByWindowsAuthentication", IsRequired = false)]
        public bool ImpersonateByWindowsAuthentication
        {
            get { return (bool)this["impersonateByWindowsAuthentication"]; }
            set { this["impersonateByWindowsAuthentication"] = value; }
        }
    }
}
