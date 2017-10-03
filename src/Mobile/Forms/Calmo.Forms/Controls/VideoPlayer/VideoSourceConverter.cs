namespace ResourceIT.Forms.Controls.VideoPlayer
{
    using System;
    using Xamarin.Forms;

    public class VideoSourceConverter : TypeConverter
    {
        public override bool CanConvertFrom(Type sourceType) =>
            (sourceType == typeof(string));

        public override object ConvertFromInvariantString(string value)
        {
            Uri uri;
            if (value == null)
            {
                return null;
            }
            if (Uri.TryCreate(value, (UriKind)UriKind.Absolute, out uri) && (uri.Scheme != "file"))
            {
                return VideoSource.FromUri(uri);
            }
            return VideoSource.FromFile(value);
        }
    }
}