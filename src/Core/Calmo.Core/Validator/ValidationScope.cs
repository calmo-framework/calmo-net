using System.Collections.Generic;

namespace Calmo.Core.Validator
{
    public class ValidationScope<TModel>
    {
        private readonly IEnumerable<TModel> _values;

        internal ValidationScope(TModel model)
        {
            _values = new List<TModel> { model };
        }

        internal ValidationScope(IEnumerable<TModel> models)
        {
            _values = models;
        }

        public ValidationResult<TModel> Using()
        {
            return new ValidationResult<TModel>(_values);
        }
    }
}