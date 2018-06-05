namespace ResourceIT.Forms.Controls.VideoPlayer
{
    using System;

    [AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
    public sealed class PreserveAttribute : Attribute
    {
        public bool AllMembers;
        public bool Conditional;
    }
}

