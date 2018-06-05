#if !__MOBILE__
using System.Collections.Specialized;
#endif
using System.Linq;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static T GetValue<T>(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary == null || !dictionary.ContainsKey(key))
                return default(T);

            return (T)dictionary[key];
        }

        public static List<KeyValuePair<TKey, TValue>> ToKeyValuePairList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null) return null;

            return (List<KeyValuePair<TKey, TValue>>)dictionary.Select(item => new KeyValuePair<TKey, TValue>(item.Key, item.Value)).ToList();
        }

        public static Dictionary<TKey, TValue> Copy<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            return source.ToDictionary(item => item.Key, item => item.Value);
        }

#if !__MOBILE__
        public static NameValueCollection ToNameValueCollection<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            var collection = new NameValueCollection();

            foreach (var item in dictionary)
            {
                collection.Add(item.Key.ToString(), item.Value != null ? item.Value.ToString() : null);
            }

            return collection;
        }
#endif
    }
}