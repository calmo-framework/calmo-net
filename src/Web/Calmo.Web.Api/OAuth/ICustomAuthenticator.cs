using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calmo.Web.Api.OAuth
{
    public interface ICustomAuthenticator
    {
        Task<string> Authenticate(AuthenticationArgs args);
        Task<IEnumerable<ClaimData>> Authorize(AuthorizationArgs args);
    }

    public enum BasicAuthResult
    {
        Success,
        Unauthorized,
        PasswordExpired,
        UserExpired,
        UserOrPasswordEmpty
    }
}