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
		internal Dictionary<AuthResult, string> CustomMessages = new Dictionary<AuthResult, string>
		{
			{AuthResult.Success, "Access granted."},
			{AuthResult.Unauthorized, "Username/password is invalid or your account is de-activated."},
			{AuthResult.PasswordExpired, "Password expired."},
			{AuthResult.UserExpired, "Username/password is invalid or your account is de-activated."},
			{AuthResult.UserOrPasswordEmpty, "Username and password cannot be empty."},
			{AuthResult.NotRegistered, "User not registered."}
		};

		public MessagesConfig Set(AuthResult authResult, string message)
		{
			this.CustomMessages[authResult] = message;

			return this;
		}
	}
}