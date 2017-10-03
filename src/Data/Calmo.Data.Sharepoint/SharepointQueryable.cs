using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.SharePoint.Client;

namespace Calmo.Data.Sharepoint
{
    public class SharepointQueryable<T>
    {
        public SharepointQueryable(string listName, object fieldsMapping)
        {
            this.ListName = listName;
            this.FieldsMapping = RepositorySharepointAccess.GetFieldsMappings(fieldsMapping);
        }

        internal string ListName { get; set; }
        internal Dictionary<string, FieldMappingDefinition> FieldsMapping { get; set; }
        private List<CalmQueryItem> QueryItems { get; set; }

        public SharepointQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            this.HandleLinqExpression(predicate.Body, predicate.Parameters);

            return this;
        }

        private void HandleLinqExpression(Expression rootExpression, IEnumerable<ParameterExpression> parameters)
        {
            if (rootExpression is MethodCallExpression)
            {
                var expression = ((MethodCallExpression)rootExpression);

                this.HandleMethodCallExpression(expression, parameters);
            }
            else if (rootExpression is BinaryExpression)
            {
                var expression = ((BinaryExpression)rootExpression);
                if (expression.NodeType == ExpressionType.Equal
                    || expression.NodeType == ExpressionType.NotEqual
                    || expression.NodeType == ExpressionType.GreaterThan
                    || expression.NodeType == ExpressionType.GreaterThanOrEqual
                    || expression.NodeType == ExpressionType.LessThan
                    || expression.NodeType == ExpressionType.LessThanOrEqual)
                {
                    this.HandleBinaryExpression(expression);
                }
                else if (expression.NodeType == ExpressionType.AndAlso || expression.NodeType == ExpressionType.OrElse)
                {
                    this.HandleLinqExpression(expression.Left, parameters);
                    this.HandleLinqExpression(expression.Right, parameters);
                }
            }
        }

        private void HandleMethodCallExpression(MethodCallExpression expression, IEnumerable<ParameterExpression> parameters)
        {
            if ((expression.Method.Name == "Contains" || expression.Method.Name == "StartsWith") && expression.Object != null)
            {
                if (this.QueryItems == null) this.QueryItems = new List<CalmQueryItem>();

                var lambda = Expression.Lambda(expression.Arguments[0], parameters);
                var d = lambda.Compile();
                var value = d.DynamicInvoke(new object[1]);

                if (expression.Object == null) throw new Exception("Cláusula inválida.");

                var propertyInfo = (PropertyInfo)((MemberExpression)expression.Object).Member;
                var itemType = propertyInfo.PropertyType;
                var itemField = this.FieldsMapping.FirstOrDefault(m => m.Key == propertyInfo.Name).Value;

                this.QueryItems.Add(new CalmQueryItem
                {
                    IsAnd = true,
                    Type = expression.Method.Name == "Contains" ? CalmQueryItemType.Contains : CalmQueryItemType.BeginsWith,
                    Value = Convert.ToString(value),
                    ValueType = itemType,
                    FieldName = itemField.Name,
                    IsUserField = itemField.IsUserField,
                    IsLookupField = itemField.IsLookupField
                });
            }
            else if (expression.Method.Name == "IsNull" || expression.Method.Name == "IsNotNull")
            {
                if (this.QueryItems == null) this.QueryItems = new List<CalmQueryItem>();

                var itemField =
                    this.FieldsMapping.FirstOrDefault(m => m.Key == ((MemberExpression)expression.Arguments[0]).Member.Name)
                        .Value;

                this.QueryItems.Add(new CalmQueryItem
                {
                    IsAnd = true,
                    Type = expression.Method.Name == "IsNull" ? CalmQueryItemType.IsNull : CalmQueryItemType.IsNotNull,
                    FieldName = itemField.Name,
                    IsUserField = itemField.IsUserField,
                    IsLookupField = itemField.IsLookupField
                });
            }
        }

