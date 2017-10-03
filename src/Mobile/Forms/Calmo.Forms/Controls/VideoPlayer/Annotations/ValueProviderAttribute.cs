namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) (AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property))]
    public sealed class ValueProviderAttribute : Attribute
    {
        public ValueProviderAttribute(string name)
        {
            this.Name = name;
        }

        [NotNull]
        public string Name { get; private set; }
    }
}

