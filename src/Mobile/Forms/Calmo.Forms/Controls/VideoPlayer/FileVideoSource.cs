namespace ResourceIT.Forms.Controls.VideoPlayer
{
    using System.Threading.Tasks;
    using Xamarin.Forms;

    [TypeConverter(typeof(FileVideoSourceConverter))]
    public class FileVideoSource : VideoSource
    {
        public static readonly BindableProperty FileProperty = BindableProperty.Create("File", typeof(string), typeof(FileVideoSource));

        public override Task<bool> Cancel() => Task.FromResult(false);

        public override bool Equals(VideoSource other)
        {
            if (other is FileVideoSource)
            {
                return ((FileVideoSource) other).File.Equals(this.File);
            }
            return true;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            if (propertyName == FileProperty.PropertyName)
                base.OnSourceChanged();

            if (propertyName != null)
                base.OnPropertyChanged(propertyName);
        }

        public static implicit operator string(FileVideoSource file) => file?.File;

        public static implicit operator FileVideoSource(string file) =>  (FileVideoSource) VideoSource.FromFile(file);

        public string File
        {
            get { return (string) base.GetValue(FileProperty); }
            set { base.SetValue(FileProperty, value); }
        }
    }
}

