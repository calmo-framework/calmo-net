namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) AttributeTargets.Class, AllowMultiple=true)]
    public sealed class AspRequiredAttributeAttribute : System.Attribute
    {
        public AspRequiredAttributeAttribute([NotNull] string attribute)
        {
            this.Attribute = attribute;
        }

        public string Attribute { get; private set; }
    }
}

