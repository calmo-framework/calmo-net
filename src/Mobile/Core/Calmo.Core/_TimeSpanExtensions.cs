using System.Collections.Generic;
using System.Globalization;

namespace System
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Trucante(this TimeSpan timeSpan)
        {
            return timeSpan.Subtract(TimeSpan.FromMilliseconds(timeSpan.Milliseconds));
        }

        public static TimeSpan? Truncate(this TimeSpan? timeSpan)
        {
            if (!timeSpan.HasValue) return null;

            return Trucante(timeSpan.Value);
        }
    }
}
