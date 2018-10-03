using Calmo.Core.Validator.Documents;

namespace Calmo.Core.Validator
{
    public static class DocumentValidationExtensions
    {
        public static bool ValidateDocument(this string value, DocumentDefinition definition)
        {
            return definition.Validate(value);
        }
    }
}