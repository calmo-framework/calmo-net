using Calmo.Forms;

namespace Xamarin.Forms
{
    public class ServicesPermissions
    {
        public static void AskForPermissionIfDisabled(DeviceServices service)
        {
            var servicesPermissions = DependencyService.Get<IServicesPermissions>();
            servicesPermissions?.AskForPermissionIfDisabled(service);
        }

        public static bool IsServiceEnabled(DeviceServices service)
        {
            var servicesPermissions = DependencyService.Get<IServicesPermissions>();
            return servicesPermissions == null || servicesPermissions.IsServiceEnabled(service);
        }
    }
}
