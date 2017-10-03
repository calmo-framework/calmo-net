using System.Runtime.CompilerServices;
using ResourceIT.Forms.Controls.VideoPlayer.Interfaces;
using UIKit;
using Xamarin.Forms;

namespace ResourceIT.Forms.iOS.Controls.VideoPlayer
{
    public sealed class FormsVideoPlayer
    {
        public static void Init(string licenseKey = null)
        {
            if (!IsInitialized)
            {
                //Log.Info($"Initializing Xamarin Forms Video Player on {UIDevice.CurrentDevice.Model} v{UIDevice.CurrentDevice.SystemVersion}");
                DependencyService.Register<IPlatformFeatures, PlatformFeatures>();
                IsInitialized = true;
            }
        }

        public static bool IsInitialized { get; set; }
    }
}

