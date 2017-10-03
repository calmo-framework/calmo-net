using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ResourceIT.Core.ExceptionHandling;

namespace System
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type GetUnderlyingType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return Nullable.GetUnderlyingType(type);
        }

        public static bool HasProperty(this Type type, string name)
        {
            return type.GetRuntimeProperties().Any(m => m.Name == name);
        }

        public static bool HasMethod(this Type type, string name)
        {
            return type.GetRuntimeMethods().Any(m => m.Name == name);
        }

        public static bool IsAnonymous(this Type type)
        {
            Throw.IfArgumentNull(type, "type");

            return type.IsConstructedGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"));
        }
    }
}
