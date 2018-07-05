using System.Configuration;
using System.DirectoryServices;
using System.Reflection;
using System.Web.Security;
using Calmo.Core;

namespace Calmo.Data.ActiveDirectory
{
    internal sealed class ActiveDirectoryProvider : ActiveDirectoryMembershipProvider
    {
        public bool ValidateUser(string login, out bool isLockedOut)
        {
            var user = GetUser(login, false);
            var valid = (user != null && user.IsApproved);
            isLockedOut = (user != null && user.IsLockedOut);

            return valid;
        }

        public bool AuthenticateUser(string login, string password, out bool isLockedOut, out bool isValid)
        {
            isLockedOut = false;
            isValid = true;

            var adUser = GetUser(login, true);
            if (adUser == null) return false;

            isValid = false;
            isLockedOut = adUser.IsLockedOut;
            return ValidateUser(login, password);
        }

        public bool ChangeUserPassword(string login, string password, string newPassword)
        {
            var adUser = GetUser(login, true);

            if (adUser == null) return false;

            try
            {
                return ChangePassword(login, password, newPassword);
            }
            catch
            {
                return false;
            }
        }

        public bool ChangeUserPassword(string login, string newPassword)
        {
            var adConfig = CustomConfiguration.Settings.ActiveDirectoryData();
            var connectionString = ConfigurationManager.ConnectionStrings[adConfig.ConnectionStringName].ConnectionString;
            var directoryEntry = new DirectoryEntry(connectionString, adConfig.UserName, adConfig.Password);

            var searcher = new DirectorySearcher(directoryEntry, $"sAMAccountName={login}") { SearchScope = SearchScope.Subtree };
            var adUser = searcher.FindOne();

            if (adUser == null) return false;

            var userPath = adUser.Properties["adspath"][0].ToString();
            directoryEntry = new DirectoryEntry(userPath, adConfig.UserName, adConfig.Password);

            try
            {
                directoryEntry.Invoke("SetPassword", new object[] { newPassword });
                directoryEntry.Properties["LockOutTime"].Value = 0;
            }
            catch (TargetInvocationException erro)
            {
                if (erro.InnerException != null)
                    throw erro.InnerException;

                throw;
            }

            directoryEntry.Close();
            return true;
        }
    }
}