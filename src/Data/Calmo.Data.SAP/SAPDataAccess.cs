using System;
using System.Collections.Generic;
using System.Linq;
using Calmo.Core.ExceptionHandling;
using Calmo.Data.SAP.Properties;
using NSAPConnector;
using SAP.Middleware.Connector;

namespace Calmo.Data.SAP
{
    public class SAPDataAccess
    {
        private Type IgnoredType => typeof(bool?);

        private SAPDataAccessQuery _query;

        internal SAPDataAccess(SAPDataAccessQuery query)
        {
            _query = query;
        }

        public SAPDataAccess UseMapping<TMap>(string table, Func<FieldDataConfig<TMap>, FieldDataConfig<TMap>> func)
        {
            Throw.IfArgumentNullOrEmpty(table, nameof(table));

            _query.SetFieldDataConfig(func, table);

            return this;
        }

        public SAPDataAccess UseMapping<TMap>(Func<FieldDataConfig<TMap>, FieldDataConfig<TMap>> func)
        {
            _query.SetFieldDataConfig(func);

            return this;
        }

        #region List methods

        public IEnumerable<T> List<T>()
        {
            var result = this.ListMultiple<T, bool?, bool?, bool?, bool?>();

            return result.Item1;

            //this.ValidateQuery();
            //this.ValidateFieldDataConfigValue<T>();

            //IEnumerable<T> items;

            //using (var connection = new SapConnection(GetConnectionConfig()))
            //{
            //    connection.Open();

            //    var command = this.GetCommand(connection);

            //    var configRaw = _query.FieldDataConfig.First();
            //    var config = new KeyValuePair<string, FieldDataConfig<T>>(configRaw.Key, (FieldDataConfig<T>)configRaw.Value);

            //    items = this.ExecuteQuery<T>(command, config);
            //}

            //return items;
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>()
        {
            var result = this.ListMultiple<T1, T2, bool?, bool?, bool?>();

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(result.Item1, result.Item2);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> List<T1, T2, T3>()
        {
            var result = this.ListMultiple<T1, T2, T3, bool?, bool?>();

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(result.Item1, result.Item2, result.Item3);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> List<T1, T2, T3, T4>()
        {
            var result = this.ListMultiple<T1, T2, T3, T4, bool?>();

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>(result.Item1, result.Item2, result.Item3, result.Item4);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> List<T1, T2, T3, T4, T5>()
        {
            return this.ListMultiple<T1, T2, T3, T4, T5>();
        }

        #endregion

        #region Get methods

        public T Get<T>()
        {
            return List<T>().FirstOrDefault();
        }

        public Tuple<T1, T2> Get<T1, T2>()
        {
            var result = this.List<T1, T2>();
            return new Tuple<T1, T2>(result.Item1.FirstOrDefault(), 
                                     result.Item2.FirstOrDefault());
        }

        public Tuple<T1, T2, T3> Get<T1, T2, T3>()
        {
            var result = this.List<T1, T2, T3>();
            return new Tuple<T1, T2, T3>(result.Item1.FirstOrDefault(), 
                                         result.Item2.FirstOrDefault(), 
                                         result.Item3.FirstOrDefault());
        }

        public Tuple<T1, T2, T3, T4> Get<T1, T2, T3, T4>()
        {
            var result = this.List<T1, T2, T3, T4>();
            return new Tuple<T1, T2, T3, T4>(result.Item1.FirstOrDefault(), 
                                             result.Item2.FirstOrDefault(), 
                                             result.Item3.FirstOrDefault(), 
                                             result.Item4.FirstOrDefault());
        }

        public Tuple<T1, T2, T3, T4, T5> Get<T1, T2, T3, T4, T5>()
        {
            var result = this.List<T1, T2, T3, T4, T5>();
            return new Tuple<T1, T2, T3, T4, T5>(result.Item1.FirstOrDefault(),
                                                 result.Item2.FirstOrDefault(),
                                                 result.Item3.FirstOrDefault(),
                                                 result.Item4.FirstOrDefault(),
                                                 result.Item5.FirstOrDefault());
        }

        #endregion

        #region Private methods

        private Dictionary<string, string> GetParameters(object parameters)
        {
            if (parameters == null) return null;

            if (parameters is Dictionary<string, string>)
                return (Dictionary<string, string>)parameters;

            if (parameters is Dictionary<string, object>)
            {
                var tempParameters = (Dictionary<string, object>)parameters;
                var temp = new Dictionary<string, string>();
                foreach (var paramKey in tempParameters.Keys)
                {
                    if (tempParameters[paramKey] == null)
                        temp.Add(paramKey, String.Empty);
                    else
                        temp.Add(paramKey, tempParameters[paramKey].ToString());
                }

                return temp;
            }

            var parametersType = parameters.GetType();
            if (parametersType.IsAnonymous())
            {
                var parametersDictionary = new Dictionary<string, string>();
                var properties = parametersType.GetProperties();

                foreach (var property in properties)
                {
                    if (!property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                        throw new InvalidCastException(String.Format(Messages.InvalidParameter, property.Name));

                    var value = property.GetValue(parameters);
                    if (value == null)
                        value = String.Empty;

                    parametersDictionary.Add(property.Name, value.ToString());
                }

                return parametersDictionary.HasItems() ? parametersDictionary : null;
            }

            throw new InvalidCastException(Messages.InvalidParameters);
        }

        private Dictionary<string, string> GetConnectionConfig()
        {
            //CustomConfiguration.Settings.SAP()
            return new Dictionary<string, string>
            {
                { RfcConfigParameters.AppServerHost, "10.18.96.246" },
                { RfcConfigParameters.SystemID, "QR3" },
                { RfcConfigParameters.Client, "300" },
                { RfcConfigParameters.User, "TPRFC" },
                { RfcConfigParameters.Password, "azbrasil" },
                { RfcConfigParameters.Language, "PT" },
                { RfcConfigParameters.Trace, "6" },
                { RfcConfigParameters.SystemNumber, "00" }
            };
        }

        private SapCommand GetCommand(SapConnection connection)
        {
            var command = new SapCommand(_query.BapiName, connection);
            var parametersDictionary = GetParameters(_query.Parameters);
            if (parametersDictionary.HasItems())
            {
                foreach (var key in parametersDictionary.Keys)
                {
                    command.Parameters.Add(key, parametersDictionary[key]);
                }
            }

            return command;
        }

        private int GetTypeQuantity<T1, T2, T3, T4, T5>()
        {
            int quantity = 0;
            if (typeof(T1) != IgnoredType)
                quantity++;
            if (typeof(T2) != IgnoredType)
                quantity++;
            if (typeof(T3) != IgnoredType)
                quantity++;
            if (typeof(T4) != IgnoredType)
                quantity++;
            if (typeof(T5) != IgnoredType)
                quantity++;

            return quantity;
        }

        private void ValidateQuery<T1,T2,T3,T4,T5>(int quantity = 1)
        {
            Throw.IfArgumentNullOrEmpty(_query.BapiName, "Bapi");
            if (_query.FieldDataConfig == null || !_query.FieldDataConfig.HasItems() || _query.FieldDataConfig.Count() != quantity)
                throw new Exception(Messages.InvalidMapping);

            for (var i = 0; i < quantity; i++)
            {
                var config = _query.FieldDataConfig.FirstOrDefault(c =>
                {
                    var pair = c.Value;
                    if (i == 0)
                        return pair.Key == typeof(T1);
                    else if (i == 1)
                        return pair.Key == typeof(T2);
                    else if (i == 2)
                        return pair.Key == typeof(T3);
                    else if (i == 3)
                        return pair.Key == typeof(T4);
                    else
                        return pair.Key == typeof(T5);
                });

                if (i == 0)
                    this.ValidateFieldDataConfigValue<T1>(config);
                else if (i == 1)
                    this.ValidateFieldDataConfigValue<T2>(config);
                else if (i == 2)
                    this.ValidateFieldDataConfigValue<T3>(config);
                else if (i == 3)
                    this.ValidateFieldDataConfigValue<T4>(config);
                else
                    this.ValidateFieldDataConfigValue<T5>(config);
            }
        }

        private void ValidateFieldDataConfigValue<TValidate>(KeyValuePair<string, KeyValuePair<Type, object>> config)
        {
            if (config.Equals(default(KeyValuePair<string, KeyValuePair<Type, object>>)))
                config = _query.FieldDataConfig.First();

            var pair = config.Value;
            if (pair.Key != typeof(TValidate))
                throw new Exception(Messages.InvalidMapping);

            if (!(pair.Value is FieldDataConfig<TValidate>))
                throw new Exception(Messages.InvalidMapping);


            var mapping = (FieldDataConfig<TValidate>)pair.Value;
            if (!mapping.Properties.HasItems())
                throw new Exception(Messages.InvalidMapping);
        }

        private KeyValuePair<string, KeyValuePair<Type, FieldDataConfig<TConfig>>> GetConfigByType<TConfig>()
        {
            var config = _query.FieldDataConfig.FirstOrDefault(c => c.Value.Key == typeof(TConfig));

            var pair = new KeyValuePair<Type, FieldDataConfig<TConfig>>(config.Value.Key, (FieldDataConfig<TConfig>)config.Value.Value);
            return new KeyValuePair<string, KeyValuePair<Type, FieldDataConfig<TConfig>>>(config.Key, pair);
        }

        private Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> ListMultiple<T1, T2, T3, T4, T5>()
        {
            var quantity = this.GetTypeQuantity<T1, T2, T3, T4, T5>();
            this.ValidateQuery<T1, T2, T3, T4, T5>(quantity);

            var items1 = Enumerable.Empty<T1>();
            var items2 = Enumerable.Empty<T2>();
            var items3 = Enumerable.Empty<T3>();
            var items4 = Enumerable.Empty<T4>();
            var items5 = Enumerable.Empty<T5>();

            using (var connection = new SapConnection(GetConnectionConfig()))
            {
                connection.Open();

                var command = this.GetCommand(connection);

                if (quantity >= 1)
                    items1 = this.ExecuteQuery<T1>(command);
                if (quantity >= 2)
                    items2 = this.ExecuteQuery<T2>(command);
                if (quantity >= 3)
                    items3 = this.ExecuteQuery<T3>(command);
                if (quantity >= 4)
                    items4 = this.ExecuteQuery<T4>(command);
                if (quantity >= 5)
                    items5 = this.ExecuteQuery<T5>(command);
            }

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>(items1, items2, items3, items4, items5);
        }

        private IEnumerable<TReturn> ExecuteQuery<TReturn>(SapCommand command)
        {
            Type t = typeof(TReturn);
            dynamic config = this.GetConfigByType<TReturn>();
            var items = new List<TReturn>();
            SapDataReader reader = command.ExecuteReader(config.Key != SAPDataAccessQuery.DefaultTableName ? config.Key : null);
            var mapping = (FieldDataConfig<TReturn>)config.Value.Value;

            while (reader.Read())
            {
                var item = Activator.CreateInstance<TReturn>();
                foreach (var property in mapping.Properties)
                {
                    var field = property.Key;
                    var data = reader.Item.GetString(property.Value);

                    field.SetValue(item, data);
                }

                items.Add(item);
            }

            return items.HasItems() ? items : Enumerable.Empty<TReturn>();
        }

        #endregion
    }
}
