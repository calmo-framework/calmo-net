using System;
using System.Globalization;

#if !NETSTANDARD && !__MOBILE__
using Calmo.Core.Properties;
#endif

#if !NETSTANDARD && __MOBILE__
using Calmo.Xamarin.Core;
#endif

namespace Calmo.Core.ExceptionHandling
{
    public static class Throw
    {
#if NETSTANDARD
        private const string ArgumentNullOrEmptyMessage = "O parâmetro {0} não pode ser nulo ou vazio.";
        private const string ExceptionCannotBeNullMessage = "A exceção a ser disparada não pode ser nula.";
        private const string ReferenceNullMessage = "A referência {0} não pode ser nula.";
        private const string ReferenceNullOrEmptyMessage = "A referência {0} não pode ser nula ou vazia.";
#else
        private static readonly string ArgumentNullOrEmptyMessage = ThrowMessages.ArgumentNullOrEmpty;
        private static readonly string ExceptionCannotBeNullMessage = ThrowMessages.ExceptionCannotBeNull;
        private static readonly string ReferenceNullMessage = ThrowMessages.ReferenceNull;
        private static readonly string ReferenceNullOrEmptyMessage = ThrowMessages.ReferenceNullOrEmpty;
#endif

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

            Throw.Now(new ArgumentException(String.Format(CultureInfo.CurrentUICulture, ArgumentNullOrEmptyMessage, argumentName)), modifier);
        }

        public static void IfReferenceNull(object reference, string referenceName, Func<Exception, Exception> modifier = null)
        {
            if (reference != null) return;

            Throw.Now(new NullReferenceException(String.Format(CultureInfo.CurrentUICulture, ReferenceNullMessage, referenceName)), modifier);
        }

        public static void IfReferenceNullOrEmpty(string reference, string referenceName, Func<Exception, Exception> modifier = null)
        {
            Throw.IfReferenceNull(reference, referenceName, modifier);

            if (!String.IsNullOrEmpty(reference)) return;

            Throw.Now(new NullReferenceException(String.Format(CultureInfo.CurrentUICulture, ReferenceNullOrEmptyMessage, referenceName)), modifier);
        }

        private static void Now(Exception exception, Func<Exception, Exception> modifier = null)
        {
            if (exception == null)
                throw new InvalidOperationException(ExceptionCannotBeNullMessage);

            var e = exception;

            if (modifier != null)
                e = modifier(exception) ?? exception;

            if (GlobalModifier != null)
                e = GlobalModifier(e) ?? exception;

            throw e;
        }
    }
}
