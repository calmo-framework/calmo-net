namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) AttributeTargets.Method, AllowMultiple=true)]
    public sealed class ContractAnnotationAttribute : Attribute
    {
        public ContractAnnotationAttribute([NotNull] string contract) : this(contract, false)
        {
        }

        public ContractAnnotationAttribute([NotNull] string contract, bool forceFullStates)
        {
            this.Contract = contract;
            this.ForceFullStates = forceFullStates;
        }

        public string Contract { get; private set; }

        public bool ForceFullStates { get; private set; }
    }
}

