using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Linq.Expressions;

namespace System.Dynamic
{
    public static class ObjectDynamicExtensions
    {
        public static T TryGetMember<T>(this object obj, string key)
        {
            var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, key, obj.GetType(),
                                                                         new[]
                                                                             {
                                                                                 CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
                                                                             });
            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            return (T)callsite.Target(callsite, obj);
        }
    }
}

namespace System
{
    public static class ObjectExtensions
    {
        public static TProperty GetValueOrDefault<T, TProperty>(this T obj, Func<T, TProperty> expectedValue, TProperty defaultValue = default(TProperty)) where T : class
        {
            return obj == null ? defaultValue : expectedValue(obj);
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
#if !__MOBILE__
            var propertyInfo = obj.GetType().GetProperty(propertyName);
#else
            var propertyInfo = obj.GetType().GetRuntimeProperty(propertyName);
#endif

            if (propertyInfo == null)
                throw new ArgumentException("Property " + propertyName + " not found!");

            return propertyInfo.GetValue(obj, null);
        }

        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
#if !__MOBILE__
            var propertyInfo = obj.GetType().GetProperty(propertyName);
#else
            var propertyInfo = obj.GetType().GetRuntimeProperty(propertyName);
#endif

            if (propertyInfo == null)
                throw new ArgumentException("Property " + propertyName + " not found!");

            return (T)Convert.ChangeType(propertyInfo.GetValue(obj, null), typeof(T));
        }

        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
#if !__MOBILE__
            var propertyInfo = obj.GetType().GetProperty(propertyName);
#else
            var propertyInfo = obj.GetType().GetRuntimeProperty(propertyName);
#endif

            if (propertyInfo == null)
                throw new ArgumentException("Property " + propertyName + " not found!");

            propertyInfo.SetValue(obj, value, null);
        }

        public static void SetFieldValue(this object obj, string fieldName, object value)
        {
#if !__MOBILE__
            var fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
#else
            var fieldInfo = obj.GetType().GetRuntimeField(fieldName);
#endif

            if (fieldInfo == null)
                throw new ArgumentException("Field " + fieldName + " not found!");

            fieldInfo.SetValue(obj, value);
        }

        public static T To<T>(this object value)
        {
            var type = typeof(T);

            if (type.IsNullable())
            {
                if (value == null || String.IsNullOrWhiteSpace(value.ToString()))
                    return default(T);

                try
                {
                    return (T)Convert.ChangeType(value, type.GetUnderlyingType());
                }
                catch
                {
#if !__MOBILE__
                    return (T)Convert.ChangeType(value, type.GetGenericArguments()[0]);
#else
                    return (T)Convert.ChangeType(value, type.GenericTypeArguments[0]);
#endif
                }
                
            }

            return (T)Convert.ChangeType(value, type);
        }

        public static object To(this object value, Type type)
        {
            if (type.IsNullable())
            {
                if (value == null || String.IsNullOrWhiteSpace(value.ToString()))
                    return null;

                try
                {
                    return Convert.ChangeType(value, type.GetUnderlyingType());
                }
                catch
                {
#if !__MOBILE__
                    return Convert.ChangeType(value, type.GetGenericArguments()[0]);
#else
                    return Convert.ChangeType(value, type.GenericTypeArguments[0]);
#endif
                }
            }

            return Convert.ChangeType(value, type);
        }

        public static bool CanConvertTo(this object obj, Type type)
        {
            try
            {
                Convert.ChangeType(obj, type);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNumeric(this object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return value is Int16 || value is Int32 || value is Int64
                   || value is Single || value is Double || value is Decimal
                   || value is UInt16 || value is UInt32 || value is UInt64;
        }

        public static bool IsValidNumeric(this object value, int minValue = 0)
        {
            if (!value.IsNumeric())
                return false;

            if (value is Int16)
                return (Int16)value > minValue;
            if (value is Int32)
                return (Int32)value > minValue;
            if (value is Int64)
                return (Int64)value > minValue;
            if (value is Single)
                return (Single)value > minValue;
            if (value is Double)
                return (Double)value > minValue;
            if (value is Decimal)
                return (Decimal)value > minValue;
            if (value is UInt16)
                return (UInt16)value > minValue;
            if (value is UInt32)
                return (UInt32)value > minValue;
            if (value is UInt64)
                return (UInt64)value > (UInt64)minValue;

            return false;
        }

        public static bool In<T>(this T value, params T[] args)
        {
            return args.Contains(value);
        }

        public static bool In<T>(this T value, IEnumerable<T> args)
        {
            return args.Contains(value);
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);

            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");

            return propInfo;
        }
    }
}
