using Calmo.Core.Validator.Formats;

namespace Calmo.Core.Validator
{
    public static class FormatValidationExtensions
    {
        public static bool ValidateFormat(this string value, FormatDefinition definition)
        {
            return definition.Validate(value);
        }
    }
}