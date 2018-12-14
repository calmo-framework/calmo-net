using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;

namespace Calmo.Data
{
    public class DbAccessResult<TResult> : DbAccessResult
    {
        public DbAccessResult(DynamicParameters parameters, TResult result) : base(parameters)
        {
            this.Result = result;
        }

        public TResult Result { get; }
    }

    public class DbAccessResult
    {
        private readonly DynamicParameters _parameters;

        public DbAccessResult(DynamicParameters parameters)
        {
            _parameters = parameters;
        }

        public T GetOutput<T>(string name)
        {
            return _parameters.Get<T>(name);
        }
    }

    public class RepositoryDbAccessWithOutput : RepositoryDbAccess
    {
        public RepositoryDbAccessWithOutput(RepositoryDbAccess from)
        {
            var fromType = typeof(RepositoryDbAccess);
            
            var fieldParameters = fromType.GetField("_parameters", BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldOutput = fromType.GetField("_output", BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldCommandType = fromType.GetField("_commandType", BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldUseLongTimeout = fromType.GetField("_useLongTimeout", BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldBuffered = fromType.GetField("_buffered", BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldConnectionStringName = fromType.GetField("_connectionStringName", BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldCustomTimeout = fromType.GetField("_customTimeout", BindingFlags.NonPublic | BindingFlags.Instance);
            var fieldOutputParameters = fromType.GetField("_outputParameters", BindingFlags.NonPublic | BindingFlags.Instance);
            var propertyDbConnectionFactory = fromType.GetProperty("DbConnectionFactory");

            fieldParameters?.SetValue(this, fieldParameters.GetValue(from));
            fieldOutput?.SetValue(this, fieldOutput.GetValue(from));
            fieldCommandType?.SetValue(this, fieldCommandType.GetValue(from));
            fieldUseLongTimeout?.SetValue(this, fieldUseLongTimeout.GetValue(from));
            fieldBuffered?.SetValue(this, fieldBuffered.GetValue(from));
            fieldConnectionStringName?.SetValue(this, fieldConnectionStringName.GetValue(from));
            fieldCustomTimeout?.SetValue(this, fieldCustomTimeout.GetValue(from));
            fieldOutputParameters?.SetValue(this, fieldOutputParameters.GetValue(from));
            propertyDbConnectionFactory?.SetValue(this, propertyDbConnectionFactory.GetValue(from));
        }

        public new RepositoryDbAccessWithOutput UseConnection(string connectionStringName)
        {
            base.UseConnection(connectionStringName);
            return this;
        }

        public new RepositoryDbAccessWithOutput AsProcedure()
        {
            base.AsProcedure();
            return this;
        }

        public new RepositoryDbAccessWithOutput AsStatement()
        {
            base.AsStatement();
            return this;
        }

        public new RepositoryDbAccessWithOutput WithParameters(object parameters)
        {
            base.WithParameters(parameters);
            return this;
        }

        public new RepositoryDbAccessWithOutput UseLongTimeout()
        {
            base.UseLongTimeout();
            return this;
        }

        public new RepositoryDbAccessWithOutput WithCustomTimeout(int timeout)
        {
            base.WithCustomTimeout(timeout);
            return this;
        }

        public new RepositoryDbAccessWithOutput IsNotBuffered()
        {
            base.IsNotBuffered();
            return this;
        }

        #region List

        public new DbAccessResult<IEnumerable<dynamic>> List(string sql)
        {
            var result = base.List(sql);
            return new DbAccessResult<IEnumerable<dynamic>>(base.GetOutputParameters(), result);
        }
        
        public new DbAccessResult<IEnumerable<T>> List<T>(string sql)
        {
            var result = base.List<T>(sql);
            return new DbAccessResult<IEnumerable<T>>(base.GetOutputParameters(), result);
        }

        public new DbAccessResult<Tuple<IEnumerable<T1>, IEnumerable<T2>>> List<T1, T2>(string sql)
        {
            var result = base.List<T1, T2>(sql);
            return new DbAccessResult<Tuple<IEnumerable<T1>, IEnumerable<T2>>>(base.GetOutputParameters(), result);
        }

        public new DbAccessResult<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> List<T1, T2, T3>(string sql)
        {
            var result = base.List<T1, T2, T3>(sql);
            return new DbAccessResult<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>>(base.GetOutputParameters(), result);
        }

        public new DbAccessResult<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>> List<T1, T2, T3, T4>(string sql)
        {
            var result = base.List<T1, T2, T3, T4>(sql);
            return new DbAccessResult<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>>(base.GetOutputParameters(), result);
        }

#endregion

#region List Async

        public new async Task<DbAccessResult<IEnumerable<T>>> ListAsync<T>(string sql)
        {
            var result = await base.ListAsync<T>(sql);
            return new DbAccessResult<IEnumerable<T>>(base.GetOutputParameters(), result);
        }

#endregion

#region Get

        public new DbAccessResult<T> Get<T>(string sql)
        {
            var result = base.Get<T>(sql);
            return new DbAccessResult<T>(base.GetOutputParameters(), result);
        }

#endregion

#region Get Async

        public new async Task<DbAccessResult<T>> GetAsync<T>(string sql)
        {
            var result = await base.GetAsync<T>(sql);
            return new DbAccessResult<T>(base.GetOutputParameters(), result);
        }

#endregion

#region Execute

        public new DbAccessResult<T> Execute<T>(string sql)
        {
            var result = base.Execute<T>(sql);
            return new DbAccessResult<T>(base.GetOutputParameters(), result);
        }

        public new DbAccessResult Execute(string sql)
        {
            base.Execute(sql);
            return new DbAccessResult(base.GetOutputParameters());
        }

#endregion

#region Execute Async

        public new async Task<DbAccessResult<T>> ExecuteAsync<T>(string sql)
        {
            var result = await base.ExecuteAsync<T>(sql);
            return new DbAccessResult<T>(base.GetOutputParameters(), result);
        }

        public new async Task<DbAccessResult> ExecuteAsync(string sql)
        {
            await base.ExecuteAsync(sql);
            return new DbAccessResult(base.GetOutputParameters());
        }

#endregion
    }
}
