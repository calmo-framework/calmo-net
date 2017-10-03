using Xamarin.Forms;

namespace Calmo.Forms
{
    public interface IToast
    {
        int Duration { get; set; }
        string Text { get; set; }
        Color BackgroundColor { get; set; }
        ToastType Type { get; set; }
        ToastGravity Gravity { get; set; }
        void Show();
    }
}