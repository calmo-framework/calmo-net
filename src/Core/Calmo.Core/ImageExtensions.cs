using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Drawing
{
    public static class ImageExtensions
    {
        public static Image Resize(this Image image, Size size)
        {
            var tempImage = new Bitmap(image);

            using (tempImage)
            {
                var initialWidth = image.Width;
                var initialHeight = image.Height;

                var widthRatio = ((float)size.Width / (float)initialWidth);
                var heightRatio = ((float)size.Height / (float)initialHeight);
                var ratio = heightRatio < widthRatio ? heightRatio : widthRatio;

                var finalWidth = (int)(initialWidth * ratio);
                var finalHeight = (int)(initialHeight * ratio);
                var bitmap = new Bitmap(finalWidth, finalHeight);

                using (var graphic = Graphics.FromImage(bitmap))
                {
                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.DrawImage(tempImage, 0, 0, finalWidth, finalHeight);
                }

                return bitmap;
            }
        }
    }
}
