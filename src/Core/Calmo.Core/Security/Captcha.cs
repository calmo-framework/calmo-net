using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Calmo.Core.Security
{
	/// <summary>
	/// Class with the logic to Create/Validade custom captcha images
	/// </summary>
    public static class Captcha
    {
		/// <summary>
		/// Creates a captcha image inside a Stream
		/// </summary>
		/// <param name="text">Text inside the captcha</param>
		/// <returns>Stream containing an image bitmap</returns>
        public static Stream GetImage(out string text)
        {
            var random = new Random();
            var imageBitmap = new Bitmap(75, 25);
            var imageGraphics = Graphics.FromImage(imageBitmap);

            text = GenerateRandomText(random);

            DrawBackground(imageGraphics);
            ImageNoise(imageGraphics, 10, random);
            DrawText(text, imageGraphics, random);

            var memoryStream = new MemoryStream();
            imageBitmap.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        private static void DrawBackground(Graphics imagemGraphics)
        {
            var retangulo = new Rectangle(new Point(0, 0), new Size(75, 25));
            var whiteBrush = Brushes.White;
            imagemGraphics.FillRectangle(whiteBrush, retangulo);
        }

        private static void DrawText(string text, Graphics imagemGraphics, Random random)
        {
            var blackBrush = Brushes.DimGray;
            var arial = new Font("arial", 12, FontStyle.Bold);
            var times = new Font("Times New Roman", 11, FontStyle.Bold);
            var verdana = new Font("Verdana", 11, FontStyle.Bold);
            var paladino = new Font("Palatino Linotype", 12, FontStyle.Bold);
            var comic = new Font("Comic Sans MS", 12, FontStyle.Bold);

            imagemGraphics.DrawString(text.Substring(0, 1), arial, blackBrush, 2, random.Next(0, 5));
            imagemGraphics.DrawString(text.Substring(1, 1), times, blackBrush, 18, random.Next(0, 5));
            imagemGraphics.DrawString(text.Substring(2, 1), verdana, blackBrush, 30, random.Next(0, 5));
            imagemGraphics.DrawString(text.Substring(3, 1), paladino, blackBrush, 43, random.Next(0, 5));
            imagemGraphics.DrawString(text.Substring(4, 1), comic, blackBrush, 55, random.Next(0, 5));
        }

        private static void ImageNoise(Graphics imageGraphics, int intensity, Random random)
        {
            var grayPen = new Pen(Color.FromArgb(230, 230, 230), 2);
            var darkGrayPen = new Pen(Color.FromArgb(180, 180, 180), 2);

            for (int i = 1; i <= intensity; i++)
            {
                imageGraphics.DrawRectangle(grayPen, random.Next(1, 75), random.Next(1, 25), 1, 2);
                imageGraphics.DrawRectangle(grayPen, random.Next(1, 75), random.Next(1, 25), 2, 1);
                imageGraphics.DrawRectangle(grayPen, random.Next(1, 75), random.Next(1, 25), 1, 2);
                imageGraphics.DrawRectangle(grayPen, random.Next(1, 75), random.Next(1, 25), 2, 1);
                imageGraphics.DrawRectangle(grayPen, random.Next(1, 75), random.Next(1, 25), 1, 2);
                imageGraphics.DrawRectangle(grayPen, random.Next(1, 75), random.Next(1, 25), 2, 1);
                imageGraphics.DrawRectangle(grayPen, random.Next(1, 75), random.Next(1, 25), 1, 2);
                imageGraphics.DrawRectangle(grayPen, random.Next(1, 75), random.Next(1, 25), 2, 1);
                imageGraphics.DrawRectangle(darkGrayPen, random.Next(1, 75), random.Next(1, 25), 2, 1);
                imageGraphics.DrawRectangle(darkGrayPen, random.Next(1, 75), random.Next(1, 25), 1, 2);
                imageGraphics.DrawRectangle(grayPen, random.Next(1, 75), random.Next(1, 25), 1, 2);
                imageGraphics.DrawRectangle(grayPen, random.Next(1, 75), random.Next(1, 25), 2, 1);
                imageGraphics.DrawRectangle(darkGrayPen, random.Next(1, 75), random.Next(1, 25), 2, 1);
                imageGraphics.DrawRectangle(darkGrayPen, random.Next(1, 75), random.Next(1, 25), 1, 2);
                imageGraphics.DrawRectangle(darkGrayPen, random.Next(1, 75), random.Next(1, 25), 2, 1);
            }
        }

        private static string GenerateRandomText(Random random)
        {
            var texto = string.Empty;
            for (var counter = 0; counter <= 4; counter++)
                texto += (char)random.Next(65, 90);

            return texto;
        }
    }
}
