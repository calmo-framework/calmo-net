using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace System
{
    public static class ConversionExtensions
    {
        #region Boolean

        public static bool ToBoolean(this string valor)
        {
            return String.IsNullOrWhiteSpace(valor) ? valor.To<bool>() : default(bool);
        }

        #endregion

        #region Integer

		public static int ToInt(this string value)
		{
			if (String.IsNullOrWhiteSpace(value))
				return 0;

			int d;

			if (int.TryParse(value, out d))
				return d;

			throw new InvalidCastException("Invalid int string.");
		}

        public static int? ToIntNullable(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            return ToInt(value);
        }

		#endregion

        #region Decimal

        public static decimal? ToDecimal(this string value, string cultureInfo = "pt-BR")
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            decimal d;

            if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo(cultureInfo), out d))
            {
                return d;
            }

            throw new InvalidCastException("Invalid decimal string.");
        }

        public static decimal ToDecimal(this string value, decimal defaultValue)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return ToDecimal(value).Value;
        }

        public static decimal? ToNullableDecimal(this string valor)
        {
            return String.IsNullOrWhiteSpace(valor) ? valor.To<decimal>() : (decimal?)null;
        }

        public static string ToStringFormat(this decimal value)
        {
            return value.ToString("N2");
        }

        public static string ToStringFormat(this decimal? value, decimal valorDefault)
        {
            if (!value.HasValue)
            {
                return valorDefault.ToStringFormat();
            }

            return ToStringFormat(value.Value);
        }

        public static string ToStringFormat(this decimal? value)
        {
            if (!value.HasValue)
            {
                return null;
            }

            return ToStringFormat(value.Value);
        }

        public static bool IsValidDecimal(this string value)
        {
            decimal decimalValue;

            return decimal.TryParse(value, out decimalValue);
        }

        public static bool IsValidDecimal(this string value, decimal minValue)
        {
            decimal valorDecimal;

            var valido = decimal.TryParse(value, out valorDecimal);

            if (valido)
                valido = valorDecimal >= minValue;

            return valido;
        }

        public static string ToCoordinateString(this decimal value)
        {
            return value.ToString(new CultureInfo("en-US"));
        }

        public static string ToCoordinateString(this decimal? value)
        {
            if (!value.HasValue)
                return null;

            return value.Value.ToCoordinateString();
        }

        public static decimal? ToCoordinate(this string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return null;
            if (value.LastIndexOf('.') > value.LastIndexOf(','))
                return Convert.ToDecimal(value, new CultureInfo("en-US"));
#if !__MOBILE__
            return Convert.ToDecimal(value, Thread.CurrentThread.CurrentUICulture);
#else
            return Convert.ToDecimal(value, CultureInfo.CurrentUICulture);
#endif
        }

