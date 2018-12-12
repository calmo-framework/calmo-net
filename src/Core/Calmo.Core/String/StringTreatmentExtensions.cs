using System.Linq;
using System.Text.RegularExpressions;
#if !__MOBILE__
using System.Globalization;
using System.Text;
#endif

namespace System
{
    public static class StringTreatmentExtensions
    {
        public static string SecureToLower(this string s, CultureInfo cultureInfo = null)
        {
            if (String.IsNullOrWhiteSpace(s)) return s;

            return cultureInfo == null ? s.ToLower() : s.ToLower(cultureInfo);
        }

        public static string SecureToUpper(this string s, CultureInfo cultureInfo = null)
        {
            if (String.IsNullOrWhiteSpace(s)) return s;

            return cultureInfo == null ? s.ToUpper() : s.ToUpper(cultureInfo);
        }

        public static string ClearIfNullOrWhiteSpace(this string s)
        {
            return String.IsNullOrWhiteSpace(s) ? String.Empty : s;
        }

        public static string NullIfEmpty(this string s)
        {
            return s == String.Empty ? null : s;
        }

        public static string EmptyIfNull(this string s)
        {
            return s ?? String.Empty;
        }

        public static string Truncate(this string value, int maxLenght, string legend = "...")
        {
            if (maxLenght == 0)
                throw new ArgumentException();

            var fim = 0;

            legend = String.IsNullOrWhiteSpace(legend) ? "" : legend;

            if (maxLenght > legend.Length)
                fim = legend.Length;

            if (!String.IsNullOrWhiteSpace(value) && value.Length > maxLenght)
                return value.Substring(0, maxLenght - fim) + (fim > 0 ? legend : "");

            return value;
        }

        public static string Trim(this string text, out int trimmedAtStart, out int trimmedAtEnd, params char[] trimChars)
        {
            trimmedAtStart = 0;
            trimmedAtEnd = 0;

            if (text == null) return null;

            foreach (char c in text)
            {
                if (trimChars.Contains(c))
                    trimmedAtStart++;
                else
                    break;
            }

            var auxText = text.TrimStart(trimChars);

            trimmedAtEnd = auxText.Length - auxText.TrimEnd(trimChars).Length;

            return text.Trim();
        }

        public static string RemoveHtmlMarkup(this string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return value;

            value = Regex.Replace(value, @"<[^>]+>", String.Empty).Trim();
            value = HtmlUtility.HtmlDecode(value);
            return Regex.Replace(value, @"\s{2,}", " ");
        }

#if !__MOBILE__
        public static string RemoveDiacritics(this string text)
        {
            return string.Concat(text.Normalize(NormalizationForm.FormD)
                                     .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark))
                         .Normalize(NormalizationForm.FormC);
        }
#endif
    }
}