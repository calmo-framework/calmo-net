namespace System
{
    public static class MathExtensions
    {
        public static decimal Percentage(this decimal value, decimal percentage)
        {
            return value * (percentage / 100m);
        }

        public static decimal AdjustPercentage(this decimal value, decimal percentage)
        {
            return value + value.Percentage(percentage);
        }

        public static decimal Truncate(this decimal value, int precision)
        {
            precision = int.Parse("1" + new string('0', precision));
            return Math.Truncate(precision * value) / precision;
        }

        public static decimal PercentageBetween(this decimal value, decimal anotherValue)
        {
            if (value == 0) return 0;

            return (anotherValue * 100) / value;
        }

        public static int? Max(this int? value1, int? value2)
        {
            if (value1 == value2)
                return value1;

            if (value1.HasValue && value2.HasValue)
                return Math.Max(value1.Value, value2.Value);

            return value1.HasValue ? value1 : value2;
        }

        public static int? Min(this int? value1, int? value2)
        {
            if (value1 == value2)
                return value1;

            if (value1.HasValue && value2.HasValue)
                return Math.Min(value1.Value, value2.Value);

            return value1.HasValue ? value1 : value2;
        }

        public static long? Max(this long? value1, long? value2)
        {
            if (value1 == value2)
                return value1;

            if (value1.HasValue && value2.HasValue)
                return Math.Max(value1.Value, value2.Value);

            return value1.HasValue ? value1 : value2;
        }

        public static long? Min(this long? value1, long? value2)
        {
            if (value1 == value2)
                return value1;

            if (value1.HasValue && value2.HasValue)
                return Math.Min(value1.Value, value2.Value);

            return value1.HasValue ? value1 : value2;
        }

        public static bool Overlap<T>(this T minA, T maxA, T minB, T maxB) where T : IComparable<T>
        {
            if (minA.CompareTo(minB) >= 0 && minA.CompareTo(maxB) <= 0)
                return true;
            if (minB.CompareTo(minA) >= 0 && minB.CompareTo(maxA) <= 0)
                return true;

            return false;
        }

        public static double Radians(this double value)
        {
            return value * Math.PI /180;
        }
    }
}