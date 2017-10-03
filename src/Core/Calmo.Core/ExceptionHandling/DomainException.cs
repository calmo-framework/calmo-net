using System.Collections.Generic;

namespace System
{
    public class DomainException : Exception
    {
        private string _separator = Environment.NewLine;

        public readonly IEnumerable<string> Errors;

        public DomainException(IEnumerable<string> errors)
        {
            this.Errors = errors;
        }

        public DomainException(string error)
        {
            this.Errors = new[] { error };
        }

        public DomainException(IEnumerable<string> errors, string separator)
        {
            this.Errors = errors;
            _separator = separator ?? _separator;
        }

        public override string Message => String.Join(_separator, this.Errors);
    }
}