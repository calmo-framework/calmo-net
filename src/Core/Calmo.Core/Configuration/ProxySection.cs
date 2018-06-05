using System;
using System.Configuration;
using System.Net;

namespace Calmo.Core.Configuration
{
    public class ProxySection : ConfigurationSection
    {
        public const string SectionName = "core.Settings/proxy.Settings";

        [ConfigurationProperty("enabled", IsRequired = true)]
        public bool Enabled
        {
            get { return (bool)this["enabled"]; }
            set { this["enabled"] = value; }
        }

        [ConfigurationProperty("address")]
        public string Address
        {
            get { return (string)this["address"]; }
            set { this["address"] = value; }
        }

        [ConfigurationProperty("user")]
        public string User
        {
            get { return (string)this["user"]; }
            set { this["user"] = value; }
        }

        [ConfigurationProperty("password")]
        public string Password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }

        public IWebProxy GetProxy()
        {
            if (!this.Enabled)
                return null;

            IWebProxy proxy;

            if (String.IsNullOrWhiteSpace(this.Address))
                proxy = WebProxy.GetDefaultProxy();
            else
                proxy = new WebProxy(this.Address);

            if (!String.IsNullOrWhiteSpace(this.User) && !String.IsNullOrWhiteSpace(this.Password))
            {
                if (this.User.Contains("\\"))
                {
                    var userParts = this.User.Split('\\');

                    proxy.Credentials = new NetworkCredential(userParts[1], this.Password, userParts[0]);
                }
                else
                {
                    proxy.Credentials = new NetworkCredential(this.User, this.Password);
                }
            }

            return proxy;
        }
    }
}