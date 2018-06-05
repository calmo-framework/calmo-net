using Calmo.Core.Validator.Documents;

namespace Calmo.Core.Validator
{
    public class DocumentValidation
    {
        public static BrazilianDocuments Brazil = new BrazilianDocuments();
    }

    public static class DocumentValidationExtensions
    {
        public static bool ValidateDocument(this string value, DocumentDefinition definition)
        {
            return definition.Validate(value);
        }
    }
}
