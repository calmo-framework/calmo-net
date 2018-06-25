using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Calmo.Core.ExceptionHandling;
using Microsoft.Owin.Security.OAuth;

namespace Calmo.Web.Api.OAuth
{
    public class AuthorizationServerProvider<T> : OAuthAuthorizationServerProvider where T : ICustomAuthenticator
    {
        private readonly T _authenticator;
        private readonly AuthorizationServerProviderConfig<T> _config;

        public AuthorizationServerProvider(T authenticator)
        {
            this._authenticator = authenticator;
            this._config = new AuthorizationServerProviderConfig<T>();
        }

        public AuthorizationServerProviderConfig<T> TokenData()
        {
            return _config;
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            if (this._config._tokenDataProperties != null)
            {
                foreach (var property in this._config._tokenDataProperties)
                {
                    var propertyName = property.Name.ToCamelCaseWord();
                    var claim = context.Identity.Claims.FirstOrDefault(x => x.Type == propertyName);
                    context.AdditionalResponseParameters.Add(propertyName, String.IsNullOrEmpty(claim?.Value) ? String.Empty : claim.Value);
                }
            }

            return Task.FromResult<object>(null);
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Run(() =>
            {
                context.Validated();
            });
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

                if (String.IsNullOrWhiteSpace(context.UserName) || String.IsNullOrWhiteSpace(context.Password))
                {
                    context.SetError("invalid_grant", "Username and password cannot be empty.");
                    return;
                }

                var authorized = await this._authenticator.Authorize(context.UserName, context.Password);
                if (!authorized)
                {
                    context.SetError("invalid_grant", "Username/password is invalid or your account is de-activated.");
                    return;
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                if (this._config._tokenDataProperties != null)
                {
                    foreach (var property in this._config._tokenDataProperties)
                    {
                        var propertyValue = property.GetValue(_authenticator);
                        identity.AddClaim(new Claim(property.Name.ToCamelCaseWord(), propertyValue?.ToString() ?? string.Empty));
                    }
                }

                context.Validated(identity);
            }
            catch (Exception ex)
            {
                context.SetError("internal_server_error","Internal Server Error. Please contact the administrator.");
            }
        }
    }
}