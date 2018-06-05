namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) AttributeTargets.Parameter)]
    public sealed class AspMvcAreaAttribute : Attribute
    {
        public AspMvcAreaAttribute()
        {
        }

        public AspMvcAreaAttribute(string anonymousProperty)
        {
            this.AnonymousProperty = anonymousProperty;
        }

        public string AnonymousProperty { get; private set; }
    }
}

