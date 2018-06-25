using System.Threading.Tasks;

namespace Calmo.Web.Api.OAuth
{
    public interface ICustomAuthenticator
    {
        Task<bool> Authorize(string username, string password);
    }
}
