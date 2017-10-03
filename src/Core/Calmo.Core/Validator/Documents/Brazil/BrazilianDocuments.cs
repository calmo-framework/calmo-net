using Calmo.Core.Validator.Documents.Brazil;

namespace Calmo.Core.Validator
{
    public class BrazilianDocuments
    {
        internal BrazilianDocuments()
        {

        }

        public CPFDocumentDefinition CPF = new CPFDocumentDefinition();
        public CNPJDocumentDefinition CNPJ = new CNPJDocumentDefinition();
    }
}