#if !__MOBILE__
using System.Globalization;
#endif
using System.Text;
using System.Text.RegularExpressions;
#if __MOBILE__
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace System
{
    public static class StringFormatExtensions
    {
#if !__MOBILE__
        public static string ToTitleCase(this string value, CultureInfo culture = null)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            if (culture == null)
                culture = CultureInfo.CurrentCulture;

            return culture.TextInfo.ToTitleCase(value.ToLower());
        }
#endif

        public static string ToCamelCaseWord(this string value)
        {
            if (value.ToUpper() == value)
                return value.ToLower();

            return String.Format("{0}{1}", value.Substring(0, 1).ToLower(), value.Substring(1));
        }

        public static string ToPascalCaseWord(this string value)
        {
            return String.Format("{0}{1}", value.Substring(0, 1).ToUpper(), value.Substring(1));
        }

        public static string ToSlugString(this string value, bool ampersand = false)
        {
            if (String.IsNullOrWhiteSpace(value))
                return String.Empty;

            const int maxLen = 80;

            var valueLength = value.Length;
            var prevDash = false;
            var sb = new StringBuilder(valueLength);

            for (var i = 0; i < valueLength; i++)
            {
                var c = value[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevDash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    sb.Append((char)(c | 32)); // ToLower
                    prevDash = false;
                }
                else if (c == '&' && ampersand)
                {
                    sb.Append('e');
                    prevDash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevDash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevDash = true;
                    }
                }
                else if (c >= 128)
                {
                    var prevlen = sb.Length;
                    sb.Append(c.ToAscii());
                    if (prevlen != sb.Length) prevDash = false;
                }
                if (i == maxLen) break;
            }

            if (prevDash)
                return sb.ToString().Substring(0, sb.Length - 1);

            return sb.ToString();
        }

        public static string ToAscii(this char c)
        {
            var s = c.ToString().ToLowerInvariant();

            if ("àåáâäãåą".Contains(s))
                return "a";

            if ("èéêëę".Contains(s))
                return "e";

            if ("ìíîïı".Contains(s))
                return "i";

            if ("òóôõöøőð".Contains(s))
                return "o";

            if ("ùúûüŭů".Contains(s))
                return "u";

            if ("çćčĉ".Contains(s))
                return "c";

            if ("żźž".Contains(s))
                return "z";

            if ("śşšŝ".Contains(s))
                return "s";

            if ("ñń".Contains(s))
                return "n";

            if ("ýÿ".Contains(s))
                return "y";

            if ("ğĝ".Contains(s))
                return "g";

            if (c == 'ř')
                return "r";

            if (c == 'ł')
                return "l";

            if (c == 'đ')
                return "d";

            if (c == 'ß')
                return "ss";

            if (c == 'Þ')
                return "th";

            if (c == 'ĥ')
                return "h";

            if (c == 'ĵ')
                return "j";

            return "";
        }

        public static bool LengthGreaterThan(this string value, int length)
        {
            return value != null && value.Length > length;
        }

        public static bool LengthGreaterThanOrEqual(this string value, int length)
        {
            return value != null && value.Length >= length;
        }

        public static bool IsValidUrlHttp(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return false;

            return value.ToLower().Contains("http://");
        }

        public static bool IsValidLatitude(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return false;

            var latitude = value.ToCoordinate();

            if (latitude.HasValue)
            {
                if (latitude.Value >= -90m && latitude.Value <= 90m)
                    return true;
            }

            return false;
        }

        public static bool IsValidLongitude(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return false;

            var longitude = value.ToCoordinate();

            if (longitude.HasValue)
            {
                if (longitude.Value >= -180m && longitude.Value <= 180m)
                    return true;
            }

            return false;
        }

        public static bool IsValidDayTime(this string text, out TimeSpan timeSpan)
        {
            timeSpan = TimeSpan.MinValue;

            if (String.IsNullOrWhiteSpace(text)) return false;

            int hours, minutes;
            if (text.Contains(":"))
            {
                hours = Convert.ToInt32(text.Split(':')[0]);
                minutes = Convert.ToInt32(text.Split(':')[1]);
            }
            else
            {
                hours = Convert.ToInt32(text);
                minutes = 0;
            }

            if (hours < 0 || hours > 23)
                return false;

            if (minutes < 0 || minutes > 59)
                return false;

            timeSpan = new TimeSpan(hours, minutes, 0);

            return true;
        }

        public static bool IsHTMLContent(this string value)
        {
            var regex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");

            return regex.IsMatch(value);
        }

#if __MOBILE__
        public static bool IsValidJson(this string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return false;

            value = value.Trim();
            if ((value.StartsWith("{") && value.EndsWith("}")) ||
                (value.StartsWith("[") && value.EndsWith("]")))
            {
                try
                {
                    JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }
#endif

        public static byte[] EncodeBase64(this string base64Imagem)
        {
            string s = base64Imagem.Trim().Replace(" ", "+");
            if (s.Length % 4 > 0)
                s = s.PadRight(s.Length + 4 - s.Length % 4, '=');
            return Convert.FromBase64String(s);
        }
    }
}