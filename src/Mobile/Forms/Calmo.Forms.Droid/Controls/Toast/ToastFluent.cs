using Android.App;
using Android.Widget;
using AndroidToast = Android.Widget.Toast;

namespace Xamarin.Forms
{
    internal class ToastFluent
    {
        private static Activity MainActivity = null;

        public static void Init(Activity mainActivity)
        {
            MainActivity = mainActivity;
        }

        public string Text { get; set; }
        public int Duration { get; set; }

        public void Show()
        {
            AndroidToast.MakeText(MainActivity, this.Text, ToastLength.Short).Show();
        }
    }
}