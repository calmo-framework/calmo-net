using System.Collections.Generic;
using System.Linq;

namespace System.DirectoryServices
{
    internal static class SearchResultExtensions
    {
        public static string ReadValue(this ResultPropertyValueCollection property, string propertyName = null)
        {
            var startString = String.IsNullOrWhiteSpace(propertyName) ? String.Empty : $"{propertyName}=";

            string value = null;
            foreach (var result in property)
            {
                value += (String.IsNullOrWhiteSpace(value) ? startString : Environment.NewLine) + result.ToString();
            }

            return value;
        }
        public static string[] ReadValues(this ResultPropertyValueCollection property)
        {
            var values = new List<string>();
            foreach (var result in property)
            {
                values.Add(result.ToString());
            }

            return values.Any() ? values.ToArray() : new string[0];
        }

        public static string[] ReadValues(this ResultPropertyCollection properties)
        {
            var values = new List<string>();
            foreach (string name in properties.PropertyNames)
            {
                values.Add(properties[name].ReadValue());
            }

            return values.Any() ? values.ToArray() : new string[0];
        }
    }
}