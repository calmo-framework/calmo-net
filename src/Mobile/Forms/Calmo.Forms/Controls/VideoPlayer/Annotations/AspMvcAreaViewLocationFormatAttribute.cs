namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class AspMvcAreaViewLocationFormatAttribute : Attribute
    {
        public AspMvcAreaViewLocationFormatAttribute(string format)
        {
            this.Format = format;
        }

        public string Format { get; private set; }
    }
}

