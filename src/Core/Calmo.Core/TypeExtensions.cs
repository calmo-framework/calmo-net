using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Calmo.Core.ExceptionHandling;

namespace System
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

#if !__MOBILE__
            if (type.IsValueType && !type.IsGenericType)
                return false;

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
#else
            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
#endif
        }

        public static Type GetUnderlyingType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return Nullable.GetUnderlyingType(type);
        }

        public static bool HasProperty(this Type type, string name)
        {
#if !__MOBILE__
            return type.GetMembers().Any(m => m.Name == name);
#else
            return type.GetRuntimeProperties().Any(m => m.Name == name);
#endif
        }

        public static bool HasMethod(this Type type, string name)
        {
#if !__MOBILE__
            return type.GetMethods().Any(m => m.Name == name);
#else
            return type.GetRuntimeMethods().Any(m => m.Name == name);
#endif
        }

        public static bool IsAnonymous(this Type type)
        {
            Throw.IfArgumentNull(type, nameof(type));

#if !__MOBILE__
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
#else
            return type.IsConstructedGenericType && type.Name.Contains("AnonymousType")
                   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"));
#endif
        }
    }
}
