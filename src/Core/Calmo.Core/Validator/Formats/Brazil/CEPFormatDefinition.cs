using System;

namespace Calmo.Core.Validator.Formats.Brazil
{
    public class CEPFormatDefinition : FormatDefinition
    {
        private const string FORMAT_VALIDATION_REGEX = @"^\d\d\d\d\d\-\d\d\d$";

        internal CEPFormatDefinition() : base(FORMAT_VALIDATION_REGEX)
        {
            
        }

        public override string Format(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            value = this.Unformat(value);

            if (value.Length > 8)
                value = value.Substring(0, 8);
            else if (value.Length < 8)
                value = value.PadLeft(8, '0');

            return value.Insert(5, "-");
        }

        public override string Unformat(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            return value.Replace("-", String.Empty);
        }
    }
}
