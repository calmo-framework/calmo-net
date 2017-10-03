using System;
using System.Web;

namespace Calmo.Web.Helpers
{
    public static class CacheHelper
    {
        public static void Add<T>(T o, string key, int seconds) where T : class
        {
            HttpContext.Current.Cache.Insert(
                key,
                o,
                null,
                DateTime.Now.AddSeconds(seconds),
                System.Web.Caching.Cache.NoSlidingExpiration);
        }

        public static void Clear(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        public static bool Exists(string key)
        {
            return HttpContext.Current.Cache[key] != null;
        }

        public static T Get<T>(string key) where T : class
        {
            try
            {
                return (T)HttpContext.Current.Cache[key];
            }
            catch
            {
                return null;
            }
        }
    }
}