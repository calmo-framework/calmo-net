using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Calmo.Web.Api.OAuth
{
    public class TokenDataConfig<T>
    {
        internal List<PropertyInfo> Properties;

        public TokenDataConfig<T> Map<TProperty>(Expression<Func<T, TProperty>> property)
        {
            if (this.Properties == null)
                this.Properties = new List<PropertyInfo>();

            var propertyInfo = property.GetPropertyInfo();

            if (this.Properties.All(p => p.Name != propertyInfo.Name))
                this.Properties.Add(property.GetPropertyInfo());

            return this;
        }
    }
}