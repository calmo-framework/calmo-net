namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;

    [Flags]
    public enum CollectionAccessType
    {
        ModifyExistingContent = 2,
        None = 0,
        Read = 1,
        UpdatedContent = 6
    }
}

