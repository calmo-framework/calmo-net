using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Calmo.Core;
using Calmo.Web;

namespace System.Net
{
    public static class HttpWebRequestExtensions
    {
        public static HttpWebResponse Post(this HttpWebRequest request, object parameters)
        {
            try
            {
                var proxy = CustomConfiguration.Settings.Proxy().GetProxy();

                if (proxy != null)
                    WebRequest.DefaultWebProxy = proxy;

                var encoding = new ASCIIEncoding();
                var data = encoding.GetBytes(GetParametersData(parameters));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.AllowAutoRedirect = true;
                request.KeepAlive = true;
                request.UserAgent =
                    @"Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.0.4) Gecko/20060508 Firefox/1.5.0.4";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                return (HttpWebResponse) request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var sr = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        var erro = sr.ReadToEnd();
                        var eventLog = new EventLog {Source = "WebRequest"};
                        eventLog.WriteEntry(erro + parameters.ToString(), EventLogEntryType.Error);
                    }
                }
                else
                {
                    var eventLog = new EventLog { Source = "WebRequest" };
                    eventLog.WriteEntry(ex.Status + parameters.ToString(), EventLogEntryType.Error);
                }
                throw ex.InnerException;
            }
        }

        private static string GetParametersData(object parameters)
        {
            if (parameters == null) return null;

            var parametersType = parameters.GetType();

            var data = String.Join("&", parametersType.GetProperties()
             .Select(p => String.Format("{0}={1}", p.Name, (p.GetValue(parameters).GetType().IsArray ? String.Join(String.Format("&{0}=",p.Name), ((object[])p.GetValue(parameters))) : p.GetValue(parameters)))));

            return data;
        }
    }
}
