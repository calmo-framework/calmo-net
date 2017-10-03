using System;
using System.Globalization;
using System.Threading;

namespace Calmo.Web.Mvc
{
    public class GlobalizedController : BaseController
    {
        protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
        {
            try
            {
                var culture = new CultureInfo(requestContext.HttpContext.Request.UserLanguages[0]);
                
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
            catch (Exception) {}

            return base.BeginExecute(requestContext, callback, state);
        }
    }
}