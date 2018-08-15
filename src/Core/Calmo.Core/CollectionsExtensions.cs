using System.Linq;
using System.Collections.Generic;
using Calmo.Core.ExceptionHandling;

namespace System.Collections.Generic
{
    public static class GenericCollectionsExtensions
    {
        #region IEnumerable

#if !__MOBILE__
        public static IEnumerable<TOutput> ConvertAll<T, TOutput>(this IEnumerable<T> enumerable, Converter<T, TOutput> converter)
#else
        public static IEnumerable<TOutput> ConvertAll<T, TOutput>(this IEnumerable<T> enumerable, Func<T, TOutput> converter)
#endif
        {
            Throw.IfArgumentNull(enumerable, nameof(enumerable));
            Throw.IfArgumentNull(converter, nameof(converter));

            int size = enumerable.Count();

            var list = new List<TOutput>(size);

            for (int i = 0; i < size; i++)
            {
                list.Add(converter(enumerable.ElementAt(i)));
            }

            return list;
        }

        public static TimeSpan Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, TimeSpan> selector)
        {
            Throw.IfArgumentNull(source, nameof(source));
            Throw.IfArgumentNull(selector, nameof(selector));

            return source.Aggregate(TimeSpan.Zero, (current, item) => current + selector(item));
        }

        public static bool HasDuplicates<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.HasDuplicates(e => e);
        }

        public static bool ContainsArray(this IEnumerable<long> enumerable, string[] itens)
        {
            foreach (var item in itens)
            {
                long numericItem;
                long.TryParse(item, out numericItem);
                var retorno = enumerable.Contains(numericItem);
                if (retorno)
                    return true;
            }
            return false;
        }

        public static bool HasDuplicates<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            if (keySelector == null)
                throw new ArgumentNullException("keySelector");

            return enumerable.GroupBy(keySelector).Any(i => i.Count() > 1);
        }

        public static bool HasMoreThanOne<T>(this IEnumerable<T> source, Func<T, object> keySelector)
        {
            return source.Distinct(keySelector).Count() > 1;
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, object> keySelector)
        {
            Throw.IfArgumentNull(source, nameof(source));
            Throw.IfArgumentNull(keySelector, nameof(keySelector));

            return source.Distinct(new KeyEqualityComparer<T>(keySelector));
        }

        public static bool IsSequential(this IEnumerable<int> source)
        {
            int? i = null;

            foreach (var item in source)
            {
                if (!i.HasValue)
                {
                    i = item;
                }
                else
                {
                    if (i.Value != item - 1)
                        return false;

                    i = item;
                }
            }

            return true;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> selector)
        {
            if (!source.HasItems())
                return -1;

            var item = source.FirstOrDefault(selector);

            if (item == null)
                return -1;

            return source.ToList().IndexOf(item);
        }

        public static int LastIndexOf<T>(this IEnumerable<T> source, Func<T, bool> selector)
        {
            if (!source.HasItems())
                return -1;

            var item = source.LastOrDefault(selector);

            if (item == null)
                return -1;

            return source.ToList().IndexOf(item);
        }

        public static int GetColumnCount<T>(this IEnumerable<T> source, int columns)
        {
            if (!source.HasItems())
                return 0;

            var count = source.Count();

            return count % 2 == 0 ? count / 2 : (count / 2) + 1;
        }

        public static IEnumerable<T> StartsWith<T>(this IEnumerable<T> source, Func<T, string> selector, string from, string to = null)
        {
            if (!source.HasItems())
                return new List<T>();

            var lowerCaseFrom = from.ToLowerInvariant()[0];
            var upperCaseFrom = from.ToUpperInvariant()[0];

            if (!String.IsNullOrWhiteSpace(to))
            {
                var lowerCaseTo = to.ToLowerInvariant()[0];
                var upperCaseTo = to.ToUpperInvariant()[0];

                return source.Where(m => (selector(m)[0] >= lowerCaseFrom && selector(m)[0] <= lowerCaseTo) ||
                                         (selector(m)[0] >= upperCaseFrom && selector(m)[0] <= upperCaseTo));
            }

            return source.Where(m => selector(m)[0] == lowerCaseFrom || selector(m)[0] == upperCaseFrom);
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            if (action == null)
                throw new ArgumentNullException("action");

            foreach (T item in enumerable)
            {
                action(item);
            }

            return enumerable;
        }

        public static bool HasItems<T>(this IEnumerable<T> value)
        {
            return value != null && value.Any();
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }

        public static bool IsFirst<T>(this IEnumerable<T> source, int index)
        {
            if (!source.HasItems())
                return false;

            return index == 0;
        }

        public static bool IsLast<T>(this IEnumerable<T> source, int index)
        {
            if (!source.HasItems())
                return false;

            return index == source.Count() - 1;
        }

        public static string ToSeparatedString<T>(this IEnumerable<T> source, string separator = ",")
        {
            if (!source.HasItems())
                return null;

            return String.Join(separator, source);
        }

#endregion

#region List

        public static List<T> Insert<T>(this List<T> source, T item, bool first = true)
        {
            Throw.IfArgumentNull(source, nameof(source));

            if (first)
                source.Insert(0, item);
            else
                source.Add(item);

            return source;
        }

        public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (comparison == null)
                throw new ArgumentNullException("comparison");

            if (!(list is List<T>))
                throw new NotSupportedException("A lista deve ser do tipo List<T>.");

            ((List<T>)list).Sort(comparison);
        }

        public static void AddRange<T>(this IList<T> source, IEnumerable<T> collection)
        {
            Throw.IfArgumentNull(source, nameof(source));
            Throw.IfArgumentNull(collection, nameof(collection));

            foreach (var item in collection)
                source.Add(item);
        }

#endregion
    }

    public class KeyEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, object> _keySelector;

        public KeyEqualityComparer(Func<T, object> keySelector)
        {
            this._keySelector = keySelector;
        }

        public bool Equals(T x, T y)
        {
            return this._keySelector(x).Equals(this._keySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return this._keySelector(obj).GetHashCode();
        }
    }
}

namespace System.Collections
{
    public static class CollectionsExtensions
    {
        public static IList ToList(this IEnumerable enumerable)
        {
#if !__MOBILE__
            var list = new ArrayList();
#else
            var list = new List<object>();
#endif

            foreach (var item in enumerable)
                list.Add(item);
            
            return list;
        }
    }
}