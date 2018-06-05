using System;
using System.Collections.Generic;
using System.Text;

namespace Calmo.WindowsServices
{
    public enum RobotServiceItemType
    {
        Synchronous,
        //SynchronousWithReturn,
        Asynchronous,
    }

    public enum RobotServiceItemStatus
    {
        WaitingStart,
        Executing,
        WaitingReturn,
        RequestingReturn,
        Processed,
        ProcessedWithErrors,
        NotProcessedWithErrors
    }
}
