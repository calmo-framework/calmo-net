using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Calmo.Data.Forms
{
    internal static class Mapper<T> where T : class
    {
        private static readonly Dictionary<string, PropertyInfo> _propertyMap;

        static Mapper()
        {
            _propertyMap = typeof(T).GetRuntimeProperties()
                .ToDictionary(
                    p => p.Name.ToLower(),
                    p => p
                );
        }

        public static T Map(ExpandoObject source)
        {
            // Might as well take care of null references early.
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var destination = Activator.CreateInstance<T>();

            // By iterating the KeyValuePair<string, object> of
            // source we can avoid manually searching the keys of
            // source as we see in your original code.
            foreach (var kv in source)
            {
                PropertyInfo p;
                if (_propertyMap.TryGetValue(kv.Key.ToLower(), out p))
                {
                    var propType = p.PropertyType;
                    if (kv.Value == null)
                    {
                        if (!propType.IsByRef && propType.Name != "Nullable`1")
                        {
                            // Throw if type is a value type 
                            // but not Nullable<>
                            throw new ArgumentException("not nullable");
                        }
                    }

                    try
                    {
                        var value = Convert.ChangeType(kv.Value, propType);
                        p.SetValue(destination, value, null);
                    }
                    catch
                    {
                        throw new ArgumentException("type mismatch");
                    }
                }
            }

            return destination;
        }
    }
}
