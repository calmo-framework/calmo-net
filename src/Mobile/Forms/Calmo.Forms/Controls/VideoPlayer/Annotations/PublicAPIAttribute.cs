namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
    public sealed class PublicAPIAttribute : Attribute
    {
        public PublicAPIAttribute()
        {
        }

        public PublicAPIAttribute([NotNull] string comment)
        {
            this.Comment = comment;
        }

        public string Comment { get; private set; }
    }
}

