using Calmo.Web.JsonSerialization;

namespace System.Web
{
    public static class JsonHelpers
    {
        public static object JsonSerialize(this string json)
        {
            var reader = new JsonReader(json);

            return reader.ReadValue();
        }
    }
}
