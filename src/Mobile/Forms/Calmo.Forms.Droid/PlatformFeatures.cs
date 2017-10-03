using Calmo.Forms;
using Xamarin.Forms;
using Android.App;
using AndroidApplication = Android.App.Application;

[assembly: Xamarin.Forms.Dependency(typeof(PlatformFeatures))]
namespace Xamarin.Forms
{
    public class PlatformFeatures : IPlatformFeatures
    {
        public void Exit()
        {
            var context = Forms.Context as Activity;
            context?.FinishAffinity();
        }
        
        public string PackageName
        {
            get
            {
                return AndroidApplication.Context.PackageName;
            }
        }
    }
}
