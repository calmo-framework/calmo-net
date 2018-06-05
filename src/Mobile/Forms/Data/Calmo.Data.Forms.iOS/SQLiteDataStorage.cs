using Calmo.Data.Forms.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteDataStorage))]
namespace Calmo.Data.Forms.iOS
{
    using System;
    using System.IO;

    public class SQLiteDataStorage : ISQLiteDataStorage
    {
        public string GetDatabasePath(string databaseName)
        {
            var sqliteFilename = $"{databaseName}.db3";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentsPath, "..", "Library");

            return  Path.Combine(libraryPath, sqliteFilename);
        }

        public bool DatabaseFileExists(string path)
        {
            return File.Exists(path);
        }
    }
}