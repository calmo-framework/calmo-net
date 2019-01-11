using Calmo.Data.Forms.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteDataStorage))]
namespace Calmo.Data.Forms.Droid
{
    using System;
    using System.IO;

    public class SQLiteDataStorage : ISQLiteDataStorage
    {
        public string GetDatabasePath(string databaseName)
        {
            var sqliteFilename = $"{databaseName}.db3";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            return Path.Combine(documentsPath, sqliteFilename);
        }

        public bool DatabaseFileExists(string path)
        {
            return File.Exists(path);
        }
    }
}