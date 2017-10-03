using CoreLocation;
using Xamarin.Forms;

[assembly: Dependency(typeof(Calmo.Forms.iOS.ServicesPermissions))]
namespace Calmo.Forms.iOS
{
    public class ServicesPermissions : IServicesPermissions
    {
        public void AskForPermissionIfDisabled(DeviceServices service)
        {
            var locationManager = new CLLocationManager();
            locationManager.RequestWhenInUseAuthorization();
        }

        public bool IsServiceEnabled(DeviceServices service)
        {
            return false;
        }
    }
}
