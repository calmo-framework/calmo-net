using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Calmo.Data.Forms
{
    public class RepositoryDbAccess
    {
        private object _parameters;

        internal RepositoryDbAccess()
        {

        }

        public RepositoryDbAccess WithParameters(object parameters)
        {
            _parameters = parameters;
            return this;
        }

        public IEnumerable<T> List<T>(string sql) where T : new()
        {
            IEnumerable<T> result;

            var connection = DataStorage.GetConnection();

            try
            {
                var parametersArray = this.GetParameters(_parameters);
                result = connection.Query<T>(sql, parametersArray);
            }
            finally
            {
                if (!DataScope.HasActiveScope)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }

            return result;
        }

        public T Get<T>(string sql) where T : new()
        {
            return this.List<T>(sql).FirstOrDefault();
        }

        public T ExecuteScalar<T>(string sql)
        {
            var connection = DataStorage.GetConnection();

            try
            {
                var parametersArray = this.GetParameters(_parameters);
                return connection.ExecuteScalar<T>(sql, parametersArray);
            }
            finally
            {
                if (!DataScope.HasActiveScope)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public int Execute(string sql)
        {
            var connection = DataStorage.GetConnection();

            try
            {
                var parametersArray = this.GetParameters(_parameters);
                return connection.Execute(sql, parametersArray);
            }
            finally
            {
                if (!DataScope.HasActiveScope)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public bool CheckIfTableExists(string tableName)
        {
            return this.ExecuteScalar<int>($"SELECT count(name) FROM sqlite_master WHERE type='table' AND name='{tableName}'") > 0;
        }

        private object[] GetParameters(object parameters)
        {
            if (parameters == null) return new object[0];

            var parametersValues = new List<object>();

            var objectType = parameters.GetType();
            var properties = objectType.GetRuntimeProperties();
            foreach (var property in properties)
            {
                parametersValues.Add(property.GetValue(parameters));
            }

            return parametersValues.ToArray();
        }
    }
}