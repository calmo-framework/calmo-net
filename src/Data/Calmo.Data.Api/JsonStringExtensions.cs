using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace System
{
    public static class JsonStringExtensions
    {
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
    }
}