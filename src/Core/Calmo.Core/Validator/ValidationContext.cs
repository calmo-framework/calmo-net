using System.Collections.Generic;

namespace Calmo.Core.Validator
{
    public static class ValidationContext
    {
        public static ValidationScope<TModel> Validate<TModel>(this TModel model)
        {
            return new ValidationScope<TModel>(model);
        }

        public static ValidationScope<TModel> Validate<TModel>(this IEnumerable<TModel> models)
        {
            return new ValidationScope<TModel>(models);
        }
    }
}
