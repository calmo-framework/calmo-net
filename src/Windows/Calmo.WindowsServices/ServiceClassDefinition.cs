using System;

namespace Calmo.WindowsServices
{
    [Serializable]
    public class ServiceClassDefinition
    {
        public string AssemblyName { get; set; }
        public string TypeFullName { get; set; }
    }
}
