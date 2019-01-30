using System.Collections.Generic;

namespace Calmo.Core.Validator
{
	/// <summary>
	/// Sarting class of a validation context
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
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

		/// <summary>
		/// Creates a new ValidationResult (With a validation rules list)
		/// </summary>
		/// <returns>New validation result for a given model.</returns>
        public ValidationResult<TModel> Using()
        {
            return new ValidationResult<TModel>(_values);
        }
    }
}