        private void HandleBinaryExpression(BinaryExpression expression, bool isAnd = true)
        {
            if (this.QueryItems == null) this.QueryItems = new List<CalmQueryItem>();

            string value = null;
            Type itemType = null;
            CalmQueryItemType type;

            var memberLeft = expression.Left as MemberExpression;
            var memberRight = expression.Right as MemberExpression;
            MemberExpression propertyMember = null;

            if (memberLeft != null && memberRight != null)
            {
                if ((memberLeft.Expression.NodeType == ExpressionType.Constant) || (memberRight.Expression.NodeType == ExpressionType.Constant))
                {
                    MemberExpression constantMember = null;

                    if (memberLeft.Expression.NodeType == ExpressionType.Constant)
                    {
                        propertyMember = memberRight;
                        constantMember = memberLeft;
                    }
                    else if (memberRight.Expression.NodeType == ExpressionType.Constant)
                    {
                        propertyMember = memberLeft;
                        constantMember = memberRight;
                    }

                    var constantExpression = (ConstantExpression) constantMember.Expression;
                    value = Convert.ToString(((FieldInfo) constantMember.Member).GetValue(constantExpression.Value));
                }
                else
                {
                    MemberExpression memberAccessMember = null;

                    if (memberLeft.Expression.NodeType == ExpressionType.MemberAccess)
                    {
                        propertyMember = memberRight;
                        memberAccessMember = memberLeft;
                    }
                    else if (memberRight.Expression.NodeType == ExpressionType.MemberAccess)
                    {
                        propertyMember = memberLeft;
                        memberAccessMember = memberRight;
                    }

                    var valuePropertyInfo = (PropertyInfo)memberAccessMember.Member;
                    var valueMember = (MemberExpression)memberAccessMember.Expression;
                    var valueField = (FieldInfo)valueMember.Member;
                    var constantExpression = (ConstantExpression)valueMember.Expression;
                    var valueObj = valueField.GetValue(constantExpression.Value);
                    value = (string)valuePropertyInfo.GetValue(valueObj, null);
                }

                type = expression.NodeType.ToCalmQueryItemType();
            }
            else
            {
                ConstantExpression constant;

                if (memberLeft == null)
                {
                    propertyMember = memberRight;
                    constant = expression.Left as ConstantExpression;
                }
                else
                {
                    propertyMember = memberLeft;
                    constant = expression.Right as ConstantExpression;
                }

                if (constant.Value == null &&
                    (expression.NodeType == ExpressionType.Equal || expression.NodeType == ExpressionType.NotEqual))
                {
                    type = expression.NodeType == ExpressionType.Equal ? CalmQueryItemType.IsNull : CalmQueryItemType.IsNotNull;
                }
                else
                {
                    type = expression.NodeType.ToCalmQueryItemType();
                    value = Convert.ToString(constant.Value);
                }
            }

            var propertyInfo = (PropertyInfo)propertyMember.Member;
            itemType = propertyInfo.PropertyType;
            var itemField = this.FieldsMapping.FirstOrDefault(m => m.Key == propertyInfo.Name).Value;

            this.QueryItems.Add(new CalmQueryItem
            {
                IsAnd = isAnd,
                Type = type,
                Value = value,
                ValueType = itemType,
                FieldName = itemField.Name,
                IsUserField = itemField.IsUserField,
                IsLookupField = itemField.IsLookupField
            });
        }

        public IEnumerable<T> Retrieve()
        {
            if (String.IsNullOrWhiteSpace(this.ListName))
                throw new Exception("Não foi selecionada uma lista para retornar os itens.");

            if (!this.FieldsMapping.HasItems())
                throw new Exception("Não foi definido o mapeamento de campos de retorno.");

            var clientContext = RepositorySharepointAccess.GetContext();
            var oList = clientContext.Web.Lists.GetByTitle(this.ListName);
            var camlQuery = new CamlQuery { ViewXml = this.GetCamlQuery() };

            /*
            var sw = new StringWriter();
            var oWebsite = clientContext.Web;
            var collList = oWebsite.Lists;
            var listInfo = clientContext.LoadQuery(
                collList.Include(
                    list => list.Title,
                    list => list.Fields.Include(field => field.Title, field => field.InternalName)));

            clientContext.ExecuteQuery();

            foreach (var list in listInfo)
            {
                var collField = list.Fields;

                foreach (var oField in collField)
                {
                    sw.WriteLine("List: {0} \n\t Field Title: {1} \n\t Field Internal Name: {2}", list.Title, oField.Title, oField.InternalName);
                }
            }
            var lalala = sw.ToString();
            */

            var collListItem = oList.GetItems(camlQuery);

            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();

            var itemType = typeof(T);
            var itens = new List<T>();

            foreach (var oListItem in collListItem)
            {
                var item = Activator.CreateInstance<T>();

                foreach (var fieldMap in this.FieldsMapping)
                {
                    var prop = itemType.GetProperty(fieldMap.Key);
                    if (prop != null && (oListItem[fieldMap.Value.Name] is FieldLookupValue) && oListItem[fieldMap.Value.Name] != null)
                        prop.SetValue(item, Convert.ChangeType(((FieldLookupValue)oListItem[fieldMap.Value.Name]).LookupValue, prop.PropertyType));
                    else if (prop != null && !(oListItem[fieldMap.Value.Name] is FieldUserValue) && oListItem[fieldMap.Value.Name] != null)
                        prop.SetValue(item, Convert.ChangeType(oListItem[fieldMap.Value.Name], prop.PropertyType));
                }

                itens.Add(item);
            }

            return itens.HasItems() ? itens : null;
        }

