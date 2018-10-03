using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Calmo.Core.Validator.Documents;
using Calmo.Core.Validator.Formats;

#if __MOBILE__
using System.Reflection;
#endif

namespace Calmo.Core.Validator
{
    public class ValidationResult<TModel> : IEnumerable<ValidationItem>
    {
        private bool _breakIfIsInvalid = false;
        private readonly List<ValidationItem> _errors = new List<ValidationItem>();

        public ValidationResult(TModel model)
        {
            Values = new List<TModel> {model};
        }

        public ValidationResult(IEnumerable<TModel> models)
        {
            Values = models;
        }

        public IEnumerable<TModel> Values { get; }

        public ValidationResult<TModel> Rule<TProperty>(Expression<Func<TModel, TProperty>> property, bool rule, string message = null)
        {
            if (this._breakIfIsInvalid && !this.Success)
                return this;
            this._breakIfIsInvalid = false;

            if (!GetMember(property, out var member)) return this;

            this.AddItem(rule, message, member.Name, member.Name);
            return this;
        }

        public ValidationResult<TModel> Rule<TProperty>(Expression<Func<TModel, TProperty>> property, Func<TModel, bool> rule, string message = null)
        {
            if (this._breakIfIsInvalid && !this.Success)
                return this;
            this._breakIfIsInvalid = false;

            if (!GetMember(property, out var member)) return this;

            var index = 0;
            foreach (var value in this.Values)
            {
                this.AddItem(rule.Invoke(value), message, member.Name, member.Name, index);
                index++;
            }

            return this;
        }

        public ValidationResult<TModel> Rule<TProperty>(Expression<Func<TModel, TProperty>> property, Func<TProperty, bool> rule, string message = null)
        {
            if (this._breakIfIsInvalid && !this.Success)
                return this;

            this._breakIfIsInvalid = false;

            if (!GetMember(property, out var member)) return this;
            if (!GetMemberProperty(member, out var propertyInfo)) return this;

            var index = 0;
            foreach (var value in this.Values)
            {
                var propertyValue = this.GetMemberValue<TProperty>(propertyInfo, value);
                this.AddItem(rule(propertyValue), message, member.Name, member.Name, index);
                index++;
            }

            return this;
        }

        public ValidationResult<TModel> Rule(Expression<Func<TModel, string>> property, FormatDefinition rule, string message = null)
        {
            if (this._breakIfIsInvalid && !this.Success)
                return this;

            this._breakIfIsInvalid = false;

            if (!GetMember(property, out var member)) return this;
            if (!GetMemberProperty(member, out var propertyInfo)) return this;

            var index = 0;
            foreach (var value in this.Values)
            {
                var propertyValue = this.GetMemberValue<string>(propertyInfo, value);
                this.AddItem(rule.Validate(propertyValue), message, member.Name, member.Name, index);
                index++;
            }

            return this;
        }

        public ValidationResult<TModel> Rule(Expression<Func<TModel, string>> property, DocumentDefinition rule, string message = null)
        {
            if (this._breakIfIsInvalid && !this.Success)
                return this;

            this._breakIfIsInvalid = false;

            if (!GetMember(property, out var member)) return this;
            if (!GetMemberProperty(member, out var propertyInfo)) return this;

            var index = 0;
            foreach (var value in this.Values)
            {
                var propertyValue = this.GetMemberValue<string>(propertyInfo, value);
                this.AddItem(rule.Validate(propertyValue), message, member.Name, member.Name, index);
                index++;
            }

            return this;
        }

        public ValidationResult<TModel> BreakIfIsInvalid()
        {
            this._breakIfIsInvalid = true;

            return this;
        }

        public void ThrowOnFail(string separator = null)
        {
            ThrowOnFail<Exception>(separator);
        }

        public void ThrowOnFail<TException>(string separator = null) where TException : Exception
        {
            if (this.Success) return;

            if (separator == null) separator = Environment.NewLine;

            if (typeof(TException) == typeof(DomainException))
                throw new DomainException(this._errors.Select(e => e.Message), separator);

            var exception = (TException)Activator.CreateInstance(typeof(TException), GetSummary(separator));
            throw exception;
        }

        public bool Success => this._errors.Count == 0;

        public string GetSummary(string separator = null)
        {
            if (this.Success) return string.Empty;

            var sep = separator ?? Environment.NewLine;
            return String.Join(sep, this._errors.Select(e => e.Message));
        }

        public IEnumerable<object> GetJson()
        {
            return this._errors.Select(e => new {e.Property, e.Message });
        }

        private bool GetMember<TProperty>(Expression<Func<TModel, TProperty>> property, out MemberInfo member)
        {
            member = null;

            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null) return false;

            member = memberExpression.Member;
            return true;
        }

        private bool GetMemberProperty(MemberInfo member, out PropertyInfo propertyInfo)
        {
#if !__MOBILE__
            propertyInfo = member.DeclaringType?.GetProperty(member.Name);
#else
            propertyInfo = member.DeclaringType?.GetRuntimeProperty(member.Name);
#endif
            return propertyInfo == null;
        }

        private T GetMemberValue<T>(PropertyInfo propertyInfo, object value)
        {
#if !__MOBILE__
            return (T)propertyInfo.GetValue(value, null);
#else
            return (T)propertyInfo.GetValue(value);
#endif
        }

        private void AddItem(bool rule, string message = null, string property = null, string legend = null, int index = 0)
        {
            if (rule) return;

            if (message == null)
                message = "{0} não atende a regra especificada.";

            if (legend == null)
                legend = property ?? "A informação";

            message = String.Format(message, legend);

            this._errors.Add(new ValidationItem { Message = message, Property = property, Index = index });
        }

        #region IEnumerable Members

        public IEnumerator<ValidationItem> GetEnumerator()
        {
            return this._errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._errors.GetEnumerator();
        }

        #endregion
    }
}