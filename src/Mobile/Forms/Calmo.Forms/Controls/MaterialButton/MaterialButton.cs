namespace Xamarin.Forms
{
    public class MaterialButton : Button
    {
        public static BindableProperty ElevationProperty = BindableProperty.Create(nameof(Elevation), typeof(float), typeof(MaterialButton), 4.0f);

        public float Elevation
        {
            get { return (float)GetValue(ElevationProperty); }
            set { SetValue(ElevationProperty, value); }
        }

        public static readonly BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation), typeof(ImageOrientation), typeof(MaterialButton), ImageOrientation.ImageToLeft);

        public ImageOrientation Orientation
        {
            get { return (ImageOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly BindableProperty ImageWidthRequestProperty = BindableProperty.Create(nameof(ImageWidthRequest), typeof(int), typeof(MaterialButton), default(int));

        public int ImageWidthRequest
        {
            get { return (int)GetValue(ImageWidthRequestProperty); }
            set { SetValue(ImageWidthRequestProperty, value); }
        }

        public static readonly BindableProperty ImageHeightRequestProperty = BindableProperty.Create(nameof(ImageHeightRequest), typeof(int), typeof(MaterialButton), default(int));

        public int ImageHeightRequest
        {
            get { return (int)GetValue(ImageHeightRequestProperty); }
            set { SetValue(ImageHeightRequestProperty, value); }
        }

        public static readonly BindableProperty PaddingProperty = BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(MaterialButton), new Thickness(10));

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }
    }
}
