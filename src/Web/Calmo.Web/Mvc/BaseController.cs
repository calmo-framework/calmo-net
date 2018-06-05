using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Calmo.Web.Mvc
{
    public class BaseController : Controller
    {
        private const string SiteMapChacheKey = "Calmo.Web.SiteMap";

        protected JsonResult GetJson(object data)
        {
            var jsonResult = Json(data, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.SetPageTitle(filterContext);

            base.OnActionExecuting(filterContext);
        }

        protected virtual void SetPageTitle(ActionExecutingContext filterContext)
        {
            this.SetPageTitle(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);
        }

        protected virtual void SetPageTitle(string controllerName, string actionName)
        {
            var currentNode = this.GetCurrentNode(controllerName, actionName);

            if (currentNode == null)
                return;

            var title = currentNode.Attribute("title").GetValueOrDefault(p => p.Value);

            this.SetPageTitle(title);
        }

        protected void SetPageTitle(string title)
        {
            ViewBag.Title = title;
        }

        protected XElement GetCurrentNode(ActionDescriptor actionDescriptor)
        {
            return this.GetCurrentNode(actionDescriptor.ControllerDescriptor.ControllerName, actionDescriptor.ActionName);
        }

        protected XElement GetCurrentNode(string controllerName, string actionName)
        {
            actionName = actionName.ToLower();
            controllerName = controllerName.ToLower();
            var elements = this.GetSiteMap();

            if (!elements.HasItems()) return null;
            return elements.FirstOrDefault(d => (d.Attribute("controller") != null && d.Attribute("controller").Value.ToLower() == controllerName)
                                             && (d.Attribute("action") != null && d.Attribute("action").Value.ToLower() == actionName));
        }

        private IEnumerable<XElement> GetSiteMap()
        {
            var cache = HttpContext == null ? System.Web.HttpContext.Current.Cache : HttpContext.Cache;

            if (cache[SiteMapChacheKey] == null)
            {
                var siteMapPath = Server.MapPath("~/Web.sitemap");
                if (!System.IO.File.Exists(siteMapPath))
                    siteMapPath = Server.MapPath("~/Mvc.sitemap");

                if (System.IO.File.Exists(siteMapPath))
                    cache[SiteMapChacheKey] = XDocument.Load(siteMapPath).Elements().First().Descendants();
                else
                    return null;
            }

            return cache[SiteMapChacheKey] as IEnumerable<XElement>;
        }
    }
}