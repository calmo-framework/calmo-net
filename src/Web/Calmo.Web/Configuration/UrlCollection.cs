using System;
using System.Configuration;
using System.Linq;
using Calmo.Core.Properties;

namespace Calmo.Web.Configuration
{
    public class UrlCollection : ConfigurationElementCollection
    {
        public const string SectionPath = "portalSettings/urlSettings/urls";

        protected override ConfigurationElement CreateNewElement()
        {
            return new UrlElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var urlElement = ((UrlElement)element);

            return String.Concat(urlElement.Name, urlElement.Path);
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

        public new string this[string name]
        {
            get
            {
                var url = this.Cast<UrlElement>().FirstOrDefault(u => u.Name.ToLower() == name.ToLower());

                if (url == null)
                    throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.KeyNotFound, name));

                return url.Path;
            }
        }
    }
}