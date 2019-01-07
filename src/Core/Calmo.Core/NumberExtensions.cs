namespace System
{
    public static class NumberExtensions
    {
        public static string ToStringNumber(this int value)
        {
            return string.Format("{0:N0}", value);
        }
    }
}
