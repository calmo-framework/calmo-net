using Calmo.Core.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calmo.Data.SAP
{
    public class SAPDataAccessQuery
    {
        internal const string DefaultTableName = "###D3f4ultT4bl3N4m3###";

        internal object Parameters { get; set; }
        internal string BapiName { get; set; }

        internal Dictionary<string, KeyValuePair<Type, object>> FieldDataConfig { get; set; }

        internal SAPDataAccessQuery()
        {

        }

        public SAPDataAccessQuery Bapi(string bapiName)
        {
            this.BapiName = bapiName;
            return this;
        }

        public SAPDataAccessQuery WithParameters(object parameters)
        {
            this.Parameters = parameters;
            return this;
        }

        public SAPDataAccess UseMapping<T>(string table, Func<FieldDataConfig<T>, FieldDataConfig<T>> func)
        {
            Throw.IfArgumentNullOrEmpty(table, nameof(table));

            this.SetFieldDataConfig(func, table);

            return new SAPDataAccess(this);
        }

        public SAPDataAccess UseMapping<T>(Func<FieldDataConfig<T>, FieldDataConfig<T>> func)
        {
            this.SetFieldDataConfig(func);

            return new SAPDataAccess(this);
        }

        internal void SetFieldDataConfig<T>(Func<FieldDataConfig<T>, FieldDataConfig<T>> func, string table = null)
        {
            if (String.IsNullOrWhiteSpace(table))
                table = DefaultTableName;

            if (this.FieldDataConfig == null)
                this.FieldDataConfig = new Dictionary<string, KeyValuePair<Type, object>>();

            var value = func.Invoke(new FieldDataConfig<T>());
            var pair = new KeyValuePair<Type, object>(typeof(T), value);

            if (this.FieldDataConfig.Keys.Any(k => k == table))
                this.FieldDataConfig[table] = pair;
            else
                this.FieldDataConfig.Add(table, pair);
        }
    }
}
