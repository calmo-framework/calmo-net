namespace Xamarin.Forms
{
    public class FontAwesomeIcon : Label
    {
        public const string Typeface = "FontAwesome";

        public static readonly BindableProperty IconProperty =
            BindableProperty.Create(nameof(Icon), typeof(FontAwesomeIcons), typeof(FontAwesomeIcon), FontAwesomeIcons.None);
        
        public FontAwesomeIcons Icon
        {
            get { return (FontAwesomeIcons)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public FontAwesomeIcon()
        {
            FontFamily = Typeface;
        }

        protected override void OnPropertyChanging(string propertyName = null)
        {
            if (propertyName == "Renderer")
                this.Text = this.Icon.GetIcon();

            base.OnPropertyChanging(propertyName);
        }
    }
}
