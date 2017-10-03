using Xamarin.Forms;

[assembly: Dependency(typeof(Forms))]
namespace Calmo.Forms
{
    using Android.App;
    using Xamarin.Forms;

    public static class Forms
    {
        public static void Init(Activity activity)
        {
            InitToast(activity);
        }

        private static void InitToast(Activity activity)
        {
            ToastFluent.Init(activity);
        }
    }
}
