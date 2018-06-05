using System;

namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    [AttributeUsage((AttributeTargets) AttributeTargets.Class, AllowMultiple=true)]
    public sealed class AspChildControlTypeAttribute : Attribute
    {
        public AspChildControlTypeAttribute(string tagName, Type controlType)
        {
            this.TagName = tagName;
            this.ControlType = controlType;
        }

        public Type ControlType { get; private set; }

        public string TagName { get; private set; }
    }
}

