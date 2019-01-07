using System.ComponentModel;

namespace Calmo.Core.Automation
{
  public enum RobotProcessStatus
  {
    [Description("Waiting Start")]
    WaitingStart = 1,

    [Description("Executing")]
    Executing = 2,

    [Description("Waiting Return")]
    WaitingReturn = 3,

    [Description("Requesting Return")]
    RequestingReturn = 4,

    [Description("Processed")]
    Processed = 5,

    [Description("Processed With Errors")]
    ProcessedWithErrors = 6,

    [Description("Not Processed With Errors")]
    NotProcessedWithErrors = 7
  }
}
