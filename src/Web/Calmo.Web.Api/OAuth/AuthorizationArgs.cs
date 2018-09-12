using Microsoft.Owin.Security.OAuth;

namespace Calmo.Web.Api.OAuth
{
    public class AuthorizationArgs
    {
        public string Username { get; set; }
        public OAuthGrantResourceOwnerCredentialsContext Context { get; set; }
    }
}
