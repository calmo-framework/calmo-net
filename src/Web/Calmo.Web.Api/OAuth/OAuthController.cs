using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;

namespace Calmo.Web.Api.OAuth
{
    public class OAuthController<TAuthenticator> : ApiController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        private string GetClaimValue(string key)
        {
            var principal = this.RequestContext.Principal as ClaimsPrincipal;

            return principal?.Claims.FirstOrDefault(c => c.Type == key)?.Value;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public TAuthenticator GetTokenData()
        {
            var type = typeof(TAuthenticator);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var returnValue = Activator.CreateInstance<TAuthenticator>();

            foreach (var property in properties)
            {
                var value = this.GetClaimValue(property.Name.ToCamelCaseWord());
                if (!String.IsNullOrWhiteSpace(value))
                    returnValue.SetPropertyValue(property.Name, value.To(property.PropertyType));
            }

            return returnValue;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public TProperty GetTokenData<TProperty>(Expression<Func<TAuthenticator, TProperty>> property)
        {
            var propertyInfo = property.GetPropertyInfo();

            var value = this.GetClaimValue(propertyInfo.Name.ToCamelCaseWord());

            if (String.IsNullOrWhiteSpace(value))
                return default(TProperty);

            return value.To<TProperty>();
        }
    }
}