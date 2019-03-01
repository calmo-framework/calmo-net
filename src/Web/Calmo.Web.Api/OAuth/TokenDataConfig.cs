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

	public class MessagesConfig
	{
		internal Dictionary<string, string> CustomMessages = new Dictionary<string, string>
		{
			{BasicAuthResult.Success.ToString(), "Access granted."},
			{BasicAuthResult.Unauthorized.ToString(), "Username/password is invalid or your account is de-activated."},
			{BasicAuthResult.PasswordExpired.ToString(), "Password expired."},
			{BasicAuthResult.UserExpired.ToString(), "Username/password is invalid or your account is de-activated."},
			{BasicAuthResult.UserOrPasswordEmpty.ToString(), "Username and password cannot be empty."}
		};

		public MessagesConfig Set(BasicAuthResult basicAuthResult, string message)
		{
			this.CustomMessages[basicAuthResult.ToString()] = message;

			return this;
		}

		public MessagesConfig Set(string authResult, string message)
		{
			if (CustomMessages.ContainsKey(authResult))
				this.CustomMessages[authResult] = message;
			else
				this.CustomMessages.Add(authResult, message);


			return this;
		}
	}
}