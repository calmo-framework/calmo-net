using System;

namespace Calmo.Web.Api.OAuth
{
    public class OnGrantAccessExceptionEventArgs : EventArgs
    {
        public OnGrantAccessExceptionEventArgs(Exception exception)
        {
            this.Exception = exception;
        }

        public Exception Exception { get; }
    }
}
