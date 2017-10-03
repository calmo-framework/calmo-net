using System;
using System.Text;
using System.Web;

namespace System
{
    public static class StringHelperExtensions
    {
        public static string ToJsNamespaceDeclaration(this string namespaceName)
        {
            if (String.IsNullOrWhiteSpace(namespaceName)) return String.Empty;

            var namespaceScript = new StringBuilder();
            var namespaceItems = namespaceName.Split('.');
            var namespaceItemName = String.Empty;

            foreach (var namespaceItem in namespaceItems)
            {
                namespaceScript.Append(namespaceItem.ToJsObjectDeclaration(namespaceItemName));
                namespaceItemName += String.IsNullOrWhiteSpace(namespaceItemName) ? String.Empty : ".";
                namespaceItemName += namespaceItem;
            }

            return namespaceScript.ToString();
        }

        public static string ToJsObjectDeclaration(this string objectName, string namespaceName = null)
        {
            return String.Format("{0}{1}{2}={1}{2}||{{}};", 
                                 String.IsNullOrWhiteSpace(namespaceName) ? "var " : String.Empty,
                                 String.IsNullOrWhiteSpace(namespaceName) ? string.Empty : namespaceName + ".", 
                                 objectName);
        }

        //public static string ToMultiLineText(this string text)
        //{
        //    if (text.IsNullOrWhiteSpace())
        //        return text;

        //    return text.Replace(Environment.NewLine, "<br />")
        //               .Replace("\n", "<br />")
        //               .Replace("\r", "<br />")
        //               .Replace(Constantes.TokenNovaLinha.ToString(), "<br />")
        //               .Replace(HttpUtility.HtmlEncode(Constantes.TokenNovaLinha.ToString()), "<br />");
        //}

        //public static string ToTextAreaText(this string text)
        //{
        //    if (text.IsNullOrWhiteSpace())
        //        return text;

        //    return text.Replace(Constantes.TokenNovaLinha.ToString(), "\n");
        //}
    }
}