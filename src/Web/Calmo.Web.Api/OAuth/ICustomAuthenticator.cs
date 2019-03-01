using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calmo.Web.Api.OAuth
{
    public interface ICustomAuthenticator
    {
        Task<AuthResult> Authenticate(AuthenticationArgs args);
        Task<IEnumerable<ClaimData>> Authorize(AuthorizationArgs args);
    }

    public enum AuthResult
    {
        Success,
        Unauthorized,
        PasswordExpired,
        UserExpired,
        UserOrPasswordEmpty,
		NotRegistered
    }
}