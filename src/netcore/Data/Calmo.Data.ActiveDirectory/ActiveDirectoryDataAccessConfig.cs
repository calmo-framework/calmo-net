namespace Calmo.Data.ActiveDirectory
{
    public class ActiveDirectoryDataAccessConfig
    {
        public ActiveDirectoryDataAccessQuery Users()
        {
            return new ActiveDirectoryDataAccessQuery(ActiveDirectoryObjectType.User);
        }
    }
}