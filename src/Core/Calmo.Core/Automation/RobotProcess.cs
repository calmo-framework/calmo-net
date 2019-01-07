using System;
using System.Collections.Generic;

namespace Calmo.Core.Automation
{
  public class RobotProcess
  {
    public RobotProcess()
    {
      this.Status = RobotProcessStatus.WaitingStart;
      this.ExecutionType = RobotProcessExecutionType.Period;
    }

    public string Id { get; set; }

    public bool Enabled { get; set; }

    public RobotProcessStatus Status { get; set; }

    public bool IsAsync { get; set; }

    public bool HasReturn { get; set; }

    public RobotProcessExecutionType ExecutionType { get; set; }

    public DateTime? ExecutionHour { get; set; }

    public DayOfWeek? ExecutionDayOfWeek { get; set; }

    public int? ExecutionDayOfMonth { get; set; }

    public int? ExecutionPeriodMinutes { get; set; }

    public DateTime? LastExecutionStart { get; set; }

    public DateTime? LastExecutionFinish { get; set; }

    public DateTime CreateTime { get; set; }

    public Dictionary<string, object> Data { get; set; }
  }
}
