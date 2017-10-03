using System;

namespace Calmo.Core.Validator.Formats.Brazil
{
    public class CPFFormatDefinition : FormatDefinition
    {
        private const string FORMAT_VALIDATION_REGEX = @"^\d\d\d\.\d\d\d\.\d\d\d\-\d\d$";

        internal CPFFormatDefinition() : base(FORMAT_VALIDATION_REGEX)
        {
            
        }

        public override string Format(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            value = this.Unformat(value);
            return $@"{Convert.ToInt64(value):000\.000\.000\-00}";
        }

        public override string Unformat(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            return value.Replace("-", String.Empty).Replace(".", String.Empty);
        }
    }
}
