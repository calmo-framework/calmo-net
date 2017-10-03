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

            var protocolException = exception as ProtocolException;
            if (protocolException != null)
            {
                var webException = protocolException.InnerException as WebException;
                if (webException != null)
                {
                    var responseStream = webException.Response.GetResponseStream();
                    if (responseStream != null)
                    {
                        var reader = new StreamReader(responseStream);
                        var json = reader.ReadToEnd();

                        var ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
                        var serializer = GetSerializer(typeof(TServiceFault));
                        var serviceFault = (TServiceFault)serializer.ReadObject(ms);
                        ms.Close();

                        if (serviceFault != null)
                        {
                            return serviceFault;
                        }
                    }
                }
            }
            else
            {
                var teste = (FaultException)exception;
            }

            if (exceptionHandler != null)
                exceptionHandler(exception);

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
