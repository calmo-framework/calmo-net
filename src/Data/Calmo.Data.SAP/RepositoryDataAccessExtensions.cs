namespace Calmo.Data.SAP
{
    public static class RepositoryDataAccessExtensions
    {
        public static SAPDataAccessQuery SAP(this RepositoryDataAccess data)
        {
            return new SAPDataAccessQuery();
        }
    }
}