namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class RazorInjectionAttribute : Attribute
    {
        public RazorInjectionAttribute(string type, string fieldName)
        {
            this.Type = type;
            this.FieldName = fieldName;
        }

        public string FieldName { get; private set; }

        public string Type { get; private set; }
    }
}

