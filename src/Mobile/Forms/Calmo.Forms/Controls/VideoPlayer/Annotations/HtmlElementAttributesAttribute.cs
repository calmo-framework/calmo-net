namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) (AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property))]
    public sealed class HtmlElementAttributesAttribute : Attribute
    {
        public HtmlElementAttributesAttribute()
        {
        }

        public HtmlElementAttributesAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
    }
}

