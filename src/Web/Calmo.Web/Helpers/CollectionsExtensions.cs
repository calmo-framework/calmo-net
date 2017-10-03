using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages.Html;

namespace System.Web.Mvc
{
    public static class CollectionsExtensions
    {
        #region AutoComplete

        public static IEnumerable ToAutoCompleteSource<T>(this IEnumerable<T> source, Func<T, object> id, Func<T, object> label)
        {
            return source.Select(s => new { id = id(s), label = label(s) });
        }

        #endregion

        #region SelectList

        public static SelectList ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, object> text, Func<T, object> value, string defaultOption = null, object selectedValue = null)
        {
            var items = new List<SelectListItem>();

            if (enumerable.HasItems())
            {
                foreach (var item in enumerable)
                {
                    var itemText = text(item);
                    var itemValue = value(item);

                    items.Add(new SelectListItem
                                  {
                                      Text = itemText == null ? String.Empty : itemText.ToString(),
                                      Value = itemValue == null ? String.Empty : itemValue.ToString()
                                  });
                }
            }

            if (defaultOption != null)
            {
                items.Insert(0, new SelectListItem
                                    {
                                        Text = defaultOption,
                                        Value = String.Empty
                                    });
            }

            if (selectedValue != null && !String.IsNullOrWhiteSpace(selectedValue.ToString()))
                return new SelectList(items, "Value", "Text", selectedValue);

            return new SelectList(items, "Value", "Text");
        }

        #endregion
    }
}
