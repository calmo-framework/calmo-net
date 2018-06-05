using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Calmo.Data.Forms.Configuration;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Calmo.Core;

namespace Calmo.Data.Forms
{
    public class RepositoryApiAccess
    {
        private object _queryParams = null;
        private string _bearerToken = null;
        private IDictionary<string, string> _headers = null;

        internal RepositoryApiAccess() { }

        public RepositoryApiAccess WithParameters(object parameters)
        {
            _queryParams = parameters;
            return this;
        }

        public RepositoryApiAccess UseBearer(string bearerToken)
        {
            _bearerToken = bearerToken;
            return this;
        }

        public RepositoryApiAccess WithHeaders(IDictionary<string, string> headers)
        {
            _headers = headers;
            return this;
        }

        public async Task<IEnumerable<T>> List<T>(string urlPath) where T : class
        {
            return await this.List<T, dynamic>(urlPath);
        }

        public async Task<IEnumerable<T>> List<T, TResponseError>(string urlPath) where T : class
        {
            try
            {
                var result = await this.Client(urlPath, _queryParams, _bearerToken).GetStringAsync();
                if (!String.IsNullOrWhiteSpace(result))
                    return JsonConvert.DeserializeObject<IEnumerable<T>>(result);

                return await Task.FromResult(Enumerable.Empty<T>());
            }
            catch (FlurlHttpException ex)
            {
                var apiException = await this.HandleException<TResponseError>(ex);
                if (ApiConfiguration.OnErrorAction == null) throw apiException;

                ApiConfiguration.OnErrorAction(apiException);
                return await Task.FromResult(default(IEnumerable<T>));
            }
        }

        public async Task<T> Get<T>(string urlPath)
        {
            return await this.Get<T, dynamic>(urlPath);
        }

        public async Task<T> Get<T, TResponseError>(string urlPath)
        {
            try
            {
                var returnType = typeof(T);

                if (returnType == typeof(string))
                {
                    var stringResult = await this.Client(urlPath, _queryParams, _bearerToken).GetStringAsync();
                    return (T)Convert.ChangeType(stringResult, returnType);
                }

                var result = await this.Client(urlPath, _queryParams, _bearerToken).GetStringAsync();
                return JsonConvert.DeserializeObject<T>(result);
            }
            catch (FlurlHttpException ex)
            {
                var apiException = await this.HandleException<TResponseError>(ex);
                if (ApiConfiguration.OnErrorAction == null) throw apiException;

                ApiConfiguration.OnErrorAction(apiException);
                return await Task.FromResult(default(T));
            }
        }

        public async Task<TReturn> Post<T, TReturn>(string urlPath, T item)
        {
            return await this.Post<T, TReturn, dynamic>(urlPath, item);
        }

        public async Task<TReturn> Post<T, TReturn, TResponseError>(string urlPath, T item)
        {
            try
            {
                var returnType = typeof(TReturn);

                if (returnType == typeof(bool))
                {
                    var boolResult = await this.Client(urlPath, _queryParams, _bearerToken).PostJsonAsync(item);
                    return await Task.FromResult((TReturn)Convert.ChangeType(boolResult.IsSuccessStatusCode, returnType));
                }

                if (returnType == typeof(string))
                {
                    var stringResult = await this.Client(urlPath, _queryParams, _bearerToken).PostJsonAsync(item).ReceiveString();
                    return (TReturn)Convert.ChangeType(stringResult, returnType);
                }

                var result = await this.Client(urlPath, _queryParams, _bearerToken).PostJsonAsync(item).ReceiveString();
                return JsonConvert.DeserializeObject<TReturn>(result);
            }
            catch (FlurlHttpException ex)
            {
                var apiException = await this.HandleException<TResponseError>(ex);
                if (ApiConfiguration.OnErrorAction == null) throw apiException;

                ApiConfiguration.OnErrorAction(apiException);
                return await Task.FromResult(default(TReturn));
            }
        }

        public async Task<TReturn> PostAsForm<T, TReturn>(string urlPath, T item)
        {
            return await this.PostAsForm<T, TReturn, dynamic>(urlPath, item);
        }

        public async Task<TReturn> PostAsForm<T, TReturn, TResponseError>(string urlPath, T item)
        {
            try
            {
                var returnType = typeof(TReturn);

                if (returnType == typeof(bool))
                {
                    var boolResult = await this.Client(urlPath, _queryParams, _bearerToken).PostUrlEncodedAsync(item);
                    return await Task.FromResult((TReturn)Convert.ChangeType(boolResult.IsSuccessStatusCode, returnType));
                }

                if (returnType == typeof(string))
                {
                    var stringResult = await this.Client(urlPath, _queryParams, _bearerToken).PostUrlEncodedAsync(item).ReceiveString();
                    return (TReturn)Convert.ChangeType(stringResult, returnType);
                }

                var result = await this.Client(urlPath, _queryParams, _bearerToken).PostUrlEncodedAsync(item).ReceiveString();
                return JsonConvert.DeserializeObject<TReturn>(result);
            }
            catch (FlurlHttpException ex)
            {
                var apiException = await this.HandleException<TResponseError>(ex);
                if (ApiConfiguration.OnErrorAction == null) throw apiException;

                ApiConfiguration.OnErrorAction(apiException);
                return await Task.FromResult(default(TReturn));
            }
        }

