using Xamarin.Forms;

[assembly: Dependency(typeof(Calmo.Forms.Droid.ServicesPermissions))]
namespace Calmo.Forms.Droid
{
    using Android.Content;
    using Android.Locations;
    using Android.Provider;

    public class ServicesPermissions : IServicesPermissions 
    {
        public void AskForPermissionIfDisabled(DeviceServices service)
        {
            if (service == DeviceServices.GPS)
            {
                var locationManager = (LocationManager)Xamarin.Forms.Forms.Context.GetSystemService(Context.LocationService);

                if (locationManager.IsProviderEnabled(LocationManager.GpsProvider)) return;
                var gpsSettingIntent = new Intent(Settings.ActionLocationSourceSettings);
                Xamarin.Forms.Forms.Context.StartActivity(gpsSettingIntent);
            }
        }

        public bool IsServiceEnabled(DeviceServices service)
        {
            if (service == DeviceServices.GPS)
            {
                var locationManager = (LocationManager)Xamarin.Forms.Forms.Context.GetSystemService(Context.LocationService);
                return locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            }
            return false;
        }
    }
}
