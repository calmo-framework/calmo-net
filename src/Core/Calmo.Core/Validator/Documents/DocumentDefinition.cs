namespace Calmo.Core.Validator.Documents
{
    public abstract class DocumentDefinition
    {
        public abstract bool Validate(string value);
    }
}