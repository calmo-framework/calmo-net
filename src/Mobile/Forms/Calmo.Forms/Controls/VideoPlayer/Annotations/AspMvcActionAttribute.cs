namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) (AttributeTargets.Parameter | AttributeTargets.Method))]
    public sealed class AspMvcActionAttribute : Attribute
    {
        public AspMvcActionAttribute()
        {
        }

        public AspMvcActionAttribute(string anonymousProperty)
        {
            this.AnonymousProperty = anonymousProperty;
        }

        public string AnonymousProperty { get; private set; }
    }
}

