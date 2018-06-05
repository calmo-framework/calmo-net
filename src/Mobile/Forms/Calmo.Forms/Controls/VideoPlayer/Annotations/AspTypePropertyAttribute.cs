namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) AttributeTargets.Property)]
    public sealed class AspTypePropertyAttribute : Attribute
    {
        public AspTypePropertyAttribute(bool createConstructorReferences)
        {
            this.CreateConstructorReferences = createConstructorReferences;
        }

        public bool CreateConstructorReferences { get; private set; }
    }
}

