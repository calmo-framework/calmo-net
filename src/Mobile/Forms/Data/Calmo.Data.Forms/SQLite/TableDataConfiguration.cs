using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Calmo.Data.Forms
{
    public class TableDataConfiguration
    {
        public TableDataConfiguration()
        {
            this.Fields = new List<TableFieldDataConfiguration>();
            this.ForeignKeys = new List<TableForeignKeyDataConfiguration>();
        }

        internal string TableName { get; set; }
        internal List<TableFieldDataConfiguration> Fields { get; set; }
        internal List<TableForeignKeyDataConfiguration> ForeignKeys { get; set; }

        public TableDataConfiguration Name(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("Table name cannot be empty.");

            this.TableName = name;

            return this;
        }

        public TableDataConfiguration Field(string name, SQLiteDbType type, bool isPrimaryKey = false, bool autoIncrement = false)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new Exception("Field name cannot be empty.");

            if (this.Fields.Any(f => f.Name == name))
                throw new Exception($"Field {name} already exists.");

            var field = new TableFieldDataConfiguration
            {
                Name = name,
                Type = type,
                IsPrimaryKey = isPrimaryKey,
                AutoIncrement = autoIncrement
            };

            this.Fields.Add(field);

            return this;
        }

        public TableDataConfiguration ForeignKey(string fieldName, string tableNameFrom, string fieldNameFrom)
        {
            if (this.Fields.All(f => f.Name != fieldName))
                throw new Exception($"Field {fieldName} referenced on foreing key doesn't exists.");

            if (this.ForeignKeys.Any(f => f.FieldName == fieldName))
                throw new Exception($"Field {fieldName} already has a foreign key referenced.");

            var foreignKey = new TableForeignKeyDataConfiguration
            {
                FieldName = fieldName,
                TableNameFrom = tableNameFrom,
                FieldNameFrom = fieldNameFrom
            };

            this.ForeignKeys.Add(foreignKey);

            return this;
        }

        internal string GetCreateCommand(IEnumerable<TableDataConfiguration> tables)
        {
            var sb = new StringBuilder();
            sb.Append($"CREATE TABLE {this.TableName}(");

            var keys = this.Fields.Where(f => f.IsPrimaryKey).Select(f => f.Name).ToArray();

            var firstField = true;
            foreach (var field in this.Fields)
            {
                if (!firstField)
                    sb.Append(",");

                if (firstField) firstField = false;

                sb.Append($"{field.Name} {field.Type.GetSQLiteDataType()} ");

                if (field.IsPrimaryKey && keys.Length <= 1)
                    sb.Append("primary key ");

                if (field.AutoIncrement)
                    sb.Append("autoincrement");
            }

            if (keys.Length > 1)
                sb.Append($",PRIMARY KEY ({String.Join(",", keys)})");

            if (this.ForeignKeys.HasItems())
            {
                foreach (var foreignKey in this.ForeignKeys)
                {
                    if (tables.HasItems())
                    {
                        if (tables.All(t => t.TableName != foreignKey.TableNameFrom))
                            throw new Exception($"Table from '{foreignKey.TableNameFrom}' referenced on foreing key doesn't exists.");

                        if (tables.First(t => t.TableName == foreignKey.TableNameFrom).Fields.All(f => f.Name != foreignKey.FieldNameFrom))
                            throw new Exception($"Field '{foreignKey.FieldNameFrom}' referenced on foreing key doesn't exists on '{foreignKey.TableNameFrom}' table.");

                        sb.Append($",FOREIGN KEY({foreignKey.FieldName}) REFERENCES {foreignKey.TableNameFrom}({foreignKey.FieldNameFrom})");
                    }
                }
            }

            sb.Append(")");

            return sb.ToString();
        }
    }
}