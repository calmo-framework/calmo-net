using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using System.Threading.Tasks;
using UIKit;
using CoreGraphics;
using Calmo.Forms.iOS.Controls;

[assembly: ExportRenderer(typeof(MaterialButton), typeof(MaterialButtonRenderer))]
namespace Calmo.Forms.iOS.Controls
{
    public class MaterialButtonRenderer : ButtonRenderer
    {
        private const int CONTROL_PADDING = 2;
        private const string IPAD = "iPad";

        private MaterialButton Button => (MaterialButton)this.Element;

        public static void Initialize()
        {
            // empty, but used for beating the linker
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            var targetButton = this.Control;
            if (this.Button != null && targetButton != null)
            {
                targetButton.LineBreakMode = UIKit.UILineBreakMode.WordWrap;

                var width = this.GetWidth(this.Button.ImageWidthRequest);
                var height = this.GetHeight(this.Button.ImageHeightRequest);

                await SetImageAsync(this.Button.Image, width, height, this.Control);

                switch (this.Button.Orientation)
                {
                    case ImageOrientation.ImageToLeft:
                        AlignToLeft(targetButton);
                        break;
                    case ImageOrientation.ImageToRight:
                        AlignToRight(this.Button.ImageWidthRequest, targetButton);
                        break;
                    case ImageOrientation.ImageOnTop:
                        AlignToTop(this.Button.ImageHeightRequest, this.Button.ImageWidthRequest, targetButton, this.Button.WidthRequest);
                        break;
                    case ImageOrientation.ImageOnBottom:
                        AlignToBottom(this.Button.ImageHeightRequest, this.Button.ImageWidthRequest, targetButton);
                        break;
                }
            }
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            UpdateShadow();
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == MaterialButton.ElevationProperty.PropertyName)
                UpdateShadow();
            else if (e.PropertyName == Xamarin.Forms.Button.ImageProperty.PropertyName)
                await SetImageAsync(this.Button.Image, this.Button.ImageWidthRequest, this.Button.ImageHeightRequest, this.Control);
        }

        private void UpdateShadow()
        {
            var materialButton = (MaterialButton)Element;
            
            Layer.ShadowRadius = materialButton.Elevation;
            Layer.ShadowColor = UIColor.Gray.CGColor;
            Layer.ShadowOffset = new CGSize(2, 2);
            Layer.ShadowOpacity = 0.80f;
            Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
            Layer.MasksToBounds = false;
        }

        private static void AlignToLeft(UIButton targetButton)
        {
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Left;

            var titleInsets = new UIEdgeInsets(0, CONTROL_PADDING, 0, -1 * CONTROL_PADDING);
            targetButton.TitleEdgeInsets = titleInsets;
        }

        private static void AlignToRight(int widthRequest, UIButton targetButton)
        {
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Right;

            var titleInsets = new UIEdgeInsets(0, 0, 0, widthRequest + CONTROL_PADDING);

            targetButton.TitleEdgeInsets = titleInsets;
            var imageInsets = new UIEdgeInsets(0, widthRequest, 0, -1 * widthRequest);
            targetButton.ImageEdgeInsets = imageInsets;
        }

        private static void AlignToTop(int heightRequest, int widthRequest, UIButton targetButton, double buttonWidthRequest)
        {
            targetButton.VerticalAlignment = UIControlContentVerticalAlignment.Top;
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Center;
            targetButton.TitleLabel.LineBreakMode = UIKit.UILineBreakMode.WordWrap;

            targetButton.SizeToFit();

            var titleWidth = targetButton.TitleLabel.IntrinsicContentSize.Width;
            CGSize titleSize = targetButton.TitleLabel.Frame.Size;

            UIEdgeInsets titleInsets;
            UIEdgeInsets imageInsets;

            if (UIDevice.CurrentDevice.Model.Contains(IPAD))
            {
                titleInsets = new UIEdgeInsets(heightRequest, Convert.ToInt32(-1 * widthRequest / 2), -1 * heightRequest, Convert.ToInt32(widthRequest / 2));
                imageInsets = new UIEdgeInsets(0, Convert.ToInt32(titleWidth / 2), 0, -1 * Convert.ToInt32(titleWidth / 2));
            }
            else
            {
                titleInsets = new UIEdgeInsets(heightRequest, Convert.ToInt32(-1 * widthRequest / 2), -1 * heightRequest, Convert.ToInt32(widthRequest / 2));
                imageInsets = new UIEdgeInsets(0, Convert.ToInt32(targetButton.IntrinsicContentSize.Width / 2 - widthRequest / 2 - titleSize.Width / 2), 0, Convert.ToInt32(-1 * (targetButton.IntrinsicContentSize.Width / 2 - widthRequest / 2 + titleSize.Width / 2)));
            }

            targetButton.TitleEdgeInsets = titleInsets;
            targetButton.ImageEdgeInsets = imageInsets;
        }

        private static void AlignToBottom(int heightRequest, int widthRequest, UIButton targetButton)
        {
            targetButton.VerticalAlignment = UIControlContentVerticalAlignment.Bottom;
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Center;
            targetButton.SizeToFit();
            var titleWidth = targetButton.TitleLabel.IntrinsicContentSize.Width;

            UIEdgeInsets titleInsets;
            UIEdgeInsets imageInsets;

            if (UIDevice.CurrentDevice.Model.Contains(IPAD))
            {
                titleInsets = new UIEdgeInsets(-1 * heightRequest, Convert.ToInt32(-1 * widthRequest / 2), heightRequest, Convert.ToInt32(widthRequest / 2));
                imageInsets = new UIEdgeInsets(0, titleWidth / 2, 0, -1 * titleWidth / 2);
            }
            else
            {
                titleInsets = new UIEdgeInsets(-1 * heightRequest, -1 * widthRequest, heightRequest, widthRequest);
                imageInsets = new UIEdgeInsets(0, 0, 0, 0);
            }

            targetButton.TitleEdgeInsets = titleInsets;
            targetButton.ImageEdgeInsets = imageInsets;
        }
        
        private static async Task SetImageAsync(ImageSource source, int widthRequest, int heightRequest, UIButton targetButton)
        {
            var handler = GetHandler(source);
            using (UIImage image = await handler.LoadImageAsync(source))
            {
                UIImage scaled = image;
                if (heightRequest > 0 && widthRequest > 0 && (image.Size.Height != heightRequest || image.Size.Width != widthRequest))
                {
                    scaled = scaled.Scale(new CGSize(widthRequest, heightRequest));
                }

                targetButton.SetImage(scaled.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (this.Button.Orientation == ImageOrientation.ImageToRight)
            {
                var imageInsets = new UIEdgeInsets(0, Control.Frame.Size.Width - CONTROL_PADDING - this.Button.ImageWidthRequest, 0, 0);
                Control.ImageEdgeInsets = imageInsets;
            }
        }

        private static IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
                returnValue = new ImageLoaderSourceHandler();
            else if (source is FileImageSource)
                returnValue = new FileImageSourceHandler();
            else if (source is StreamImageSource)
                returnValue = new StreamImagesourceHandler();

            return returnValue;
        }

        private int GetWidth(int requestedWidth)
        {
            const int DefaultWidth = 50;
            return requestedWidth <= 0 ? DefaultWidth : requestedWidth;
        }

        private int GetHeight(int requestedHeight)
        {
            const int DefaultHeight = 50;
            return requestedHeight <= 0 ? DefaultHeight : requestedHeight;
        }
    }
}
