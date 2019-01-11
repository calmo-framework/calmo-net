namespace Calmo.Data.Forms
{
    public abstract class Repository
    {
        public RepositoryDataAccess Data { get; } = new RepositoryDataAccess();
    }
}
