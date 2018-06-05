using System.Collections.Generic;
using Calmo.Core.ExceptionHandling;

namespace System
{
    public static class ExceptionExtension
    {
        public static IEnumerable<String> GetErrors(this Exception exception)
        {
            if (exception is DomainException)
                return ((DomainException) exception).Errors;

            throw exception;
        }

        public static IEnumerable<String> GetErrorsToJson(this Exception exception)
        {
            if (exception is DomainException)
                return ((DomainException)exception).Errors;

            return new[] { exception.Message };
        }
    }
}
