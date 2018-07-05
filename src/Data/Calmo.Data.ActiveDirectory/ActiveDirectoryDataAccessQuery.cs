namespace Calmo.Data.ActiveDirectory
{
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
}