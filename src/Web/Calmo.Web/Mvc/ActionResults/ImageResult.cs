using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace System.Web.Mvc
{
    public class ImageResult : ActionResult
    {
        public Image Image { get; set; }
        public ImageFormat Format { get; set; }

        private static Dictionary<ImageFormat, string> _formatMap;

        static ImageResult()
        {
            CreateContentTypeMap();
        }

        public ImageResult(Image image, ImageFormat format)
        {
            CreateContentTypeMap();

            if (image == null) throw new ArgumentNullException("image");
            if (format == null) throw new ArgumentNullException("format");

            Image = image;
            Format = format;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = _formatMap[Format];

            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.HttpContext.Response.Cache.SetOmitVaryStar(true);
            context.HttpContext.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
            context.HttpContext.Response.Cache.SetValidUntilExpires(true);
            context.HttpContext.Response.Cache.SetLastModified(DateTime.Now);

            //Image.Save(context.HttpContext.Response.OutputStream, Format);
            using (var ms = new MemoryStream())
            {
                Image.Save(ms, Format);
                ms.WriteTo(context.HttpContext.Response.OutputStream);
            }

            Image.Dispose();
            context.HttpContext.Response.End();
        }

        private static void CreateContentTypeMap()
        {
            _formatMap = new Dictionary<ImageFormat, string>
                             {
                                 {ImageFormat.Bmp, "image/bmp"},
                                 {ImageFormat.Gif, "image/gif"},
                                 {ImageFormat.Icon, "image/vnd.microsoft.icon"},
                                 {ImageFormat.Jpeg, "image/Jpeg"},
                                 {ImageFormat.Png, "image/png"},
                                 {ImageFormat.Tiff, "image/tiff"},
                                 {ImageFormat.Wmf, "image/wmf"}
                             };
        }
    }
}
