using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Calmo.Data.SAP
{
    public class FieldDataConfig<T>
    {
        internal Dictionary<PropertyInfo, string> Properties;

        public FieldDataConfig<T> Map<TProperty>(Expression<Func<T, TProperty>> property, string fieldName)
        {
            this.SetMap(property, fieldName);

            return this;
        }
        public FieldDataConfig<T> Map<TProperty>(Expression<Func<T, TProperty>> property)
        {
            this.SetMap(property, property.GetPropertyInfo().Name);

            return this;
        }

        private void SetMap<TProperty>(Expression<Func<T, TProperty>> property, string fieldName)
        {
            if (this.Properties == null)
                this.Properties = new Dictionary<PropertyInfo, string>();

            var propertyInfo = property.GetPropertyInfo();

            if (this.Properties.Keys.All(p => p.Name != propertyInfo.Name))
                this.Properties.Add(propertyInfo, fieldName);
        }
    }
}
