using Foundation;
using Calmo.Forms;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformFeatures))]

namespace Xamarin.Forms
{
    public class PlatformFeatures : IPlatformFeatures
    {
        public void Exit()
        {
            NSThread.MainThread.Cancel();
        }
        
        public string PackageName
        {
            get { return NSBundle.MainBundle.BundleIdentifier; }
        }
    }
}