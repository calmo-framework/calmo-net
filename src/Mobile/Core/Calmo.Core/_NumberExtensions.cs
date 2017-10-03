using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
