using System;
using System.Data;

namespace Calmo.Data.Forms
{
    internal static class DbTypeExtensions
    {
        public static string GetSQLiteDataType(this SQLiteDbType dbType)
        {
            switch (dbType)
            {
                case SQLiteDbType.String:
                    return "TEXT";
                    
                case SQLiteDbType.Decimal:
                    return "REAL";

                case SQLiteDbType.Integer:
                    return "INTEGER";
                    
                case SQLiteDbType.Object:
                    return "BLOB";

                default:
                    throw new ArgumentOutOfRangeException(nameof(dbType), dbType, null);
            }
        }
    }
}