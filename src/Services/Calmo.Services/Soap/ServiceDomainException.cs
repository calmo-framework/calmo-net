using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using Calmo.Services.FaultContracts;

namespace Calmo.Services.Soap
{
    public class ServiceDomainException : FaultException<DomainFaultContract>
    {
        private ServiceDomainException(DomainFaultContract detail)
            : base(detail)
        {
        }

        private ServiceDomainException(DomainFaultContract detail, FaultReason reason)
            : base(detail, reason)
        {
        }

        private ServiceDomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private ServiceDomainException(DomainFaultContract detail, FaultReason reason, FaultCode code)
            : base(detail, reason, code)
        {
        }

        private ServiceDomainException(DomainFaultContract detail, FaultReason reason, FaultCode code, string action)
            : base(detail, reason, code, action)
        {
        }

        private ServiceDomainException(DomainFaultContract detail, string reason)
            : base(detail, reason)
        {
        }

        private ServiceDomainException(DomainFaultContract detail, string reason, FaultCode code)
            : base(detail, reason, code)
        {
        }

        private ServiceDomainException(DomainFaultContract detail, string reason, FaultCode code, string action)
            : base(detail, reason, code, action)
        {
        }

        public ServiceDomainException(string message)
            : base(new DomainFaultContract(new List<string> { message }), "DomainException")
        {

        }

        public ServiceDomainException(string message, FaultReason reason)
            : base(new DomainFaultContract(new[] { message }), reason)
        {

        }

        public ServiceDomainException(string message, FaultReason reason, FaultCode code)
            : base(new DomainFaultContract(new[] { message }), reason, code)
        {

        }

        public ServiceDomainException(IEnumerable<string> messages)
            : base(new DomainFaultContract(messages), "DomainException")
        {

        }

        public ServiceDomainException(IEnumerable<string> messages, FaultReason reason)
            : base(new DomainFaultContract(messages), reason)
        {

        }

        public ServiceDomainException(IEnumerable<string> messages, FaultReason reason, FaultCode code)
            : base(new DomainFaultContract(messages), reason, code)
        {

        }
    }
}
