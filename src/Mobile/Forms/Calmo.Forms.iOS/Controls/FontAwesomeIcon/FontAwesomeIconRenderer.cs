using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FontAwesomeIcon), typeof(FontAwesomeIconRenderer))]

namespace Xamarin.Forms.Platform.iOS
{
    public class FontAwesomeIconRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (this.Control == null) return;

            var label = (UILabel)this.Control;
            label.Font = UIFont.FromName(FontAwesomeIcon.Typeface, 22f);
        }
    }
}