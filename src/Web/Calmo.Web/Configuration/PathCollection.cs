using System;
using System.Configuration;
using System.Linq;
using Calmo.Core.Properties;

namespace Calmo.Web.Configuration
{
    public class PathCollection : ConfigurationElementCollection
    {
        public const string SectionPath = "portalSettings/pathSettings/paths";

        protected override ConfigurationElement CreateNewElement()
        {
            return new PathElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var urlElement = ((PathElement)element);

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
                var url = this.Cast<PathElement>().FirstOrDefault(u => u.Name.ToLower() == name.ToLower());

                if (url == null)
                    throw new ConfigurationErrorsException(String.Format(ConfigurationMessages.KeyNotFound, name));

                return url.Path;
            }
        }
    }
}