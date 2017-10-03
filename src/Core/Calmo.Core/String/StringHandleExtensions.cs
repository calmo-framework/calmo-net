using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class StringHandleExtensions
    {
        public static string[] Split(this string value, string separator)
        {
            return value.Split(new[] { separator }, StringSplitOptions.None);
        }

        public static T GetValueFromToken<T>(this string s, string token)
        {
            if (String.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            if (String.IsNullOrWhiteSpace(s))
                return default(T);
#if !__MOBILE__
            var index = s.IndexOf(token, StringComparison.InvariantCultureIgnoreCase) + token.Length;
#else
            var index = s.IndexOf(token, StringComparison.CurrentCultureIgnoreCase) + token.Length;
#endif
            string value = null;

            if (index >= token.Length)
            {
                var textPart = s.Substring(index).Trim();
                var textIndex = textPart.IndexOf(' ');

                value = textPart.Substring(0, textIndex >= 0 ? textIndex : textPart.Length);
            }

            return String.IsNullOrWhiteSpace(value) ? default(T) : value.Trim().To<T>();
        }

        public static string Cut(this string s, string initialToken, string finalToken)
        {
            int finishIndex;
            return s.Cut(initialToken, finalToken, 0, out finishIndex);
        }

        public static string Cut(this string s, string initialToken, string finalToken, out int finishIndex)
        {
            return s.Cut(initialToken, finalToken, 0, out finishIndex);
        }

        public static string Cut(this string s, string initialToken, string finalToken, int startIndex)
        {
            int finishIndex;
            return s.Cut(initialToken, finalToken, startIndex, out finishIndex);
        }

        public static string Cut(this string s, string initialToken, string finalToken, int startIndex, out int finishIndex)
        {
            if (String.IsNullOrWhiteSpace(initialToken))
                throw new ArgumentNullException(nameof(initialToken));

            if (String.IsNullOrWhiteSpace(finalToken))
                throw new ArgumentNullException(nameof(finalToken));

            if (String.IsNullOrWhiteSpace(s))
            {
                finishIndex = -1;
                return null;
            }

#if !__MOBILE__
            var initialIndex = s.IndexOf(initialToken, startIndex, StringComparison.InvariantCultureIgnoreCase) + initialToken.Length;
            var finalIndex = s.IndexOf(finalToken, initialIndex, StringComparison.InvariantCultureIgnoreCase);
#else
            var initialIndex = s.IndexOf(initialToken, startIndex, StringComparison.CurrentCultureIgnoreCase) + initialToken.Length;
            var finalIndex = s.IndexOf(finalToken, initialIndex, StringComparison.CurrentCultureIgnoreCase);
#endif
            finishIndex = finalIndex + finalToken.Length;

            return s.Substring(initialIndex, finalIndex - initialIndex);
        }

        public static T[] ToArray<T>(this string value, char separator = ',')
        {
            if (String.IsNullOrWhiteSpace(value))
                return new T[0];

            return value.Split(separator)
                     .Where(v => !String.IsNullOrWhiteSpace(v))
                     .ConvertAll(v => (T)Convert.ChangeType(v, typeof(T)))
                     .ToArray();
        }

        public static IEnumerable<string> EnumerateByLength(this string text, int length)
        {
            var index = 0;
            while (index < text.Length)
            {
                var charCount = Math.Min(length, text.Length - index);
                yield return text.Substring(index, charCount);
                index += length;
            }
        }
        public static string Split(this string text, char separator, int partIndex)
        {
            if (text == null) return null;
            if (String.IsNullOrEmpty(text)) return String.Empty;

            return text.Split(separator)[partIndex];
        }
    }
}