using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
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

        public AuthorizationServerProviderConfig<T> Configure()
        {
            return _config;
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            if (this._config.TokenDataConfig.Properties != null)
            {
                foreach (var property in this._config.TokenDataConfig.Properties)
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
            
            await Task.Run(() => { context.Validated(); });
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", !this._config.AllowedOrigins.HasItems() ? new[] { "*" } : this._config.AllowedOrigins);

                if (this._config.AllowCredentials)
                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });

                var username = this._config.IsWindowsAuthentication
                    ? HttpContext.Current.Request.ServerVariables["LOGON_USER"]
                    : context.UserName;
                var password = context.Password;

                if (String.IsNullOrWhiteSpace(username))
                {
                    var message = this._config.MessagesConfig.CustomMessages[AuthResult.UserOrPasswordEmpty];
                    context.SetError("invalid_grant", message);
                    return;
                }

                if (!this._config.IsWindowsAuthentication && String.IsNullOrWhiteSpace(password))
                {
                    var message = this._config.MessagesConfig.CustomMessages[AuthResult.UserOrPasswordEmpty];
                    context.SetError("invalid_grant", message);
                    return;
                }

                var authenticationArgs = new AuthenticationArgs { Username = username, Password = password, Context = context };
                var authResult = await this._authenticator.Authenticate(authenticationArgs);
                context = authenticationArgs.Context;

                if (authResult.In(AuthResult.Unauthorized, AuthResult.UserExpired, AuthResult.PasswordExpired))
                {
                    var message = this._config.MessagesConfig.CustomMessages[authResult];
                    context.SetError("invalid_grant", message);
                    return;
                }

	            if (authResult != AuthResult.Success)
	            {
		            var message = this._config.MessagesConfig.CustomMessages[authResult];
		            context.SetError("invalid_grant", message);
		            return;
	            }

				var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                if (this._config.TokenDataConfig.Properties != null)
                {
                    foreach (var property in this._config.TokenDataConfig.Properties)
                    {
                        var propertyValue = property.GetValue(_authenticator);

                        var claimName = property.Name.ToCamelCaseWord();
                        var claimValue = propertyValue?.ToString() ?? string.Empty;

                        var claim = new Claim(claimName, claimValue);
                        identity.AddClaim(claim);
                    }
                }

                var authorizationArgs = new AuthorizationArgs { Username = username, Context = context };
                var userClaims = await this._authenticator.Authorize(authorizationArgs);
                context = authorizationArgs.Context;

                if (userClaims.HasItems())
                {
                    foreach (var claim in userClaims)
                        identity.AddClaim(new Claim(claim.Type, claim.Value));
                }

                context.Validated(identity);
            }
            catch (Exception ex)
            {
                this._config.OnGrantAccessExceptionEvent(new OnGrantAccessExceptionEventArgs(ex));

                context.SetError("internal_server_error","Internal Server Error. Please contact the administrator.");
            }
        }
    }
}