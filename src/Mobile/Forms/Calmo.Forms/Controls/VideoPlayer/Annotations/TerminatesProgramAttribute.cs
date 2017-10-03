namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;

    [Obsolete("Use [ContractAnnotation('=> halt')] instead"), AttributeUsage((AttributeTargets) AttributeTargets.Method)]
    public sealed class TerminatesProgramAttribute : Attribute
    {
    }
}

