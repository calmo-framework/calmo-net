using System;
using System.Net;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace Calmo.Web
{
    public class GoogleUrlShortner
    {
        private class GoogleShortenedURLResponse
        {
            public string id { get; set; }
            public string kind { get; set; }
            public string longUrl { get; set; }
        }

        private class GoogleShortenedURLRequest
        {
            public string longUrl { get; set; }
        }

        public static string ShortenURL(string longurl)
        {
            try
            {
                string googReturnedJson = string.Empty;
                JavaScriptSerializer javascriptSerializer = new JavaScriptSerializer();

                GoogleShortenedURLRequest googSentJson = new GoogleShortenedURLRequest();
                googSentJson.longUrl = longurl;
                string jsonData = javascriptSerializer.Serialize(googSentJson);

                byte[] bytebuffer = Encoding.UTF8.GetBytes(jsonData);

                WebRequest webreq = WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url");
                webreq.Method = WebRequestMethods.Http.Post;
                webreq.ContentLength = bytebuffer.Length;
                webreq.ContentType = "application/json";

                using (Stream stream = webreq.GetRequestStream())
                {
                    stream.Write(bytebuffer, 0, bytebuffer.Length);
                    stream.Close();
                }

                using (HttpWebResponse webresp = (HttpWebResponse)webreq.GetResponse())
                {
                    using (Stream dataStream = webresp.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            googReturnedJson = reader.ReadToEnd();
                        }
                    }
                }

                GoogleShortenedURLResponse googUrl = javascriptSerializer.Deserialize<GoogleShortenedURLResponse>(googReturnedJson);

                return googUrl.id;
            }
            catch (Exception)
            {
                return longurl;
            }
        }
    }
}