using Calmo.Core.Validator.Formats;
using Calmo.Core.Validator.Formats.Brazil;

namespace Calmo.Core.Validator
{
    public class BrazilianFormats
    {
        internal BrazilianFormats()
        {

        }
        
        public FormatDefinition CPF = new CPFFormatDefinition();
        public FormatDefinition CNPJ = new CNPJFormatDefinition();
        public FormatDefinition CEP = new CEPFormatDefinition();
        public FormatDefinition Phone = new PhoneFormatDefinition();
    }
}
