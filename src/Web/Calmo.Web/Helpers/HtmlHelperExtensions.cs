using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ToJs(this bool value)
        {
            return new MvcHtmlString(value.ToString().ToLower());
        }

        public static MvcHtmlString Conditional(this HtmlHelper helper, bool condition, string trueHtml, string falseHtml = null)
        {
            return condition ? new MvcHtmlString(trueHtml) : new MvcHtmlString(falseHtml);
        }

        public static MvcHtmlString Conditional<TEnum>(this HtmlHelper helper, TEnum value, Dictionary<TEnum, string> htmls)
        {
            return htmls.ContainsKey(value) ? new MvcHtmlString(htmls[value]) : new MvcHtmlString(string.Empty);
        }

        public static MvcHtmlString Checked(this HtmlHelper helper, bool condition)
        {
            return condition ? MvcHtmlString.Create("checked=\"checked\"") : MvcHtmlString.Empty;
        }

        public static MvcHtmlString AmountText(this HtmlHelper helper, int amount, string singular, string plural)
        {
            var texto = amount == 1 ? singular : plural;

            return MvcHtmlString.Create(texto);
        }

        public static MvcHtmlString EnumDropDownList(this HtmlHelper helper, Enum enumerator, string name, string optionLabel = null, object htmlAttributes = null)
        {
            var enumType = enumerator.GetType();
            var items = new List<SelectListItem>();

            foreach (var value in Enum.GetValues(enumType))
            {
                items.Add(new SelectListItem { Value = value.To<int>().ToString(), Text = value.To<Enum>().GetDescription() });
            }

            return helper.DropDownList(name, items, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel = null, object htmlAttributes = null)
        {
            var type = Nullable.GetUnderlyingType(typeof(TEnum)) ?? typeof(TEnum);

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var values = Enum.GetValues(type).Cast<object>().ConvertAll(c => Enum.Parse(type, c.ToString()));
            var items = values.Select(value => new SelectListItem
                                                   {
                                                       Text = value.To<Enum>().GetDescription(),
                                                       Value = value.ToString(),
                                                       Selected = value.Equals(metadata.Model)
                                                   });

            return htmlHelper.DropDownListFor(expression, items, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropDownList<T, TValue, TText>(this HtmlHelper helper, string name, IEnumerable<T> items, Func<T, TValue> value, Func<T, TText> text)
        {
            return DropDownList(helper, name, items, value, text, null, null);
        }

        public static MvcHtmlString DropDownList<T, TValue, TText>(this HtmlHelper helper, string name, IEnumerable<T> items, Func<T, TValue> value, Func<T, TText> text, object htmlAttributes)
        {
            return DropDownList(helper, name, items, value, text, null, htmlAttributes);
        }

        public static MvcHtmlString DropDownList<T, TValue, TText>(this HtmlHelper helper, string name, IEnumerable<T> items, Func<T, TValue> value, Func<T, TText> text, IDictionary<string, object> htmlAttributes)
        {
            return DropDownList(helper, name, items, value, text, null, htmlAttributes);
        }

        public static MvcHtmlString DropDownList<T, TValue, TText>(this HtmlHelper helper, string name, IEnumerable<T> items, Func<T, TValue> value, Func<T, TText> text, string optionLabel)
        {
            return DropDownList(helper, name, items, value, text, optionLabel, null);
        }

        public static MvcHtmlString DropDownList<T, TValue, TText>(this HtmlHelper helper, string name, IEnumerable<T> items, Func<T, TValue> value, Func<T, TText> text, string optionLabel, object htmlAttributes)
        {
            var selectItems = items.Select(item => new SelectListItem
            {
                Value = Convert.ToString(value.Invoke(item)),
                Text = Convert.ToString(text.Invoke(item))
            });

            return helper.DropDownList(name, selectItems, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropDownList<T, TValue, TText>(this HtmlHelper helper, string name, IEnumerable<T> items, Func<T, TValue> value, Func<T, TText> text, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            var selectItems = items.Select(item => new SelectListItem
                                                       {
                                                           Value = Convert.ToString(value.Invoke(item)),
                                                           Text = Convert.ToString(text.Invoke(item))
                                                       });

            return helper.DropDownList(name, selectItems, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString EnumToJson<TEnum>(this HtmlHelper helper)
        {
            var builder = new StringBuilder();
            var enumType = typeof(TEnum);
            var enumValues = Enum.GetValues(enumType);

            builder.AppendFormat("var {0} = {{", enumType.Name.ToCamelCaseWord());

            for (var i = 0; i < enumValues.Length; i++)
            {
                var enumValue = enumValues.GetValue(i);

                builder.AppendFormat("{0}: {1}", enumValue.ToString().ToCamelCaseWord(), (int)enumValue);

                if (i < enumValues.Length - 1)
                    builder.Append(",");
            }

            builder.Append("};");

            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString EnumToJson<TEnum, TAttribute, TProperty>(this HtmlHelper helper, Expression<Func<TAttribute, TProperty>> property)
        {
            var enumType = typeof(TEnum);
            var memberInfos = enumType.GetMembers();

            if (memberInfos != null && memberInfos.Length > 0)
            {
                var builder = new StringBuilder();
                builder.AppendFormat("var {0} = {{", enumType.Name.ToCamelCaseWord());

                var i = 0;
                foreach (var memberInfo in memberInfos)
                {
                    var attributes = memberInfo.GetCustomAttributes(typeof(TAttribute), false);

                    if (attributes == null || attributes.Length <= 0) continue;

                    builder.AppendFormat("{0}: '{1}'", memberInfo.Name.ToCamelCaseWord(), property.Compile()((TAttribute)attributes[0]));

                    if (i < memberInfos.Length - 1)
                        builder.Append(",");

                    i++;
                }

                builder.Append("};");

                return MvcHtmlString.Create(builder.ToString());
            }

            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString ConstantsToJson<TClass>(this HtmlHelper helper) where TClass : class
        {
            var builder = new StringBuilder();
            var classType = typeof(TClass);
            var stringType = typeof(string);
            var int16Type = typeof(Int16);
            var int32Type = typeof(Int32);
            var int64Type = typeof(Int64);

            var constants = classType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                     .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType.In(stringType, int16Type, int32Type, int64Type))
                                     .ToArray();

            builder.AppendFormat("var {0} = {{", classType.Name.ToCamelCaseWord());

            var i = 0;
            foreach (var constant in constants)
            {
                var format = "{0}: {1}";

                if (constant.FieldType == stringType)
                    format = "{0}: '{1}'";

                builder.AppendFormat(format, constant.Name.ToCamelCaseWord(), constant.GetRawConstantValue());

                if (i < constants.Length - 1)
                    builder.Append(",");

                i++;
            }

            builder.Append("};");

            return MvcHtmlString.Create(builder.ToString());
        }

        public static IHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Dictionary<KeyValuePair<int, string>, IEnumerable<SelectListItem>> selectList, object htmlAttributes = null)
        {

            var select = new TagBuilder("select");
            select.Attributes.Add("name", ExpressionHelper.GetExpressionText(expression));

            if (htmlAttributes != null)
            {
                var properties = htmlAttributes.GetType().GetProperties();

                foreach (var propertyInfo in properties)
                {
                    select.Attributes.Add(propertyInfo.Name, htmlAttributes.GetPropertyValue<string>(propertyInfo.Name));
                }
            }

            var optgroups = new StringBuilder();

            foreach (var kvp in selectList)
            {
                var optgroup = new TagBuilder("optgroup");
                optgroup.Attributes.Add("label", kvp.Key.Value);

                var options = new StringBuilder();

                foreach (var item in kvp.Value)
                {
                    var option = new TagBuilder("option");

                    option.Attributes.Add("value", item.Value);
                    option.SetInnerText(item.Text);
                    var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
                    if (kvp.Value.Equals(metadata.Model))
                    {
                        option.Attributes.Add("selected", "selected");
                    }
                    option.Attributes.Add("groupKey", kvp.Key.Key.ToString("N0"));

                    options.Append(option.ToString(TagRenderMode.Normal));
                }

                optgroup.InnerHtml = options.ToString();

                optgroups.Append(optgroup.ToString(TagRenderMode.Normal));
            }

            select.InnerHtml = optgroups.ToString();

            return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
        }

        public static IHtmlString DropDownListFor(this HtmlHelper htmlHelper, string name, KeyValuePair<int, string>? defaultValue, Dictionary<KeyValuePair<int, string>, IEnumerable<SelectListItem>> selectList, object htmlAttributes = null, string id = null)
        {

            var select = new TagBuilder("select");
            select.Attributes.Add("name", name);
            if (!String.IsNullOrWhiteSpace(id))
                select.Attributes.Add("id", id);

            if (htmlAttributes != null)
            {
                var properties = htmlAttributes.GetType().GetProperties();

                foreach (var propertyInfo in properties)
                {
                    select.Attributes.Add(propertyInfo.Name, htmlAttributes.GetPropertyValue<string>(propertyInfo.Name));
                }
            }

            var optgroups = new StringBuilder();

            foreach (var kvp in selectList)
            {
                var optgroup = new TagBuilder("optgroup");
                optgroup.Attributes.Add("label", kvp.Key.Value);
                optgroup.Attributes.Add("value", kvp.Key.Key.ToString("N0"));

                var options = new StringBuilder();

                foreach (var item in kvp.Value)
                {
                    var option = new TagBuilder("option");

                    option.Attributes.Add("value", string.Format("{0}_{1}", kvp.Key.Key.ToString("N0"), item.Value));
                    option.Attributes.Add("data-value", item.Value);
                    option.SetInnerText(item.Text);

                    if (item.Value != null && defaultValue.HasValue &&
                        item.Value.Equals(defaultValue.Value.Value) &&
                        kvp.Key.Key == defaultValue.Value.Key)
                    {
                        option.Attributes.Add("selected", "selected");
                    }
                    option.Attributes.Add("groupKey", kvp.Key.Key.ToString("N0"));

                    options.Append(option.ToString(TagRenderMode.Normal));
                }

                optgroup.InnerHtml = options.ToString();

                optgroups.Append(optgroup.ToString(TagRenderMode.Normal));
            }

            select.InnerHtml = optgroups.ToString();

            return MvcHtmlString.Create(select.ToString(TagRenderMode.Normal));
        }

        public static IHtmlString HelpLink(this HtmlHelper helper)
        {
            var funcionalidadesCorrentes = (string[])helper.ViewBag.FuncionalidadesCorrentes;
            if (funcionalidadesCorrentes.HasItems())
            {
                var link = new TagBuilder("i");
                link.Attributes.Add("class", "icon-question-sign help");
                link.Attributes.Add("data-funcionalidade", funcionalidadesCorrentes.First());

                return MvcHtmlString.Create(link.ToString(TagRenderMode.Normal));
            }
            return null;
        }

        public static string IsSelected(this HtmlHelper html, IEnumerable<string> controllers = null, string action = null)
        {
            var cssClass = "active";
            var currentAction = (string)html.ViewContext.RouteData.Values["action"];
            var currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (!controllers.HasItems())
                controllers = new[] { currentController };

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controllers.Any(c => c == currentController) && action == currentAction ?
                cssClass : String.Empty;
        }
    }
}