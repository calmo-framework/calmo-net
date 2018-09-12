using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calmo.Web.Api.OAuth
{
    public interface ICustomAuthenticator
    {
        Task<bool> Authenticate(string username, string password);
        Task<IEnumerable<ClaimData>> Authorize(string username);
    }
}
