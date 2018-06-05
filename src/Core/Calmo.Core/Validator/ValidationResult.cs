using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
#if __MOBILE__
using System.Reflection;
#endif

namespace Calmo.Core.Validator
{
    public class ValidationResult<TModel> : List<ValidationItem>
    {
        public ValidationResult(TModel model)
        {
            Values = new List<TModel> {model};
        }

        public ValidationResult(IEnumerable<TModel> models)
        {
            Values = models ;
        }

        public IEnumerable<TModel> Values { get; private set; }

        public ValidationResult<TModel> Rule<TProperty>(Expression<Func<TModel, TProperty>> property, bool rule, string message = null)
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression != null)
            {
                var member = memberExpression.Member;
                

                return this.Rule(rule, message, member.Name, member.Name);
            }

            return this;
        }

        public ValidationResult<TModel> Rule<TProperty>(Expression<Func<TModel, TProperty>> property, Func<TModel, bool> rule, string message = null)
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null) return this;

            var member = memberExpression.Member;

            var index = 0;
            foreach (var value in this.Values)
            {
                this.Rule(rule.Invoke(value), message, member.Name, member.Name, index);
                index++;
            }

            return this;
        }

        public ValidationResult<TModel> Rule<TProperty>(Expression<Func<TModel, TProperty>> property, Func<TProperty, bool> rule, string message = null)
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null) return this;

            var member = memberExpression.Member;
            
            var index = 0;
            foreach (var value in this.Values)
            {
#if !__MOBILE__
                var propertyInfo = member.DeclaringType?.GetProperty(member.Name);
#else
                var propertyInfo = member.DeclaringType?.GetRuntimeProperty(member.Name);
#endif
                if (propertyInfo == null) continue;

#if !__MOBILE__
                var propertyValue = (TProperty)propertyInfo.GetValue(value, null);
#else
                var propertyValue = (TProperty)propertyInfo.GetValue(value);
#endif

                this.Rule(rule(propertyValue), message, member.Name, member.Name, index);
                index++;
            }

            return this;
        }

        //private T GetValue<T>(MemberExpression member)
        //{
        //    var objectMember = Expression.Convert(member, typeof(T));
        //    var getterLambda = Expression.Lambda<Func<T>>(objectMember);
        //    var getter = getterLambda.Compile();

        //    return getter();
        //}

        public ValidationResult<TModel> Rule(bool rule, string message = null, string property = null, string legend = null, int index = 0)
        {
            if (rule) return this;

            if (message == null)
                message = "{0} não atende a regra especificada.";

            if (legend == null)
                legend = property ?? "A informação";

            message = String.Format(message, legend);

            this.Add(message, property, index);

            return this;
        }

        public bool Success => this.Count == 0;

        public string GetSummary(string separator = null)
        {
            if (this.Success) return string.Empty;

            var sep = separator ?? Environment.NewLine;
            return String.Join(sep, this.Select(e => e.Message));
        }

        public void ThrowOnFail(string separator = null)
        {
            ThrowOnFail<Exception>(separator);
        }

        public void ThrowOnFail<TException>(string separator = null) where TException : Exception
        {
            if (this.Success) return;

            if (typeof (TException) == typeof (DomainException))
                throw new DomainException(this.Select(e => e.Message), separator);

            var exception = (TException)Activator.CreateInstance(typeof(TException), GetSummary(separator));
            throw exception;
        }

        public IEnumerable<object> GetJson()
        {
            return this.Select(e => new {e.Property, e.Message });
        }

        public void Add(string message, string property = null, int index = 0)
        {
            this.Add(new ValidationItem { Message = message, Property = property, Index = index});
        }
    }
}