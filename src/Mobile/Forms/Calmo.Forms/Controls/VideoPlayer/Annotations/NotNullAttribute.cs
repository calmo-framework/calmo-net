namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;

    [AttributeUsage((AttributeTargets) (AttributeTargets.Delegate | AttributeTargets.Parameter | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method))]
    public sealed class NotNullAttribute : Attribute
    {
    }
}

