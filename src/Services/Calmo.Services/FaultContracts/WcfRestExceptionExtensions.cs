using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using Calmo.Core.ExceptionHandling;

namespace System
{
    public static class WcfRestExceptionExtensions
    {
        private static readonly IDictionary<Type, DataContractJsonSerializer> CachedSerializers = new Dictionary<Type, DataContractJsonSerializer>();

        public static TServiceFault HandleRestServiceFault<TServiceFault>(this Exception exception, Action<Exception> exceptionHandler = null) where TServiceFault : class
        {
            Throw.IfArgumentNull(exception, "exception");

            if (exception is ProtocolException protocolException)
            {
                if (protocolException.InnerException is WebException webException)
                {
                    var responseStream = webException.Response.GetResponseStream();
                    if (responseStream != null)
                    {
                        var reader = new StreamReader(responseStream);
                        var json = reader.ReadToEnd();

                        var ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
                        var serializer = GetSerializer(typeof(TServiceFault));
                        var serviceFault = default(TServiceFault);
                        try
                        {
                            serviceFault = (TServiceFault)serializer.ReadObject(ms);
                        }
                        catch
                        {
                            // ignored 
                        }
                        ms.Close();

                        return serviceFault;
                    }
                }
            }

            exceptionHandler?.Invoke(exception);
            throw exception;
        }

        private static DataContractJsonSerializer GetSerializer(Type classSpecificSerializer)
        {
            if (!CachedSerializers.ContainsKey(classSpecificSerializer))
            {
                CachedSerializers.Add(classSpecificSerializer, new DataContractJsonSerializer(classSpecificSerializer));
            }
            return CachedSerializers[classSpecificSerializer];
        }
    }
}
