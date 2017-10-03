using CoreGraphics;
using System;
using System.Collections.Generic;
using UIKit;

namespace Xamarin.Forms
{
    public class ToastSetting : ICloneable
    {
        public static ToastSetting SharedSettings { get; set; }
        public nfloat bgAlpha;
        public nfloat bgBlue;
        public nfloat bgGreen;
        public nfloat bgRed;
        public nfloat cornerRadius;
        public nint duration;
        public nfloat fontSize;
        public ToastGravity gravity;
        //public ToastImageLocation imageLocation;
        public Dictionary<ToastType, UIImage> images;
        public nint offsetLeft;
        public nint offsetTop;
        public CGPoint position;
        public bool positionIsSet;
        public ToastType toastType;
        public bool useShadow;

        public object Clone()
        {
            var setting = new ToastSetting {
                gravity = this.gravity,
                duration = this.duration,
                position = this.position,
                fontSize = this.fontSize,
                useShadow = this.useShadow,
                cornerRadius = this.cornerRadius,
                bgRed = this.bgRed,
                bgGreen = this.bgGreen,
                bgBlue = this.bgBlue,
                bgAlpha = this.bgAlpha,
                offsetLeft = this.offsetLeft,
                offsetTop = this.offsetTop,
                //imageLocation = this.imageLocation,
                images = new Dictionary<ToastType, UIImage>()
            };
            foreach (KeyValuePair<ToastType, UIImage> pair in this.images)
            {
                setting.images[pair.Key] = pair.Value;
            }
            return setting;
        }

        private static UIImage ResizeImage(UIImage sourceImage, float width, float height)
        {
            if (sourceImage == null) return null;

            UIGraphics.BeginImageContext(new CGSize(width, height));
            sourceImage.Draw(new CGPoint(width, height));

            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return resultImage;
        }

        public static ToastSetting GetSharedSettings()
        {
            if (SharedSettings == null)
            {
                SharedSettings = new ToastSetting();
                SharedSettings.gravity = ToastGravity.Bottom;
                SharedSettings.duration = 0x3e8;
                SharedSettings.fontSize = 16f;
                SharedSettings.useShadow = true;
                SharedSettings.cornerRadius = 5f;
                SharedSettings.bgRed = 0;
                SharedSettings.bgGreen = 0;
                SharedSettings.bgBlue = 0;
                SharedSettings.bgAlpha = 0.7f;
                SharedSettings.offsetLeft = 0;
                SharedSettings.offsetTop = 0;
                //SharedSettings.imageLocation = ToastImageLocation.Left;
                SharedSettings.images = new Dictionary<ToastType, UIImage>
                {
                    [ToastType.Error] = ResizeImage(UIImage.FromBundle("error.png"), 20, 20),
                    [ToastType.Info] = ResizeImage(UIImage.FromBundle("info.png"), 20, 20),
                    [ToastType.Default] = null,
                    [ToastType.Notice] = ResizeImage(UIImage.FromBundle("notice.png"), 20, 20),
                    [ToastType.Warning] = ResizeImage(UIImage.FromBundle("warning.png"), 20, 20)
                };
                return SharedSettings;
            }
            return SharedSettings;
        }

        public static void MakeNewSetting()
        {
            SharedSettings = null;
        }

        /*public void SetImage(UIImage img, ToastType type)
        {
            this.SetImage(img, ToastImageLocation.Left, type);
        }*/

        /*public void SetImage(UIImage img, ToastType type)
        {
            if (type != ToastType.None)
            {
                if (this.images == null)
                {
                }
                this.images = new Dictionary<ToastType, UIImage>();

                if (img != null)
                    this.images[type] = img;
                //this.imageLocation = location;
            }
        }*/
    }
}