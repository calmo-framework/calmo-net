using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Calmo.Web.Api.OAuth
{
    public class AuthorizationServerProviderConfig<T>
    {
        internal List<PropertyInfo> _tokenDataProperties;

        public AuthorizationServerProviderConfig<T> Use<TProperty>(Expression<Func<T, TProperty>> property)
        {
            if (_tokenDataProperties == null)
                _tokenDataProperties = new List<PropertyInfo>();

            var propertyInfo = this.GetPropertyInfo(property);

            if (_tokenDataProperties.All(p => p.Name != propertyInfo.Name))
                _tokenDataProperties.Add(this.GetPropertyInfo(property));

            return this;
        }

        private PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");

            return propInfo;
        }
    }
}