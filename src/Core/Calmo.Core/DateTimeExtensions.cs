using System.Collections.Generic;
using System.Globalization;

namespace System
{
    public static class DateTimeExtensions
    {
        public static string GetMonthName(this DateTime date, CultureInfo culture = null)
        {
	        if (culture == null) culture = CultureInfo.CurrentCulture;
			

            return culture.DateTimeFormat.GetMonthName(date.Month);
        }

        public static string GetAbbreviatedMonthName(this DateTime date, CultureInfo culture = null)
		{
			if (culture == null) culture = CultureInfo.CurrentCulture;
            

#if !__MOBILE__
            return culture.DateTimeFormat.GetAbbreviatedMonthName(date.Month).ToTitleCase();
#else
            return culture.DateTimeFormat.GetAbbreviatedMonthName(date.Month);
#endif
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="date"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
        public static string GetDayName(this DateTime date, CultureInfo culture = null)
		{
			if (culture == null) culture = CultureInfo.CurrentCulture;

			return date.DayOfWeek.GetDayName(culture);
        }

        public static IEnumerable<DateTime> GetEarlierDates(this DateTime initialDate, int interval)
        {
            var resultado = new DateTime[interval];

            for (var i = 0; i < interval; i++)
                resultado[i] = initialDate.AddMonths(-i);

            return resultado;
        }

        public static int GetAge(this DateTime date)
        {
            var age = DateTime.Now.Year - date.Year;
            if (DateTime.Now.Month < date.Month || (DateTime.Now.Month == date.Month && DateTime.Now.Day < date.Day))
                age--;
            return age;
        }

        public static string GetDayName(this DayOfWeek dayOfWeek, CultureInfo culture = null)
        {
            if (culture == null)
                culture = CultureInfo.CurrentCulture;

            if (!Enum.IsDefined(typeof(DayOfWeek), dayOfWeek))
                throw new ArgumentOutOfRangeException("dayOfWeek");

#if !__MOBILE__
            return culture.DateTimeFormat.GetDayName(dayOfWeek).ToTitleCase();
#else
            return culture.DateTimeFormat.GetDayName(dayOfWeek);
#endif
        }

        public static string GetDayName(this DayOfWeek? dayOfWeek)
        {
            return !dayOfWeek.HasValue ? null : dayOfWeek.Value.GetDayName();
        }

        public static DateTime Truncate(this DateTime dateTime)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));
        }

        public static DateTime? Truncate(this DateTime? dateTime)
        {
            if (!dateTime.HasValue) return null;

            return Truncate(dateTime.Value);
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year,date.Month,date.Day,23,59,59,999);
        }

        public static DateTime BeginOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }
    }
}
