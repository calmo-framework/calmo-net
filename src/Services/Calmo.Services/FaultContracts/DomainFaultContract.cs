using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Calmo.Services.FaultContracts
{
    /// <summary>
    /// Contrato base para exceções de domínio.
    /// </summary>
    [DataContract]
    public class DomainFaultContract : ServiceFaultContract
    {
        public DomainFaultContract(IEnumerable<string> messages)
        {
            Messages = messages;
        }

        [DataMember]
        public IEnumerable<string> Messages { get; private set; }
    }
}
