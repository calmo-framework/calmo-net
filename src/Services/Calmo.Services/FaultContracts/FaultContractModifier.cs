using System;

namespace Calmo.Services.FaultContracts
{
    public static class FaultContractModifier
    {
        public static Exception Modify(Exception exception)
        {
            return exception;
        }
    }
}
