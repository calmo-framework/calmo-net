using System;
using System.Data;
using Calmo.Core.Threading;

namespace Calmo.Core.Data
{
    public class TransactionScope : IDisposable
    {
        public const string ActiveScopeKey = "activeScope";
        public const string ScopeTransactionKey = "scopeTransaction";
        private bool _complete;

        public TransactionScope()
        {
            ThreadStorage.SetData(ActiveScopeKey, true);
        }

        public void Complete()
        {
            _complete = true;
        }

        public void Dispose()
        {
            var transaction = ThreadStorage.GetData<IDbTransaction>(ScopeTransactionKey);
            if (transaction == null || !ThreadStorage.GetData<bool>(ActiveScopeKey)) return;

            var connection = transaction.Connection;

            if (_complete)
                transaction.Commit();
            else
                transaction.Rollback();

            if (connection != null && connection.State != ConnectionState.Closed)
                connection.Dispose();

            ThreadStorage.ClearData(ActiveScopeKey);
            ThreadStorage.ClearData(ScopeTransactionKey);
        }
    }
}