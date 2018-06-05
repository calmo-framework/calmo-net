namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) (AttributeTargets.Parameter | AttributeTargets.Method), AllowMultiple=true)]
    public sealed class MacroAttribute : Attribute
    {
        public int Editable { get; set; }

        public string Expression { get; set; }

        public string Target { get; set; }
    }
}

