using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Calmo.Core.Data;
using Calmo.Core.ExceptionHandling;
using Calmo.Core.Threading;
using Calmo.Data.Configuration;
using Dapper;

#if NETSTANDARD
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
#else
using Calmo.Core;
using Calmo.Data.Properties;
#endif

namespace Calmo.Data
{
    public class RepositoryDbAccess
    {
        private const int TOO_LONG_COMMANDTIMEOUT = 14400;

        private object _parameters;
        private Dictionary<string, DbType> _output;
        private CommandType _commandType = CommandType.StoredProcedure;
        private bool _useLongTimeout;
        private bool _buffered = true;
        private string _connectionStringName;
        private int _customTimeout = -1;
        private DynamicParameters _outputParameters;
        internal static string ForcedConnectionString;

#if !NETSTANDARD
        private static readonly DataSection DataSection = CustomConfiguration.Settings.Data();
#else
        private static DataSection DataSection;
        public static IConfiguration Configuration;
#endif

        public IDbConnectionFactory DbConnectionFactory { get; set; }

#if NETSTANDARD
        public RepositoryDbAccess()
        {
            var settings = new DataSection();
            Configuration.GetSection("calmoData").Bind(settings);
            DataSection = settings;
        }
#endif

#if !NETSTANDARD
        public string GetConnectionString(string name, string providerTypeName, string providerName)
        {
            var connectionStringData = ConfigurationManager.ConnectionStrings[name];

            if (connectionStringData == null)
                throw new ConfigurationErrorsException($"A conexão \"{name}\" não foi encontrada no arquivo de configuração.");

            if (!String.IsNullOrEmpty(connectionStringData.ProviderName))
            {
                if (!String.Equals(connectionStringData.ProviderName, providerTypeName, StringComparison.InvariantCultureIgnoreCase))
                    throw new InvalidOperationException(String.Format(Messages.IncorrectProvider, connectionStringData.ProviderName, name, providerName));
            }

            return connectionStringData.ConnectionString;
        }
#else
        public string GetConnectionString(string name)
        {
            if (!string.IsNullOrWhiteSpace(ForcedConnectionString))
                return ForcedConnectionString;

            return Configuration.GetConnectionString(name);
        }
#endif

        public RepositoryDbAccess UseConnection(string connectionStringName)
        {
            Throw.IfArgumentNullOrEmpty(connectionStringName, nameof(connectionStringName));

            _connectionStringName = connectionStringName;
            return this;
        }

        public RepositoryDbAccess AsProcedure()
        {
            _commandType = CommandType.StoredProcedure;
            return this;
        }

        public RepositoryDbAccess AsStatement()
        {
            _commandType = CommandType.Text;
            return this;
        }

        public RepositoryDbAccess WithParameters(object parameters)
        {
            _parameters = parameters;
            return this;
        }

        public RepositoryDbAccessWithOutput WithOutput(string name, DbType type)
        {
            if (_output == null)
                _output = new Dictionary<string, DbType>();

            if (_output.ContainsKey(name))
                _output[name] = type;
            else
                _output.Add(name, type);

            return new RepositoryDbAccessWithOutput(this);
        }

        public RepositoryDbAccess UseLongTimeout()
        {
            _useLongTimeout = true;
            return this;
        }

        public RepositoryDbAccess WithCustomTimeout(int timeout)
        {
            if (timeout <= 0) throw new InvalidOperationException("Invalid timeout");
            _customTimeout = timeout;
            return this;
        }

        public RepositoryDbAccess IsNotBuffered()
        {
            _buffered = false;
            return this;
        }

        private IDbConnection GetConnection()
        {
            IDbConnection connection;

            if (ScopeIsActive)
            {
                var transaction = ThreadStorage.GetData<IDbTransaction>(TransactionScope.ScopeTransactionKey);

                if (transaction != null)
                    connection = transaction.Connection;
                else
                {
                    connection = this.GetDbConnection();

                    connection.Open();

                    transaction = connection.BeginTransaction();
                    ThreadStorage.SetData(TransactionScope.ScopeTransactionKey, transaction);
                }
            }
            else
            {
                connection = this.GetDbConnection();

                connection.Open();
            }

            return connection;
        }

