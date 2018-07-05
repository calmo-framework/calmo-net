using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web.Security;
using Calmo.Core;

namespace Calmo.Data.ActiveDirectory
{
    public class ActiveDirectoryDataAccessConfig
    {
        internal const string CalmoProviderName = "CalmoActiveDirectoryProvider";

        public ActiveDirectoryDataAccessQuery Users()
        {
            return new ActiveDirectoryDataAccessQuery(ActiveDirectoryObjectType.User);
        }

        public ActiveDirectoryProviderDataAccess User()
        {
            var providers = Membership.Providers.ToList();
            if (providers.All(p => p.Name != CalmoProviderName))
                AddProvider();

            return new ActiveDirectoryProviderDataAccess();
        }

        private void AddProvider()
        {
            var adConfig = CustomConfiguration.Settings.ActiveDirectoryData();
            var provider = new ActiveDirectoryProvider();
            var config = new NameValueCollection
            {
                ["name"] = CalmoProviderName,
                ["connectionStringName"] = adConfig.ConnectionStringName,
                ["connectionUsername"] = adConfig.UserName,
                ["connectionPassword"] = adConfig.Password,
                ["attributeMapUsername"] = "sAMAccountName"
            };
            provider.Initialize(CalmoProviderName, config);
            
            var membershipProviders = new MembershipProviderCollection {provider};
            membershipProviders.SetReadOnly();

            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
            var membershipType = typeof(Membership);
            membershipType.GetField("s_Initialized", bindingFlags)?.SetValue(null, true);
            membershipType.GetField("s_InitializeException", bindingFlags)?.SetValue(null, null);
            membershipType.GetField("s_Provider", bindingFlags)?.SetValue(null, provider);
            membershipType.GetField("s_Providers", bindingFlags)?.SetValue(null, membershipProviders);
        }
    }
}