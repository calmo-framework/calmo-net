using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Calmo.Core;
using Calmo.Core.ExceptionHandling;
using Microsoft.SharePoint.Client;

namespace Calmo.Data.Sharepoint
{
    public static class RepositorySharepointAccessExtensions
    {
        public static RepositorySharepointAccessQuery Sharepoint(this RepositoryDataAccess data)
        {
            return new RepositorySharepointAccessQuery();
        }
    }

    public class RepositorySharepointAccessQuery
    {
        public RepositorySharepointAccess UseMapping(object fieldsMapping)
        {
            Throw.IfArgumentNull(fieldsMapping, nameof(fieldsMapping));

            return new RepositorySharepointAccess(fieldsMapping);
        }
    }

    public class RepositorySharepointAccess
    {
        private readonly object _fieldsMapping;

        internal RepositorySharepointAccess(object fieldsMapping)
        {
            _fieldsMapping = fieldsMapping;
        }

        internal static ClientContext GetContext()
        {
            var context = new ClientContext(CustomConfiguration.Settings.Sharepoint().Url)
            {
                AuthenticationMode = ClientAuthenticationMode.Default,
                Credentials = CustomConfiguration.Settings.Sharepoint().GetServiceCredentials()
            };

            return context;
        }

        internal static Dictionary<string, FieldMappingDefinition> GetFieldsMappings(object fieldsMapping)
        {
            var mapType = fieldsMapping.GetType();
            var mapProps = mapType.GetProperties();
            var fieldMappingDefinitionType = typeof(FieldMappingDefinition);

            return mapProps.ToDictionary(prop => prop.Name, prop =>
            {
                if (prop.PropertyType == fieldMappingDefinitionType)
                {
                    var definition = prop.GetValue(fieldsMapping) as FieldMappingDefinition;
                    return definition;
                }

                return FieldMappingDefinition.GetField(Convert.ToString(prop.GetValue(fieldsMapping)));
            });
        }

        public SharepointQueryable<T> List<T>(string list)
        {
            return new SharepointQueryable<T>(list, _fieldsMapping);
        }

        public void Create<T>(string list, T item)
        {
            Throw.IfArgumentNull(item, nameof(item));

            var listFieldsMapping = GetFieldsMappings(_fieldsMapping);
            var itemType = typeof(T);

            var clientContext = GetContext();
            var oList = clientContext.Web.Lists.GetByTitle(list);

            var itemCreateInfo = new ListItemCreationInformation();
            var oListItem = oList.AddItem(itemCreateInfo);

            if (!listFieldsMapping.ContainsKey("Title"))
                oListItem["Title"] = "-";

            foreach (var fieldMap in listFieldsMapping)
            {
                var prop = itemType.GetProperty(fieldMap.Key);
                if (prop != null)
                {
                    var value = prop.GetValue(item);

                    if (fieldMap.Value.IsUserField)
                    {
                        var user = clientContext.Web.EnsureUser(value as string);
                        clientContext.Load(user);
                        clientContext.ExecuteQuery();

                        var userField = new FieldUserValue { LookupId = user.Id };
                        oListItem[fieldMap.Value.Name] = userField;
                    }
                    else if (fieldMap.Value.IsLookupField)
                    {
                        var lookupList = clientContext.Web.Lists.GetByTitle(fieldMap.Value.LookupListName);
                        var camlQuery = new CamlQuery { ViewXml = GetLookupCamlQuery(fieldMap.Value, value) };

                        var collListItem = lookupList.GetItems(camlQuery);

                        clientContext.Load(collListItem);
                        clientContext.ExecuteQuery();

                        var lookup = collListItem.Count > 0 ? collListItem[0] : null;
                        if (lookup == null)
                            throw new Exception(String.Format("O valor informado não encontrado na Lista referênciada ({0})", fieldMap.Value.LookupListName));

                        var lookupField = new FieldLookupValue { LookupId = lookup.Id };
                        oListItem[fieldMap.Value.Name] = lookupField;
                    }
                    else
                    {
                        oListItem[fieldMap.Value.Name] = value;
                    }
                }

                oListItem.Update();

                clientContext.ExecuteQuery();
            }
        }

        #region Private methods

        private static string GetLookupCamlQuery(FieldMappingDefinition definition, object value)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            var sw = new StringWriter();
            sw.Write("<View><Query><Where>");
            sw.WriteLine("<{0}><FieldRef Name=\"{1}\" /><Value Type=\"{2}\">{3}</Value></{0}>", CalmQueryItemType.Equal.GetCalmTag(), definition.LookupFieldKey, (value is string) ? "Text" : "Number", value);
            sw.Write("</Where></Query></View>");

            return sw.ToString();
        }

        #endregion
    }
}
