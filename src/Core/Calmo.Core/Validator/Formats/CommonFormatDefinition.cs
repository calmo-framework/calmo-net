namespace Calmo.Core.Validator.Formats
{
    public class CommonFormatDefinition : FormatDefinition
    {
        internal CommonFormatDefinition(string regex) : base(regex)
        {
        }

        public override string Format(string value)
        {
            throw new System.NotImplementedException();
        }

        public override string Unformat(string value)
        {
            throw new System.NotImplementedException();
        }
    }
}
