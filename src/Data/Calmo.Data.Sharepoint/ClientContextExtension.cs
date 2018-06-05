using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using Calmo.Core.ExceptionHandling;
using Microsoft.SharePoint.Client;

namespace Calmo.Data.Sharepoint
{
    public static class ClientContextExtension
    {
        #region ClientContext

        public static void ExecuteLoad<T>(this ClientContext context, T clientObject, params Expression<Func<T, object>>[] retrievals) where T : ClientObject
        {
            context.Load(clientObject, retrievals);
            context.ExecuteQuery();
        }

        public static void ExecuteLoad<T>(this ClientRuntimeContext context, T clientObject, params Expression<Func<T, object>>[] retrievals) where T : ClientObject
        {
            context.Load(clientObject, retrievals);
            context.ExecuteQuery();
        }

        public static string GetUserName(this ClientContext context, FieldUserValue userValue)
        {
            Throw.IfArgumentNull(userValue, "user");

            var userInfoList = context.Web.SiteUserInfoList;

            context.Load(userInfoList);
            var query = new CamlQuery
            {
                ViewXml = "<View Scope='RecursiveAll'><Query><Where><Eq><FieldRef Name='ID' /><Value Type='int'>" + userValue.LookupId + "</Value></Eq></Where></Query></View>"
            };

            var users = userInfoList.GetItems(query);
            context.ExecuteLoad(users, items => items.Include(
                item => item.Id,
                item => item["Name"]));

            var user = users.GetById(userValue.LookupId);

            context.ExecuteLoad(user);

            return user["Name"] as string;
        }

        #endregion

        #region FieldCollection

        public static void AddField(this FieldCollection fields, FieldType type, string displayName, string internalName, AddFieldOptions addFieldOptions, bool addToDefaultView, bool? readOnly = null)
        {
            var field = new XElement("Field");
            field.SetAttributeValue("Type", type);
            field.SetAttributeValue("DisplayName", displayName);
            field.SetAttributeValue("Name", internalName);

            if (readOnly.HasValue)
                field.SetAttributeValue("ReadOnly", readOnly);

            fields.AddFieldAsXml(field.ToString(), addToDefaultView, addFieldOptions);
        }

        #endregion
    }
}
