using System.Collections.Generic;

namespace Calmo.WindowsServices
{
    public interface IRobotServiceClass
    {
        IEnumerable<RobotServiceItemClass> GetCurrentProcesses();
        bool Process(RobotServiceItemClass process);
        void UpdateProcessStatus(RobotServiceItemClass process);

        bool EnforceExecution { get; set; }
    }
}