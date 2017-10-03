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
            ToastFluent.MakeText(this.Text, this.Duration)
                .SetBgAlpha(new nfloat(this.BackgroundColor.A))
                .SetBgRed(new nfloat(this.BackgroundColor.R))
                .SetBgGreen(new nfloat(this.BackgroundColor.G))
                .SetBgBlue(new nfloat(this.BackgroundColor.B))
                .SetType(this.Type)
                .SetGravity(this.Gravity)
                .Show(ToastType.Info);
        }
    }
}
