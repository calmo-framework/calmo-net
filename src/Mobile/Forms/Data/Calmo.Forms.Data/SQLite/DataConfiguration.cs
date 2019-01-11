using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace Calmo.Data.Forms
{
    public class DataConfiguration
    {
        private readonly string _databaseName;
        private readonly List<TableDataConfiguration> _tableConfigs;

        internal DataConfiguration(string databaseName)
        {
            this._databaseName = databaseName;
            this._tableConfigs = new List<TableDataConfiguration>();
        }

        public DataConfiguration Table(Action<TableDataConfiguration> tableConfig)
        {
            var config = new TableDataConfiguration();
            tableConfig.Invoke(config);

            if (String.IsNullOrWhiteSpace(config.TableName))
                throw new Exception("Table name cannot be empty.");

            if (this._tableConfigs.Any(c => c.TableName == config.TableName))
                throw new Exception($"Table {config.TableName} already exists.");
            this._tableConfigs.Add(config);

            return this;
        }

        public void Initialize()
        {
            DataStorage.DatabaseName = this._databaseName;

            bool exists;
            var connection = DataStorage.GetConnection(out exists);

            if (!exists)
            {
                var tableCommands = this._tableConfigs.Select(c => c.GetCreateCommand(this._tableConfigs)).ToArray();
                foreach (var createTableCommand in tableCommands)
                {
                    var command = connection.CreateCommand(createTableCommand);
                    command.ExecuteNonQuery();
                }
            }

            DataStorage.Initialized = true;
        }
    }
}