#endregion

        #region TimeSpan

        public static string ToStringFormat(this TimeSpan value)
        {
            return String.Format("{0:00}:{1:00}:{2:00}", value.Hours, value.Minutes, value.Seconds);
        }

        public static string ToStringFormat(this TimeSpan? value)
        {
            if (!value.HasValue)
            {
                return null;
            }

            return String.Format("{0:00}:{1:00}:{2:00}", value.Value.Hours, value.Value.Minutes, value.Value.Seconds);
        }

        public static string ToStringFormat(this TimeSpan value, bool totalHours)
        {
            return totalHours ? String.Format("{0:00}:{1:00}:{2:00}", Math.Truncate(value.TotalHours), Math.Abs(value.Minutes), Math.Abs(value.Seconds)) : value.ToStringFormat();
        }

        public static string ToStringTimerFormat(this TimeSpan value)
        {
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", Math.Truncate(value.TotalDays), Math.Abs(value.Hours), Math.Abs(value.Minutes), Math.Abs(value.Seconds));
        }

        public static string ToStringFormat(this TimeSpan? value, bool totalHours)
        {
            if (!value.HasValue) return null;

            return ToStringFormat(value.Value, totalHours);
        }

        public static string ToStringFormat(this TimeSpan value, bool totalHours, bool absoluteValue)
        {
            return absoluteValue?  String.Format("{0:00}:{1:00}:{2:00}",Math.Abs( Math.Truncate( value.TotalHours)), Math.Abs(Math.Truncate((double)value.Minutes)), Math.Abs(value.Seconds)):
                                                value.ToStringFormat(totalHours);

        }

        public static TimeSpan ToTimeSpan(this string value)
        {
#if !__MOBILE__
            var containsTimeSeparator = value.Contains(':');
#else
            var containsTimeSeparator = value.Contains(":");
#endif

            if (String.IsNullOrWhiteSpace(value) || !containsTimeSeparator)
                throw new ArgumentNullException(nameof(value));

            var horario = value.Split(':').ConvertAll(Convert.ToInt32);

            return new TimeSpan(horario.ElementAt(0), horario.ElementAt(1), horario.Count() == 3 ? horario.ElementAt(2) : 0);
        }

        public static TimeSpan? ToTimeSpanNullable(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            return ToTimeSpan(value);
        }

        public static bool IsValidTime(this string value)
        {
#if !__MOBILE__
            var containsTimeSeparator = value.Contains(':');
#else
            var containsTimeSeparator = value.Contains(":");
#endif

            if (String.IsNullOrWhiteSpace(value) || !containsTimeSeparator)
                return false;

            var time = value.Split(':').ConvertAll(Convert.ToInt32);
            int hours = time.ElementAt(0);
            int minutes = time.ElementAt(1);

            bool valid = (hours >= 0 && hours <= 23) && (minutes >= 0 && minutes <= 59);

            if (time.Count() == 3)
            {
                int seconds = time.ElementAt(2);

                return valid && (seconds >= 0 && seconds <= 59);
            }

            return valid;
        }

        #endregion

        #region DateTime

        public static string ToStringFormat(this DateTime value, bool formatTime = false, bool addSeconds = true)
        {
#if !__MOBILE__
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
#else
            var currentCulture = CultureInfo.CurrentUICulture;
#endif
        
            if (currentCulture.Name.Equals("pt-BR"))
                return formatTime ? value.ToString("dd/MM/yyyy HH:mm" + (addSeconds ? ":ss" : string.Empty)) : value.ToString("dd/MM/yyyy");

            return formatTime ? value.ToString("MM/dd/yyyy HH:mm" + (addSeconds ? ":ss" : string.Empty)) : value.ToString("MM/dd/yyyy");
        }

        public static string ToStringFormat(this DateTime? value, bool formatTime = false)
        {
            if (value.HasValue)
            {
                return value.Value.ToStringFormat(formatTime);
            }

            return String.Empty;
        }

        public static DateTime ToDateTime(this string value, bool isUSValid = false)
        {
            DateTime data;

            if (DateTime.TryParse(value, new CultureInfo("pt-BR"), DateTimeStyles.None, out data))
                return data;

            if (isUSValid && DateTime.TryParse(value, new CultureInfo("en-US"), DateTimeStyles.None, out data))
                return data;

            throw new InvalidOperationException("Invalid date string.");
        }

        public static DateTime ToDateTime(this string value, DateTime defaultValue)
        {
            DateTime data;
#if !__MOBILE__
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
#else
            var currentCulture = CultureInfo.CurrentUICulture;
#endif

            if (currentCulture.Name.Equals("pt-BR"))
                return DateTime.TryParse(value, new CultureInfo("pt-BR"), DateTimeStyles.None, out data) ? data : defaultValue;

            return DateTime.TryParse(value, new CultureInfo("en-US"), DateTimeStyles.None, out data) ? data : defaultValue;
        }

        public static bool IsValidDateTime(this string value)
        {
            DateTime data;
#if !__MOBILE__
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
#else
            var currentCulture = CultureInfo.CurrentUICulture;
#endif

            if (currentCulture.Name.Equals("pt-BR"))
                return DateTime.TryParse(value, new CultureInfo("pt-BR"), DateTimeStyles.None, out data);

            return DateTime.TryParse(value, new CultureInfo("en-US"), DateTimeStyles.None, out data);
        }
        public static DateTime? ToDateTimeNullable(this string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return null; 
            
            DateTime data;
#if !__MOBILE__
            var currentCulture = Thread.CurrentThread.CurrentUICulture;
#else
            var currentCulture = CultureInfo.CurrentUICulture;
#endif

            if (currentCulture.Name.Equals("pt-BR"))
            {
                if (DateTime.TryParse(value, new CultureInfo("pt-BR"), DateTimeStyles.None, out data))
                    return data;
            }
            else
            {
                if (DateTime.TryParse(value, new CultureInfo("en-US"), DateTimeStyles.None, out data))
                    return data;
            }

            return null;
        }

        public static string ToStringFormatYearMonth(this DateTime value)
        {
            return value.ToString("MM/yyyy");
        }

        #endregion

        #region Long

        public static long ToLong(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return 0;

            long d;

            if (long.TryParse(value, out d))
                return d;

            throw new InvalidCastException("invalid long string.");
        }

        public static long? ToNullableLong(this string valor)
        {
            return String.IsNullOrWhiteSpace(valor) ? valor.To<long>() : (long?)null;
        }

        #endregion
    }
}
