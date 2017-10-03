using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using Calmo.Core.ExceptionHandling;

namespace Calmo.Web.Session
{
    [Flags]
    public enum ManagedDataStorage
    {
        Session = 1,
        Cookie = 2
    }

    public class ManagedSessionData
    {
        private static readonly IDictionary<Type, DataContractJsonSerializer> CachedSerializers = new Dictionary<Type, DataContractJsonSerializer>();
        private Dictionary<string, string> _cookieValues;

        public ManagedSessionData(ManagedSession session, ManagedDataStorage dataStorage, string cookieName = null, bool sessionCookie = false)
        {
            Throw.IfArgumentNull(session, "session");

            if ((dataStorage & ManagedDataStorage.Cookie) == ManagedDataStorage.Cookie)
            {
                Throw.IfArgumentNullOrEmpty(cookieName, "cookieName");
                Throw.IfReferenceNull(session.Controller, "Controller");
            }

            _session = session;
            _cookieName = cookieName;
            _dataStorage = dataStorage;
            _sessionCookie = sessionCookie;
        }

        public void StoreCookieValues()
        {
            if (_cookieValues == null) return;

            var cookie = HttpContext.Current.Request.Cookies[CookieName] ?? new HttpCookie(CookieName);
            cookie.HttpOnly = true;
            cookie.Expires = this.SessionCookie ? DateTime.MinValue : DateTime.Now.AddMonths(1);

            foreach (var key in _cookieValues.Keys)
            {
                cookie[key] = HttpContext.Current.Server.UrlEncode(_cookieValues[key]);
            }

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private readonly ManagedSession _session;
        public ManagedSession Session
        {
            get { return _session; }
        }

        private readonly ManagedDataStorage _dataStorage;
        public ManagedDataStorage DataStorage
        {
            get { return _dataStorage; }
        }

        private readonly string _cookieName;
        public string CookieName
        {
            get { return _cookieName; }
        }

        private readonly bool _sessionCookie;
        public bool SessionCookie
        {
            get { return _sessionCookie; }
        }

        public void ClearCookie()
        {
            if ((DataStorage & ManagedDataStorage.Cookie) != ManagedDataStorage.Cookie)
                return;

            var cookie = HttpContext.Current.Request.Cookies[CookieName];

            if (cookie == null) return;

            HttpContext.Current.Response.Cookies.Remove(CookieName);
        }

        private HttpCookie ReadCookie()
        {
            foreach (string cookieName in HttpContext.Current.Response.Cookies.AllKeys)
            {
                if (cookieName == CookieName)
                    return HttpContext.Current.Response.Cookies[cookieName];
            }

            foreach (string cookieName in HttpContext.Current.Request.Cookies.AllKeys)
            {
                if (cookieName == CookieName)
                    return HttpContext.Current.Request.Cookies[cookieName];
            }

            return null;
        }

        public void TrySetMember<T>(string name, T value)
        {
            name = String.Concat(name, GetClientIdentificationHashCode());

            SetSessionValue(name, value);
        }

        private void SetSessionValue<T>(string key, T value)
        {
            Throw.IfArgumentNullOrEmpty(key, "key");

            if ((DataStorage & ManagedDataStorage.Session) == ManagedDataStorage.Session)
                SetSessionValueHandler(key, value);

            if ((DataStorage & ManagedDataStorage.Cookie) == ManagedDataStorage.Cookie)
                SetCookieValueHandler<T>(key, value);
        }

        public T TryGetMember<T>(string name)
        {
            name = String.Concat(name, GetClientIdentificationHashCode());

            return GetSessionValue<T>(name);
        }

        private T GetSessionValue<T>(string key)
        {
            Throw.IfArgumentNullOrEmpty(key, "key");

            object value = null;

            if ((DataStorage & ManagedDataStorage.Session) == ManagedDataStorage.Session)
                value = GetSessionValueHandler(key);

            if (value == null && ((DataStorage & ManagedDataStorage.Cookie) == ManagedDataStorage.Cookie))
            {
                value = GetCookieValueHandler<T>(key);

                if (value != null && ((DataStorage & ManagedDataStorage.Session) == ManagedDataStorage.Session))
                    SetSessionValueHandler(key, value);
            }

            return (T)value;
        }

        private object GetSessionValueHandler(string key)
        {
            return HttpContext.Current.Session[key];
        }

        private void SetSessionValueHandler(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        private object GetCookieValueHandler<T>(string key)
        {
            var cookieValues = this.GetCookieValues();

            if (cookieValues == null || !cookieValues.ContainsKey(key) || String.IsNullOrWhiteSpace(cookieValues[key]))
                return null;

            var json = HttpContext.Current.Server.UrlDecode(cookieValues[key]);

            if (String.IsNullOrWhiteSpace(json))
                return null;

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var serializer = GetSerializer(typeof(T));
            var value = (T)serializer.ReadObject(ms);

            ms.Close();

            return value;
        }

        private void SetCookieValueHandler<T>(string key, object value)
        {
            var cookieValues = this.GetCookieValues();

            var ms = new MemoryStream();
            var serializer = GetSerializer(typeof(T));

            serializer.WriteObject(ms, value);

            if (ms.CanSeek)
                ms.Seek(0, SeekOrigin.Begin);

            cookieValues[key] = HttpContext.Current.Server.UrlEncode(Encoding.UTF8.GetString(ms.ToArray()));
        }

        private Dictionary<string, string> GetCookieValues()
        {
            if (_session.Controller.ViewData["CookieWasRead"] == null && (_cookieName == null || !_cookieValues.HasItems()))
            {
                var cookie = this.ReadCookie();
                _cookieValues = new Dictionary<string, string>();

                if (cookie != null)
                {
                    _session.Controller.ViewData["CookieWasRead"] = true;

                    foreach (var key in cookie.Values.AllKeys.Where(key => !String.IsNullOrWhiteSpace(key)))
                    {
                        _cookieValues[key] = HttpContext.Current.Server.UrlDecode(cookie.Values[key]);
                    }
                }
            }

            return _cookieValues;
        }

        private static DataContractJsonSerializer GetSerializer(Type classSpecificSerializer)
        {
            if (!CachedSerializers.ContainsKey(classSpecificSerializer))
            {
                CachedSerializers.Add(classSpecificSerializer, new DataContractJsonSerializer(classSpecificSerializer));
            }

            return CachedSerializers[classSpecificSerializer];
        }

        private string GetClientIdentificationHashCode()
        {
            var identification = String.Format("{0}|{1}|{2}", HttpContext.Current.Request["REMOTE_ADDR"],
                                                              HttpContext.Current.Request["HTTP_USER_AGENT"],
                                                              !String.IsNullOrWhiteSpace(HttpContext.Current.Request.Headers["PROXY_FORWARDED_FOR"])
                                                                 ? HttpContext.Current.Request.Headers["PROXY_FORWARDED_FOR"]
                                                                 : HttpContext.Current.Request["PROXY_FORWARDED_FOR"]);

            return identification.GetHashCode().ToString(new CultureInfo("pt-BR"));
        }
    }
}
