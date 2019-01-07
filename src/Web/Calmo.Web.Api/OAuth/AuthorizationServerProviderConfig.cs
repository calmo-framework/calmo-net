using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Calmo.Web.Api.OAuth
{
    public delegate void OnGrantAccessExceptionEventHandler(OnGrantAccessExceptionEventArgs e);

    public class AuthorizationServerProviderConfig<T>
    {
        internal TokenDataConfig<T> TokenDataConfig { get; private set; }
        internal MessagesConfig MessagesConfig { get; private set; }
        internal string[] AllowedOrigins { get; private set; }
        internal bool AllowCredentials { get; private set; }
        internal bool IsWindowsAuthentication { get; private set; }

        private event OnGrantAccessExceptionEventHandler _grantAccessExceptionEvent;
        internal void OnGrantAccessExceptionEvent(OnGrantAccessExceptionEventArgs e)
        {
            _grantAccessExceptionEvent?.Invoke(e);
        }

        public AuthorizationServerProviderConfig<T> OnError(OnGrantAccessExceptionEventHandler onErrorEventHandler)
        {
            _grantAccessExceptionEvent += onErrorEventHandler;

            return this;
        }

        public AuthorizationServerProviderConfig<T> TokenData(Func<TokenDataConfig<T>, TokenDataConfig<T>> func)
        {
            this.TokenDataConfig = func.Invoke(this.TokenDataConfig ?? new TokenDataConfig<T>());

            return this;
        }

        public AuthorizationServerProviderConfig<T> Messages(Func<MessagesConfig, MessagesConfig> func)
        {
            this.MessagesConfig = func.Invoke(this.MessagesConfig ?? new MessagesConfig());

            return this;
        }

        public AuthorizationServerProviderConfig<T> AccessControlAllowOrigin(params string[] value)
        {
            this.AllowedOrigins = value;

            return this;
        }

        public AuthorizationServerProviderConfig<T> AccessControlAllowCredentials(bool allowCredentials)
        {
            this.AllowCredentials = allowCredentials;

            return this;
        }

        public AuthorizationServerProviderConfig<T> UseWindowsAuthentication(bool useWindowsAuthentication = true)
        {
            this.IsWindowsAuthentication = useWindowsAuthentication;

            return this;
        }
    }
}