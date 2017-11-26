using System;
using System.Collections.Generic;
using System.Linq;

namespace EconSimVisual.Extensions
{
    internal static class EnumUtils
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
