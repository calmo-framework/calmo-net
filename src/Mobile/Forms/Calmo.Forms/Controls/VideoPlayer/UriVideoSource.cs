namespace ResourceIT.Forms.Controls.VideoPlayer
{
    using Xamarin.Forms;

    public sealed class UriVideoSource : VideoSource
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create("Uri", typeof(System.Uri), typeof(UriVideoSource));

        public override bool Equals(VideoSource other)
        {
            if (other is UriVideoSource)
            {
                return ((UriVideoSource) other).Uri.Equals(this.Uri);
            }
            return true;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            if (propertyName == UriProperty.PropertyName)
            {
                base.OnSourceChanged();
            }
            base.OnPropertyChanged(propertyName);
        }

        [TypeConverter(typeof(UriTypeConverter))]
        public System.Uri Uri
        {
            get { return ((System.Uri) base.GetValue(UriProperty)); }
            set { base.SetValue(UriProperty, value); }
        }
    }
}

