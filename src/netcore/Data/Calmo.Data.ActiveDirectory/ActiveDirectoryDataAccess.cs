using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace Calmo.Data.ActiveDirectory
{
    public class ActiveDirectoryDataAccess
    {
        private readonly IConfiguration _configuration;
        private readonly ActiveDirectoryObjectType _activeDirectoryObjectType;
        private readonly object _parameters;
        private readonly bool _loadAllProperties;

        private bool _checkAll = true;

        internal ActiveDirectoryDataAccess(ActiveDirectoryObjectType activeDirectoryObjectType, object parameters, bool loadAllProperties)
        {
            _configuration = RepositoryDbAccess.Configuration;
            _activeDirectoryObjectType = activeDirectoryObjectType;
            _parameters = parameters;
            _loadAllProperties = loadAllProperties;
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
                        throw new InvalidCastException($"O parâmetro {property.Name} informado é inválido. São aceitos apenas parâmetros de tipos primitivos.");

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

                    throw new InvalidCastException($"O parâmetro {property.Name} informado é inválido. São aceitos apenas parâmetros de tipos primitivos.");
                }

                return parametersDictionary.HasItems() ? parametersDictionary : null;
            }

            throw new InvalidCastException("Os parâmetros informados são inválidos. São aceitos apenas parâmetros do tipo \"Dictionary<string, object>\" e tipos anônimos.");
        }

        private string GetSearchPattern(ActiveDirectoryObjectType objectType, Dictionary<string, object> parameters, bool checkAll)
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

        private IEnumerable<ActiveDirectoryUserInfo> ExecuteSearch(string searchPattern)
        {
            var settings = new AdDataSection();
            _configuration.GetSection(AdDataSection.SectionName).Bind(settings);

            var searcher = new DirectorySearcher();
            var connectionString = _configuration.GetConnectionString(settings.LdapConnectionString);

            if (!String.IsNullOrWhiteSpace(settings.LdapServiceAccount))
            {
                searcher.SearchRoot = new DirectoryEntry(connectionString, settings.LdapServiceAccount, settings.LdapServiceAccountPwd);
            }
            else
            {
                searcher.SearchRoot = new DirectoryEntry(connectionString);
            }

            if (!_loadAllProperties)
            {
                searcher.PropertiesToLoad.Add("mail");
                searcher.PropertiesToLoad.Add("name");
                searcher.PropertiesToLoad.Add("sAMAccountName");
                searcher.PropertiesToLoad.Add("memberOf");
                searcher.PropertiesToLoad.Add("givenname");
                searcher.PropertiesToLoad.Add("sn");
            }
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

            userInfo.Mail = searchResult.Properties["mail"].ReadValue();
            userInfo.MemberOf = searchResult.Properties["memberOf"].ReadValues();
            userInfo.Name = searchResult.Properties["name"].ReadValue();
            userInfo.Login = searchResult.Properties["sAMAccountName"].ReadValue();
            userInfo.FirstName = searchResult.Properties["givenname"].ReadValue();
            userInfo.LastName = searchResult.Properties["sn"].ReadValue();

            userInfo.Properties = searchResult.Properties.ReadValues();

            return userInfo;
        }

        #endregion
    } 

    public class AdDataSection
    {
        internal const string SectionName = "calmoData";

        public string LdapConnectionString { get; set; }
        public string LdapServiceAccount { get; set; }
        public string LdapServiceAccountPwd { get; set; }
    }
}
