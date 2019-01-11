using System.Data;

namespace Calmo.Data.Forms
{
    public class TableFieldDataConfiguration
    {
        public string Name { get; set; }
        public SQLiteDbType Type { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool AutoIncrement { get; set; }
    }
}