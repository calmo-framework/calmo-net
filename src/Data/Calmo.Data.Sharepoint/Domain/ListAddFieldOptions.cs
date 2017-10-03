using System;

namespace Calmo.Data.Sharepoint.Domain
{
    [Flags]
    public enum ListAddFieldOptions
    {
        DefaultValue = 0,
        AddToDefaultContentType = 1,
        AddToNoContentType = 2,
        AddToAllContentTypes = 4,
        AddFieldInternalNameHint = 8,
        AddFieldToDefaultView = 16,
        AddFieldCheckDisplayName = 32,
    }
}
