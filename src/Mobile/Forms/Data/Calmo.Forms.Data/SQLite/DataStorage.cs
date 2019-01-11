using System;
using SQLite;
using Xamarin.Forms;

namespace Calmo.Data.Forms
{
    public class DataStorage
    {
        internal static bool Initialized { get; set; }
        internal static string DatabaseName { get; set; }

        public static DataConfiguration Configure(string databaseName)
        {
            return new DataConfiguration(databaseName);
        }

        internal static SQLiteConnection GetConnection(out bool fileExists)
        {
            var dataStorage = DependencyService.Get<ISQLiteDataStorage>();
            var databasePath = dataStorage.GetDatabasePath(DatabaseName);
            fileExists = dataStorage.DatabaseFileExists(databasePath);

            SQLiteConnection connection;

            if (DataScope.HasActiveScope)
            {
                if (DataScope.ScopeConnection == null)
                {
                    connection = new SQLiteConnection(databasePath);
                    try
                    {
                        if (!connection.IsInTransaction)
                            connection.BeginTransaction();
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                    connection = DataScope.ScopeConnection;
            }
            else
            {
                connection = new SQLiteConnection(databasePath);
            }

            return connection;
        }

        internal static SQLiteConnection GetConnection()
        {
            bool fileExists;
            return GetConnection(out fileExists);
        }
    }
}
