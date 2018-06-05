namespace Calmo.Data.Forms
{
    public interface ISQLiteDataStorage
    {
        string GetDatabasePath(string databaseName);
        bool DatabaseFileExists(string path);
    }
}
