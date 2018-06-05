using System.ServiceProcess;

namespace Calmo.WindowsServices.Robot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new RobotService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
