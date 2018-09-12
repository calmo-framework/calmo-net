using Microsoft.Owin.Security.OAuth;

namespace Calmo.Web.Api.OAuth
{
    public class AuthenticationArgs
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public OAuthGrantResourceOwnerCredentialsContext Context { get; set; }
    }
}