        public T RetrieveFirstOrDefault(T defaultValue = default(T))
        {
            var result = this.Retrieve();

            if (result.HasItems())
                return result.First();

            return defaultValue;
        }

        private string GetCamlQuery()
        {
            var sw = new StringWriter();
            sw.Write("<View>");

            if (this.QueryItems.HasItems())
            {
                sw.Write("<Query><Where>");

                string lastGrouping = null;
                foreach (var item in this.QueryItems)
                {
                    if (String.IsNullOrWhiteSpace(lastGrouping) && this.QueryItems.Count > 1)
                    {
                        lastGrouping = this.QueryItems[1].IsAnd ? "And" : "Or";

                        sw.WriteLine("<{0}>", lastGrouping);
                    }
                    else
                    {
                        var groupingInfo = item.IsAnd ? "And" : "Or";
                        if (lastGrouping != groupingInfo)
                            sw.WriteLine("</{0}>", lastGrouping);

                        lastGrouping = groupingInfo;
                    }

                    switch (item.Type)
                    {
                        case CalmQueryItemType.Contains:
                        case CalmQueryItemType.Equal:
                        case CalmQueryItemType.NotEqual:
                        case CalmQueryItemType.GreaterThan:
                        case CalmQueryItemType.GreaterThanOrEqual:
                        case CalmQueryItemType.LowerThan:
                        case CalmQueryItemType.LowerThanOrEqual:
                        case CalmQueryItemType.BeginsWith:
                            sw.WriteLine("<{0}><FieldRef Name=\"{1}\" {4}/><Value Type=\"{2}\">{3}</Value></{0}>", item.Type.GetCalmTag(), item.FieldName, item.GetValueTypeText(), item.Value, item.GetLookupText());
                            break;

                        case CalmQueryItemType.IsNull:
                        case CalmQueryItemType.IsNotNull:
                            sw.WriteLine("<{0}><FieldRef Name=\"{1}\" /></{0}>", item.Type.GetCalmTag(), item.FieldName);
                            break;
                    }
                }

                if (!String.IsNullOrWhiteSpace(lastGrouping))
                    sw.WriteLine("</{0}>", lastGrouping);

                sw.Write("</Where></Query>");
            }

            sw.Write("<ViewFields>");

            foreach (var item in this.FieldsMapping)
            {
                sw.WriteLine("<FieldRef Name=\"{0}\" />", item.Value.Name);
            }

            sw.Write("</ViewFields></View>");

            return sw.ToString();
        }

        private class CalmQueryItem
        {
            public bool IsAnd { get; set; }
            public CalmQueryItemType Type { get; set; }
            public string Value { get; set; }
            public Type ValueType { get; set; }
            public string FieldName { get; set; }
            public bool IsUserField { get; set; }
            public bool IsLookupField { get; set; }

            public string GetValueTypeText()
            {
                if (this.IsUserField)
                    return "User";
                if (this.ValueType == typeof(String))
                    return "Text";

                return "Number";
            }

            public string GetLookupText()
            {
                return this.IsLookupField ? "LookupId=\"True\"" : String.Empty;
            }
        }

        /*
        private class CalmQueryGrouping
        {
            public IList<CalmQueryItem> Items { get; set; }
            public CalmQueryGroupingType Type { get; set; }
        }

        private enum CalmQueryGroupingType
        {
            And,
            Or
        }
        */
    }

    public static class SharepointQueryableExtensions
    {
        public static bool IsNull(this int number)
        {
            return true;
        }

        public static bool IsNull(this long number)
        {
            return true;
        }

        public static bool IsNull(this decimal number)
        {
            return true;
        }

        public static bool IsNull(this short number)
        {
            return true;
        }

        public static bool IsNull(this float number)
        {
            return true;
        }
        public static bool IsNotNull(this int number)
        {
            return true;
        }

        public static bool IsNotNull(this long number)
        {
            return true;
        }

        public static bool IsNotNull(this decimal number)
        {
            return true;
        }

        public static bool IsNotNull(this short number)
        {
            return true;
        }

        public static bool IsNotNull(this float number)
        {
            return true;
        }
    }
}