        private IDbConnection GetDbConnection()
        {
            Throw.IfReferenceNull(this.DbConnectionFactory, "DbConnectionFactory");

            if (_connectionStringName == null)
                _connectionStringName = DataSection.DefaultConnectionString;

            return this.DbConnectionFactory.GetDbConnection(_connectionStringName, this);
        }

        private bool ScopeIsActive => ThreadStorage.GetData<bool>(TransactionScope.ActiveScopeKey);

        private IDbTransaction ActiveTransaction
        {
            get
            {
                return !ScopeIsActive ? null : ThreadStorage.GetData<IDbTransaction>(TransactionScope.ScopeTransactionKey);
            }
        }

        private object GetParameters()
        {
            if (!_output.HasItems()) return _parameters;

            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(_parameters);
            foreach (var item in _output)
            {
                parameters.Add(item.Key, null, item.Value, ParameterDirection.Output);
            }

            _outputParameters = parameters;
            return parameters;
        }

        internal DynamicParameters GetOutputParameters()
        {
            return _outputParameters;
        }

#region List

        public IEnumerable<dynamic> List(string sql)
        {
            Throw.IfArgumentNullOrEmpty(sql, nameof(sql));

            IEnumerable<dynamic> result;

            var connection = this.GetConnection();

            try
            {
                result = connection.Query(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?) null),
                    commandType: _commandType,
                    buffered: _buffered,
                    transaction: this.ActiveTransaction);
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }

            return result;
        }
        
        public IEnumerable<T> List<T>(string sql)
        {
            Throw.IfArgumentNullOrEmpty(sql, nameof(sql));

            IEnumerable<T> result;

            var connection = this.GetConnection();

            try
            {
                result = connection.Query<T>(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?) null),
                    commandType: _commandType,
                    buffered: _buffered,
                    transaction: this.ActiveTransaction);
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }

            return result;
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string sql)
        {
            Throw.IfArgumentNullOrEmpty(sql, nameof(sql));

            IEnumerable<T1> result1;
            IEnumerable<T2> result2;

            var connection = this.GetConnection();

            try
            {
                var reader = connection.QueryMultiple(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?) null),
                    commandType: _commandType,
                    transaction: this.ActiveTransaction);

                result1 = reader.Read<T1>(_buffered);
                result2 = reader.Read<T2>(_buffered);
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(result1, result2);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> List<T1, T2, T3>(string sql)
        {
            Throw.IfArgumentNullOrEmpty(sql, nameof(sql));

            IEnumerable<T1> result1;
            IEnumerable<T2> result2;
            IEnumerable<T3> result3;

            var connection = this.GetConnection();

            try
            {
                var reader = connection.QueryMultiple(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?) null),
                    commandType: _commandType,
                    transaction: this.ActiveTransaction);

                result1 = reader.Read<T1>(_buffered);
                result2 = reader.Read<T2>(_buffered);
                result3 = reader.Read<T3>(_buffered);
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(result1, result2, result3);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> List<T1, T2, T3, T4>(string sql)
        {
            Throw.IfArgumentNullOrEmpty(sql, nameof(sql));

            IEnumerable<T1> result1;
            IEnumerable<T2> result2;
            IEnumerable<T3> result3;
            IEnumerable<T4> result4;

            var connection = this.GetConnection();

            try
            {
                var reader = connection.QueryMultiple(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?) null),
                    commandType: _commandType,
                    transaction: this.ActiveTransaction);

                result1 = reader.Read<T1>(_buffered);
                result2 = reader.Read<T2>(_buffered);
                result3 = reader.Read<T3>(_buffered);
                result4 = reader.Read<T4>(_buffered);
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>(result1, result2, result3, result4);
        }

