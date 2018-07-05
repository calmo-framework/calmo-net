namespace Calmo.Data.ActiveDirectory
{
    public static class RepositoryDataAccessExtensions
    {
        public static ActiveDirectoryDataAccessConfig ActiveDirectory(this RepositoryDataAccess data)
        {
            return new ActiveDirectoryDataAccessConfig();
        }
    }
}