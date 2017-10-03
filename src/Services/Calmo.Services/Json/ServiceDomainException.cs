using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using Calmo.Services.FaultContracts;

namespace Calmo.Services.Json
{
    public class ServiceDomainException : WebFaultException<DomainFaultContract>
    {
        private ServiceDomainException(DomainFaultContract detail, HttpStatusCode statusCode)
            : base(detail, statusCode)
        {
        }

        private ServiceDomainException(DomainFaultContract detail, HttpStatusCode statusCode, IEnumerable<Type> knownTypes)
            : base(detail, statusCode, knownTypes)
        {
        }

        private ServiceDomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ServiceDomainException(string message)
            : base(new DomainFaultContract(new List<string> { message }), HttpStatusCode.BadRequest)
        {

        }

        public ServiceDomainException(IEnumerable<string> messages)
            : base(new DomainFaultContract(messages), HttpStatusCode.BadRequest)
        {

        }

        public ServiceDomainException(string message, HttpStatusCode statusCode)
            : base(new DomainFaultContract(new[] { message }), statusCode)
        {

        }

        public ServiceDomainException(IEnumerable<string> messages, HttpStatusCode statusCode)
            : base(new DomainFaultContract(messages), statusCode)
        {

        }
    }
}
