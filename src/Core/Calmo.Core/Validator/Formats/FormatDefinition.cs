namespace Calmo.Core.Validator.Formats
{
    public abstract class FormatDefinition
    {
        private string Regex { get; }

        internal FormatDefinition(string regex)
        {
            this.Regex = regex;
        }

        public bool Validate(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value, this.Regex);
        }

        public abstract string Format(string value);

        public abstract string Unformat(string value);
    }
}