        public void PostAsForm<T>(string urlPath, T item)
        {
            this.Client(urlPath, _queryParams, _bearerToken).PostUrlEncodedAsync(item).FireAndForget();
        }

        #region Private Methods

        private IFlurlClient Client(string urlPath, object queryParams, string bearerToken)
        {
            var urlHasBase = false;
            if (!String.IsNullOrWhiteSpace(urlPath))
            {
                var urlLower = urlPath.ToLower();
                if (urlLower.Contains("http://") || urlLower.Contains("https://"))
                    urlHasBase = true;
            }

            string urlApi;
            if (urlHasBase)
            {
                var httpMethod = $"{urlPath.Split("//")[0]}//";
                var urlWithoutHttpMEthod = urlPath.Remove(0, httpMethod.Length);

                urlApi = $"{httpMethod}{urlWithoutHttpMEthod.Split('/')[0]}/";
                urlPath = urlPath.Remove(0, urlApi.Length);
            }
            else
            {
                urlApi = CustomConfiguration.Settings.Api().Url;
            }

            var urlPaths = this.GetUrlPaths(urlPath);

            if (String.IsNullOrWhiteSpace(urlApi))
                throw new ApiException($"The '{ApiSection.ApiUrlKey}' appSettings key is not found.");

            var urlClient = urlApi.SetQueryParams(new { nocache = DateTime.Now.Ticks });

            if (queryParams != null)
                urlClient.SetQueryParams(queryParams);

            var client = urlClient.AppendPathSegments(urlPaths)
                                  .AllowAnyHttpStatus();

            if (!String.IsNullOrWhiteSpace(bearerToken))
                client.WithOAuthBearerToken(bearerToken);

            var headers = CustomConfiguration.Settings.Api().Headers;
            foreach (var header in headers)
                client.WithHeader(header.Key, header.Value);

            if (_headers.HasItems())
            {
                foreach (var header in _headers)
                    client.WithHeader(header.Key, header.Value);
            }

            if (ApiConfiguration.SerializerSettings != null)
                client.Settings.JsonSerializer = new NewtonsoftJsonSerializer(ApiConfiguration.SerializerSettings);

            return client;
        }

        private List<string> GetUrlPaths(string urlPath)
        {
            if (String.IsNullOrWhiteSpace(urlPath)) return new string[0].ToList();

            return urlPath.Split('/').ToList();
        }

        private async Task<ApiException> HandleException<TResponseError>(FlurlHttpException exception)
        {
            ApiException returnException;
            if (exception.Call.Response != null)
            {
                string response = null;
                try
                {
                    response = await exception.Call.Response.Content.ReadAsStringAsync();
                }
                catch
                {
                    // ignored
                }

                if (String.IsNullOrWhiteSpace(response))
                    returnException = new ApiException(exception.Call.Response.StatusCode);
                else if (response.IsValidJson())
                {
                    var exceptionType = typeof(TResponseError);
                    if (exceptionType.FullName == "System.Object")
                    {
                        dynamic jsonObject = ApiConfiguration.SerializerSettings != null
                            ? JsonConvert.DeserializeObject<dynamic>(response, ApiConfiguration.SerializerSettings)
                            : JsonConvert.DeserializeObject<dynamic>(response);

                        returnException = new ApiException(exception.Call.Response.StatusCode, jsonObject);
                    }
                    else
                    {
                        var jsonObject = ApiConfiguration.SerializerSettings != null
                            ? JsonConvert.DeserializeObject<TResponseError>(response, ApiConfiguration.SerializerSettings)
                            : JsonConvert.DeserializeObject<TResponseError>(response);

                        returnException = new ApiException<TResponseError>(exception.Call.Response.StatusCode, jsonObject);
                    }
                }
                else
                    returnException = new ApiException($"Failed with response '{response}'", exception.Call.Response.StatusCode);
            }
            else if (exception.Call.Exception.Message.ToLower().Contains("fail"))
                returnException = new ApiException(exception.Message, HttpStatusCode.GatewayTimeout);
            else
                returnException = new ApiException(exception.Message);

            return await Task.FromResult(returnException);
        }

        #endregion
    }
}