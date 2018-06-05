namespace System.Drawing
{
    public static class DrawingExtensions
    {
        public static void GetImageFromSprite(this Bitmap sprite, Rectangle spritePosition, ref Bitmap destination, Rectangle destinationPosition)
        {
            using (var grD = Graphics.FromImage(destination))
            {
                grD.DrawImage(sprite, destinationPosition, spritePosition, GraphicsUnit.Pixel);
            }
        }
    }
}
