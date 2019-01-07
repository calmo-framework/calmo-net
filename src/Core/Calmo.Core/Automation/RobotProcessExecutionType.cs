using System.ComponentModel;

namespace Calmo.Core.Automation
{
  public enum RobotProcessExecutionType
  {
    [Description("Daily")]
    Daily = 1,

    [Description("Weekly")]
    Weekly = 2,

    [Description("Monthly")]
    Monthly = 3,

    [Description("Period")]
    Period = 4
  }
}
