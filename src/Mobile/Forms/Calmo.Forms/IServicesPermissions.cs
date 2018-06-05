using Xamarin.Forms;

namespace Calmo.Forms
{
    public interface IServicesPermissions
    {
        void AskForPermissionIfDisabled(DeviceServices service);
        bool IsServiceEnabled(DeviceServices service);
    }
}