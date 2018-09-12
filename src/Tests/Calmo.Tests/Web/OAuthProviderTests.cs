using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calmo.Web.Api.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calmo.Tests.Web
{
    [TestClass]
    public class OAuthProviderTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var provider = new AuthorizationServerProvider<OAuthAuthenticator>(new OAuthAuthenticator());
            provider.Configure()
                .AccessControlAllowOrigin("*")
                .AccessControlAllowCredentials(true)
                .TokenData(c => c.Map(p => p.Id)
                                 .Map(p => p.Name)
                                 .Map(p => p.IsAdmin))
                .OnError(args => { });
        }

        [TestMethod]
        public void TestMethod2()
        {
            var controller = new TestController();
            var value = controller.GetTokenData();
        }
    }

    public class OAuthAuthenticator : ICustomAuthenticator
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        
        public Task<bool> Authenticate(AuthenticationArgs args)
        {
            return Task.Run(() => true);
        }

        public Task<IEnumerable<ClaimData>> Authorize(AuthorizationArgs args)
        {
            return Task.Run(() => Enumerable.Empty<ClaimData>());
        }
    }

    public class TestController : OAuthController<OAuthAuthenticator>
    {

    }
}
