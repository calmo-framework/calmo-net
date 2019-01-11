using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
#if __MOBILE__
using System.Reflection;
#endif

namespace System
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumerator)
        {
            var description = GetEnumAttributeData<DescriptionAttribute, string>(enumerator, a => a.Description);

            if (!String.IsNullOrWhiteSpace(description))
                return description;

            return enumerator.ToString();
        }

        public static T ToEnumFromDescription<T>(this string description)
        {
            return ToEnumFromAttributeData<T, DescriptionAttribute, string>(description, a => a.Description);
        }

        public static string GetDataKey(this Enum enumerator)
        {
            var dataKey = GetEnumAttributeData<DataKeyAttribute, string>(enumerator, a => a.DataKey);

            if (!String.IsNullOrWhiteSpace(dataKey))
                return dataKey;

            return enumerator.ToString();
        }

        public static T ToEnumFromDataKey<T>(this string dataKey)
        {
            return ToEnumFromAttributeData<T, DataKeyAttribute, string>(dataKey, a => a.DataKey);
        }

        private static TProperty GetEnumAttributeData<T, TProperty>(Enum enumerator, Expression<Func<T, TProperty>> property) where T : Attribute
        {
            var enumType = enumerator.GetType();
#if !__MOBILE__
            var memberInfos = enumType.GetMember(enumerator.ToString());
            if (memberInfos.Length <= 0) return default(TProperty);

            var memberInfo = memberInfos[0];
#else

            var memberInfo = enumType.GetRuntimeField(enumerator.ToString());
            if (memberInfo == null) return default(TProperty);
#endif
            var attributes = memberInfo.GetCustomAttributes(typeof(T), false);

            if(attributes.HasItems())
                return property.Compile()((T)attributes.First());

            return default(TProperty);
        }

        private static T ToEnumFromAttributeData<T, TAttribute, TProperty>(TProperty value, Expression<Func<TAttribute, TProperty>> property) where TAttribute : Attribute
        {
#if !__MOBILE__
            var memberInfos = typeof(T).GetMembers();
#else
            var memberInfos = typeof(T).GetRuntimeFields();
#endif

            if (memberInfos.HasItems())
            {
                foreach (var memberInfo in memberInfos)
                {
                    var attributes = memberInfo.GetCustomAttributes(typeof(TAttribute), false);
                    if (!attributes.HasItems()) continue;

                    var attributteValue = property.Compile()((TAttribute)attributes.First());
                    if (attributteValue.Equals(value))
                        return (T)Enum.Parse(typeof(T), memberInfo.Name);
                }
            }

            throw new NullReferenceException($"A DataKey '{value}' não foi referênciada em nenhum membro do Enum '{typeof(T).FullName}'.");
        }
        
        public static bool IsValidEnumDataKey<T>(this string dataKey)
        {
            try
            {
                ToEnumFromAttributeData<T, DataKeyAttribute, string>(dataKey, a => a.DataKey);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static IEnumerable<T> GetEnumSelectList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static List<T> GetItemsList<T>(this int enums) where T : struct
        {
#if !__MOBILE__
            if (!typeof(T).IsEnum)
                throw new Exception("Type given must be an Enum");
#endif

            try
            {
                return (from int item in Enum.GetValues(typeof(T))
                    where (enums & item) == item
                    select (T)Enum.Parse(typeof(T), item.ToString(new CultureInfo("en")))).ToList();
            }
            catch
            {
                throw new Exception("Type given must be an Enum");
            }
        }
    }
}
