namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) (AttributeTargets.Delegate | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor))]
    public sealed class StringFormatMethodAttribute : Attribute
    {
        public StringFormatMethodAttribute(string formatParameterName)
        {
            this.FormatParameterName = formatParameterName;
        }

        public string FormatParameterName { get; private set; }
    }
}

