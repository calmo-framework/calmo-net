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

		/// <summary>
		/// Fixed rule value validation method
		/// </summary>
		/// <typeparam name="TProperty">Property type being validated</typeparam>
		/// <param name="property">Property value</param>
		/// <param name="rule">Rule result</param>
		/// <param name="message">Error message</param>
		/// <returns>Validation context</returns>
		public ValidationResult<TModel> Rule<TProperty>(Expression<Func<TModel, TProperty>> property, bool rule, string message = null)
        {
            if (this._breakIfIsInvalid && !this.Success)
                return this;
            this._breakIfIsInvalid = false;

            if (!GetMember(property, out var member)) return this;

            this.AddItem(rule, message, member.Name, member.Name);
            return this;
        }

		/// <summary>
		/// General rule value validation method
		/// </summary>
		/// <param name="ruleName">Rule name</param>
		/// <param name="rule">Validation function, must return true if valid</param>
		/// <param name="message">Error message</param>
		/// <returns>Validation context</returns>
		public ValidationResult<TModel> Rule(string ruleName, Func<TModel, bool> rule, string message = null)
        {
            if (this._breakIfIsInvalid && !this.Success)
                return this;

            this._breakIfIsInvalid = false;

            var index = 0;
            foreach (var value in this.Values)
            {
                this.AddItem(rule.Invoke(value), message, ruleName, null, index);
                index++;
            }

            return this;
        }

		/// <summary>
		/// Class Property Validation Rule Method
		/// </summary>
		/// <typeparam name="TProperty">Property type being validated</typeparam>
		/// <param name="property">Property value</param>
		/// <param name="rule">Validation against the TProperty, must return true if valid</param>
		/// <param name="message">Error message</param>
		/// <returns>Validation context</returns>
		public ValidationResult<TModel> Rule<TProperty>(Expression<Func<TModel, TProperty>> property, Func<TProperty, bool> rule, string message = null)
        {
            if (this._breakIfIsInvalid && !this.Success)
                return this;

            this._breakIfIsInvalid = false;

            if (!GetMember(property, out var member)) return this;

            var index = 0;
            foreach (var value in this.Values)
            {
                var propertyValue = value.GetPropertyValue<TProperty>(member.Name);
                this.AddItem(rule(propertyValue), message, member.Name, member.Name, index);
                index++;
            }

            return this;
        }

		/// <summary>
		/// Class String Property Validation Rule Method
		/// </summary>
		/// <param name="property">String property being validated</param>
		/// <param name="rule">String format rule definition (Regex) being used</param>
		/// <param name="message">Error message</param>
		/// <returns>Validation context</returns>
		public ValidationResult<TModel> Rule(Expression<Func<TModel, string>> property, FormatDefinition rule, string message = null)
        {
            if (this._breakIfIsInvalid && !this.Success)
                return this;

            this._breakIfIsInvalid = false;

            if (!GetMember(property, out var member)) return this;

            var index = 0;
            foreach (var value in this.Values)
            {
                var propertyValue = value.GetPropertyValue<string>(member.Name);
                this.AddItem(rule.Validate(propertyValue), message, member.Name, member.Name, index);
                index++;
            }

            return this;
        }

		/// <summary>
		/// Document format/rules validation method
		/// </summary>
		/// <param name="property">String property being validated</param>
		/// <param name="rule">DocumentDefition rule being used</param>
		/// <param name="message">ErrorMessage</param>
		/// <returns>Validation context</returns>
		public ValidationResult<TModel> Rule(Expression<Func<TModel, string>> property, DocumentDefinition rule, string message = null)
        {
            if (this._breakIfIsInvalid && !this.Success)
                return this;

            this._breakIfIsInvalid = false;

            if (!GetMember(property, out var member)) return this;

            var index = 0;
            foreach (var value in this.Values)
            {
                var propertyValue = value.GetPropertyValue<string>(member.Name);
                this.AddItem(rule.Validate(propertyValue), message, member.Name, member.Name, index);
                index++;
            }

            return this;
        }

		/// <summary>
		/// Inform to the validation context if the validation must be stopped when an error occur
		/// </summary>
		/// <returns>Validation context</returns>
        public ValidationResult<TModel> BreakIfIsInvalid()
        {
            this._breakIfIsInvalid = true;

            return this;
        }

		/// <summary>
		/// Throw an Exception if the validation fails
		/// </summary>
		/// <param name="separator">String separator between error messages</param>
        public void ThrowOnFail(string separator = null)
        {
            ThrowOnFail<Exception>(separator);
        }

		/// <summary>
		/// Throw a custom exception type if the validation fails
		/// </summary>
		/// <typeparam name="TException">Exception type to be throw</typeparam>
		/// <param name="separator">String separator between error messages</param>
		public void ThrowOnFail<TException>(string separator = null) where TException : Exception
        {
            if (this.Success) return;

            if (separator == null) separator = Environment.NewLine;

            if (typeof(TException) == typeof(DomainException))
                throw new DomainException(this._errors.Select(e => e.Message), separator);

            var exception = (TException)Activator.CreateInstance(typeof(TException), GetSummary(separator));
            throw exception;
        }

		/// <summary>
		/// Indicates if the validation has no errors
		/// </summary>
        public bool Success => this._errors.Count == 0;

		/// <summary>
		/// Get all the errors messages in one string
		/// </summary>
		/// <param name="separator">String separator between error messages</param>
		/// <returns>All the error messages concatenated using the separator provided</returns>
		public string GetSummary(string separator = null)
        {
            if (this.Success) return string.Empty;

            var sep = separator ?? Environment.NewLine;
            return String.Join(sep, this._errors.Select(e => e.Message));
        }

		/// <summary>
		/// Create a json mapping the properties validaded and the validation errors
		/// </summary>
		/// <returns>JSON array of Property/Message</returns>
        public IEnumerable<object> GetJson()
        {
            return this._errors.Select(e => new {e.Property, e.Message });
        }

        private bool GetMember<TProperty>(Expression<Func<TModel, TProperty>> property, out MemberInfo member)
        {
            member = null;

	        if (!(property.Body is MemberExpression memberExpression)) return false;

            member = memberExpression.Member;
            return true;
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