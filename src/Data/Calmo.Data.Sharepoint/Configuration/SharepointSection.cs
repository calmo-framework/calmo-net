using System;
using System.Configuration;
using System.Net;
using System.Security.Cryptography;

namespace Calmo.Data.Sharepoint.Configuration
{
    public class SharepointSection : ConfigurationSection
    {
        internal const string SectionName = "core.Settings/sharepoint.Settings";

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)this["url"]; }
            set { this["url"] = value; }
        }

        //[ConfigurationProperty("encrypted", IsRequired = false)]
        //public bool Encrypted
        //{
        //    get { return (bool)this["encrypted"]; }
        //    set { this["encrypted"] = value; }
        //}

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

        public ICredentials GetServiceCredentials()
        {
            if (String.IsNullOrWhiteSpace(this.UserName) || String.IsNullOrWhiteSpace(this.Password))
                return null;

            ICredentials credentials;

            var username = this.UserName;
            var password = this.Password;

            //if (this.Encrypted)
            //{
            //    username = this.UserName.Decrypt();
            //    password = this.Password.Decrypt();
            //}

            if (username.Contains("\\"))
            {
                var userParts = username.Split('\\');

                credentials = new NetworkCredential(userParts[1], password, userParts[0]);
            }
            else
            {
                credentials = new NetworkCredential(username, password);
            }

            return credentials;
        }
    }
}
