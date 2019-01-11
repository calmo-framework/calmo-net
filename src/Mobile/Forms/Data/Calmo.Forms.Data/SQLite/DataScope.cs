using System;
using SQLite;

namespace Calmo.Data.Forms
{
    public class DataScope : IDisposable
    {
        internal static bool HasActiveScope;
        internal static SQLiteConnection ScopeConnection;

        private bool _complete;
        private bool _isNested;

        public DataScope()
        {
            if (HasActiveScope)
                this._isNested = true;

            HasActiveScope = true;
            ScopeConnection = DataStorage.GetConnection();
        }

        public void Complete()
        {
            this._complete = true;
        }

        public void Dispose()
        {
            try
            {

                if (this._isNested || !HasActiveScope || ScopeConnection == null) return;

                if (ScopeConnection.IsInTransaction)
                {
                    if (this._complete)
                        ScopeConnection.Commit();
                    else
                        ScopeConnection.Rollback();
                }

                ScopeConnection.Close();
                ScopeConnection.Dispose();
                ScopeConnection = null;
                HasActiveScope = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}