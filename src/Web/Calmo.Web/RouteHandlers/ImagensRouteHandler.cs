using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Calmo.Web.RouteHandlers
{
    public class ImagensRouteHandler : MvcRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var action = ((string)requestContext.RouteData.Values["action"]).ClearIfNullOrWhiteSpace().ToLower();

            if (action == "obterboxquilometragem")
                TratarValoresImagemBoxQuilometragem(requestContext);

            return base.GetHttpHandler(requestContext);
        }

        private static void TratarValoresImagemBoxQuilometragem(RequestContext requestContext)
        {
            var value = ((string)requestContext.RouteData.Values["valor"]).ClearIfNullOrWhiteSpace().ToLower();

            value = value.Replace("_", ".");

            decimal valor = -1;
            decimal.TryParse(value, NumberStyles.Any, new CultureInfo("en-US"), out valor);

            requestContext.RouteData.Values["valor"] = valor == -1M ? 0M : valor;
        }
    }
}
