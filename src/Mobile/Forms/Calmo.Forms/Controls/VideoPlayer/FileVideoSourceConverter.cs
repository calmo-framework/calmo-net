namespace ResourceIT.Forms.Controls.VideoPlayer
{
    using System;
    using Xamarin.Forms;

    internal class FileVideoSourceConverter : TypeConverter
    {
        public override bool CanConvertFrom(Type sourceType) => 
            (sourceType == typeof(string));

        public override object ConvertFromInvariantString(string file)
        {
            if (file != null)
            {
                return (FileVideoSource) VideoSource.FromFile(file);
            }
            object[] objArray1 = new object[] { typeof(FileVideoSource) };
            throw new InvalidOperationException($"Cannot convert file into {(object[]) objArray1}");
        }
    }
}

