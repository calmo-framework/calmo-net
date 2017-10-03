using System;
using System.Configuration;
using System.Linq;
using Calmo.Core.Properties;

namespace Calmo.Data.Api.Configuration
{
    public class HeaderCollection : ConfigurationElementCollection
    {
        public const string SectionPath = "coreSettings/apiSettings/headers";

        protected override ConfigurationElement CreateNewElement()
        {
            return new HeaderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var urlElement = ((HeaderElement)element);

            return String.Concat(urlElement.Key, urlElement.Value);
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get
            {
                return "add";
            }
        }

        public new HeaderElement this[string key]
        {
            get
            {
                var url = this.Cast<HeaderElement>().FirstOrDefault(u => u.Key.ToLower() == key.ToLower());

                if (url == null)
                    throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.KeyNotFound, key));

                return url;
            }
        }
    }
}