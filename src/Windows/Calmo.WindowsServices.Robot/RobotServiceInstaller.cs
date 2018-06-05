using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using Calmo.Core;

namespace Calmo.WindowsServices.Robot
{
    [RunInstaller(true)]
    public partial class RobotServiceInstaller : Installer
    {
        public RobotServiceInstaller()
        {
            InitializeComponent();

            var serviceName = CustomConfiguration.Settings.Robot().ServiceName;
            var installer = new ServiceInstaller
                {
                    DisplayName = serviceName,
                    ServiceName = serviceName,
                    StartType = ServiceStartMode.Automatic
                };
            this.Installers.Add(installer);

            var spi = new ServiceProcessInstaller
                {
                    Account = ServiceAccount.LocalSystem,
                    Username = null,
                    Password = null
                };
            this.Installers.Add(spi);
        }
    }
}
