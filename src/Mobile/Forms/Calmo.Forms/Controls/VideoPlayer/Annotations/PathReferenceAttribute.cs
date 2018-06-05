namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) AttributeTargets.Parameter)]
    public sealed class PathReferenceAttribute : Attribute
    {
        public PathReferenceAttribute()
        {
        }

        public PathReferenceAttribute([PathReference] string basePath)
        {
            this.BasePath = basePath;
        }

        public string BasePath { get; private set; }
    }
}

