using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using Calmo.Core;
using Calmo.Data.ActiveDirectory.Properties;

namespace Calmo.Data.ActiveDirectory
{
    internal enum ActiveDirectoryObjectType
    {
        User
    }

    public static class RepositoryDataAccessExtensions
    {
        public static ActiveDirectoryDataAccessConfig ActiveDirectory(this RepositoryDataAccess data)
        {
            return new ActiveDirectoryDataAccessConfig();
        }
    }

    public class ActiveDirectoryDataAccessConfig
    {
        public ActiveDirectoryDataAccessQuery Users()
        {
            return new ActiveDirectoryDataAccessQuery(ActiveDirectoryObjectType.User);
        }
    }

    public class ActiveDirectoryDataAccessQuery
    {
        private readonly ActiveDirectoryObjectType _activeDirectoryObjectType;
        private object _parameters;

        internal ActiveDirectoryDataAccessQuery(ActiveDirectoryObjectType activeDirectoryObjectType)
        {
            _activeDirectoryObjectType = activeDirectoryObjectType;
        }

        public ActiveDirectoryDataAccess WithParameters(object parameters)
        {
            _parameters = parameters;
            return new ActiveDirectoryDataAccess(_activeDirectoryObjectType, _parameters);
        }
    }

    public class ActiveDirectoryDataAccess
    {
        private ActiveDirectoryObjectType _activeDirectoryObjectType;
        private object _parameters;
        private bool _checkAll = true;

        internal ActiveDirectoryDataAccess(ActiveDirectoryObjectType activeDirectoryObjectType, object parameters)
        {
            _activeDirectoryObjectType = activeDirectoryObjectType;
            _parameters = parameters;
        }

        public ActiveDirectoryDataAccess CheckOneOrMoreParameters()
        {
            _checkAll = false;
            return this;
        }

        private readonly Dictionary<string, string> _validParameterNames = new Dictionary<string, string>
            {
                {"sAMAccountName", "login"},
                {"mail", "email"},
                {"name", "name"}
            };

        public IEnumerable<ActiveDirectoryUserInfo> List()
        {
            var parametersDictionary = GetParameters(_parameters);
            var searchPattern = GetSearchPattern(_activeDirectoryObjectType, parametersDictionary, _checkAll);

            return ExecuteSearch(searchPattern);
        }

        public ActiveDirectoryUserInfo Get()
        {
            return List().FirstOrDefault();
        }

        #region Private methods

        private Dictionary<string, object> GetParameters(object parameters)
        {
            if (parameters == null) return null;

            if (parameters is Dictionary<string, object>)
                return (Dictionary<string, object>)parameters;

            var parametersType = parameters.GetType();
            if (parametersType.IsAnonymous())
            {
                var parametersDictionary = new Dictionary<string, object>();
                var properties = parametersType.GetProperties();

                foreach (var property in properties)
                {
                    if (!property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                        throw new InvalidCastException(String.Format(Messages.InvalidParameter, property.Name));

                    if (_validParameterNames.Any(p => String.Equals(p.Key, property.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        parametersDictionary.Add(property.Name, property.GetValue(parameters));
                        continue;
                    }

                    if (_validParameterNames.Any(p => String.Equals(p.Value, property.Name, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        parametersDictionary.Add(_validParameterNames.First(p => String.Equals(p.Value, property.Name, StringComparison.InvariantCultureIgnoreCase)).Key, property.GetValue(parameters));
                        continue;
                    }

                    throw new InvalidCastException(String.Format(Messages.InvalidParameterName, property.Name));
                }

                return parametersDictionary.HasItems() ? parametersDictionary : null;
            }

            throw new InvalidCastException(Messages.InvalidParameters);
        }

        private static string GetSearchPattern(ActiveDirectoryObjectType objectType, Dictionary<string, object> parameters, bool checkAll)
        {
            var searchPattern = String.Empty;

            if (objectType == ActiveDirectoryObjectType.User)
                searchPattern = "(&(objectClass=user){0})";

            var parametersPattern = String.Empty;
            if (parameters != null)
                parametersPattern = parameters.Aggregate(parametersPattern, (pattern, param) => pattern + String.Format("({0}={1})", param.Key, param.Value));

            return String.Format(searchPattern, !String.IsNullOrWhiteSpace(parametersPattern)
                                                ? String.Format("({0} {1})", checkAll ? "&" : "|", parametersPattern)
                                                : String.Empty);
        }

        private static IEnumerable<ActiveDirectoryUserInfo> ExecuteSearch(string searchPattern)
        {
            var searcher = new DirectorySearcher();
            var connectionString = ConfigurationManager.ConnectionStrings[CustomConfiguration.Settings.ActiveDirectoryData().ConnectionStringName].ConnectionString;

            if (!String.IsNullOrWhiteSpace(CustomConfiguration.Settings.ActiveDirectoryData().UserName))
            {
                searcher.SearchRoot = new DirectoryEntry(connectionString,
                                                         CustomConfiguration.Settings.ActiveDirectoryData().UserName,
                                                         CustomConfiguration.Settings.ActiveDirectoryData().Password);
                //"resource_brsp\\webmotors.tv", "resource@123");
            }
            else
            {
                searcher.SearchRoot = new DirectoryEntry(connectionString);
            }

            searcher.PropertiesToLoad.Add("mail");
            searcher.PropertiesToLoad.Add("name");
            searcher.PropertiesToLoad.Add("sAMAccountName");
            searcher.PropertiesToLoad.Add("memberOf");
            searcher.Filter = searchPattern;

            var collection = searcher.FindAll();
            var logins = new List<ActiveDirectoryUserInfo>();
            foreach (SearchResult result in collection)
            {
                logins.Add(FillUserInfo(result));
            }

            return logins;
        }

        private static ActiveDirectoryUserInfo FillUserInfo(SearchResult searchResult)
        {
            var userInfo = new ActiveDirectoryUserInfo();

            var resultCollection = searchResult.Properties["mail"];
            foreach (var result in resultCollection)
            {
                userInfo.Mail = result.ToString();
            }

            resultCollection = searchResult.Properties["memberOf"];
            var memberOf = new List<string>();
            foreach (object result in resultCollection)
            {
                memberOf.Add(result.ToString());
            }
            userInfo.MemberOf = memberOf.HasItems() ? memberOf.ToArray() : null;

            resultCollection = searchResult.Properties["name"];
            foreach (object result in resultCollection)
            {
                userInfo.Name = result.ToString();
            }

            resultCollection = searchResult.Properties["sAMAccountName"];
            foreach (object result in resultCollection)
            {
                userInfo.Login = result.ToString();
            }

            return userInfo;
        }

        #endregion
    }
}