#endregion

#region List Async

        public async Task<IEnumerable<T>> ListAsync<T>(string sql)
        {
            Throw.IfArgumentNullOrEmpty(sql, nameof(sql));

            IEnumerable<T> result;
            var connection = this.GetConnection();

            try
            {
                result = await connection.QueryAsync<T>(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?) null),
                    commandType: _commandType,
                    transaction: this.ActiveTransaction);
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }

            return result;
        }

#endregion

#region Get

        public T Get<T>(string sql)
        {
            Throw.IfArgumentNullOrEmpty(sql, nameof(sql));

            T result;
            var connection = this.GetConnection();

            try
            {
                result = connection.Query<T>(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?) null),
                    commandType: _commandType,
                    buffered: false,
                    transaction: this.ActiveTransaction).FirstOrDefault();
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }

            return result;
        }

#endregion

#region Get Async

        public async Task<T> GetAsync<T>(string sql)
        {
            Throw.IfArgumentNullOrEmpty(sql, nameof(sql));

            T result;
            var connection = this.GetConnection();

            try
            {
                var queryResult = await connection.QueryAsync<T>(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?)null),
                    commandType: _commandType,
                    transaction: this.ActiveTransaction);
                result = queryResult.FirstOrDefault();
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }

            return result;
        }

#endregion

#region Execute

        public T Execute<T>(string sql)
        {
            Throw.IfArgumentNullOrEmpty(sql, nameof(sql));

            var connection = GetConnection();

            try
            {
                return connection.Query<T>(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?) null),
                    commandType: _commandType,
                    buffered: false,
                    transaction: this.ActiveTransaction).FirstOrDefault();
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }
        }

        public void Execute(string sql)
        {
            Throw.IfArgumentNullOrEmpty(sql, nameof(sql));

            var connection = GetConnection();

            try
            {
                connection.Execute(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?)null),
                    commandType: _commandType,
                    transaction: this.ActiveTransaction);
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }
        }

#endregion

#region Execute Async

        public async Task<T> ExecuteAsync<T>(string sql)
        {
            Throw.IfArgumentNullOrEmpty(sql, nameof(sql));

            var connection = GetConnection();

            try
            {
                var queryResult = await connection.QueryAsync<T>(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?) null),
                    commandType: _commandType,
                    transaction: this.ActiveTransaction);

                return queryResult.FirstOrDefault();
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }
        }

        public async Task ExecuteAsync(string sql)
        {
            var connection = GetConnection();

            try
            {
                await connection.ExecuteAsync(sql, GetParameters(),
                    commandTimeout: _customTimeout > 0
                        ? _customTimeout
                        : (_useLongTimeout ? TOO_LONG_COMMANDTIMEOUT : (int?) null),
                    commandType: _commandType,
                    transaction: this.ActiveTransaction);
            }
            finally
            {
                if (!ScopeIsActive)
                    connection.Dispose();
            }
        }

#endregion

#region Paginagion methods (in future)

        //private const string SplitOnColumnName = "RowNumber";

        //public int GetPageInitialIndex(int page)
        //{
        //    return this.GetPageInitialIndex(page, DataSection.PageSize);
        //}

        //public int GetPageInitialIndex(int page, int pageSize)
        //{
        //    if (pageSize <= 0)
        //        pageSize = 10;

        //    return ((page - 1) * pageSize) + 1;
        //}

        //public int GetPageFinalIndex(int page)
        //{
        //    return this.GetPageFinalIndex(page, DataSection.PageSize);
        //}

        //public int GetPageFinalIndex(int page, int pageSize)
        //{
        //    if (pageSize <= 0)
        //        pageSize = 10;

        //    return page * pageSize;
        //}

        //public int GetDefaultPageSize()
        //{
        //    var pageSize = DataSection.PageSize;

        //    return pageSize <= 0 ? 10 : pageSize;
        //}

#endregion
    }
}
