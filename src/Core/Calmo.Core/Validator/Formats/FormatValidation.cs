using Calmo.Core.Validator.Formats;

namespace Calmo.Core.Validator
{
    public class FormatValidation
    {
        public static BrazilianFormats Brazil = new BrazilianFormats();
        public static FormatDefinition Email = new CommonFormatDefinition(@"^([a-zA-Z0-9\!\%\$\%\*\/\?\|\^\{\}\`\~\&\'\+\-\=_]\.?)*[a-zA-Z0-9\!\%\$\%\*\/\?\|\^\{\}\`\~\&\'\+\-\=_]@((([a-zA-Z0-9\!\%\$\%\*\/\?\|\^\{\}\`\~\&\'\+\-\=_]\.?)*[a-zA-Z0-9\!\%\$\%\*\/\?\|\^\{\}\`\~\&\'\+\-\=_])|(\[\d+\.\d+\.\d+\.\d+\]))$");
    }

    public static class FormatValidationExtensions
    {
        public static bool ValidateFormat(this string value, FormatDefinition definition)
        {
            return definition.Validate(value);
        }
    }
}
