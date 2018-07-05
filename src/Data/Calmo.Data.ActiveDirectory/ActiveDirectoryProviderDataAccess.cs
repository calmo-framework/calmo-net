using System.Configuration;
using System.Web.Security;

namespace Calmo.Data.ActiveDirectory
{
    public class ActiveDirectoryProviderDataAccess
    {
        private ActiveDirectoryProvider GetProvider()
        {
            if (!(Membership.Provider is ActiveDirectoryProvider provider))
                throw new ConfigurationErrorsException();

            return provider;
        }

        public bool Validate(string login, out bool isLockedOut)
        {
            var provider = this.GetProvider();
            return provider.ValidateUser(login, out isLockedOut);
        }

        public bool Authenticate(string login, string password)
        {
            return this.Authenticate(login, password, out var _, out var _);
        }

        public bool Authenticate(string login, string password, out bool isLockedOut, out bool isValid)
        {
            var provider = this.GetProvider();
            return provider.AuthenticateUser(login, password, out isLockedOut, out isValid);
        }

        public bool ChangePassword(string login, string password, string newPassword)
        {
            var provider = this.GetProvider();
            return provider.ChangeUserPassword(login, password, newPassword);
        }

        public bool ChangePassword(string login, string newPassword)
        {
            var provider = this.GetProvider();
            return provider.ChangeUserPassword(login, newPassword);
        }
    }
}
