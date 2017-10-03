using Calmo.Forms;

namespace Xamarin.Forms
{
    public class Toast
    {
        public static Toast MakeText(string text)
        {
            return new Toast(text);
        }
        
        public int Duration { get; set; }
        public string Text { get; set; }
        public Color BackgroundColor { get; set; }
        public ToastType Type { get; set; }
        public ToastGravity Gravity { get; set; }

        private Toast(string text)
        {
            Text = text;
            Duration = (int) ToastDuration.Normal;
            BackgroundColor = Color.Black;
            Type = ToastType.Default;
            Gravity = ToastGravity.Bottom;
        }

        public Toast SetDuration(int duration)
        {
            this.Duration = duration;
            return this;
        }

        public Toast SetDuration(ToastDuration duration)
        {
            this.Duration = (int)duration;
            return this;
        }

        public Toast SetBackgroundColor(Color color)
        {
            this.BackgroundColor = color;
            return this;
        }

        public Toast SetType(ToastType type)
        {
            this.Type = type;
            return this;
        }

        public Toast SetGravity(ToastGravity gravity)
        {
            this.Gravity = gravity;
            return this;
        }

        public void Show()
        {
            var baseToast = DependencyService.Get<IToast>();
            baseToast.Text = this.Text;
            baseToast.Duration = this.Duration;
            baseToast.BackgroundColor = this.BackgroundColor;
            baseToast.Type = this.Type;
            baseToast.Gravity = this.Gravity;
            baseToast.Show();
        }
    }
}
