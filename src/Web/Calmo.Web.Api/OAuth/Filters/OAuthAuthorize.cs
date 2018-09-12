using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace System.Web.Http.Filters
{
    public class OAuthAuthorize : AuthorizationFilterAttribute
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var authorized = actionContext.RequestContext.Principal is ClaimsPrincipal principal
                            && principal.Identity.IsAuthenticated
                            && principal.HasClaim(x => (string.IsNullOrWhiteSpace(this.ClaimType) || x.Type == this.ClaimType)
                                                        && (string.IsNullOrWhiteSpace(this.ClaimValue) || x.Value == this.ClaimValue));

            if (authorized)
                return Task.FromResult<object>(null);

            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            return Task.FromResult<object>(null);
        }
    }
}
