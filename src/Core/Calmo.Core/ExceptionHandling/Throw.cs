using System;
using System.Globalization;
using Calmo.Core.Properties;

namespace Calmo.Core.ExceptionHandling
{
    public static class Throw
    {
        public static Func<Exception, Exception> GlobalModifier { get; set; }

        public static void IfArgumentNull(object argument, string argumentName, Func<Exception, Exception> modifier = null)
        {
            if (argument != null) return;

            Throw.Now(new ArgumentNullException(argumentName), modifier);
        }

        public static void IfArgumentNullOrEmpty(string argument, string argumentName, Func<Exception, Exception> modifier = null)
        {
            Throw.IfArgumentNull(argument, argumentName, modifier);

            if (!String.IsNullOrEmpty(argument)) return;

            Throw.Now(new ArgumentException(string.Format(CultureInfo.CurrentUICulture, ThrowMessages.ArgumentNullOrEmpty, argumentName)), modifier);
        }

        public static void IfReferenceNull(object reference, string referenceName, Func<Exception, Exception> modifier = null)
        {
            if (reference != null) return;

            Throw.Now(new NullReferenceException(string.Format(CultureInfo.CurrentUICulture, ThrowMessages.ReferenceNull, referenceName)), modifier);
        }

        public static void IfReferenceNullOrEmpty(string reference, string referenceName, Func<Exception, Exception> modifier = null)
        {
            Throw.IfReferenceNull(reference, referenceName, modifier);

            if (!String.IsNullOrEmpty(reference)) return;

            Throw.Now(new NullReferenceException(string.Format(CultureInfo.CurrentUICulture, ThrowMessages.ReferenceNullOrEmpty, referenceName)), modifier);
        }

        private static void Now(Exception exception, Func<Exception, Exception> modifier = null)
        {
            if (exception == null)
                throw new InvalidOperationException(ThrowMessages.ExceptionCannotBeNull);

            var e = exception;

            if (modifier != null)
                e = modifier(exception) ?? exception;

            if (GlobalModifier != null)
                e = GlobalModifier(e) ?? exception;

            throw e;
        }
    }
}
