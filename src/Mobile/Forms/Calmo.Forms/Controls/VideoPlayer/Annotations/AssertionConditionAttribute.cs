namespace ResourceIT.Forms.Controls.VideoPlayer.Annotations
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage((AttributeTargets) AttributeTargets.Parameter)]
    public sealed class AssertionConditionAttribute : Attribute
    {
        public AssertionConditionAttribute(AssertionConditionType conditionType)
        {
            this.ConditionType = conditionType;
        }

        public AssertionConditionType ConditionType { get; private set; }
    }
}

