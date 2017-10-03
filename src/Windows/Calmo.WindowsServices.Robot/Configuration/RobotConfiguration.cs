using System.Collections.Generic;

namespace Calmo.WindowsServices.Robot.Configuration
{
    public class RobotConfiguration
    {
        public RobotConfiguration()
        {
            this.ServiceClasses = new List<ServiceClassDefinition>();
        }

        public string ServiceName { get; set; }
        public int ExecutionMilliseconds { get; set; }
        public int ExecutionTimeout { get; set; }
        public bool LogSteps { get; set; }
        public bool LogTextFile { get; set; }
        public string TextFilePath { get; set; }
        public List<ServiceClassDefinition> ServiceClasses { get; set; }
    }
}