using System;
using Calmo.Forms;
using Xamarin.Forms;

[assembly: Dependency(typeof(Toast))]
namespace Xamarin.Forms
{
    public class Toast : IToast
    {
        public string Text { get; set; }
        public Color BackgroundColor { get; set; }
        public ToastType Type { get; set; }
        public ToastGravity Gravity { get; set; }
        public int Duration { get; set; }

        public void Show()
        {
            try
            {
                var toast = new ToastFluent {Text = this.Text, Duration = this.Duration};
                toast.Show();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
