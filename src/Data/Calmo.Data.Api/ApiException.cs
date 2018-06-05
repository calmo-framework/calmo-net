using System;
using System.Net;

namespace Calmo.Data.Api
{
    public class ApiException : Exception
    {
        internal const string DEFAULT_MESSAGE = "Api request failed.";

        public ApiException() : base(ApiException.DEFAULT_MESSAGE)
        {

        }

        public ApiException(string message) : base(message)
        {
            
        }

        public ApiException(HttpStatusCode statusCode) : base(ApiException.DEFAULT_MESSAGE)
        {
            this.StatusCode = statusCode;
        }

        public ApiException(string message, HttpStatusCode statusCode) : base(message)
        {
            this.StatusCode = statusCode;
        }

        public ApiException(HttpStatusCode statusCode, dynamic jsonResult) : base(ApiException.DEFAULT_MESSAGE)
        {
            this.StatusCode = statusCode;
            this.JsonResult = jsonResult;
        }

        public ApiException(string message, HttpStatusCode statusCode, dynamic jsonResult) : base(message)
        {
            this.StatusCode = statusCode;
            this.JsonResult = jsonResult;
        }

        public HttpStatusCode StatusCode { get; }

        public dynamic JsonResult { get; }
    }

    public class ApiException<T> : ApiException
    {
        public ApiException() : base(ApiException.DEFAULT_MESSAGE)
        {

        }

        public ApiException(string message) : base(message)
        {

        }

        public ApiException(HttpStatusCode statusCode) : base(ApiException.DEFAULT_MESSAGE, statusCode)
        {
            
        }

        public ApiException(string message, HttpStatusCode statusCode) : base(message, statusCode)
        {
            
        }

        public ApiException(HttpStatusCode statusCode, T jsonResult) : base(ApiException.DEFAULT_MESSAGE, statusCode)
        {
            this.JsonResult = jsonResult;
        }

        public ApiException(string message, HttpStatusCode statusCode, T jsonResult) : base(message, statusCode)
        {
            this.JsonResult = jsonResult;
        }

        public new T JsonResult { get; }
    }
}
