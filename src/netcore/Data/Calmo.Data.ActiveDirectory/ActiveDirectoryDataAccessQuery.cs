using Microsoft.Extensions.Configuration;

namespace Calmo.Data.ActiveDirectory
{
    public class ActiveDirectoryDataAccessQuery
    {
        private readonly ActiveDirectoryObjectType _activeDirectoryObjectType;
        private object _parameters;
        private bool _loadAllProperties = false;

        internal ActiveDirectoryDataAccessQuery(ActiveDirectoryObjectType activeDirectoryObjectType)
        {
            _activeDirectoryObjectType = activeDirectoryObjectType;
        }

        public ActiveDirectoryDataAccessQuery LoadAllProperties(bool loadAllProperties)
        {
            _loadAllProperties = loadAllProperties;
            return this;
        }

        public ActiveDirectoryDataAccess WithParameters(object parameters)
        {
            _parameters = parameters;

            return new ActiveDirectoryDataAccess(_activeDirectoryObjectType, _parameters, _loadAllProperties);
        }
    }
}