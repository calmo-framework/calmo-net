using System.Collections.Specialized;
using System.Configuration;
using System.Web.Routing;
using Calmo.Core;
using Calmo.Web;
using Calmo.Web.Properties;

namespace System.Web.Mvc
{
    public static class UrlHelperExtensions
    {
        private const string CDNUrlKey = "CDN";
        private const string CDNHttpsUrlKey = "CDNHttps";

        public static RouteValueDictionary ToRouteValues(this NameValueCollection queryString)
        {
            if (queryString==null || queryString.HasKeys() == false) return new RouteValueDictionary();

            var routeValues = new RouteValueDictionary();
            foreach (string key in queryString.AllKeys)
                routeValues.Add(key, queryString[key]);

            return routeValues;
        }

        public static string CDNContent(this UrlHelper helper, string contentPath)
        {
            if (helper.RequestContext.HttpContext.Request.IsSecureConnection)
            {
                if (!String.IsNullOrWhiteSpace(CustomConfiguration.Settings.Urls().List[CDNHttpsUrlKey]))
                    return ContentPath(CDNHttpsUrlKey, contentPath);
            }

            return ContentPath(CDNUrlKey, contentPath);
        }

        private static string ContentPath(string urlKey, string path)
        {
            if (String.IsNullOrWhiteSpace(CustomConfiguration.Settings.Urls().List[urlKey]))
                throw new ConfigurationErrorsException(String.Format(Messages.CDNConfigurationNotFound, urlKey));

            var url = CustomConfiguration.Settings.Urls().List[urlKey];

            path = path ?? string.Empty;
            path = path.Replace("~/", "/");

            return String.Concat(url.TrimEnd('/'), "/", path.TrimStart('/'));
        }

        public static string GetVersion(this UrlHelper helper)
        {
            var assembly    = Reflection.Assembly.GetExecutingAssembly();
            var fileInfo    = new IO.FileInfo(assembly.Location);

            return fileInfo.LastWriteTime.ToString("yyyyMMddhhmmss");
        }
    }
}