using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calmo.Web.Api.OAuth
{
    public interface ICustomAuthenticator
    {
        Task<bool> Authenticate(AuthenticationArgs args);
        Task<IEnumerable<ClaimData>> Authorize(AuthorizationArgs args);
    }
}
